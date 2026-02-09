#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.UI;

namespace UnityEditor.UI
{
    [CustomEditor(typeof(SubdividedImage), true)]
    [CanEditMultipleObjects]
    public class SubdividedImageEditor : GraphicEditor
    {
        SerializedProperty m_Sprite;
        SerializedProperty m_FillType;
        SerializedProperty m_FillMethod;
        SerializedProperty m_FillOrigin;
        SerializedProperty m_FillAmount;
        SerializedProperty m_FillClockwise;
        SerializedProperty m_Subdivisions;
        SerializedProperty m_PreserveAspect;

        protected override void OnEnable()
        {
            base.OnEnable();

            m_Sprite = serializedObject.FindProperty("m_Sprite");
            m_FillType = serializedObject.FindProperty("m_FillType");
            m_FillMethod = serializedObject.FindProperty("m_FillMethod");
            m_FillOrigin = serializedObject.FindProperty("m_FillOrigin");
            m_FillAmount = serializedObject.FindProperty("m_FillAmount");
            m_FillClockwise = serializedObject.FindProperty("m_FillClockwise");
            m_Subdivisions = serializedObject.FindProperty("m_Subdivisions");
            m_PreserveAspect = serializedObject.FindProperty("m_PreserveAspect");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(m_Sprite);

            AppearanceControlsGUI();

            // Fill Type
            EditorGUILayout.PropertyField(m_FillType);

            if ((SubdividedImage.FillType)m_FillType.enumValueIndex == SubdividedImage.FillType.Filled)
            {
                EditorGUI.indentLevel++;

                EditorGUILayout.PropertyField(m_FillMethod);

                var fillMethod = (SubdividedImage.FillMethod)m_FillMethod.enumValueIndex;
                switch (fillMethod)
                {
                    case SubdividedImage.FillMethod.Horizontal:
                        m_FillOrigin.intValue = (int)(SubdividedImage.OriginHorizontal)EditorGUILayout.EnumPopup("Fill Origin",
                            (SubdividedImage.OriginHorizontal)m_FillOrigin.intValue);
                        break;
                    case SubdividedImage.FillMethod.Vertical:
                        m_FillOrigin.intValue = (int)(SubdividedImage.OriginVertical)EditorGUILayout.EnumPopup("Fill Origin",
                            (SubdividedImage.OriginVertical)m_FillOrigin.intValue);
                        break;
                    case SubdividedImage.FillMethod.Radial90:
                        m_FillOrigin.intValue = (int)(SubdividedImage.Origin90)EditorGUILayout.EnumPopup("Fill Origin",
                            (SubdividedImage.Origin90)m_FillOrigin.intValue);
                        break;
                    case SubdividedImage.FillMethod.Radial180:
                        m_FillOrigin.intValue = (int)(SubdividedImage.Origin180)EditorGUILayout.EnumPopup("Fill Origin",
                            (SubdividedImage.Origin180)m_FillOrigin.intValue);
                        break;
                    case SubdividedImage.FillMethod.Radial360:
                        m_FillOrigin.intValue = (int)(SubdividedImage.Origin360)EditorGUILayout.EnumPopup("Fill Origin",
                            (SubdividedImage.Origin360)m_FillOrigin.intValue);
                        break;
                }

                EditorGUILayout.PropertyField(m_FillAmount);

                if (fillMethod == SubdividedImage.FillMethod.Radial90
                    || fillMethod == SubdividedImage.FillMethod.Radial180
                    || fillMethod == SubdividedImage.FillMethod.Radial360)
                {
                    EditorGUILayout.PropertyField(m_FillClockwise);
                }

                EditorGUI.indentLevel--;
            }

            // Subdivisions
            EditorGUILayout.PropertyField(m_Subdivisions);
            int subs = m_Subdivisions.intValue;
            int vertCount = (subs + 1) * (subs + 1);
            int triCount = 2 * subs * subs;
            EditorGUILayout.HelpBox($"Vertices: {vertCount}  |  Triangles: {triCount}", MessageType.Info);

            EditorGUILayout.PropertyField(m_PreserveAspect);

            RaycastControlsGUI();

            serializedObject.ApplyModifiedProperties();

            // Native Size button
            NativeSizeButtonGUI();
        }

        protected void SetShowNativeSize(bool show, bool instant)
        {
            var image = target as SubdividedImage;
            base.SetShowNativeSize(show && image != null && image.sprite != null, instant);
        }
    }
}
#endif
