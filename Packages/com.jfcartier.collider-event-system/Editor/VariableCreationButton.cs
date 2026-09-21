using ColliderEventSystem;
using UnityEditor;
using UnityEngine;

namespace ColliderEventSystem.Editor
{
    /// <summary>
    /// Draws a Variable object field with a small "+" button glued to its right edge, that creates a
    /// Variable asset on the spot and assigns it, so you don't have to leave the Inspector to create one
    /// via Assets > Create first.
    /// </summary>
    public static class VariableCreationButton
    {
        private const float ButtonWidth = 20f;
        private const float Spacing = 2f;

        public static void DrawFieldWithNewButton(Rect rect, SerializedProperty variableProp, bool draw)
        {
            if (!draw) return;

            Rect fieldRect = new Rect(rect.x, rect.y, rect.width - ButtonWidth - Spacing, rect.height);
            Rect buttonRect = new Rect(rect.x + rect.width - ButtonWidth, rect.y, ButtonWidth, rect.height);

            EditorGUI.PropertyField(fieldRect, variableProp);

            // Same icon ReorderableList uses for an Add button that opens a menu instead of adding directly.
            GUIContent icon = EditorGUIUtility.IconContent("Toolbar Plus More");
            icon.tooltip = "Create a new Variable asset and assign it here.";

            if (GUI.Button(buttonRect, icon, "RL FooterButton"))
            {
                ShowTypeMenu(buttonRect, variableProp);
            }
        }

        private static void ShowTypeMenu(Rect rect, SerializedProperty variableProp)
        {
            var menu = new GenericMenu();
            menu.AddItem(new GUIContent("Float"), false, () => CreateAndAssign<FloatVariable>(variableProp));
            menu.AddItem(new GUIContent("Int"), false, () => CreateAndAssign<IntVariable>(variableProp));
            menu.AddItem(new GUIContent("Bool"), false, () => CreateAndAssign<BoolVariable>(variableProp));
            menu.AddItem(new GUIContent("String"), false, () => CreateAndAssign<StringVariable>(variableProp));
            menu.DropDown(rect);
        }

        private static void CreateAndAssign<T>(SerializedProperty variableProp) where T : Variable
        {
            string typeName = typeof(T).Name.Replace("Variable", "");
            string defaultName = "New " + typeName + " Variable";

            string path = EditorUtility.SaveFilePanelInProject("Create Variable", defaultName, "asset", "Choose where to save the new Variable.");
            if (string.IsNullOrEmpty(path)) return;

            T asset = ScriptableObject.CreateInstance<T>();
            AssetDatabase.CreateAsset(asset, path);
            AssetDatabase.SaveAssets();
            EditorGUIUtility.PingObject(asset);

            variableProp.objectReferenceValue = asset;
            variableProp.serializedObject.ApplyModifiedProperties();
        }
    }
}
