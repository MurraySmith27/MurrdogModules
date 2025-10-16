using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Rendering;

namespace Coffee.UIExtensions
{
    internal class InjectionPropertyListDrawer : ReorderableList
    {
        public Action<SerializedProperty, string> postAddCallback;
        public Action resetCallback;
        private readonly ShaderPropertyDropdown _dropdown = new ShaderPropertyDropdown();

        private static readonly Regex s_RegexOthers =
            new Regex(
                "_ST$|_HDR$|_TexelSize$|^_Stencil|^_MainTex$|^_Color$|^_ClipRect$|^_UseUIAlphaClip$|^_ColorMask$" +
                "|^_TextureSampleAdd$|^_(UI)?MaskSoftnessX$|^_(UI)?MaskSoftnessY$");

        public InjectionPropertyListDrawer(SerializedProperty prop) : base(prop.serializedObject, prop)
        {
            drawHeaderCallback = DrawHeaderCallback;
            drawElementCallback = DrawElementCallback;
            onAddDropdownCallback = OnAddDropdownCallback;
        }

        private void DrawElementCallback(Rect r, int i, bool _, bool __)
        {
            r = new Rect(r.x, r.y + 1, r.width, r.height - 2);
            EditorGUI.PropertyField(r, serializedProperty.GetArrayElementAtIndex(i));
        }

        private void DrawHeaderCallback(Rect r)
        {
            EditorGUI.LabelField(new Rect(r.x, r.y + 1, 100, r.height - 2), "Properties");
            var buttonRect = new Rect(r.x + r.width - 100, r.y + 1, 100, r.height - 2);
            if (resetCallback != null && GUI.Button(buttonRect, "Reset Values"))
            {
                EditorApplication.delayCall += resetCallback.Invoke;
            }
        }

        private void OnAddDropdownCallback(Rect r, ReorderableList _)
        {
            var propArray = serializedProperty;
            var target = propArray.serializedObject.targetObject;
            var current = target as UIMaterialPropertyInjector;
            if (!current && target is UIMaterialPropertyTweener tweener)
            {
                current = tweener.target;
            }

            if (!current) return;

            var included = new HashSet<string>(Enumerable.Range(0, propArray.arraySize)
                .Select(i =>
                {
                    var ps = propArray.GetArrayElementAtIndex(i);
                    var propName = ps.type == "InjectionPropertyPair" ? "m_From.m_PropertyName" : "m_PropertyName";
                    return ps.FindPropertyRelative(propName).stringValue;
                }));
            var shader = current.defaultMaterialForRendering.shader;
            var properties = shader.GetAllProperties()
                .Where(p => 0 == (p.flags & ShaderPropertyFlags.PerRendererData) && !included.Contains(p.name))
                .Append(new ShaderProperty("", PropertyType.Undefined)) // Separator
                .OrderBy(p => s_RegexOthers.IsMatch(p.name));

            _dropdown.SetProperties(shader, properties);
            _dropdown.SetCallback(s =>
            {
                var prop = propArray.GetArrayElementAtIndex(propArray.arraySize++);
                postAddCallback?.Invoke(prop, s.name);
                propArray.serializedObject.ApplyModifiedProperties();
            });
            _dropdown.Show(r);
        }
    }
}
