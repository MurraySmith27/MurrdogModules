using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CitizenIconsOverlayController : MonoBehaviour
{
    [SerializeField] private CitizenLockIcon citizenLockIconPrefab;
    
    [SerializeField] private RectTransform iconsParent;

    [SerializeField] private float iconsDistanceTowardCamera = 1f;
    
    [SerializeField] private float iconsScale = 1f;

    private Dictionary<Vector2Int, CitizenLockIcon> _instantiatedIcons = new();
    
    private void Start()
    {
        TileFrustrumCulling.Instance.OnTileCullingUpdated -= OnTileCullingChanged;
        TileFrustrumCulling.Instance.OnTileCullingUpdated += OnTileCullingChanged;

        CitizenController.Instance.OnCitizenAddedToTile -= OnCitizenAddedToTile;
        CitizenController.Instance.OnCitizenAddedToTile += OnCitizenAddedToTile;
        
        CitizenController.Instance.OnCitizenRemovedFromTile -= OnCitizenRemovedFromTile;
        CitizenController.Instance.OnCitizenRemovedFromTile += OnCitizenRemovedFromTile;

        CitizenController.Instance.OnCitizenLocked -= OnCitizenLocked;
        CitizenController.Instance.OnCitizenLocked += OnCitizenLocked;
        
        CitizenController.Instance.OnCitizenUnlocked -= OnCitizenUnlocked;
        CitizenController.Instance.OnCitizenUnlocked += OnCitizenUnlocked;
    }

    private void OnDestroy()
    {
        if (TileFrustrumCulling.IsAvailable)
        {
            TileFrustrumCulling.Instance.OnTileCullingUpdated -= OnTileCullingChanged;
        }

        if (CitizenController.IsAvailable)
        {
            CitizenController.Instance.OnCitizenAddedToTile -= OnCitizenAddedToTile;
            CitizenController.Instance.OnCitizenRemovedFromTile -= OnCitizenRemovedFromTile;
            CitizenController.Instance.OnCitizenLocked -= OnCitizenLocked;
            CitizenController.Instance.OnCitizenUnlocked -= OnCitizenUnlocked;
        }
    }

    private void OnTileCullingChanged(int row, int col, int width, int height)
    {
        RectInt positionRect = new RectInt(row, col, width, height);

        List<Vector2Int> removedPositions = new();
        //first clear all instantiated icons:
        foreach (Vector2Int position in _instantiatedIcons.Keys)
        {
            if (!positionRect.Contains(position))
            {
                CitizenLockIcon icon = _instantiatedIcons[position];
                
                icon.Hide(() =>
                {
                    Destroy(icon.gameObject);
                });
                
                removedPositions.Add(position);
            }
        }

        foreach (Vector2Int position in removedPositions)
        {
            _instantiatedIcons.Remove(position);
        }

        List<CitizenController.CitizenPlacement> citizenPositions = CitizenController.Instance.GetAllCitizenPositions();

        foreach (CitizenController.CitizenPlacement placement in citizenPositions)
        {
            if (positionRect.Contains(placement.Position) && !_instantiatedIcons.ContainsKey(placement.Position))
            {
                SpawnIconAtTile(placement);
            }
        }
    }

    private void OnCitizenAddedToTile(Guid cityGuid, Vector2Int position)
    {
        bool isLocked = CitizenController.Instance.IsCitizenAtTileLocked(position);
        
        SpawnIconAtTile(position, isLocked);
    }
    
    private void OnCitizenRemovedFromTile(Guid cityGuid, Vector2Int position)
    {
        if (_instantiatedIcons.ContainsKey(position))
        {
            CitizenLockIcon temp = _instantiatedIcons[position];
            temp.Hide(() =>
            {
               Destroy(temp.gameObject);
            });
            
            _instantiatedIcons.Remove(position);
        }
    }

    private void OnCitizenLocked(Vector2Int position)
    {
        if (_instantiatedIcons.ContainsKey(position))
        {
            _instantiatedIcons[position].ToggleLocked(true);
        }
    }
    
    private void OnCitizenUnlocked(Vector2Int position)
    {
        if (_instantiatedIcons.ContainsKey(position))
        {
            _instantiatedIcons[position].ToggleLocked(false);
        }
    }

    private void SpawnIconAtTile(Vector2Int position, bool isLocked)
    {
        SpawnIconAtTile(new CitizenController.CitizenPlacement(position, isLocked));
    }
    
    private void SpawnIconAtTile(CitizenController.CitizenPlacement placement)
    {
        if (_instantiatedIcons.ContainsKey(placement.Position))
        {
            _instantiatedIcons[placement.Position].ToggleLocked(placement.IsLocked);
            return;
        }
        
        CitizenLockIcon instance = Instantiate(citizenLockIconPrefab, iconsParent);
        
        instance.transform.localScale = Vector3.one * iconsScale;
        
        Vector3 tileWorldPosition = MapUtils.GetTileWorldPositionFromGridPosition(placement.Position);

        Vector3 canvasPosition = iconsParent.InverseTransformPoint(tileWorldPosition);

        instance.transform.localPosition = canvasPosition;
        
        instance.ToggleLocked(placement.IsLocked);

        instance.Show();
        
        _instantiatedIcons.Add(placement.Position, instance);
    }
}
