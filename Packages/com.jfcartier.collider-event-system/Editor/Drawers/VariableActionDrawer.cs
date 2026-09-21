using ColliderEventSystem;
using ColliderEventSystem.Editor;
using UnityEditor;
using UnityEngine;

namespace ColliderEventSystem.Editor.Drawers
{
    /// <summary>
    /// Shows only the value field matching the referenced Variable's type, and only shows the
    /// Set/Additive mode for Float/Int Variables (Additive is meaningless for Bool/String).
    /// </summary>
    public static class VariableActionDrawer
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

            Variable variable = variableProp.objectReferenceValue as Variable;

            if (SupportsAdditive(variable))
            {
                SerializedProperty modeProp = so.FindProperty("valueMode");
                Rect modeRect = cursor.NextField(modeProp);
                if (draw) EditorGUI.PropertyField(modeRect, modeProp);
            }

            SerializedProperty valueProp = GetValueProperty(so, variable);

            if (valueProp != null)
            {
                Rect valueRect = cursor.NextField(valueProp);
                if (draw) EditorGUI.PropertyField(valueRect, valueProp, new GUIContent("Value"));
            }

            return cursor.ConsumedHeight;
        }

        private static bool SupportsAdditive(Variable variable)
        {
            return variable is FloatVariable || variable is IntVariable;
        }

        private static SerializedProperty GetValueProperty(SerializedObject so, Variable variable)
        {
            switch (variable)
            {
                case FloatVariable _: return so.FindProperty("floatValue");
                case IntVariable _: return so.FindProperty("intValue");
                case BoolVariable _: return so.FindProperty("boolValue");
                case StringVariable _: return so.FindProperty("stringValue");
                default: return null;
            }
        }
    }
}
