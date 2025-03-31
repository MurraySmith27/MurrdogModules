using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class CityBorderVisuals : MonoBehaviour
{
    [SerializeField] private MeshFilter cityBorderMeshFilter;
    [SerializeField] private LineRenderer cityBorderOutskirtsLineRenderer;
    [SerializeField] private float borderLineYOffset;
    
    private Mesh _generatedCityBorderMesh;
    
    public void PopulateCityOwnedTiles(List<Vector2Int> ownedTiles)
    {
        List<Vector3> outskirtsLinePoints = GenerateMesh(ownedTiles);
        
        Vector2Int centerPosition = Vector2Int.zero;

        foreach (Vector2Int position in ownedTiles)
        {
            centerPosition += position;
        }

        centerPosition /= ownedTiles.Count;
        
        Vector3 centerPositionWorldSpace = MapUtils.GetTileWorldPositionFromGridPosition(centerPosition);
        
        Vector3 yOffset = new Vector3(0, borderLineYOffset, 0);
        //need to convert to world space
        for (int i = 0; i < outskirtsLinePoints.Count; i++)
        {
            outskirtsLinePoints[i] = (Vector3)(cityBorderMeshFilter.transform.localToWorldMatrix * outskirtsLinePoints[i]) + cityBorderMeshFilter.transform.position + yOffset;
        }
        
        cityBorderOutskirtsLineRenderer.positionCount = outskirtsLinePoints.Count;
        cityBorderOutskirtsLineRenderer.SetPositions(outskirtsLinePoints.ToArray());

        transform.position = centerPositionWorldSpace;
    }

    private List<Vector3> GenerateMesh(List<Vector2Int> ownedTiles)
    {
        if (_generatedCityBorderMesh != null)
        {
            _generatedCityBorderMesh.Clear();
        }
        
        _generatedCityBorderMesh = new Mesh();
        
        int minX = Int32.MaxValue;
        int maxX = Int32.MinValue;
        int minY = Int32.MaxValue;
        int maxY = Int32.MinValue;

        Vector2 averageTilePosition = Vector2.zero;

        foreach (Vector2Int tile in ownedTiles)
        {
            if (tile.x < minX) minX = tile.x;
            if (tile.x > maxX) maxX = tile.x;
            
            if (tile.y < minY) minY = tile.y;
            if (tile.y > maxY) maxY = tile.y;

            averageTilePosition += tile;
        }

        averageTilePosition /= (float)ownedTiles.Count;
        
        bool[,] isOwned = new bool[1 + maxX - minX, 1 + maxY - minY];

        foreach (Vector2Int tile in ownedTiles)
        {
            isOwned[tile.x - minX, tile.y - minY] = true;
        }
        
        //create sets of all corners inside and all corners outside, the union of these sets is our set of vertices.
        HashSet<Vector3> cornerOfOutside = new HashSet<Vector3>();
        HashSet<Vector3> cornerOfInside = new HashSet<Vector3>();

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
                    cornerOfOutside.Add(corners[3]);
                }
                else if (j == 0)
                {
                    //add top right
                    cornerOfOutside.Add(corners[1]);
                }
                
                if (i == xDim - 1 || j == yDim - 1)
                {
                    //add bottom right
                    cornerOfOutside.Add(corners[2]);
                }
                
                if (isOwned[i, j])
                {
                    cornerOfInside.AddRange(corners);
                }
                else
                {
                    cornerOfOutside.AddRange(corners);
                }
            }
        }
        
        cornerOfOutside.IntersectWith(cornerOfInside);
        
        List<Vector3> verticesWorldSpace = cornerOfOutside.ToList();
        
        float width = 1 + maxX - minX;
        float height = 1 + maxY - minY;
        
        Debug.LogError($"width: {width}");
        Debug.LogError($"height: {height}");
        
        Debug.LogError($"maxX: {maxX}, minX: {minX}");
        
        Debug.LogError($"maxY: {maxY}, minY: {minY}");
        
        Vector3 centerPointWorldSpace = 
            MapUtils.GetWorldPostiionFromPlanePosition(new Vector3(averageTilePosition.x, 0, averageTilePosition.y));
        
        //convert the vertices to polar coordinates in order to sort them by angle
        List<Vector3> verticesPolarCoordinates = new List<Vector3>();

        foreach (Vector3 vertex in verticesWorldSpace)
        {
            Vector3 relativeVertex = vertex - centerPointWorldSpace;

            float r = relativeVertex.magnitude;
            
            Vector3 newPolarVertex = (new Vector3(Mathf.Acos(relativeVertex.x / r), 0, r));
            if (relativeVertex.z < 0)
            {
                newPolarVertex = new Vector3(newPolarVertex.x + (Mathf.PI - newPolarVertex.x) * 2f, 0, newPolarVertex.z);
            }
            
            verticesPolarCoordinates.Add(newPolarVertex);
        }
        
        //sort
        verticesPolarCoordinates.Sort((Vector3 a, Vector3 b) =>
        {
            return a.x.CompareTo(b.x);
        });

        List<Vector3> verticesObjectSpace = new List<Vector3>();
        
        foreach (Vector3 vertex in verticesPolarCoordinates)
        {
            verticesObjectSpace.Add(new Vector3(vertex.z * Mathf.Cos(vertex.x), 0, vertex.z * Mathf.Sin(vertex.x)));
        }
        
        //add one extra vertex on the end for the center point
        verticesObjectSpace.Add(new Vector3(0,0,0));

        //now cornerOfOutside contains only our desired vertices, we now create triangles and UVs.
        int[] triangles = new int[(verticesObjectSpace.Count - 1) * 3];
        Vector2[] uv = new Vector2[verticesObjectSpace.Count];
        
        for (int i = 0; i < verticesObjectSpace.Count - 1; i++)
        {
            int nextIndex = (i + 1);

            if (nextIndex >= verticesObjectSpace.Count - 1)
            {
                nextIndex = 0;
            }

            triangles[i * 3] = verticesObjectSpace.Count - 1;
            triangles[i * 3 + 1] = nextIndex;
            triangles[i * 3 + 2] = i;
            
            uv[i] = new Vector2((verticesObjectSpace[i].x + width / 2f) / width, (verticesObjectSpace[i].z + height / 2f) / height);
        }

        uv[verticesObjectSpace.Count - 1] = new Vector2(0.5f, 0.5f);
        
        _generatedCityBorderMesh.vertices = verticesObjectSpace.ToArray();
        _generatedCityBorderMesh.triangles = triangles;
        _generatedCityBorderMesh.uv = uv;
        
        cityBorderMeshFilter.mesh = _generatedCityBorderMesh;

        //return all object space vertices, except the center one
        return verticesObjectSpace.GetRange(0, verticesObjectSpace.Count - 1);
    }

    private List<Vector3> GetCornersWorldSpace(int x, int y)
    {
        Vector3 centerPosition = MapUtils.GetTileWorldPositionFromGridPosition(new Vector2Int(x, y)) + new Vector3(GameConstants.TILE_SIZE / 2f, 0f, GameConstants.TILE_SIZE / 2f);
        
        List<Vector3> corners = new List<Vector3>();
        
        //goes bottom left, top left, top right, bottom right
        corners.Add(centerPosition + new Vector3(-(GameConstants.TILE_SIZE / 2f), 0, -(GameConstants.TILE_SIZE / 2f)));
        corners.Add(centerPosition + new Vector3((GameConstants.TILE_SIZE / 2f), 0, -(GameConstants.TILE_SIZE / 2f)));
        corners.Add(centerPosition + new Vector3((GameConstants.TILE_SIZE / 2f), 0, (GameConstants.TILE_SIZE / 2f)));
        corners.Add(centerPosition + new Vector3(-(GameConstants.TILE_SIZE / 2f), 0, (GameConstants.TILE_SIZE / 2f)));

        return corners;
    }
}
