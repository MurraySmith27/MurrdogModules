using System;
using System.Reflection;
using UnityEngine;

namespace Coffee.UIExtensions
{
    [Serializable]
    public class MaterialAccessor
    {
        private const BindingFlags k_Flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance |
                                             BindingFlags.FlattenHierarchy;

        [SerializeField] private string m_Target = "";
        [SerializeField] private string m_Getter = "";
        [SerializeField] private string m_Setter = "";

        private Component _target;
        private Func<Material> _getter;
        private Action<Material> _setter;

        public Material Get()
        {
            if (!IsValid()) return null;
            return _getter.Invoke();
        }

        public void Set(Material material)
        {
            if (!IsValid()) return;
            _setter.Invoke(material);
        }

        public void SetDirty()
        {
            _getter = null;
            _setter = null;
        }

        public bool InitializeIfNeeded(GameObject parent)
        {
            if (IsValid()) return true;

            _getter = null;
            _setter = null;
            if (string.IsNullOrEmpty(m_Target) || string.IsNullOrEmpty(m_Getter) || string.IsNullOrEmpty(m_Setter))
            {
                return false;
            }

            var type = Type.GetType(m_Target);
            if (type == null || !type.IsSubclassOf(typeof(Component)))
            {
                Debug.LogError($"Type '{m_Target}' is not a subclass of Component.");
                return false;
            }

            if (!parent.TryGetComponent(type, out _target))
            {
                Debug.LogError($"Target '{m_Target}' is not found in '{parent.name}'");
                return false;
            }

            try
            {
                var method = _target.GetType().GetMethod(m_Getter, k_Flags);
                _getter = Delegate.CreateDelegate(typeof(Func<Material>), _target, method) as Func<Material>;
            }
            catch (Exception)
            {
                Debug.LogException(new Exception($"Getter<Material> '{m_Getter}' is not found in '{_target}'"));
                return false;
            }

            try
            {
                var method = _target.GetType().GetMethod(m_Setter, k_Flags);
                _setter = Delegate.CreateDelegate(typeof(Action<Material>), _target, method) as Action<Material>;
            }
            catch (Exception)
            {
                Debug.LogException(new Exception($"Setter<Material> '{m_Setter}' is not found in '{_target}'"));
                return false;
            }

            return true;
        }

        public bool IsValid()
        {
            return _target && (Component)_getter?.Target == _target && (Component)_setter?.Target == _target;
        }
    }
}
