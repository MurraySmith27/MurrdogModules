using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextSizeController : MonoBehaviour
{
    private TextMeshProUGUI textMesh;
    void Awake()
    {
        textMesh = GetComponent<TextMeshProUGUI>();

        if (textMesh == null)
        {
            Debug.LogError($"Text mesh is not available on an object with a TextSizeController! object: {GameObjectUtils.GetHeirarchyPath(gameObject)}");
            return;
        }
        else
        {
            textMesh.fontSize *= GlobalSettings.TextSizeMultiplier;
        }
    }
}
