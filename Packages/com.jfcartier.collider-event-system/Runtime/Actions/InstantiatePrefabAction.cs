using UnityEngine;

namespace ColliderEventSystem
{
    /// <summary>
    /// Instantiates a prefab.
    /// </summary>
    public sealed class InstantiatePrefabAction : ActionBase
    {
        public TargetMode targetMode = TargetMode.SpecificObject;

        [Tooltip("Where to spawn it. Used when Target Mode is Specific Object. Leave empty to spawn at this GameObject's position/rotation.")]
        public Transform destination;

        [Tooltip("The prefab to instantiate.")]
        public GameObject prefab;

        public override bool RequiresCollisionObjectData => targetMode == TargetMode.EnteringObjects;

        public override void Execute()
        {
            Spawn(destination != null ? destination : transform);
        }

        public override void Execute(GameObject collidingObject)
        {
            Spawn(collidingObject != null ? collidingObject.transform : transform);
        }

        private void Spawn(Transform spawnPoint)
        {
            if (prefab == null) return;

            Instantiate(prefab, spawnPoint.position, spawnPoint.rotation);
        }
    }
}
