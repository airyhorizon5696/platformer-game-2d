using UnityEngine;

namespace ColliderEventSystem
{
    /// <summary>
    /// Changes one or more properties on a Rigidbody. Mass/Linear Damping/Angular Damping each have their
    /// own on/off toggle so only the ones you check are touched. Use Gravity and Is Kinematic are always
    /// applied - they're already booleans, so there's nothing a second "should I touch this" toggle adds.
    /// </summary>
    public sealed class RigidbodyAction : ActionBase
    {
        public TargetMode targetMode = TargetMode.SpecificObject;

        [Tooltip("The Rigidbody to modify. Used when Target Mode is Specific Object.")]
        public Rigidbody target;

        public bool modifyMass;
        public float mass = 1f;

        public bool modifyLinearDamping;
        public float linearDamping;

        public bool modifyAngularDamping;
        public float angularDamping;

        public bool useGravity = true;

        public bool isKinematic;

        public override bool RequiresCollisionObjectData => targetMode == TargetMode.EnteringObjects;

        public override void Execute()
        {
            Apply(target);
        }

        public override void Execute(GameObject collidingObject)
        {
            Apply(collidingObject != null ? collidingObject.GetComponent<Rigidbody>() : null);
        }

        private void Apply(Rigidbody rb)
        {
            if (rb == null) return;

            if (modifyMass) rb.mass = mass;
            if (modifyLinearDamping) rb.linearDamping = linearDamping;
            if (modifyAngularDamping) rb.angularDamping = angularDamping;
            rb.useGravity = useGravity;
            rb.isKinematic = isKinematic;
        }
    }
}
