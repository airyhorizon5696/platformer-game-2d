using UnityEngine;

namespace ColliderEventSystem
{
    /// <summary>
    /// Enables, disables, or destroys a GameObject.
    /// </summary>
    public sealed class GameObjectAction : ActionBase
    {
        public enum Operation
        {
            Enable,
            Disable,
            Destroy,
        }

        public TargetMode targetMode = TargetMode.SpecificObject;

        [Tooltip("The GameObject to modify. Used when Target Mode is Specific Object.")]
        public GameObject target;

        public Operation operation = Operation.Disable;

        public override bool RequiresCollisionObjectData => targetMode == TargetMode.EnteringObjects;

        public override void Execute()
        {
            Apply(target);
        }

        public override void Execute(GameObject collidingObject)
        {
            Apply(collidingObject);
        }

        private void Apply(GameObject go)
        {
            if (go == null) return;

            switch (operation)
            {
                case Operation.Enable:
                    go.SetActive(true);
                    break;

                case Operation.Disable:
                    go.SetActive(false);
                    break;

                case Operation.Destroy:
                    Destroy(go);
                    break;
            }
        }
    }
}
