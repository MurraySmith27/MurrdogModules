using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Coffee.UIExtensions
{
    [CustomPropertyDrawer(typeof(MaterialAccessor))]
    internal class MaterialAccessorDrawer : PropertyDrawer
    {
        private static readonly Dictionary<Type, MethodInfo[]> s_GetMethodsCache = new Dictionary<Type, MethodInfo[]>();
        private static readonly Dictionary<Type, MethodInfo[]> s_SetMethodsCache = new Dictionary<Type, MethodInfo[]>();

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var spTarget = property.FindPropertyRelative("m_Target");
            var spGetter = property.FindPropertyRelative("m_Getter");
            var spSetter = property.FindPropertyRelative("m_Setter");
            var row = IsValidTarget(spTarget, out _) ? 3 : 1;

            if (string.IsNullOrEmpty(spGetter.stringValue) || string.IsNullOrEmpty(spSetter.stringValue))
            {
                row++;
            }

            return (EditorGUIUtility.singleLineHeight + 2) * row;
        }

        public override void OnGUI(Rect p, SerializedProperty property, GUIContent label)
        {
            var rect = new Rect(p.x, p.y, p.width, EditorGUIUtility.singleLineHeight);
            var spTarget = property.FindPropertyRelative("m_Target");
            DrawComponentSelector(rect, property);

            var isValid = false;
            if (IsValidTarget(spTarget, out var target))
            {
                EditorGUI.indentLevel++;
                GetMaterialAccessors(target.GetType(), out var getMethods, out var setMethods);

                rect.y += rect.height + 2;
                var spGetter = property.FindPropertyRelative("m_Getter");
                DrawMethodSelector(rect, spGetter, getMethods);

                rect.y += rect.height + 2;
                var spSetter = property.FindPropertyRelative("m_Setter");
                DrawMethodSelector(rect, spSetter, setMethods);
                EditorGUI.indentLevel--;

                isValid = !string.IsNullOrEmpty(spGetter.stringValue) && !string.IsNullOrEmpty(spSetter.stringValue);
            }
            else
            {
                spTarget.stringValue = "";
            }

            if (!isValid)
            {
                rect.y += rect.height + 2;
                EditorGUI.HelpBox(rect, "Invalid Material Accessor", MessageType.Warning);
            }
        }

        private static void DrawComponentSelector(Rect p, SerializedProperty property)
        {
            var spTarget = property.FindPropertyRelative("m_Target");
            var label = EditorGUIUtility.TrTextContent(spTarget.displayName);
            label = EditorGUI.BeginProperty(p, label, property);
            p = EditorGUI.PrefixLabel(p, label);

            var name = "No Target";
            if (!string.IsNullOrEmpty(spTarget.stringValue))
            {
                var type = Type.GetType(spTarget.stringValue);
                if (type?.IsSubclassOf(typeof(Component)) == true)
                {
                    name = type.Name;
                }
                else
                {
                    spTarget.stringValue = "";
                }
            }

            if (GUI.Button(p, EditorGUIUtility.TrTempContent(name), EditorStyles.popup))
            {
                var parent = property.serializedObject.targetObject as Component;
                var menu = new GenericMenu();
                var components = parent.GetComponents<Component>();
                foreach (var c in components)
                {
                    var type = c.GetType();
                    if (!IsValidType(type)) continue;

                    var typeName = $"{type.FullName}, {type.Assembly.GetName().Name}";
                    menu.AddItem(EditorGUIUtility.TrTextContent(type.Name), spTarget.stringValue == typeName, () =>
                    {
                        spTarget.stringValue = typeName;
                        property.FindPropertyRelative("m_Getter").stringValue = "";
                        property.FindPropertyRelative("m_Setter").stringValue = "";
                        spTarget.serializedObject.ApplyModifiedProperties();
                    });
                }

                menu.DropDown(p);
            }

            EditorGUI.EndProperty();
        }

        private static void DrawMethodSelector(Rect p, SerializedProperty prop, MethodInfo[] methods)
        {
            var label = EditorGUI.BeginProperty(p, null, prop);
            p = EditorGUI.PrefixLabel(p, label);
            var name = prop.stringValue;
            if (GUI.Button(p, EditorGUIUtility.TrTempContent(name), EditorStyles.popup))
            {
                var menu = new GenericMenu();
                foreach (var method in methods)
                {
                    menu.AddItem(EditorGUIUtility.TrTextContent(method.Name), prop.stringValue == method.Name, () =>
                    {
                        prop.stringValue = method.Name;
                        prop.serializedObject.ApplyModifiedProperties();
                    });
                }

                menu.DropDown(p);
            }

            EditorGUI.EndProperty();
        }

        private static void GetMaterialAccessors(Type type, out MethodInfo[] getMethods, out MethodInfo[] setMethods)
        {
            if (s_GetMethodsCache.TryGetValue(type, out getMethods)
                && s_SetMethodsCache.TryGetValue(type, out setMethods))
            {
                return;
            }

            const BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance
                                       | BindingFlags.FlattenHierarchy;
            s_GetMethodsCache[type] = getMethods = type.GetMethods(flags)
                .Where(m => m.ReturnType == typeof(Material) && m.GetParameters().Length == 0)
                .ToArray();

            s_SetMethodsCache[type] = setMethods = type.GetMethods(flags)
                .Where(m => m.ReturnType == typeof(void) && m.GetParameters().Length == 1 &&
                            m.GetParameters()[0].ParameterType == typeof(Material))
                .ToArray();
        }

        private static bool IsValidType(Type type)
        {
            GetMaterialAccessors(type, out var getMethods, out var setMethods);
            return 0 < getMethods.Length && 0 < setMethods.Length;
        }

        private static bool IsValidTarget(SerializedProperty spTarget, out Component target)
        {
            target = null;
            if (string.IsNullOrEmpty(spTarget.stringValue)) return false;

            var type = Type.GetType(spTarget.stringValue);
            if (type == null || !type.IsSubclassOf(typeof(Component))) return false;

            var parent = spTarget.serializedObject.targetObject as Component;
            return parent && parent.TryGetComponent(type, out target);
        }
    }
}
