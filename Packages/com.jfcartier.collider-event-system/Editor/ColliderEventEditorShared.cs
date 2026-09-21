using ColliderEventSystem;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace ColliderEventSystem.Editor
{
    /// <summary>
    /// Field-drawing code shared by ColliderEventEditor and ConditionWatcherEditor.
    /// </summary>
    public static class ColliderEventEditorShared
    {
        public static void DrawCommonFields(SerializedProperty holdTime, SerializedProperty afterTrigger)
        {
            EditorGUILayout.PropertyField(holdTime, new GUIContent("Hold Time", holdTime.tooltip));
            EditorGUILayout.PropertyField(afterTrigger);
        }

        public static void DrawDebugLogging(SerializedProperty debugLogging)
        {
            EditorGUILayout.PropertyField(debugLogging);
        }

        /// <summary>
        /// Target Mode only shows when the owner is a Collider Event (Condition Watcher has no entering
        /// object, so it's locked to Specific Object). Target/Material only show once Show Hint Material
        /// is on.
        /// </summary>
        public static void DrawHintMaterial(SerializedObject so, SerializedProperty showHintMaterial, SerializedProperty hintTargetMode, SerializedProperty hintRenderer, SerializedProperty hintMaterial)
        {
            EditorGUILayout.PropertyField(showHintMaterial, new GUIContent("Hint Material"));
            if (!showHintMaterial.boolValue) return;

            EditorGUI.indentLevel++;

            bool supportsEnteringObjects = TargetModeGui.SupportsEnteringObjects(so);

            if (supportsEnteringObjects)
            {
                EditorGUILayout.PropertyField(hintTargetMode, new GUIContent("Target Mode"));
            }
            else
            {
                hintTargetMode.enumValueIndex = (int)TargetMode.SpecificObject;
            }

            if (!supportsEnteringObjects || (TargetMode)hintTargetMode.enumValueIndex == TargetMode.SpecificObject)
            {
                EditorGUILayout.PropertyField(hintRenderer, new GUIContent("Target"));
            }

            EditorGUILayout.PropertyField(hintMaterial, new GUIContent("Material"));

            EditorGUI.indentLevel--;
        }

        /// <summary>
        /// Draws lists already built with ConditionActionListDrawer.Build() in the owning Editor's OnEnable().
        /// </summary>
        public static void DrawLists(ReorderableList conditionsList, ReorderableList actionsList, ReorderableList exitActionsList, SerializedProperty afterTrigger)
        {
            EditorGUILayout.Space(8f);
            conditionsList.DoLayoutList();

            EditorGUILayout.Space(8f);
            actionsList.DoLayoutList();

            if ((AfterTrigger)afterTrigger.enumValueIndex == AfterTrigger.ExecuteExitActions)
            {
                EditorGUILayout.Space(8f);
                exitActionsList.DoLayoutList();
            }
        }

        /// <summary>
        /// Warns when a listed Condition/Action needs the colliding GameObject, but the owner has no way
        /// to provide one (Condition Watcher has no zone/Collider).
        /// </summary>
        public static void DrawMissingCollisionDataWarning(SerializedProperty conditions, SerializedProperty actions, SerializedProperty exitActions)
        {
            if (AnyRequiresCollisionObjectData(conditions) || AnyRequiresCollisionObjectData(actions) || AnyRequiresCollisionObjectData(exitActions))
            {
                EditorGUILayout.HelpBox(
                    "One or more Conditions/Actions here need the GameObject that entered a zone, but Condition Watcher isn't tied to a zone - they'll receive no object.",
                    MessageType.Warning);
            }
        }

        private static bool AnyRequiresCollisionObjectData(SerializedProperty listProperty)
        {
            if (listProperty == null) return false;

            for (int i = 0; i < listProperty.arraySize; i++)
            {
                Object element = listProperty.GetArrayElementAtIndex(i).objectReferenceValue;

                if (element is ConditionBase condition && condition.RequiresCollisionObjectData) return true;
                if (element is ActionBase action && action.RequiresCollisionObjectData) return true;
            }

            return false;
        }
    }
}
