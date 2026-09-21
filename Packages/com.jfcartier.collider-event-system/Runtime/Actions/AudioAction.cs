using UnityEngine;

namespace ColliderEventSystem
{
    /// <summary>
    /// Plays or stops an AudioSource, using whatever clip/volume/loop it's already configured with.
    /// </summary>
    public sealed class AudioAction : ActionBase
    {
        public enum Operation
        {
            Play,
            Stop,
        }

        [Tooltip("The AudioSource to play/stop. Leave empty to use an AudioSource on this GameObject, if there is one.")]
        public AudioSource target;

        public Operation operation = Operation.Play;

        public override void Execute()
        {
            AudioSource source = target != null ? target : GetComponent<AudioSource>();
            if (source == null) return;

            if (operation == Operation.Play)
            {
                source.Play();
            }
            else
            {
                source.Stop();
            }
        }
    }
}
