using ColliderEventSystem;
using UnityEditor;
using UnityEngine;

namespace ColliderEventSystem.Editor.Drawers
{
    /// <summary>
    /// Clearer labels than the raw field names, and hides Obstacle Layers unless Check For Obstacles is on.
    /// </summary>
    public static class LookingAtConditionDrawer
    {
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
            var cursor = new RectLayoutCursor(rect);

            SerializedProperty targetProp = so.FindProperty("target");
            Rect targetRect = cursor.NextField(targetProp);
            if (draw) EditorGUI.PropertyField(targetRect, targetProp, new GUIContent("Object", "The object to check."));

            SerializedProperty cameraProp = so.FindProperty("targetCamera");
            Rect cameraRect = cursor.NextField(cameraProp);
            if (draw) EditorGUI.PropertyField(cameraRect, cameraProp, new GUIContent("Camera", "Uses the Main Camera if left empty."));

            SerializedProperty visibilityProp = so.FindProperty("visibility");
            Rect visibilityRect = cursor.NextField(visibilityProp);
            if (draw) EditorGUI.PropertyField(visibilityRect, visibilityProp, new GUIContent("Condition"));

            SerializedProperty checkObstructionProp = so.FindProperty("checkObstruction");
            Rect checkObstructionRect = cursor.NextField(checkObstructionProp);
            if (draw) EditorGUI.PropertyField(checkObstructionRect, checkObstructionProp, new GUIContent("Check For Obstacles"));

            if (checkObstructionProp.boolValue)
            {
                SerializedProperty maskProp = so.FindProperty("obstructionMask");
                Rect maskRect = cursor.NextField(maskProp);
                if (draw) EditorGUI.PropertyField(maskRect, maskProp, new GUIContent("Obstacle Layers"));
            }

            return cursor.ConsumedHeight;
        }
    }
}
