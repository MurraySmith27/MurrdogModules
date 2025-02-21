using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GenerateMapButton : MonoBehaviour
{
    [SerializeField] private TMP_InputField widthInput;
    [SerializeField] private TMP_InputField heightInput;

    public void OnClick()
    {
        int width = int.Parse(widthInput.text);
        int height = int.Parse(heightInput.text);

        if (width > 0 && height > 0)
        {
            MapSystem.Instance.GenerateMapChunk(0, 0, width, height);
        }
    }
}
