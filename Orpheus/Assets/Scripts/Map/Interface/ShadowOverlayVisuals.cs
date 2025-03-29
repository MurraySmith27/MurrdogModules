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
        Timing.RunCoroutineSingleton(DisintegrateCoroutine(), this.gameObject, SingletonBehavior.Overwrite);
    }

    private IEnumerator<float> DisintegrateCoroutine()
    {
        MaterialPropertyBlock block = new MaterialPropertyBlock();
        
        shadowRenderer.GetPropertyBlock(block);
        
        
        for (float t = 0; t < disintegrateTime; t += Time.deltaTime)
        {
            block.SetFloat("_DisintegrateProgress", t / disintegrateTime);

            shadowRenderer.SetPropertyBlock(block);
            yield return Timing.WaitForOneFrame;
        }
        
        block.SetFloat("_DisintegrateProgress", 1f);

        shadowRenderer.SetPropertyBlock(block);
        
        shadowRenderer.enabled = false;
    }
}
