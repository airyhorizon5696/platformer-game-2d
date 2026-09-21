using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace ColliderEventSystem
{
    /// <summary>
    /// Plays or stops an animation on an Animator.
    /// </summary>
    public sealed class AnimationAction : ActionBase
    {
        public TargetMode targetMode = TargetMode.SpecificObject;

        [Tooltip("The Animator to control. Used when Target Mode is Specific Object.")]
        public Animator target;

        [Tooltip("If true, stops the Animator instead of playing anything.")]
        public bool stopAnimation;

        [Tooltip("Plays this clip directly, regardless of what's set up in the Animator Controller. Takes priority over Trigger Name if both are set.")]
        public AnimationClip clipToPlay;

        [Tooltip("Sets this Trigger parameter on the Animator Controller. Requires a transition already set up to use it. Ignored if Clip To Play is set.")]
        public string triggerName;

        public override bool RequiresCollisionObjectData => targetMode == TargetMode.EnteringObjects;

        private PlayableGraph m_Graph;
        private Animator m_ControllerOwner;
        private RuntimeAnimatorController m_SavedController;

        public override void Execute()
        {
            Apply(target);
        }

        public override void Execute(GameObject collidingObject)
        {
            Apply(collidingObject != null ? collidingObject.GetComponent<Animator>() : null);
        }

        public override void CancelExecution()
        {
            DestroyGraph();
        }

        private void Apply(Animator animator)
        {
            if (animator == null) return;

            if (stopAnimation)
            {
                DestroyGraph();
                animator.enabled = false;
                return;
            }

            animator.enabled = true;

            if (clipToPlay != null)
            {
                PlayClipDirectly(animator, clipToPlay);
            }
            else if (!string.IsNullOrEmpty(triggerName))
            {
                DestroyGraph();
                animator.SetTrigger(triggerName);
            }
        }

        // Animator.Play(string) looks up a *state* in the Animator Controller by name, which only works
        // if that state happens to be named exactly like the clip - easy to break and fails silently.
        // Playing through a PlayableGraph instead plays the clip itself, with no naming requirement.
        private void PlayClipDirectly(Animator animator, AnimationClip clip)
        {
            DestroyGraph();

            // If a Controller is still assigned, Unity keeps driving the Animator with it in parallel
            // with our graph, so both end up visibly blending together. Detach it while our clip plays,
            // and hand it back in DestroyGraph() once we're done (Stop, a Trigger, or getting interrupted).
            m_ControllerOwner = animator;
            m_SavedController = animator.runtimeAnimatorController;
            animator.runtimeAnimatorController = null;

            m_Graph = PlayableGraph.Create(name + " (Animation Action)");
            AnimationPlayableOutput output = AnimationPlayableOutput.Create(m_Graph, "Output", animator);
            AnimationClipPlayable clipPlayable = AnimationClipPlayable.Create(m_Graph, clip);
            output.SetSourcePlayable(clipPlayable);
            m_Graph.Play();
        }

        private void DestroyGraph()
        {
            if (m_Graph.IsValid())
            {
                m_Graph.Destroy();
            }

            if (m_ControllerOwner != null)
            {
                m_ControllerOwner.runtimeAnimatorController = m_SavedController;
                m_ControllerOwner = null;
                m_SavedController = null;
            }
        }

        private void OnDisable()
        {
            DestroyGraph();
        }
    }
}
