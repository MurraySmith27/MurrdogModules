using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class CityBorderVisuals : MonoBehaviour
{
    [SerializeField] private MeshFilter cityBorderMeshFilter;
    [SerializeField] private LineRenderer cityBorderOutskirtsLineRenderer;
    [SerializeField] private float borderLineYOffset;
    
    private Mesh _generatedCityBorderMesh;

    private List<GameObject> _instantiatedLineRendererObjects = new();

    private void OnDestroy()
    {
        Reset();
    }

    private void Reset()
    {
        foreach (GameObject lineRendererObject in _instantiatedLineRendererObjects)
        {
            Destroy(lineRendererObject);
        }
        
        _instantiatedLineRendererObjects.Clear();
    }
    
    public void PopulateCityOwnedTiles(List<Vector2Int> ownedTiles, Vector3 cityCenterPosition)
    {
        Reset();
        List<List<Vector3>> outskirtsLines = GenerateMeshForHex(ownedTiles, cityCenterPosition);
        cityBorderOutskirtsLineRenderer.gameObject.SetActive(false);

        foreach (List<Vector3> outskirtsLinePoints in outskirtsLines)
        {
            Vector3 centerPosition = Vector3.zero;

            foreach (Vector2Int position in ownedTiles)
            {
                centerPosition += MapUtils.GetTileWorldPositionFromGridPosition(position);
            }

            centerPosition /= ownedTiles.Count;

            Vector3 yOffset = new Vector3(0, borderLineYOffset, 0);
            //need to convert to world space
            for (int i = 0; i < outskirtsLinePoints.Count; i++)
            {
                outskirtsLinePoints[i] =
                    (Vector3)(cityBorderMeshFilter.transform.localToWorldMatrix * outskirtsLinePoints[i]) + yOffset;
            }
            
            GameObject newLineRenderer = Instantiate(cityBorderOutskirtsLineRenderer.gameObject, cityBorderOutskirtsLineRenderer.transform.parent);
            newLineRenderer.SetActive(true);
            LineRenderer lineRendererComponent = newLineRenderer.GetComponent<LineRenderer>();
            
            lineRendererComponent.positionCount = outskirtsLinePoints.Count;
            lineRendererComponent.SetPositions(outskirtsLinePoints.ToArray());
            
            _instantiatedLineRendererObjects.Add(newLineRenderer);

            transform.position = centerPosition;
            cityBorderMeshFilter.transform.position = cityCenterPosition + yOffset / 2f;
        }
    }

    private List<Vector3> GenerateMeshForTiles(List<Vector2Int> ownedTiles)
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
                List<Vector3> corners = GetCornersWorldSpaceForTile(i + minX, j + minY);
                
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
        
        Vector3 centerPointWorldSpace = 
            MapUtils.GetWorldPostiionFromPlanePosition(new Vector3(averageTilePosition.x + 0.5f, 0, averageTilePosition.y + 0.5f));
        
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
    
    private List<List<Vector3>> GenerateMeshForHex(List<Vector2Int> ownedTiles, Vector3 cityCenterPosition)
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

        float epsilon = 0.01f;

        bool IsPositionCloseEnough(Vector3 a, Vector3 b)
        {
            Vector3 diff = a - b;
            
            return Mathf.Abs(diff.x) < epsilon && Mathf.Abs(diff.z) < epsilon;
        }

        
        HashSet<(Vector3, Vector3)> borderEdges = new HashSet<(Vector3, Vector3)>();
        HashSet<(Vector3, Vector3)> internalEdges = new HashSet<(Vector3, Vector3)>();
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();

        int GetIndexOfVertex(Vector3 pos)
        {
            int i = 0;
            foreach (Vector3 position in vertices)
            {
                if (IsPositionCloseEnough(position, pos))
                {
                    return i;
                }

                i++;
            }

            return -1;
        }

        Vector3 WorldToObjectSpace(Vector3 worldPos)
        {
            return worldPos - cityCenterPosition;
        }
        
        foreach (Vector2Int tile in ownedTiles)
        {
            List<Vector3> corners = GetCornersWorldSpaceForHex(tile.x, tile.y);

            List<int> indicesInOrder = new();

            foreach (Vector3 corner in corners)
            {
                int index = GetIndexOfVertex(corner);
                if (index == -1)
                {
                    vertices.Add(WorldToObjectSpace(corner));
                    indicesInOrder.Add(vertices.Count - 1);
                }
                else
                {
                    indicesInOrder.Add(index);
                }
            }
            
            //now that we have indices, add a center vertex and create triangles
            Vector3 centerPosition = MapUtils.GetTileWorldPositionFromGridPosition(new Vector2Int(tile.x, tile.y));
            
            vertices.Add(WorldToObjectSpace(centerPosition));

            for (int i = 0; i < indicesInOrder.Count; i++)
            {
                if (i == indicesInOrder.Count - 1)
                {
                    triangles.Add(indicesInOrder[0]);
                }
                else
                {
                    triangles.Add(indicesInOrder[i] + 1);
                }
                triangles.Add(vertices.Count - 1); //index of center position of hex
                triangles.Add(indicesInOrder[i]);
            }
            
            if (!borderEdges.Contains((corners[0], corners[1])) && !internalEdges.Contains((corners[0], corners[1])))
            {
                if (!ownedTiles.Contains(new Vector2Int(tile.x, tile.y + 1)))
                {
                    borderEdges.Add((corners[0], corners[1]));
                }
                else
                {
                    internalEdges.Add((corners[0], corners[1]));
                }
            }
            
            if (!borderEdges.Contains((corners[1], corners[2])) && !internalEdges.Contains((corners[1], corners[2])))
            {
                if (!ownedTiles.Contains(new Vector2Int(tile.x + 1, tile.y)))
                {
                    borderEdges.Add((corners[1], corners[2]));
                }
                else {
                    internalEdges.Add((corners[1], corners[2]));    
                }
            }
            
            if (!borderEdges.Contains((corners[2], corners[3])) && !internalEdges.Contains((corners[2], corners[3])))
            {
                if (!ownedTiles.Contains(new Vector2Int(tile.x + 1, tile.y - 1)))
                {
                    borderEdges.Add((corners[2], corners[3]));
                }
                else {
                    internalEdges.Add((corners[2], corners[3]));    
                }
            }

            if (!borderEdges.Contains((corners[3], corners[4])) && !internalEdges.Contains((corners[3], corners[4])))
            {
                if (!ownedTiles.Contains(new Vector2Int(tile.x, tile.y - 1)))
                {
                    borderEdges.Add((corners[3], corners[4]));
                }
                else {
                    internalEdges.Add((corners[3], corners[4]));    
                }
            }
            
            if (!borderEdges.Contains((corners[4], corners[5])) && !internalEdges.Contains((corners[4], corners[5])))
            {
                if (!ownedTiles.Contains(new Vector2Int(tile.x - 1, tile.y)))
                {
                    borderEdges.Add((corners[4], corners[5]));
                }
                else {
                    internalEdges.Add((corners[4], corners[5]));    
                }
            }
            
            if (!borderEdges.Contains((corners[5], corners[0])) && !internalEdges.Contains((corners[5], corners[0])))
            {
                if (!ownedTiles.Contains(new Vector2Int(tile.x - 1, tile.y + 1)))
                {
                    borderEdges.Add((corners[5], corners[0]));
                }
                else {
                    internalEdges.Add((corners[5], corners[0]));
                }
            }
        }
        
        List<List<Vector3>> cornersInOrder = new List<List<Vector3>>();
        List<(Vector3, Vector3)> borderEdgesList = borderEdges.ToList();

        List<Vector3> currentCornersInOrder = new();
        currentCornersInOrder.Add(borderEdgesList[0].Item2);
        
        int numFound = 1;
        int numToFind = borderEdgesList.Count;
        
        borderEdgesList.RemoveAt(0);
        
        while (numFound < numToFind)
        {
            bool found = false;
            for (int j = 0; j < borderEdgesList.Count; j++)
            {
                if (IsPositionCloseEnough(currentCornersInOrder[^1], borderEdgesList[j].Item1))
                {
                    currentCornersInOrder.Add(borderEdgesList[j].Item2);
                    found = true;
                    numFound++;
                    borderEdgesList.RemoveAt(j);
                    break;
                }
            }

            if (!found)
            {
                cornersInOrder.Add(currentCornersInOrder);

                currentCornersInOrder = new();
                currentCornersInOrder.Add(borderEdgesList[0].Item2);
                borderEdgesList.RemoveAt(0);
                numFound++;
            }
        }
        
        cornersInOrder.Add(currentCornersInOrder);

        // List<Vector3> verticesWorldSpace = cornersInOrder[0];
        //
        // float width = 1 + maxX - minX;
        // float height = 1 + maxY - minY;
        //
        // Vector3 centerPointWorldSpace = 
        //     MapUtils.GetWorldPostiionFromPlanePosition(new Vector3(averageTilePosition.x, 0, averageTilePosition.y));
        //
        // //convert the vertices to polar coordinates in order to sort them by angle
        // List<Vector3> verticesPolarCoordinates = new List<Vector3>();
        //
        // foreach (Vector3 vertex in verticesWorldSpace)
        // {
        //     Vector3 relativeVertex = vertex - centerPointWorldSpace;
        //
        //     float r = relativeVertex.magnitude;
        //     
        //     Vector3 newPolarVertex = (new Vector3(Mathf.Acos(relativeVertex.x / r), 0, r));
        //     if (relativeVertex.z < 0)
        //     {
        //         newPolarVertex = new Vector3(newPolarVertex.x + (Mathf.PI - newPolarVertex.x) * 2f, 0, newPolarVertex.z);
        //     }
        //     
        //     verticesPolarCoordinates.Add(newPolarVertex);
        // }
        //
        // //sort
        // verticesPolarCoordinates.Sort((Vector3 a, Vector3 b) =>
        // {
        //     return a.x.CompareTo(b.x);
        // });
        //
        // List<Vector3> verticesObjectSpace = new List<Vector3>();
        //
        // foreach (Vector3 vertex in verticesPolarCoordinates)
        // {
        //     verticesObjectSpace.Add(new Vector3(vertex.z * Mathf.Cos(vertex.x), 0, vertex.z * Mathf.Sin(vertex.x)));
        // }
        //
        // //add one extra vertex on the end for the center point
        // verticesObjectSpace.Add(new Vector3(0,0,0));
        //
        // //now cornerOfOutside contains only our desired vertices, we now create triangles and UVs.
        // // int[] triangles = new int[(verticesObjectSpace.Count - 1) * 3];
        // Vector2[] uv = new Vector2[verticesObjectSpace.Count];
        //
        // for (int i = 0; i < verticesObjectSpace.Count - 1; i++)
        // {
        //     int nextIndex = (i + 1);
        //
        //     if (nextIndex >= verticesObjectSpace.Count - 1)
        //     {
        //         nextIndex = 0;
        //     }
        //
        //     triangles[i * 3] = verticesObjectSpace.Count - 1;
        //     triangles[i * 3 + 1] = nextIndex;
        //     triangles[i * 3 + 2] = i;
        //     
        //     uv[i] = new Vector2((verticesObjectSpace[i].x + width / 2f) / width, (verticesObjectSpace[i].z + height / 2f) / height);
        // }
        //
        // uv[verticesObjectSpace.Count - 1] = new Vector2(0.5f, 0.5f);
        
        _generatedCityBorderMesh.vertices = vertices.ToArray();
        _generatedCityBorderMesh.triangles = triangles.ToArray();
        _generatedCityBorderMesh.uv = new Vector2[vertices.Count];
        
        cityBorderMeshFilter.mesh = _generatedCityBorderMesh;

        return cornersInOrder;
    }

    private List<Vector3> GetCornersWorldSpaceForTile(int x, int y)
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
    
    private List<Vector3> GetCornersWorldSpaceForHex(int x, int y)
    {
        Vector3 centerPosition = MapUtils.GetTileWorldPositionFromGridPosition(new Vector2Int(x, y));
        
        return HexUtils.GetCornersOfHexWorldSpace(centerPosition);
    }
}
