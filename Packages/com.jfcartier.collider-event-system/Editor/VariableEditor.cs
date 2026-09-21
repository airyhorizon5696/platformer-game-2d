using ColliderEventSystem;
using UnityEditor;
using UnityEngine;

namespace ColliderEventSystem.Editor
{
    [CustomEditor(typeof(Variable), true)]
    public sealed class VariableEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            var variable = (Variable)target;

            using (new EditorGUI.DisabledScope(true))
            {
                EditorGUILayout.TextField("Id", variable.Id);
            }

            SerializedProperty persistentProp = serializedObject.FindProperty("Persistent");
            EditorGUILayout.PropertyField(persistentProp);

            SerializedProperty initialValueProp = serializedObject.FindProperty("m_InitialValue");
            if (initialValueProp != null)
            {
                EditorGUILayout.PropertyField(initialValueProp, new GUIContent("Initial Value"));
            }

            serializedObject.ApplyModifiedProperties();

            if (Application.isPlaying)
            {
                EditorGUILayout.Space(6f);
                EditorGUILayout.LabelField("Runtime Value (Play Mode)", EditorStyles.boldLabel);
                DrawRuntimeValueField(variable);
            }

            EditorGUILayout.Space(6f);
            if (GUILayout.Button("Clear Persisted Value"))
            {
                variable.ClearPersisted();
            }
        }

        public override bool RequiresConstantRepaint()
        {
            return Application.isPlaying;
        }

        private static void DrawRuntimeValueField(Variable variable)
        {
            switch (variable)
            {
                case FloatVariable f:
                    f.RuntimeValue = EditorGUILayout.FloatField("Runtime Value", f.RuntimeValue);
                    break;

                case IntVariable i:
                    i.RuntimeValue = EditorGUILayout.IntField("Runtime Value", i.RuntimeValue);
                    break;

                case BoolVariable b:
                    b.RuntimeValue = EditorGUILayout.Toggle("Runtime Value", b.RuntimeValue);
                    break;

                case StringVariable s:
                    s.RuntimeValue = EditorGUILayout.TextField("Runtime Value", s.RuntimeValue);
                    break;
            }
        }
    }
}
