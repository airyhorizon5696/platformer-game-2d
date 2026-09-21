using UnityEngine;

namespace ColliderEventSystem
{
    /// <summary>
    /// True when Object is (or isn't) visible on Camera's screen.
    /// </summary>
    public sealed class LookingAtCondition : ConditionBase
    {
        public enum Visibility
        {
            Visible,
            NotVisible,
        }

        [Tooltip("Uses the Main Camera if left empty.")]
        public Camera targetCamera;

        [Tooltip("The object to check.")]
        public GameObject target;

        public Visibility visibility = Visibility.Visible;

        [Tooltip("If on, something between the camera and the object (like a wall) blocks it from counting as visible, even if it would otherwise be on-screen.")]
        public bool checkObstruction = true;

        [Tooltip("Which layers can block the view. Only used when Check For Obstacles is on.")]
        public LayerMask obstructionMask = ~0;

        public override bool Evaluate()
        {
            Camera cam = targetCamera != null ? targetCamera : Camera.main;
            if (cam == null || target == null) return false;

            bool visible = IsInView(cam) && (!checkObstruction || !IsObstructed(cam));

            return visibility == Visibility.Visible ? visible : !visible;
        }

        private bool IsInView(Camera cam)
        {
            Vector3 viewportPoint = cam.WorldToViewportPoint(target.transform.position);

            return viewportPoint.z > 0f
                && viewportPoint.x >= 0f && viewportPoint.x <= 1f
                && viewportPoint.y >= 0f && viewportPoint.y <= 1f;
        }

        private bool IsObstructed(Camera cam)
        {
            Vector3 origin = cam.transform.position;
            Vector3 offset = target.transform.position - origin;
            float distance = offset.magnitude;

            if (distance <= 0f) return false;

            if (Physics.Raycast(origin, offset / distance, out RaycastHit hit, distance, obstructionMask))
            {
                return hit.transform != target.transform && !hit.transform.IsChildOf(target.transform);
            }

            return false;
        }
    }
}
