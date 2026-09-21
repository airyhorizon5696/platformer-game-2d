using ColliderEventSystem;
using ColliderEventSystem.Editor;
using UnityEditor;
using UnityEngine;

namespace ColliderEventSystem.Editor.Drawers
{
    /// <summary>
    /// Shows the Vector3 field for Position/Rotation/Scale only when its toggle is on AND Value Source is
    /// Fixed Value, Target only when Target Mode is Specific Object, Value Reference only when Value
    /// Source is Reference Transform, and Duration/Ease/Cancel Physics/Custom Curve (indented) only when
    /// Animate is checked (Custom Curve further gated on Ease being Custom). Target Mode itself only shows
    /// when the owner has a ColliderEvent - Condition Watcher has no entering object to offer, so it's
    /// locked to Specific Object instead of just warning about the mismatch after the fact.
    /// </summary>
    public static class TransformActionDrawer
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

            DrawSectionLabel(cursor.NextLine(), "Target", draw);
            EditorGUI.indentLevel++;

            SerializedProperty targetModeProp = so.FindProperty("targetMode");
            bool supportsEnteringObjects = TargetModeGui.SupportsEnteringObjects(so);

            if (supportsEnteringObjects)
            {
                DrawField(cursor.NextField(targetModeProp), targetModeProp, draw);
            }
            else
            {
                targetModeProp.enumValueIndex = (int)TargetMode.SpecificObject;
            }

            if (!supportsEnteringObjects || (TargetMode)targetModeProp.enumValueIndex == TargetMode.SpecificObject)
            {
                SerializedProperty targetProp = so.FindProperty("target");
                DrawField(cursor.NextField(targetProp), targetProp, draw);
            }

            EditorGUI.indentLevel--;
            cursor.Spacer();
            DrawSectionLabel(cursor.NextLine(), "Effect", draw);
            EditorGUI.indentLevel++;

            SerializedProperty valueSourceProp = so.FindProperty("valueSource");
            bool useReference = (TransformAction.ValueSource)valueSourceProp.enumValueIndex == TransformAction.ValueSource.ReferenceTransform;

            DrawField(cursor.NextField(valueSourceProp), valueSourceProp, draw);

            if (useReference)
            {
                SerializedProperty valueReferenceProp = so.FindProperty("valueReference");
                DrawField(cursor.NextField(valueReferenceProp), valueReferenceProp, draw);
            }

            DrawToggleField(so, "modifyPosition", "position", "Position", ref cursor, draw, !useReference);
            DrawToggleField(so, "modifyRotation", "rotation", "Rotation", ref cursor, draw, !useReference);
            DrawToggleField(so, "modifyScale", "scale", "Scale", ref cursor, draw, !useReference);

            SerializedProperty valueModeProp = so.FindProperty("valueMode");
            DrawField(cursor.NextField(valueModeProp), valueModeProp, draw);

            SerializedProperty spaceProp = so.FindProperty("space");
            Rect spaceRect = cursor.NextField(spaceProp);
            if (draw) EditorGUI.PropertyField(spaceRect, spaceProp, new GUIContent("Relative To", "Applies to Position and Rotation. Scale is always Local."));

            SerializedProperty animateProp = so.FindProperty("animate");
            Rect animateRect = cursor.NextField(animateProp);
            if (draw) EditorGUI.PropertyField(animateRect, animateProp, new GUIContent("Animate"));

            if (animateProp.boolValue)
            {
                EditorGUI.indentLevel++;

                SerializedProperty durationProp = so.FindProperty("duration");
                DrawField(cursor.NextField(durationProp), durationProp, draw);

                SerializedProperty easeProp = so.FindProperty("ease");
                DrawField(cursor.NextField(easeProp), easeProp, draw);

                if ((Ease)easeProp.enumValueIndex == Ease.Custom)
                {
                    SerializedProperty curveProp = so.FindProperty("customCurve");
                    DrawField(cursor.NextField(curveProp), curveProp, draw);
                }

                SerializedProperty cancelPhysicsProp = so.FindProperty("cancelPhysicsWhileMoving");
                Rect cancelPhysicsRect = cursor.NextField(cancelPhysicsProp);
                if (draw) EditorGUI.PropertyField(cancelPhysicsRect, cancelPhysicsProp, new GUIContent("Cancel Physics"));

                EditorGUI.indentLevel--;
            }

            EditorGUI.indentLevel--;

            return cursor.ConsumedHeight;
        }

        private static void DrawToggleField(SerializedObject so, string toggleName, string valueName, string label, ref RectLayoutCursor cursor, bool draw, bool showValue)
        {
            SerializedProperty toggleProp = so.FindProperty(toggleName);
            Rect toggleRect = cursor.NextField(toggleProp);
            if (draw) EditorGUI.PropertyField(toggleRect, toggleProp, new GUIContent(label));

            if (toggleProp.boolValue && showValue)
            {
                SerializedProperty valueProp = so.FindProperty(valueName);
                Rect valueRect = cursor.NextField(valueProp);
                if (draw) EditorGUI.PropertyField(valueRect, valueProp, GUIContent.none);
            }
        }

        private static void DrawField(Rect rect, SerializedProperty property, bool draw)
        {
            if (draw) EditorGUI.PropertyField(rect, property);
        }

        private static void DrawSectionLabel(Rect rect, string text, bool draw)
        {
            if (draw) EditorGUI.LabelField(rect, text, EditorStyles.boldLabel);
        }
    }
}
