using ColliderEventSystem;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace ColliderEventSystem.Editor
{
    [CustomEditor(typeof(ConditionWatcher))]
    public sealed class ConditionWatcherEditor : UnityEditor.Editor
    {
        private SerializedProperty m_HoldTime;
        private SerializedProperty m_AfterTrigger;
        private SerializedProperty m_DebugLogging;
        private SerializedProperty m_ShowHintMaterial;
        private SerializedProperty m_HintTargetMode;
        private SerializedProperty m_HintRenderer;
        private SerializedProperty m_HintMaterial;
        private SerializedProperty m_Conditions;
        private SerializedProperty m_Actions;
        private SerializedProperty m_ExitActions;

        private ReorderableList m_ConditionsList;
        private ReorderableList m_ActionsList;
        private ReorderableList m_ExitActionsList;

        private void OnEnable()
        {
            m_HoldTime = serializedObject.FindProperty("holdTime");
            m_AfterTrigger = serializedObject.FindProperty("afterTrigger");
            m_DebugLogging = serializedObject.FindProperty("debugLogging");
            m_ShowHintMaterial = serializedObject.FindProperty("showHintMaterial");
            m_HintTargetMode = serializedObject.FindProperty("hintTargetMode");
            m_HintRenderer = serializedObject.FindProperty("hintRenderer");
            m_HintMaterial = serializedObject.FindProperty("hintMaterial");
            m_Conditions = serializedObject.FindProperty("conditions");
            m_Actions = serializedObject.FindProperty("actions");
            m_ExitActions = serializedObject.FindProperty("exitActions");

            GameObject owner = ((ConditionWatcher)target).gameObject;
            m_ConditionsList = ConditionActionListDrawer.Build(m_Conditions, typeof(ConditionBase), owner);
            m_ActionsList = ConditionActionListDrawer.Build(m_Actions, typeof(ActionBase), owner);
            m_ExitActionsList = ConditionActionListDrawer.Build(m_ExitActions, typeof(ActionBase), owner);
        }

        public override void OnInspectorGUI()
        {
            // See ColliderEventEditor.OnInspectorGUI - same guard against redrawing a destroyed target.
            if (target == null) return;

            serializedObject.Update();

            ColliderEventEditorShared.DrawDebugLogging(m_DebugLogging);
            ColliderEventEditorShared.DrawCommonFields(m_HoldTime, m_AfterTrigger);
            ColliderEventEditorShared.DrawHintMaterial(serializedObject, m_ShowHintMaterial, m_HintTargetMode, m_HintRenderer, m_HintMaterial);
            ColliderEventEditorShared.DrawMissingCollisionDataWarning(m_Conditions, m_Actions, m_ExitActions);
            ColliderEventEditorShared.DrawLists(m_ConditionsList, m_ActionsList, m_ExitActionsList, m_AfterTrigger);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
