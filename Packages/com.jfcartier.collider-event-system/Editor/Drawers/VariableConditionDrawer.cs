using ColliderEventSystem;
using ColliderEventSystem.Editor;
using UnityEditor;
using UnityEngine;

namespace ColliderEventSystem.Editor.Drawers
{
    /// <summary>
    /// Hides Operator/Threshold until a Variable is picked (their meaning depends on its type). Bool gets
    /// a plain Value checkbox and String gets an Equals/Not Equals choice instead of Operator - Greater
    /// Than/Less Than doesn't mean anything useful for either.
    /// </summary>
    public static class VariableConditionDrawer
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

            SerializedProperty variableProp = so.FindProperty("targetVariable");
            Rect variableRect = cursor.NextField(variableProp);
            VariableCreationButton.DrawFieldWithNewButton(variableRect, variableProp, draw);

            switch (variableProp.objectReferenceValue)
            {
                case BoolVariable _:
                    SerializedProperty boolProp = so.FindProperty("boolThreshold");
                    Rect boolRect = cursor.NextField(boolProp);
                    if (draw) EditorGUI.PropertyField(boolRect, boolProp, new GUIContent("Value", "The condition is met when the variable equals this."));
                    break;

                case StringVariable _:
                    SerializedProperty textOperatorProp = so.FindProperty("textOperator");
                    Rect textOperatorRect = cursor.NextField(textOperatorProp);
                    if (draw) EditorGUI.PropertyField(textOperatorRect, textOperatorProp, new GUIContent("Comparison"));

                    SerializedProperty stringProp = so.FindProperty("stringThreshold");
                    Rect stringRect = cursor.NextField(stringProp);
                    if (draw) EditorGUI.PropertyField(stringRect, stringProp, new GUIContent("Value"));
                    break;

                case FloatVariable _:
                case IntVariable _:
                    DrawOperatorAndThreshold(so, ref cursor, draw);
                    break;

                case null:
                    break;

                default:
                    Rect warningRect = cursor.NextLine(EditorGUIUtility.singleLineHeight * 2f);
                    if (draw) EditorGUI.HelpBox(warningRect, "Unrecognized Variable type.", MessageType.Warning);
                    break;
            }

            return cursor.ConsumedHeight;
        }

        private static void DrawOperatorAndThreshold(SerializedObject so, ref RectLayoutCursor cursor, bool draw)
        {
            SerializedProperty operatorProp = so.FindProperty("operatorValue");
            Rect operatorRect = cursor.NextField(operatorProp);
            if (draw) EditorGUI.PropertyField(operatorRect, operatorProp);

            SerializedProperty thresholdProp = GetThresholdProperty(so);
            Rect thresholdRect = cursor.NextField(thresholdProp);
            if (draw) EditorGUI.PropertyField(thresholdRect, thresholdProp, new GUIContent("Threshold"));
        }

        private static SerializedProperty GetThresholdProperty(SerializedObject so)
        {
            return so.FindProperty("targetVariable").objectReferenceValue is FloatVariable
                ? so.FindProperty("floatThreshold")
                : so.FindProperty("intThreshold");
        }
    }
}
