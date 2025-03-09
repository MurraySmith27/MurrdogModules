using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class CityBorderVisuals : MonoBehaviour
{
    [SerializeField] private MeshFilter cityBorderMeshFilter;
    
    private Mesh _generatedCityBorderMesh;
    
    public void PopulateCityOwnedTiles(List<Vector2Int> ownedTiles)
    {
        if (_generatedCityBorderMesh != null)
        {
            _generatedCityBorderMesh.Clear();
        }
        
        _generatedCityBorderMesh = new Mesh();

        //TODO: Generate mesh, and apply the material, make the mesh bounds the outskirts of the border.
        
        int minX = Int32.MaxValue;
        int maxX = Int32.MinValue;
        int minY = Int32.MaxValue;
        int maxY = Int32.MinValue;

        foreach (Vector2Int tile in ownedTiles)
        {
            if (tile.x < minX) minX = tile.x;
            else if (tile.x > maxX) maxX = tile.x;
            
            if (tile.y < minY) minY = tile.y;
            else if (tile.y > maxY) maxY = tile.y;
        }
        
        bool[,] isOwned = new bool[1 + maxX - minX, 1 + maxY - minY];

        foreach (Vector2Int tile in ownedTiles)
        {
            isOwned[tile.x - minX, tile.y - minY] = true;
        }
        
        //create sets of all corners inside and all corners outside, the union of these sets is our set of vertices.
        HashSet<Vector3> cornerOfOutside = new HashSet<Vector3>();
        HashSet<Vector3> cornerOfInside = new HashSet<Vector3>();
        
        // HashSet<Vector3> innerVertices = new HashSet<Vector3>();

        int xDim = isOwned.GetLength(0);
        int yDim = isOwned.GetLength(1);
        for (int i = 0; i < xDim; i++)
        {
            for (int j = 0; j < yDim; j++)
            {
                List<Vector3> corners = GetCornersWorldSpace(i + minX, j + minY);
                
                if (i == 0 && j == 0)
                {
                    //add bottom left, top left, and top right
                    cornerOfOutside.Add(corners[0]);
                    cornerOfOutside.Add(corners[1]);
                    cornerOfOutside.Add(corners[3]);
                }
                else if (i == 0)
                {
                    //add bottom left
                    cornerOfOutside.Add(corners[0]);
                }
                else if (j == 0)
                {
                    //add top right
                    cornerOfOutside.Add(corners[2]);
                }
                
                if (i == xDim - 1 || j == yDim - 1)
                {
                    //add bottom right
                    cornerOfOutside.Add(corners[3]);
                }
                
                if (isOwned[i, j])
                {
                    cornerOfInside.AddRange(corners);
                    
                    // //also for owned tiles start to generate inner vertices
                    // if (i < isOwned.GetLength(0) - 1 && j < isOwned.GetLength(1) - 1 &&
                    //     isOwned[i + 1, j] && isOwned[i, j + 1] && isOwned[i + 1, j + 1])
                    // {
                    //     //if this is true, add bottom right corner, which is index 3
                    //     innerVertices.Add(corners[3]);
                    // }
                }
                else
                {
                    cornerOfOutside.AddRange(corners);
                }
            }
        }
        
        cornerOfOutside.IntersectWith(cornerOfInside);
        // cornerOfOutside.UnionWith(innerVertices);
        
        List<Vector3> vertices = cornerOfOutside.ToList();

        Vector3 centerPointWorldSpace =
            MapUtils.GetWorldPostiionFromPlanePosition(new Vector3((maxX - minX) / 2f + minX, 0,
                (maxY - minY) / 2f + minY));

        
        //convert the vertices to polar coordinates in order to sort them by angle
        List<Vector3> verticesPolarCoordinates = new List<Vector3>();

        foreach (Vector3 vertex in vertices)
        {
            float r = Vector3.Distance(vertex, centerPointWorldSpace);
            verticesPolarCoordinates.Add(new Vector3(Mathf.Acos((vertex.x - centerPointWorldSpace.x) / r), 0, r));
        }
        
        //sort
        verticesPolarCoordinates.Sort((Vector3 a, Vector3 b) =>
        {
            return a.x.CompareTo(b.x);
        });

        vertices.Clear();
        
        foreach (Vector3 vertex in verticesPolarCoordinates)
        {
            vertices.Add(new Vector3(vertex.z * Mathf.Cos(vertex.x), 0, vertex.z * Mathf.Sin(vertex.x)));
        }
        
        //add one extra vertex on the end for the center point
        vertices.Add(new Vector3(0,0,0));

        //now cornerOfOutside contains only our desired vertices, we now create triangles and UVs.
        int[] triangles = new int[vertices.Count - 1];
        Vector2[] uv = new Vector2[vertices.Count];
        
        for (int i = 0; i < vertices.Count; i++)
        {
            int nextIndex = (i + 1);

            if (nextIndex >= vertices.Count - 1)
            {
                nextIndex = 0;
            }

            triangles[i * 3] = i;
            triangles[i * 3 + 1] = nextIndex;
            triangles[i * 3 + 2] = vertices.Count - 1;
            
            //TODO: Add UVs
        }
        
        _generatedCityBorderMesh.vertices = vertices.ToArray();
        _generatedCityBorderMesh.triangles = triangles;
        
        cityBorderMeshFilter.mesh = _generatedCityBorderMesh;
    }

    private List<Vector3> GetCornersWorldSpace(int x, int y)
    {
        Vector3 centerPosition = MapUtils.GetTileWorldPositionFromGridPosition(new Vector2Int(x, y));
        
        List<Vector3> corners = new List<Vector3>();
        
        //goes bottom left, top left, top right, bottom right
        corners.Add(centerPosition + new Vector3(-(GameConstants.TILE_SIZE / 2f), 0, -(GameConstants.TILE_SIZE / 2f)));
        corners.Add(centerPosition + new Vector3((GameConstants.TILE_SIZE / 2f), 0, -(GameConstants.TILE_SIZE / 2f)));
        corners.Add(centerPosition + new Vector3((GameConstants.TILE_SIZE / 2f), 0, (GameConstants.TILE_SIZE / 2f)));
        corners.Add(centerPosition + new Vector3(-(GameConstants.TILE_SIZE / 2f), 0, (GameConstants.TILE_SIZE / 2f)));

        return corners;
    }
}
