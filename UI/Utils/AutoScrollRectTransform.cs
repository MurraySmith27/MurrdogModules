using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AutoScrollRectTransform : MonoBehaviour
{
    [SerializeField] private ScrollRect scrollRect;
    
    [SerializeField] private float duration = 30f;

    [SerializeField] private float normalizedStartPosition = 1f;
    
    [SerializeField] private float normalizedEndPosition = 0f;

    private Coroutine _scrollCoroutine;
    
    public void Start()
    {
        scrollRect.verticalNormalizedPosition = normalizedStartPosition;

        if (_scrollCoroutine != null)
        {
            StopCoroutine(_scrollCoroutine);
        }

        _scrollCoroutine = StartCoroutine(ScrollCoroutine());
    }

    private IEnumerator ScrollCoroutine()
    {
        for (float t = 0; t <= duration; t += Time.unscaledDeltaTime)
        {
            scrollRect.verticalNormalizedPosition = Mathf.Lerp(normalizedStartPosition, normalizedEndPosition, t / duration);
            yield return null;
        }
    }
}
