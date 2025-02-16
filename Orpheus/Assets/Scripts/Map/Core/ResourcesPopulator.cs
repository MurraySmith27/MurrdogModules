using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourcesPopulator : Singleton<ResourcesPopulator>
{

    public void Start()
    {
        //TODO: Add callbgack to map system onchunkgenerated to populate the generated chunk with resources.
    }

    public void CanAddResourceToTile(Vector2Int position, ResourceType resourceType)
    {
        throw new NotImplementedException();
    }
    
    public void TileHasResource(Vector2Int position, ResourceType resourceType)
    {
        throw new NotImplementedException();
    }
}
