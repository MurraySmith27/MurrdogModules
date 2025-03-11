using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloomingHarvestController : Singleton<BloomingHarvestController>
{
    [SerializeField] private float cityStartAnimationTime = 1f;
    [SerializeField] private float cityEndAnimationTime = 1f;
    [SerializeField] private float tileAnimationTime = 1f;

    public event Action OnHarvestStart;
    public event Action OnHarvestEnd;
    
    public event Action<Guid> OnCityHarvestStart;
    public event Action<Guid> OnCityHarvestEnd;
    
    public event Action<Vector2Int, Dictionary<ResourceType, int>> OnTileHarvestStart;
    public event Action<Vector2Int> OnTileHarvestEnd;
    
    public void StartHarvest()
    {
        StartCoroutine(HarvestCoroutine());
    }

    private IEnumerator HarvestCoroutine()
    {
        Dictionary<ResourceType, int> resources = new Dictionary<ResourceType, int>();

        foreach (ResourceType resourceType in Enum.GetValues(typeof(ResourceType)))
        {
            resources[resourceType] = 0;
        }
        
        OnHarvestStart?.Invoke();
        
        List<Guid> cityGuids = MapSystem.Instance.GetAllCityGuids();
        
        foreach (Guid cityGuid in cityGuids)
        {
            OnCityHarvestStart?.Invoke(cityGuid);
            
            yield return new WaitForSeconds(cityStartAnimationTime);

            List<Vector2Int> cityTiles = MapSystem.Instance.GetOwnedTilesOfCity(cityGuid);

            foreach (Vector2Int cityTile in cityTiles)
            {
                Dictionary<ResourceType, int> resourcesChange = TileHarvestController.Instance.GetResourceChangeOnTileHarvest(cityGuid, cityTile, resources);
                
                OnTileHarvestStart?.Invoke(cityTile, resourcesChange);

                foreach (KeyValuePair<ResourceType, int> resource in resourcesChange)
                {
                    resources[resource.Key] += resource.Value;
                }

                yield return new WaitForSeconds(tileAnimationTime);
                
                OnTileHarvestEnd?.Invoke(cityTile);
            }
            
            OnCityHarvestEnd?.Invoke(cityGuid);
            
            yield return new WaitForSeconds(cityEndAnimationTime);
        }
        
        OnHarvestEnd?.Invoke();
        
        PlayerResourcesSystem.Instance.RegisterCurrentRoundResources(resources);
    }
    
}
