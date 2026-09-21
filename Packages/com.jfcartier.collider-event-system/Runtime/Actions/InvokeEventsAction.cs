using UnityEngine;
using UnityEngine.Events;

namespace ColliderEventSystem
{
    /// <summary>
    /// Calls one or more functions, using Unity's native Event system (the same one behind a Button's
    /// On Click) - drag a GameObject, pick a component, pick a method from the dropdown, done.
    /// </summary>
    public sealed class InvokeEventsAction : ActionBase
    {
        [Tooltip("Called when this Action runs. Supports one static parameter (int/float/string/bool/Object) if the chosen method takes one.")]
        public UnityEvent Events = new();

        public override void Execute()
        {
            Events.Invoke();
        }
    }
}
