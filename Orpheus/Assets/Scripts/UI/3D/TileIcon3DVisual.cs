using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TileIcon3DVisual : Icon3DVisual
{
    [SerializeField] private BuildingsVisualsSO buildingVisualsSO;
    [SerializeField] private TilesVisualsSO tileVisualsSO;
    [SerializeField] private Transform tileVisualsParent;

    private TileVisuals _tileVisualsInstance;
    
    public void Populate(TileInformation tileInformation)
    {
        Clear();
        
        TileVisualsData tileVisuals = tileVisualsSO.TilesVisualsData.FirstOrDefault((TileVisualsData data) =>
        {
            return data.Type == tileInformation.Type;
        });

        if (tileVisuals != null)
        {
            TileVisuals prefab = tileVisuals.Prefab;
            
            _tileVisualsInstance = Instantiate(prefab, tileVisualsParent);

            if (tileInformation.Buildings.Count > 0)
            {
                BuildingVisualsData buildingVisuals = buildingVisualsSO.BuildingsVisualsData.FirstOrDefault(
                    (BuildingVisualsData data) => { return data.Type == tileInformation.Buildings[0].Type; });

                
                if (buildingVisuals != null)
                {
                    BuildingBehaviour buildingPrefab = buildingVisuals.Prefab;

                    BuildingBehaviour buildingInstance = Instantiate(buildingPrefab, null);
                    
                    _tileVisualsInstance.AttachBuilding(buildingInstance);
                }
            }

            if (tileInformation.Resources.Count > 0)
            {
                _tileVisualsInstance.PopulateResourceVisuals(tileInformation.Resources);
            }
        }
    }

    private void Clear()
    {
        if (_tileVisualsInstance != null)
        {
            Destroy(_tileVisualsInstance.gameObject);
        }
    }
}
