using UnityEngine;

namespace ColliderEventSystem
{
    /// <summary>
    /// Replaces every material slot on a Renderer with a different Material, or restores whatever was
    /// there before - e.g. a highlight applied on enter and restored on exit, without having to know or
    /// re-specify the original material.
    /// </summary>
    public sealed class MaterialAction : ActionBase
    {
        public enum Mode
        {
            Apply,
            RestoreOriginal,
        }

        public TargetMode targetMode = TargetMode.SpecificObject;

        [Tooltip("The Renderer to modify. Used when Target Mode is Specific Object.")]
        public Renderer targetRenderer;

        public Mode mode = Mode.Apply;

        [Tooltip("Applied to every material slot on the Renderer. Used when Mode is Apply.")]
        public Material newMaterial;

        public override bool RequiresCollisionObjectData => targetMode == TargetMode.EnteringObjects;

        public override void Execute()
        {
            Apply(targetRenderer);
        }

        public override void Execute(GameObject collidingObject)
        {
            Apply(collidingObject != null ? collidingObject.GetComponent<Renderer>() : null);
        }

        private void Apply(Renderer renderer)
        {
            if (renderer == null) return;

            if (mode == Mode.RestoreOriginal)
            {
                RendererMaterialOverride.Restore(renderer);
            }
            else
            {
                RendererMaterialOverride.Apply(renderer, newMaterial);
            }
        }
    }
}
