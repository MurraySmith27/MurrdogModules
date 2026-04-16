using UnityEngine;
using UnityEngine.VFX;

[RequireComponent(typeof(VisualEffect))]
public class VFXBindTransformPosition : MonoBehaviour
{
    public Transform target;
    public string vfxPropertyName = "TargetPosition";
    public bool isLocalSpace = true;

    private VisualEffect vfx;
    private int propertyID;

    void Awake()
    {
        vfx = GetComponent<VisualEffect>();
        propertyID = Shader.PropertyToID(vfxPropertyName);
    }

    public void SetTarget(Transform _target)
    {
        target = _target;
    }

    void Update()
    {
        if (target == null) return;

        // Send world position every frame
        Vector3 pos = target.position;
        if (isLocalSpace)
        {
            pos = transform.InverseTransformPoint(target.position);
        }
        vfx.SetVector3(propertyID, pos);
    }
}