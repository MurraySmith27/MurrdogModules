using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public static class ListUtils
{
    public static void ShuffleInPlace<T>(this IList<T> list)  
    {  
        int n = list.Count;
        List<int> remainingIndices = new();

        for (int i = 0; i < n; i++)
        {
            remainingIndices.Add(i);
        }
        
        for (int i = 0; i < n - 1; i++)
        {
            int j = Random.Range(0, remainingIndices.Count);
            int nextIndex = remainingIndices[j];
            remainingIndices.RemoveAt(j);
            int nextTargetIndex = remainingIndices[Random.Range(0, remainingIndices.Count)];
            T temp = list[nextIndex];
            list[nextIndex] = list[nextTargetIndex];
            list[nextTargetIndex] = temp;
        }
        //
        // while (n > 1) {  
        //     n--;
        //     int k = Random.Range(0, n + 1);  
        //     T value = list[k];  
        //     list[k] = list[n];  
        //     list[n] = value;  
        // }  
    }
}
