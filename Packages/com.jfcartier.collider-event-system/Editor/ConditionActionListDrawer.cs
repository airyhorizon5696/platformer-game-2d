using System;
using System.Collections.Generic;
using ColliderEventSystem;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace ColliderEventSystem.Editor
{
    /// <summary>
    /// Draws a Condition or Action list as a ReorderableList: drag to reorder, native +/- buttons, an
    /// "Add" dropdown populated from ComponentTypeCache, and a foldout per element with either a
    /// registered custom drawer (ElementDrawerRegistry) or a generic SerializedProperty field loop.
    /// This replaces reflection-based field drawing entirely.
    /// </summary>
    public static class ConditionActionListDrawer
    {
        private static readonly Dictionary<EntityId, bool> Expanded = new Dictionary<EntityId, bool>();

        /// <summary>
        /// Builds a ReorderableList for a Conditions/Actions list. Call this once, from the owning Editor's
        /// OnEnable(), and keep the result in an instance field - its lifetime then matches the Editor's own
        /// SerializedObject lifetime. Do NOT cache this across Editor instances (e.g. in a static field):
        /// Unity can recreate the Editor/SerializedObject for the same target (selection churn, Undo,
        /// recompiles), and a longer-lived cache would keep calling back into a disposed SerializedProperty.
        /// </summary>
        public static ReorderableList Build(SerializedProperty listProperty, Type baseType, GameObject ownerGameObject)
        {
            bool isConditionList = baseType == typeof(ConditionBase);

            ReapplyHideFlags(listProperty);

            var list = new ReorderableList(listProperty.serializedObject, listProperty, true, true, true, true);

            list.drawHeaderCallback = rect => EditorGUI.LabelField(rect, listProperty.displayName);
            list.elementHeightCallback = index => GetElementHeight(listProperty, index, isConditionList);
            list.drawElementCallback = (rect, index, active, focused) => DrawElement(rect, listProperty, index, isConditionList);
            list.onAddDropdownCallback = (buttonRect, reorderableList) => ShowAddMenu(buttonRect, listProperty, baseType, ownerGameObject);
            list.onRemoveCallback = reorderableList => RemoveElement(listProperty, reorderableList.index);

            return list;
        }

        /// <summary>
        /// AddElement() sets HideFlags.HideInInspector once, when a Condition/Action is first added, so it
        /// only ever shows up via this drawer's own inline UI - never as its own "(Script)" block. Some
        /// Prefab operations (apply/revert overrides, prefab creation from an object that already has
        /// hidden components) can silently drop that flag on the resulting instance. Reapply it here, on
        /// every Editor.OnEnable, so a stray duplicate block can't stick around.
        /// </summary>
        private static void ReapplyHideFlags(SerializedProperty listProperty)
        {
            for (int i = 0; i < listProperty.arraySize; i++)
            {
                UnityEngine.Object element = listProperty.GetArrayElementAtIndex(i).objectReferenceValue;
                if (element == null || element.hideFlags == HideFlags.HideInInspector) continue;

                element.hideFlags = HideFlags.HideInInspector;
                EditorUtility.SetDirty(element);
            }
        }

        private static float GetElementHeight(SerializedProperty listProperty, int index, bool isConditionList)
        {
            float lineHeight = EditorGUIUtility.singleLineHeight;
            UnityEngine.Object element = listProperty.GetArrayElementAtIndex(index).objectReferenceValue;

            if (element == null) return lineHeight + 4f;

            float height = lineHeight + 6f; // header row

            if (isConditionList && index > 0) height += lineHeight + 2f; // And/Or row

            if (IsExpanded(element))
            {
                var elementSo = new SerializedObject(element);
                height += GetContentHeight(elementSo, element.GetType());

                if (TryGetWarning(element, out _))
                {
                    height += lineHeight * 2f + 4f;
                }
            }

            return height + 4f;
        }

        private static float GetContentHeight(SerializedObject elementSo, Type elementType)
        {
            if (ElementDrawerRegistry.TryGetDrawer(elementType, out ElementDrawerRegistry.Entry entry))
            {
                return entry.GetHeight(elementSo);
            }

            return GetRemainingFieldsHeight(elementSo);
        }

        // ReorderableList draws its drag handle in a strip at the left edge of each element's rect
        // without excluding it from the rect passed to drawElementCallback - inset our own content so it
        // doesn't sit underneath the handle.
        private const float LeftPadding = 14f;

        private static void DrawElement(Rect rect, SerializedProperty listProperty, int index, bool isConditionList)
        {
            rect = new Rect(rect.x + LeftPadding, rect.y, rect.width - LeftPadding, rect.height);

            SerializedProperty elementProp = listProperty.GetArrayElementAtIndex(index);
            UnityEngine.Object element = elementProp.objectReferenceValue;

            float lineHeight = EditorGUIUtility.singleLineHeight;
            float y = rect.y + 2f;

            if (element == null)
            {
                EditorGUI.LabelField(new Rect(rect.x, y, rect.width, lineHeight), "(missing - will be removed)");
                return;
            }

            var elementSo = new SerializedObject(element);
            elementSo.Update();

            if (isConditionList && index > 0)
            {
                SerializedProperty opProp = elementSo.FindProperty("Operator");
                if (opProp != null)
                {
                    EditorGUI.PropertyField(new Rect(rect.x, y, 80f, lineHeight), opProp, GUIContent.none);
                }

                y += lineHeight + 2f;
            }

            string label = ComponentTypeCache.DisplayName(element.GetType());
            bool expanded = IsExpanded(element);
            Rect foldoutRect = new Rect(rect.x, y, rect.width - 24f, lineHeight);
            bool newExpanded = EditorGUI.Foldout(foldoutRect, expanded, label, true);
            SetExpanded(element, newExpanded);

            Rect removeRect = new Rect(rect.x + rect.width - 22f, y, 22f, lineHeight);
            if (GUI.Button(removeRect, "X"))
            {
                RemoveElementAndComponent(listProperty, index, element);
                return;
            }

            y += lineHeight + 2f;

            if (newExpanded)
            {
                Rect contentRect = new Rect(rect.x, y, rect.width, rect.height - (y - rect.y));
                float consumed;

                if (ElementDrawerRegistry.TryGetDrawer(element.GetType(), out ElementDrawerRegistry.Entry entry))
                {
                    consumed = entry.GetHeight(elementSo);
                    entry.Draw(new Rect(contentRect.x, contentRect.y, contentRect.width, consumed), elementSo);
                }
                else
                {
                    consumed = DrawRemainingFields(contentRect, elementSo);
                }

                if (TryGetWarning(element, out string message))
                {
                    Rect warningRect = new Rect(rect.x, y + consumed + 2f, rect.width, lineHeight * 2f);
                    EditorGUI.HelpBox(warningRect, message, MessageType.Warning);
                }
            }

            elementSo.ApplyModifiedProperties();
        }

        /// <summary>
        /// Draws every remaining serialized field on an element generically (skipping m_Script, Operator,
        /// and anything listed in extraSkipNames). Custom drawers that only need to special-case a couple
        /// of fields can draw those by hand, then call this for the rest instead of writing a full drawer.
        /// </summary>
        public static float DrawRemainingFields(Rect rect, SerializedObject elementSo, params string[] extraSkipNames)
        {
            float y = rect.y;
            SerializedProperty iterator = elementSo.GetIterator();
            bool enterChildren = true;

            while (iterator.NextVisible(enterChildren))
            {
                enterChildren = false;
                if (!ShouldDrawGenericProperty(iterator.name, extraSkipNames)) continue;

                float propHeight = EditorGUI.GetPropertyHeight(iterator, true);
                EditorGUI.PropertyField(new Rect(rect.x, y, rect.width, propHeight), iterator, true);
                y += propHeight + 2f;
            }

            return y - rect.y;
        }

        /// <summary>Height DrawRemainingFields() will consume, without drawing anything. See DrawRemainingFields.</summary>
        public static float GetRemainingFieldsHeight(SerializedObject elementSo, params string[] extraSkipNames)
        {
            float height = 0f;
            SerializedProperty iterator = elementSo.GetIterator();
            bool enterChildren = true;

            while (iterator.NextVisible(enterChildren))
            {
                enterChildren = false;
                if (!ShouldDrawGenericProperty(iterator.name, extraSkipNames)) continue;
                height += EditorGUI.GetPropertyHeight(iterator, true) + 2f;
            }

            return height;
        }

        private static bool ShouldDrawGenericProperty(string propertyName, string[] extraSkipNames)
        {
            if (propertyName == "m_Script" || propertyName == "Operator") return false;

            if (extraSkipNames != null)
            {
                for (int i = 0; i < extraSkipNames.Length; i++)
                {
                    if (propertyName == extraSkipNames[i]) return false;
                }
            }

            return true;
        }

        private static bool TryGetWarning(UnityEngine.Object element, out string message)
        {
            switch (element)
            {
                case ConditionBase condition:
                    return condition.TryGetValidationWarning(out message);
                case ActionBase action:
                    return action.TryGetValidationWarning(out message);
                default:
                    message = null;
                    return false;
            }
        }

        private static void ShowAddMenu(Rect buttonRect, SerializedProperty listProperty, Type baseType, GameObject ownerGameObject)
        {
            var menu = new GenericMenu();
            IReadOnlyList<Type> types = baseType == typeof(ConditionBase) ? ComponentTypeCache.ConditionTypes : ComponentTypeCache.ActionTypes;

            if (types.Count == 0)
            {
                menu.AddDisabledItem(new GUIContent("(none available)"));
            }

            foreach (Type type in types)
            {
                Type capturedType = type;
                menu.AddItem(new GUIContent(ComponentTypeCache.DisplayName(capturedType)), false, () => AddElement(listProperty, capturedType, ownerGameObject));
            }

            menu.DropDown(buttonRect);
        }

        private static void AddElement(SerializedProperty listProperty, Type type, GameObject ownerGameObject)
        {
            SerializedObject ownerSerializedObject = listProperty.serializedObject;

            UnityEngine.Object added = Undo.AddComponent(ownerGameObject, type);
            added.hideFlags = HideFlags.HideInInspector;

            ownerSerializedObject.Update();
            listProperty.arraySize++;
            listProperty.GetArrayElementAtIndex(listProperty.arraySize - 1).objectReferenceValue = added;
            ownerSerializedObject.ApplyModifiedProperties();
        }

        private static void RemoveElement(SerializedProperty listProperty, int index)
        {
            if (index < 0 || index >= listProperty.arraySize) return;

            UnityEngine.Object element = listProperty.GetArrayElementAtIndex(index).objectReferenceValue;
            RemoveElementAndComponent(listProperty, index, element);
        }

        private static void RemoveElementAndComponent(SerializedProperty listProperty, int index, UnityEngine.Object element)
        {
            SerializedObject ownerSerializedObject = listProperty.serializedObject;

            // SerializedProperty quirk: DeleteArrayElementAtIndex on a non-null object reference slot only
            // nulls it out the first time. Null it explicitly first so the second call actually removes the slot.
            SerializedProperty elementProp = listProperty.GetArrayElementAtIndex(index);
            if (elementProp.objectReferenceValue != null)
            {
                elementProp.objectReferenceValue = null;
            }

            listProperty.DeleteArrayElementAtIndex(index);
            ownerSerializedObject.ApplyModifiedProperties();

            if (element != null)
            {
                Undo.DestroyObjectImmediate(element);
            }
        }

        private static bool IsExpanded(UnityEngine.Object element)
        {
            return !Expanded.TryGetValue(element.GetEntityId(), out bool expanded) || expanded;
        }

        private static void SetExpanded(UnityEngine.Object element, bool expanded)
        {
            Expanded[element.GetEntityId()] = expanded;
        }
    }
}
