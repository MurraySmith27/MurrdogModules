using System.Collections;
using System.Collections.Generic;
using MEC;
using UnityEngine;

public class TileHoveredOverIndicator : MonoBehaviour
{
    [SerializeField] private float animateToPositionDuration = 0.1f;
    [SerializeField] private AnimationCurve animateToPositionCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
    
    private void Start()
    {
        MapInteractionController.Instance.OnTileHoveredOver -= OnTileHoveredOver;
        MapInteractionController.Instance.OnTileHoveredOver += OnTileHoveredOver;

        MapInteractionController.Instance.OnMapInteractionModeChanged -= OnMapInteractionModeChanged;
        MapInteractionController.Instance.OnMapInteractionModeChanged += OnMapInteractionModeChanged;
    }

    private void OnDestroy()
    {
        if (MapInteractionController.IsAvailable)
        {
            MapInteractionController.Instance.OnTileSelected -= OnTileHoveredOver;
            MapInteractionController.Instance.OnMapInteractionModeChanged -= OnMapInteractionModeChanged;
        }
    }

    private Vector3 _nextPostion;    
    
    private void OnTileHoveredOver(TileVisuals tile, Vector2Int position)
    {
        if (tile != null)
        {
            if (transform.position == new Vector3(0, -100, 0))
            {
                transform.position = tile.transform.position;
                _nextPostion = tile.transform.position;
            }
            else
            {
                transform.position = _nextPostion;
                _nextPostion = tile.transform.position;
                Timing.RunCoroutineSingleton(AnimateToPosition(_nextPostion).CancelWith(this.gameObject),
                    this.gameObject,
                    SingletonBehavior.Overwrite);
            }
        }
        else
        {
            StopAllCoroutines();
            transform.position = new Vector3(0, -100, 0);
        }
    }

    private IEnumerator<float> AnimateToPosition(Vector3 position)
    {
        Vector3 initialPosition = transform.position;
        
        for (float t = 0f; t < animateToPositionDuration; t += Time.deltaTime)
        {
            float progress = animateToPositionCurve.Evaluate(t / animateToPositionDuration);
            
            transform.position = Vector3.Lerp(initialPosition, position, progress);

            yield return Timing.WaitForOneFrame;
        }

        transform.position = position;
    }

    private void OnMapInteractionModeChanged(MapInteractionMode mapInteractionMode)
    {
        transform.position = new Vector3(0, -100, 0);
    }
}
