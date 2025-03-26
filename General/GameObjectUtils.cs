using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public static class GameObjectUtils
{
    public static string GetHeirarchyPath(GameObject gameObject)
    {
        StringBuilder stringBuilder = new();

        Transform current = gameObject.transform;

        while (current != null)
        {
            stringBuilder.Append(current.name);
            stringBuilder.Append("/");
            current = current.parent;
        }

        if (stringBuilder.Length > 0)
        {
            stringBuilder.Remove(stringBuilder.Length - 1, 1);
        }
        
        return stringBuilder.ToString();
    }
}
