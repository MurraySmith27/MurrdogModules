using System;
using System.Collections;
using System.Collections.Generic;
using Simplex.Procedures;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Experimental.Rendering;

public class SignedDistanceFieldGenerator : Singleton<SignedDistanceFieldGenerator>
{
    public struct Pixel
    {
        public float distance;
    }
    
    [SerializeField] private string m_rendererPropertyName = "_TopDownDistanceField";

    [SerializeField] private string m_prevTextureRendererPropertyName = "_PrevTopDownDistanceField";
    
    [SerializeField] private float m_maxPixelSquaredDistanceDf = 50f;
    
    [SerializeField] float _sourceValueThreshold = 0.5f;
    [SerializeField] ScalarTextureToSdfTextureProcedure.DownSampling _downSampling = ScalarTextureToSdfTextureProcedure.DownSampling.None;
    [SerializeField] ScalarTextureToSdfTextureProcedure.Precision _precision = ScalarTextureToSdfTextureProcedure.Precision._32;
    
    private int m_texWidth, m_texHeight;

    private Pixel[] m_texBuffer;

    [SerializeField] private SDFMaskCameraDepthWriter m_SDFMaskCameraDepthWriter;

    private ScalarTextureToSdfTextureProcedure m_scalarTextureToSdfProcedure1;
    private ScalarTextureToSdfTextureProcedure m_scalarTextureToSdfProcedure2;

    private UnityEvent<RenderTexture> _sdfTextureEvent = new UnityEvent<RenderTexture>();

    private Texture2D distanceField1;
    
    private Texture2D distanceField2;

    private bool useDistanceField1;

    public Action<RenderTexture> OnSDFUpdated;

    
    private void Start()
    {
        SDFMaskCameraDepthWriter.Instance.m_onSDFDepthTextureChanged += OnSDFDepthTextureChanged;

        m_scalarTextureToSdfProcedure1 = new ScalarTextureToSdfTextureProcedure();
        m_scalarTextureToSdfProcedure2 = new ScalarTextureToSdfTextureProcedure();
        // _sdfTextureEvent.AddListener(ApplyPixelDataToRendererMaterialBlock);
    }

    private void OnSDFDepthTextureChanged(Texture2D depthTexture)
    {
    
        if (useDistanceField1)
        {
            m_scalarTextureToSdfProcedure1.Update( depthTexture, _sourceValueThreshold, _downSampling, _precision, false, false );
            _sdfTextureEvent.Invoke( m_scalarTextureToSdfProcedure1.sdfTexture );
            
            OnSDFUpdated?.Invoke( m_scalarTextureToSdfProcedure1.sdfTexture );
        }
        else
        {
            m_scalarTextureToSdfProcedure2.Update( depthTexture, _sourceValueThreshold, _downSampling, _precision, false, false );
            _sdfTextureEvent.Invoke( m_scalarTextureToSdfProcedure2.sdfTexture );

            OnSDFUpdated?.Invoke( m_scalarTextureToSdfProcedure2.sdfTexture );
        }

        useDistanceField1 = !useDistanceField1;

        // m_texWidth = depthTexture.width;
        // m_texHeight = depthTexture.height;
        //
        // m_texBuffer = new Pixel[m_texWidth * m_texHeight];
        //
        // for (int i = 0; i < m_texBuffer.Length; i++)
        // {
        //     m_texBuffer[i].distance = float.MaxValue;
        // }
        //
        // Color[] texpixels = depthTexture.GetPixels();
        //
        // for (int i = 0; i < m_texBuffer.Length; i++)
        // {
        //     if (texpixels[i].r > 0.5f)
        //         m_texBuffer[i].distance = -99999f;
        //     else
        //         m_texBuffer[i].distance = 99999f;
        // }


        // MaterialPropertyBlock block = new MaterialPropertyBlock();
        // m_sdfDestinationRenderer.GetPropertyBlock(block);
        //
        // block.SetTexture(m_rendererPropertyName, depthTexture);
        //
        // m_sdfDestinationRenderer.SetPropertyBlock(block);
    }

    private void JumpFloodingAlgorithm(Texture2D tex)
    {
        Color[] pixelData = tex.GetPixels();

        int[] seedPixels = new int[pixelData.Length];

        for(int i = 0; i < pixelData.Length; i++)
        {
            if (pixelData[i].r > 0)
            {
                pixelData[i].r = 1f;
                pixelData[i].g = 1f;
                seedPixels[i] = i;
            }
            else
            {
                pixelData[i].g = 0f;
                seedPixels[i] = -1;
            }
        }

        float SquaredDistanceToSeed(int x, int y, int seed)
        {
            int seedX = seed % tex.width;
            int seedY = Mathf.FloorToInt(seed / (float)tex.width);
            float distanceFromPToSeedX = x - seedX;
            float distanceFromPToSeedY = y - seedY;
            float squaredDistanceFromPToSeed =
                (distanceFromPToSeedX * distanceFromPToSeedX) +
                (distanceFromPToSeedY * distanceFromPToSeedY);
            return squaredDistanceFromPToSeed;
        }

        float d = 2f;
        int k = Mathf.FloorToInt(tex.width * tex.height / d);
        while (k > 0)
        {
            
            for (int y = 0; y < tex.height; y++)
            {
                for (int x = 0; x < tex.width; x++)
                {
                    Color p = pixelData[y * tex.width + x];
                    
                    for (int xoffset = -k; xoffset < k+1; xoffset += k)
                    {
                        if (x + xoffset < 0 || x + xoffset >= tex.width)
                        {
                            continue;
                        }
                        for (int yoffset = -k; yoffset < k + 1; yoffset += k)
                        {
                            if (y + yoffset < 0 || y + yoffset >= tex.height)
                            {
                                continue;
                            }
                            Color q = pixelData[(y + yoffset) * tex.width + (x + xoffset)];

                            bool pDefined = p.g > 0;
                            bool qDefined = q.g > 0;
                            
                            if (qDefined)
                            {
                                int seedOfQ = seedPixels[(y + yoffset) * tex.width + (x + xoffset)];

                                float squaredDistanceFromPToSeedOfQ = SquaredDistanceToSeed(x, y, seedOfQ);
                                if (!pDefined)
                                {
                                    p.r = Mathf.Min(1f, (squaredDistanceFromPToSeedOfQ / m_maxPixelSquaredDistanceDf));
                                    p.g = 1;
                                    seedPixels[y * tex.width + x] = seedPixels[(y + yoffset) * tex.width + (x + xoffset)];
                                }
                                
                                int seed = seedPixels[y * tex.width + x];
                                float squaredDistanceFromPToSeedOfP =
                                    SquaredDistanceToSeed(x, y, seed);

                                if (squaredDistanceFromPToSeedOfP > squaredDistanceFromPToSeedOfQ)
                                {
                                    p.r = Mathf.Min(1f, (squaredDistanceFromPToSeedOfQ / m_maxPixelSquaredDistanceDf));
                                    seedPixels[seed] = seedOfQ;
                                }
                            }
                        }
                    }
                }
            }
            
            d *= 2f;
            k = Mathf.FloorToInt(tex.width * tex.height / d);
        }

        tex.SetPixels(pixelData);
    }
    
    public void Sweep()
    {
        //clean the field so any none edge pixels simply contain 99999 for outer
        //pixels, or -99999 for inner pixels
        ClearNoneEdgePixels();

        //seperate the field into 2 grids - 1 for inner pixels and 1 for outer pixels
        float[] outside_grid,inside_grid;
        BuildSweepGrids(out outside_grid, out inside_grid);

        //run the 8PSSEDT sweep on each grid
        SweepGrid(outside_grid);
        SweepGrid(inside_grid);

        //write results back
        for (int i = 0; i < m_texBuffer.Length; i++)
            m_texBuffer[i].distance = outside_grid[i] - inside_grid[i];
    }

    private bool IsOuterPixel(int x, int y)
    {
        if (x < 0 || x >= m_texWidth || y < 0 || y >= m_texHeight)
        {
            return true;
        }
        else
        {
            return GetPixel(x, y).distance >= 0;
        }
    }

    bool IsEdgePixel(int x, int y)
    {
        bool is_outer = IsOuterPixel(x, y);
        if (is_outer != IsOuterPixel(x - 1, y - 1)) return true; //[-1,-1]
        if (is_outer != IsOuterPixel(x, y - 1)) return true;     //[ 0,-1]
        if (is_outer != IsOuterPixel(x + 1, y - 1)) return true; //[+1,-1]
        if (is_outer != IsOuterPixel(x - 1, y)) return true;     //[-1, 0]
        if (is_outer != IsOuterPixel(x + 1, y)) return true;     //[+1, 0]
        if (is_outer != IsOuterPixel(x - 1, y + 1)) return true; //[-1,+1]
        if (is_outer != IsOuterPixel(x, y + 1)) return true;     //[ 0,+1]
        if (is_outer != IsOuterPixel(x + 1, y + 1)) return true; //[+1,+1]
        return false;
    }

    public void ClearNoneEdgePixels()
    {
        for (int y = 0; y < m_texHeight; y++)
        {
            for (int x = 0; x < m_texWidth; x++)
            {
                Pixel pix = GetPixel(x, y);
                bool isEdge = IsEdgePixel(x, y);
                if (!isEdge)
                    pix.distance = pix.distance > 0 ? 99999f : -99999f;
                SetPixel(x,y,pix);
            }
        }
    }
    
    void BuildSweepGrids(out float[] outside_grid, out float[] inside_grid)
    {
        outside_grid = new float[m_texBuffer.Length];
        inside_grid = new float[m_texBuffer.Length];
        for (int i = 0; i < m_texBuffer.Length; i++)
        {
            if (m_texBuffer[i].distance < 0)
            {
                //inside pixel. outer distance is set to 0, inner distance
                //is preserved (albeit negated to make it positive)
                outside_grid[i] = 0f;
                inside_grid[i] = -m_texBuffer[i].distance;
            }
            else
            {
                //outside pixel. inner distance is set to 0,
                //outer distance is preserved
                inside_grid[i] = 0f;
                outside_grid[i] = m_texBuffer[i].distance;
            }
        }
    }
    
    public void Compare(float[] grid, int x, int y, int xoffset, int yoffset)
    {
        //calculate the location of the other pixel, and bail if in valid
        int otherx = x + xoffset;
        int othery = y + yoffset;
        if (otherx < 0 || otherx >= m_texWidth || othery < 0 || othery >= m_texHeight)
            return;

        //read the distance values stored in both this and the other pixel
        float curr_dist = grid[y * m_texWidth + x];
        float other_dist = grid[othery * m_texWidth + otherx];

        //calculate a potential new distance, using the one stored in the other pixel,
        //PLUS the distance to the other pixel
        float new_dist = other_dist + Mathf.Sqrt(xoffset * xoffset + yoffset * yoffset);

        //if the potential new distance is better than our current one, update!
        if (new_dist < curr_dist)
            grid[y * m_texWidth + x] = new_dist;
    }
    
    public void SweepGrid(float[] grid)
    {
        // Pass 0
        //loop over rows from top to bottom
        for (int y = 0; y < m_texHeight; y++)
        {
            //loop over pixels from left to right
            for (int x = 0; x < m_texWidth; x++)
            {
                Compare(grid, x, y, -1, 0);
                Compare(grid, x, y, 0, -1);
                Compare(grid, x, y, -1, -1);
                Compare(grid, x, y, 1, -1);
            }

            //loop over pixels from right to left
            for (int x = m_texWidth - 1; x >= 0; x--)
            {
                Compare(grid, x, y, 1, 0);
            }
        }

        // Pass 1
        //loop over rows from bottom to top
        for (int y = m_texHeight - 1; y >= 0; y--)
        {
            //loop over pixels from right to left
            for (int x = m_texWidth - 1; x >= 0; x--)
            {
                Compare(grid, x, y, 1, 0);
                Compare(grid, x, y, 0, 1);
                Compare(grid, x, y, -1, 1);
                Compare(grid, x, y, 1, 1);
            }

            //loop over pixels from left to right
            for (int x = 0; x < m_texWidth; x++)
            {
                Compare(grid, x, y, -1, 0);
            }
        }
    }

    Pixel GetPixel(int x, int y)
    {
        return m_texBuffer[y * m_texWidth + x];
    }

    void SetPixel(int x, int y, Pixel p)
    {
        m_texBuffer[y * m_texWidth + x] = p;
    }


    public void ApplyPixelDataToRendererMaterialBlock(RenderTexture rt)
    {
        
        // MaterialPropertyBlock block = new MaterialPropertyBlock();
        // m_sdfDestinationRenderer.GetPropertyBlock(block);
        //
        // float prevToCurrentMultiplier = 0f;
        // if (useDistanceField1)
        // {
        //     block.SetTexture(m_prevTextureRendererPropertyName, rt);
        //     block.SetFloat("_TimeSinceDistanceFieldUpdate", 0f);
        //     prevToCurrentMultiplier = 1f;
        // }
        // else
        // {
        //     block.SetTexture(m_rendererPropertyName, rt);
        //     block.SetFloat("_TimeSinceDistanceFieldUpdate", 0f);
        // }
        //
        // block.SetFloat("_PrevToCurrentDistanceField", prevToCurrentMultiplier);
        //
        //
        // m_sdfDestinationRenderer.SetPropertyBlock(block);
        
    }

    // public void Update()
    // {
    //     MaterialPropertyBlock block = new MaterialPropertyBlock();
    //     m_sdfDestinationRenderer.GetPropertyBlock(block);
    //
    //     block.SetFloat("_TimeSinceDistanceFieldUpdate", block.GetFloat("_TimeSinceDistanceFieldUpdate") + Time.deltaTime);
    //
    //     m_sdfDestinationRenderer.SetPropertyBlock(block);
    // }
}
