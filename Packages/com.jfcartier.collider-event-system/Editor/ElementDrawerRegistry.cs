using System;
using System.Collections.Generic;
using ColliderEventSystem;
using ColliderEventSystem.Editor.Drawers;
using UnityEditor;
using UnityEngine;

namespace ColliderEventSystem.Editor
{
    /// <summary>
    /// Maps a Condition/Action type to a custom drawer, for the few types whose fields need conditional
    /// show/hide logic beyond a flat property list (e.g. a field that only appears for one enum value).
    /// Types not registered here fall back to ConditionActionListDrawer's generic SerializedProperty loop.
    /// </summary>
    public static class ElementDrawerRegistry
    {
        public struct Entry
        {
            /// <summary>Draws at the given Rect's position/width using EditorGUI (not EditorGUILayout).</summary>
            public Action<Rect, SerializedObject> Draw;

            /// <summary>Returns the total height Draw() will consume, without drawing anything.</summary>
            public Func<SerializedObject, float> GetHeight;
        }

        private static readonly Dictionary<Type, Entry> Drawers = new Dictionary<Type, Entry>();

        static ElementDrawerRegistry()
        {
            Register(typeof(AnimationAction), AnimationActionDrawer.Draw, AnimationActionDrawer.GetHeight);
            Register(typeof(VariableCondition), VariableConditionDrawer.Draw, VariableConditionDrawer.GetHeight);
            Register(typeof(MaterialAction), MaterialActionDrawer.Draw, MaterialActionDrawer.GetHeight);
            Register(typeof(GameObjectAction), GameObjectActionDrawer.Draw, GameObjectActionDrawer.GetHeight);
            Register(typeof(SceneAction), SceneActionDrawer.Draw, SceneActionDrawer.GetHeight);
            Register(typeof(RigidbodyAction), RigidbodyActionDrawer.Draw, RigidbodyActionDrawer.GetHeight);
            Register(typeof(VariableAction), VariableActionDrawer.Draw, VariableActionDrawer.GetHeight);
            Register(typeof(TransformAction), TransformActionDrawer.Draw, TransformActionDrawer.GetHeight);
            Register(typeof(InputCondition), InputConditionDrawer.Draw, InputConditionDrawer.GetHeight);
            Register(typeof(LookingAtCondition), LookingAtConditionDrawer.Draw, LookingAtConditionDrawer.GetHeight);
            Register(typeof(InstantiatePrefabAction), InstantiatePrefabActionDrawer.Draw, InstantiatePrefabActionDrawer.GetHeight);
            Register(typeof(RotationCondition), RotationConditionDrawer.Draw, RotationConditionDrawer.GetHeight);
        }

        public static void Register(Type type, Action<Rect, SerializedObject> draw, Func<SerializedObject, float> getHeight)
        {
            Drawers[type] = new Entry { Draw = draw, GetHeight = getHeight };
        }

        public static bool TryGetDrawer(Type type, out Entry entry)
        {
            return Drawers.TryGetValue(type, out entry);
        }
    }
}
