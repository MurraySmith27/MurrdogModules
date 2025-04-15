using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Preview3DController : Singleton<Preview3DController>
{
    [SerializeField] private Transform previewTransformsParent;

    [SerializeField] private Vector2Int previewTransformDimensions = new Vector2Int(8, 4);

    public class PreviewTransform
    {
        public Transform Transform;
        public bool Occupied;
        public Rect UVRect;
        public int Id;
    }
    
    private List<PreviewTransform> _previewTransforms;
    
    private void Awake()
    {
        _previewTransforms = new();

        float width = 1 / (float)previewTransformDimensions.x;
        float height = 1 / (float)previewTransformDimensions.y;

        for (int i = 0; i < previewTransformsParent.childCount; i++)
        {
            PreviewTransform previewTransform = new();
            
            previewTransform.Transform = previewTransformsParent.GetChild(i);
            previewTransform.Occupied = false;
            previewTransform.UVRect = new Rect((i % previewTransformDimensions.x) / (float)previewTransformDimensions.x, 
                1f - Mathf.FloorToInt(i / (float)previewTransformDimensions.x) / (float)previewTransformDimensions.y - height, 
                width,
                height);

            previewTransform.Id = i;
            
            _previewTransforms.Add(previewTransform);
        }
    }

    public bool GetPreviewTransform(out PreviewTransform previewTransform)
    {
        previewTransform = _previewTransforms.FirstOrDefault((PreviewTransform pt) =>
        {
            return !pt.Occupied;
        });

        if (previewTransform != null)
        {
            previewTransform.Occupied = true;
            return true;
        }
        else return false;
    }

    public void FreePreviewTransform(PreviewTransform transformToFree)
    {
        foreach (var previewTransform in _previewTransforms)
        {
            if (previewTransform.Id == transformToFree.Id)
            {
                previewTransform.Occupied = false;
                break;
            }
        }
    }
    
}
