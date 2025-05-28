using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorImageSetter : MonoBehaviour
{
    
    [SerializeField] private Texture2D cursorTexture;

    [SerializeField] private Texture2D mouseDownCursorTexture;
    
    void Awake()
    {
        Cursor.SetCursor(cursorTexture, Vector2.zero, CursorMode.Auto);
    }

    private void OnMouseDown()
    {
        Cursor.SetCursor(mouseDownCursorTexture, Vector2.zero, CursorMode.Auto);
    }
    
    private void OnMouseUp()
    {
        Cursor.SetCursor(cursorTexture, Vector2.zero, CursorMode.Auto);
    }
}
