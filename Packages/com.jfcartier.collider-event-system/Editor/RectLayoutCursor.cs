using UnityEditor;
using UnityEngine;

namespace ColliderEventSystem.Editor
{
    /// <summary>
    /// Stacks fields vertically inside a fixed Rect without GUILayout. GUILayout.BeginArea nested inside
    /// a ReorderableList element callback does not render reliably inside Unity's UIElements-hosted
    /// Inspector, so every ElementDrawerRegistry entry draws with this instead.
    /// </summary>
    public struct RectLayoutCursor
    {
        private readonly Rect m_Bounds;
        private float m_Y;

        public RectLayoutCursor(Rect bounds)
        {
            m_Bounds = bounds;
            m_Y = bounds.y;
        }

        public float ConsumedHeight => m_Y - m_Bounds.y;

        public Rect NextLine(float height)
        {
            Rect line = new Rect(m_Bounds.x, m_Y, m_Bounds.width, height);
            m_Y += height + 2f;
            return line;
        }

        public Rect NextLine()
        {
            return NextLine(EditorGUIUtility.singleLineHeight);
        }

        public Rect NextField(SerializedProperty property)
        {
            return NextLine(EditorGUI.GetPropertyHeight(property, true));
        }

        /// <summary>Adds blank vertical space, e.g. to separate a "what this acts on" field group from
        /// the "what it does" fields that follow, without an extra label.</summary>
        public void Spacer(float height = 6f)
        {
            m_Y += height;
        }
    }
}
