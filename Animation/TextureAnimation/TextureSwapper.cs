using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureSwapper : MonoBehaviour
{
    [SerializeField] private Material textureSwapMaterial;

    [SerializeField] private string currentTextureShaderVarName = "_SliceRange";

    [SerializeField] private int defaultTextureNum = 0;

    public event Action<int, int> OnTextureNumChanged;

    private int _currentTextureNum;

    private void Awake()
    {
        _currentTextureNum = defaultTextureNum;
        ChangeTextureNum(_currentTextureNum);
    }
    
    public void ChangeTextureNum(int newTextureNum)
    {
        if (newTextureNum != _currentTextureNum)
        {
            int oldTextureNum = _currentTextureNum;

            _currentTextureNum = newTextureNum;

            textureSwapMaterial.SetFloat(currentTextureShaderVarName, newTextureNum);

            OnTextureNumChanged?.Invoke(oldTextureNum, newTextureNum);
        }
    }
}
