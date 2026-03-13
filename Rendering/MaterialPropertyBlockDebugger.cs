#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;

public class MaterialPropertyBlockDebugger : MonoBehaviour
{
    public Renderer targetRenderer;

    [Tooltip("Enter shader property names to inspect")]
    public List<string> propertyNames = new();

    [System.Serializable]
    public class PropertyValue
    {
        public string name;
        public string type;
        public string value;
    }

    [SerializeField]
    public List<PropertyValue> values = new();

    MaterialPropertyBlock mpb;

    void OnEnable()
    {
        if (!targetRenderer)
            targetRenderer = GetComponent<Renderer>();

        mpb = new MaterialPropertyBlock();
    }

    public void Refresh()
    {
        if (!targetRenderer || targetRenderer.sharedMaterial == null)
            return;

        targetRenderer.GetPropertyBlock(mpb);

        values.Clear();

        var shader = targetRenderer.sharedMaterial.shader;

        foreach (var prop in propertyNames)
        {
            if (string.IsNullOrEmpty(prop))
                continue;

            int id = Shader.PropertyToID(prop);

            var entry = new PropertyValue();
            entry.name = prop;

            bool found = false;

            for (int i = 0; i < shader.GetPropertyCount(); i++)
            {
                if (shader.GetPropertyName(i) != prop)
                    continue;

                var type = shader.GetPropertyType(i);

                switch (type)
                {
                    case UnityEngine.Rendering.ShaderPropertyType.Float:
                    case UnityEngine.Rendering.ShaderPropertyType.Range:
                        entry.type = "Float";
                        entry.value = mpb.GetFloat(id).ToString();
                        break;

                    case UnityEngine.Rendering.ShaderPropertyType.Vector:
                        entry.type = "Vector";
                        entry.value = mpb.GetVector(id).ToString();
                        break;

                    case UnityEngine.Rendering.ShaderPropertyType.Color:
                        entry.type = "Color";
                        entry.value = mpb.GetColor(id).ToString();
                        break;

                    case UnityEngine.Rendering.ShaderPropertyType.Texture:
                        entry.type = "Texture";
                        var tex = mpb.GetTexture(id);
                        entry.value = tex ? tex.name : "null";
                        break;
                }

                found = true;
                break;
            }

            if (!found)
            {
                entry.type = "Unknown";
                entry.value = "Property not found in shader";
            }

            values.Add(entry);
        }
    }
}
#endif