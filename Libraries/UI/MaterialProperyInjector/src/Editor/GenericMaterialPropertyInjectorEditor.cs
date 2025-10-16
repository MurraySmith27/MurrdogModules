using UnityEditor;

namespace Coffee.UIExtensions
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(GenericMaterialPropertyInjector))]
    internal class GenericMaterialPropertyInjectorEditor : UIMaterialPropertyInjectorEditor
    {
        private SerializedProperty _accessor;
        private Editor _settingsEditor;

        protected override void OnEnable()
        {
            base.OnEnable();
            _accessor = serializedObject.FindProperty("m_Accessor");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.UpdateIfRequiredOrScript();
            EditorGUILayout.PropertyField(_accessor);
            serializedObject.ApplyModifiedProperties();

            base.OnInspectorGUI();
            SyncMaterialPropertySystem.instance.OnInspectorGUI(target as GenericMaterialPropertyInjector);
        }
    }
}
