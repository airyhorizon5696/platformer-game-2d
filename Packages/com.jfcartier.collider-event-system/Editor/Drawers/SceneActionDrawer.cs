using ColliderEventSystem;
using ColliderEventSystem.Editor;
using UnityEditor;
using UnityEngine;

namespace ColliderEventSystem.Editor.Drawers
{
    /// <summary>
    /// Shows Additive only when Operation is Load or Load Async - unloading has no additive/single
    /// equivalent, so the field is meaningless (and misleading) while Operation is Unload.
    /// </summary>
    public static class SceneActionDrawer
    {
        private static readonly string[] SkipNames = { "operation", "sceneAsset", "additive" };

        public static void Draw(Rect rect, SerializedObject so)
        {
            Layout(rect, so, true);
        }

        public static float GetHeight(SerializedObject so)
        {
            return Layout(default, so, false);
        }

        private static float Layout(Rect rect, SerializedObject so, bool draw)
        {
            float lineHeight = EditorGUIUtility.singleLineHeight;
            float y = rect.y;

            SerializedProperty operationProp = so.FindProperty("operation");
            if (draw) EditorGUI.PropertyField(new Rect(rect.x, y, rect.width, lineHeight), operationProp);
            y += lineHeight + 2f;

            SerializedProperty sceneAssetProp = so.FindProperty("sceneAsset");
            if (draw) EditorGUI.PropertyField(new Rect(rect.x, y, rect.width, lineHeight), sceneAssetProp);
            y += lineHeight + 2f;

            if ((SceneAction.Operation)operationProp.enumValueIndex != SceneAction.Operation.Unload)
            {
                SerializedProperty additiveProp = so.FindProperty("additive");
                if (draw) EditorGUI.PropertyField(new Rect(rect.x, y, rect.width, lineHeight), additiveProp);
                y += lineHeight + 2f;
            }

            Rect remainingRect = new Rect(rect.x, y, rect.width, rect.height);
            y += draw
                ? ConditionActionListDrawer.DrawRemainingFields(remainingRect, so, SkipNames)
                : ConditionActionListDrawer.GetRemainingFieldsHeight(so, SkipNames);

            return y - rect.y;
        }
    }
}
