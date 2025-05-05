using System.Collections;
using System.Collections.Generic;
using MEC;
using Unity.VisualScripting;
using UnityEngine;

public class ShadowOverlayVisuals : MonoBehaviour
{
    [SerializeField] private Material shadowDisintegrateMaterial;

    [SerializeField] private Renderer shadowRenderer;

    [SerializeField] private float disintegrateTime = 1f;

    private void Start()
    {
        MaterialPropertyBlock block = new MaterialPropertyBlock();
        
        shadowRenderer.GetPropertyBlock(block);
        
        block.SetFloat("_HorizontalScrollOffset", Random.Range(0, 1.0f));

        shadowRenderer.SetPropertyBlock(block);
    }

    public void OnDisappear()
    {
        Timing.RunCoroutineSingleton(DisintegrateCoroutine(1f), this.gameObject, SingletonBehavior.Overwrite);
    }

    public void OnReappear()
    {
        Timing.RunCoroutineSingleton(DisintegrateCoroutine(0f), this.gameObject, SingletonBehavior.Overwrite);   
    }

    private IEnumerator<float> DisintegrateCoroutine(float finalValue)
    {
        MaterialPropertyBlock block = new MaterialPropertyBlock();
        
        shadowRenderer.GetPropertyBlock(block);
        
        float initialValue = block.GetFloat("_DisintegrateProgress");
        
        for (float t = 0; t < disintegrateTime; t += Time.deltaTime)
        {
            block.SetFloat("_DisintegrateProgress", Mathf.Lerp(initialValue, finalValue, t / disintegrateTime));

            shadowRenderer.SetPropertyBlock(block);
            yield return Timing.WaitForOneFrame;
        }
        
        block.SetFloat("_DisintegrateProgress", finalValue);

        shadowRenderer.SetPropertyBlock(block);
        
        shadowRenderer.enabled = false;
    }
}
