using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace Coffee.UIExtensions
{
    public abstract class MaterialPropertyAttribute
    {
        private static readonly List<Func<MaterialProperty, string, MaterialPropertyAttribute>> s_FactoryMethods =
            new List<Func<MaterialProperty, string, MaterialPropertyAttribute>>()
            {
                Toggle.FactoryMethod,
                Enum.FactoryMethod,
                PowerSlider.FactoryMethod,
                IntSlider.FactoryMethod
            };

        private static readonly Dictionary<string, MaterialPropertyAttribute> s_Cache =
            new Dictionary<string, MaterialPropertyAttribute>();

        public static MaterialPropertyAttribute Find(MaterialProperty mp, string[] attributes)
        {
            foreach (var attribute in attributes)
            {
                var attr = Find(mp, attribute);
                if (attr != null) return attr;
            }

            return null;
        }

        private static MaterialPropertyAttribute Find(MaterialProperty mp, string attribute)
        {
            if (s_Cache.TryGetValue(attribute, out var attr))
            {
                return attr;
            }

            return s_Cache[attribute] = s_FactoryMethods
                .Select(f => f(mp, attribute))
                .FirstOrDefault(x => x != null);
        }

        public abstract void OnGUI(Rect rect, string label, MaterialProperty mp);
    }

    public class Toggle : MaterialPropertyAttribute
    {
        internal static MaterialPropertyAttribute FactoryMethod(MaterialProperty mp, string attribute)
        {
#if UNITY_2021_1_OR_NEWER
            if (mp.type != MaterialProperty.PropType.Float && mp.type != MaterialProperty.PropType.Int) return null;
#else
            if (mp.type != MaterialProperty.PropType.Float) return null;
#endif

            var m = Regex.Match(attribute, @"^(Material)?Toggle(Drawer)?$");
            if (!m.Success) return null;

            return new Toggle();
        }

        public override void OnGUI(Rect r, string label, MaterialProperty mp)
        {
            if (mp.type == MaterialProperty.PropType.Float)
            {
                mp.floatValue = EditorGUI.Toggle(r, label, 0 < mp.floatValue) ? 1 : 0;
            }
#if UNITY_2021_1_OR_NEWER
            else if (mp.type == MaterialProperty.PropType.Int)
            {
                mp.intValue = EditorGUI.Toggle(r, label, 0 < mp.intValue) ? 1 : 0;
            }
#endif
        }
    }

    public class Enum : MaterialPropertyAttribute
    {
        private static Type[] s_EnumTypes;
        private string[] _names;
        private int[] _values;

        internal static MaterialPropertyAttribute FactoryMethod(MaterialProperty mp, string attribute)
        {
#if UNITY_2021_1_OR_NEWER
            if (mp.type != MaterialProperty.PropType.Float && mp.type != MaterialProperty.PropType.Int) return null;
#else
            if (mp.type != MaterialProperty.PropType.Float) return null;
#endif

            var m = Regex.Match(attribute, @"^(Material)?Enum(Drawer)?\s*\(([^)]+)\)");
            if (!m.Success) return null;

            var values = m.Groups[3].Value.Replace(" ", "").Split(',');
            if (values.Length == 1)
            {
                if (s_EnumTypes == null)
                {
                    s_EnumTypes = TypeCache.GetTypesDerivedFrom(typeof(System.Enum)).ToArray();
                }

                var typeName = values[0];
                var type = s_EnumTypes.FirstOrDefault(x => x.Name == typeName || x.FullName == typeName);
                if (type == null) return null;

                return new Enum()
                {
                    _names = System.Enum.GetNames(type),
                    _values = System.Enum.GetValues(type).Cast<int>().ToArray()
                };
            }

            if (values.Length % 2 == 0)
            {
                return new Enum()
                {
                    _names = values
                        .Where((_, i) => i % 2 == 0)
                        .ToArray(),
                    _values = values
                        .Where((_, i) => i % 2 == 1)
                        .Select(x => int.TryParse(x, out var v) ? v : 0)
                        .ToArray()
                };
            }

            return null;
        }

        public override void OnGUI(Rect r, string label, MaterialProperty mp)
        {
            if (mp.type == MaterialProperty.PropType.Float)
            {
                mp.floatValue = EditorGUI.IntPopup(r, label, (int)mp.floatValue, _names, _values);
            }
#if UNITY_2021_1_OR_NEWER
            else if (mp.type == MaterialProperty.PropType.Int)
            {
                mp.intValue = EditorGUI.IntPopup(r, label, mp.intValue, _names, _values);
            }
#endif
        }
    }

    public class PowerSlider : MaterialPropertyAttribute
    {
        private static Func<Rect, MaterialProperty, GUIContent, float, float> s_DoPowerRangeProperty;
        private static bool s_NotFoundMethod;
        private float _power;

        internal static MaterialPropertyAttribute FactoryMethod(MaterialProperty mp, string attribute)
        {
            if (s_NotFoundMethod) return null;
            if (mp.type != MaterialProperty.PropType.Range) return null;

            var m = Regex.Match(attribute, @"^(Material)?PowerSlider(Drawer)?\s*\(([^)]+)\)");
            if (!m.Success) return null;

            if (float.TryParse(m.Groups[3].Value, out var power) == false) return null;

            if (s_DoPowerRangeProperty == null)
            {
                try
                {
                    const BindingFlags flags = BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public;
                    s_DoPowerRangeProperty = typeof(MaterialEditor)
                            .GetMethod("DoPowerRangeProperty", flags)
                            .CreateDelegate(typeof(Func<Rect, MaterialProperty, GUIContent, float, float>), null)
                        as Func<Rect, MaterialProperty, GUIContent, float, float>;
                }
                catch (Exception)
                {
                    s_NotFoundMethod = true;
                    Debug.LogWarning(
                        "MaterialEditor.DoPowerRangeProperty(Rect, MaterialProperty, GUIContent, float) not found.");
                    return null;
                }
            }

            return new PowerSlider() { _power = power };
        }

        public override void OnGUI(Rect r, string label, MaterialProperty mp)
        {
            mp.floatValue = s_DoPowerRangeProperty(r, mp, EditorGUIUtility.TrTextContent(label), _power);
        }
    }

    public class IntSlider : MaterialPropertyAttribute
    {
        private static Func<Rect, MaterialProperty, GUIContent, int> s_DoIntRangeProperty;
        private static bool s_NotFoundMethod;
        private float _power;

        internal static MaterialPropertyAttribute FactoryMethod(MaterialProperty mp, string attribute)
        {
            if (s_NotFoundMethod) return null;
            if (mp.type != MaterialProperty.PropType.Range) return null;

            var m = Regex.Match(attribute, @"^(Material)?IntRange(Drawer)?$");
            if (!m.Success) return null;

            if (s_DoIntRangeProperty == null)
            {
                try
                {
                    const BindingFlags flags = BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public;
                    s_DoIntRangeProperty = typeof(MaterialEditor)
                            .GetMethod("DoIntRangeProperty", flags)
                            .CreateDelegate(typeof(Func<Rect, MaterialProperty, GUIContent, int>), null)
                        as Func<Rect, MaterialProperty, GUIContent, int>;
                }
                catch (Exception)
                {
                    s_NotFoundMethod = true;
                    Debug.LogWarning(
                        "MaterialEditor.DoIntRangeProperty(Rect, MaterialProperty, GUIContent) not found.");
                    return null;
                }
            }

            return new IntSlider();
        }

        public override void OnGUI(Rect r, string label, MaterialProperty mp)
        {
            mp.floatValue = s_DoIntRangeProperty(r, mp, EditorGUIUtility.TrTextContent(label));
        }
    }
}
