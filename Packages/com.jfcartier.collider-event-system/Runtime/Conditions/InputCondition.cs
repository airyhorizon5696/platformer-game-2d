using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

namespace ColliderEventSystem
{
    /// <summary>
    /// Checks a keyboard key directly (no setup needed), or an Input Action from an Input Actions asset
    /// (supports rebinding, gamepads, etc.).
    /// </summary>
    public sealed class InputCondition : ConditionBase
    {
        public enum InputSource
        {
            Key,
            ActionReference,
        }

        public enum TriggerOn
        {
            Press,
            Release,
            Held,
        }

        [Tooltip("Key listens directly to a keyboard key - the simplest option, works with no extra setup. Action Reference points at an action from an Input Actions asset, which supports rebinding and other devices like gamepads.")]
        public InputSource source = InputSource.Key;

        [Tooltip("The keyboard key to listen for. Used when Source is Key.")]
        public Key key = Key.E;

        [Tooltip("The Input Action to check. Used when Source is Action Reference.")]
        public InputActionReference actionReference;

        [Tooltip("Press fires the frame it's first pressed. Release fires the frame it's let go. Held is true for as long as it's held down.")]
        public TriggerOn triggerOn = TriggerOn.Press;

        public override void OnAwake()
        {
            if (source == InputSource.ActionReference && actionReference != null && actionReference.action != null)
            {
                actionReference.action.Enable();
            }
        }

        private void OnDisable()
        {
            if (source == InputSource.ActionReference && actionReference != null && actionReference.action != null)
            {
                actionReference.action.Disable();
            }
        }

        public override bool Evaluate()
        {
            return source == InputSource.Key ? EvaluateKey() : EvaluateAction();
        }

        private bool EvaluateKey()
        {
            if (Keyboard.current == null) return false;

            KeyControl control = Keyboard.current[key];

            switch (triggerOn)
            {
                case TriggerOn.Press:
                    return control.wasPressedThisFrame;

                case TriggerOn.Release:
                    return control.wasReleasedThisFrame;

                default:
                    return control.isPressed;
            }
        }

        private bool EvaluateAction()
        {
            if (actionReference == null || actionReference.action == null) return false;

            InputAction action = actionReference.action;

            switch (triggerOn)
            {
                case TriggerOn.Press:
                    return action.WasPerformedThisFrame();

                case TriggerOn.Release:
                    return action.WasReleasedThisFrame();

                default:
                    return action.IsPressed();
            }
        }
    }
}
