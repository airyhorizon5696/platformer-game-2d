using UnityEngine;
using UnityEngine.InputSystem;

namespace ColliderEventSystem.Samples
{
    /// <summary>
    /// Bare-bones first-person controller for the example scene - just enough to walk into trigger zones
    /// and look around. Reads Input System directly (no .inputactions asset) to keep the sample
    /// self-contained.
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    public sealed class PlayerMovement : MonoBehaviour
    {
        public float moveSpeed = 6f;
        public float sprintMultiplier = 2f;
        public float gravity = -9.81f;

        [Tooltip("The camera to pitch up/down. Leave empty to auto-find a child Camera.")]
        public Transform cameraTransform;

        public float lookSensitivity = 3f;

        [Tooltip("How far up/down the camera can pitch, in degrees.")]
        public float maxLookAngle = 90f;

        private CharacterController m_Controller;
        private float m_VerticalVelocity;
        private float m_Pitch;

        private void Awake()
        {
            m_Controller = GetComponent<CharacterController>();

            if (cameraTransform == null)
            {
                var cam = GetComponentInChildren<Camera>();
                if (cam != null) cameraTransform = cam.transform;
            }

            // Otherwise m_Pitch starts at 0 and HandleLook snaps the camera level on the very first
            // frame, discarding whatever pitch was set on it in the Editor. DeltaAngle normalizes out of
            // eulerAngles' [0, 360) range so a downward tilt (e.g. 350) reads as -10, not clamped to +90.
            if (cameraTransform != null)
            {
                m_Pitch = Mathf.DeltaAngle(0f, cameraTransform.localEulerAngles.x);
            }

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void Update()
        {
            HandleLook();
            HandleMove();

            // Convenient while testing in the Editor - Escape gives the mouse back without stopping Play Mode.
            if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }

        private void HandleLook()
        {
            if (Mouse.current == null || cameraTransform == null) return;

            Vector2 delta = Mouse.current.delta.ReadValue() * (lookSensitivity * 0.1f);

            // Yaw turns the whole player, so movement direction follows where you're facing. Pitch only
            // tilts the camera - tilting the player itself would tip the CharacterController over.
            transform.Rotate(Vector3.up, delta.x);

            m_Pitch = Mathf.Clamp(m_Pitch - delta.y, -maxLookAngle, maxLookAngle);
            cameraTransform.localRotation = Quaternion.Euler(m_Pitch, 0f, 0f);
        }

        private void HandleMove()
        {
            Keyboard keyboard = Keyboard.current;
            if (keyboard == null) return;

            Vector3 input = Vector3.zero;
            if (keyboard.wKey.isPressed) input.z += 1f;
            if (keyboard.sKey.isPressed) input.z -= 1f;
            if (keyboard.dKey.isPressed) input.x += 1f;
            if (keyboard.aKey.isPressed) input.x -= 1f;

            float speed = keyboard.shiftKey.isPressed ? moveSpeed * sprintMultiplier : moveSpeed;

            // Relative to the player's own facing (set by yaw above), not the world axes.
            Vector3 move = transform.TransformDirection(input.normalized) * speed;

            // CharacterController doesn't apply gravity on its own - without this the player floats the
            // instant it leaves the ground (e.g. walking off a ledge).
            if (m_Controller.isGrounded && m_VerticalVelocity < 0f)
            {
                m_VerticalVelocity = -2f;
            }
            m_VerticalVelocity += gravity * Time.deltaTime;
            move.y = m_VerticalVelocity;

            m_Controller.Move(move * Time.deltaTime);
        }
    }
}
