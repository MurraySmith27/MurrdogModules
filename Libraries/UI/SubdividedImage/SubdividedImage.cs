using System;
using UnityEngine.Serialization;

namespace UnityEngine.UI
{
    [AddComponentMenu("UI/Subdivided Image")]
    [RequireComponent(typeof(RectTransform), typeof(CanvasRenderer))]
    public class SubdividedImage : MaskableGraphic, ILayoutElement
    {
        public enum FillType
        {
            Simple,
            Filled,
        }

        public enum FillMethod
        {
            Horizontal,
            Vertical,
            Radial90,
            Radial180,
            Radial360,
        }

        public enum OriginHorizontal
        {
            Left,
            Right,
        }

        public enum OriginVertical
        {
            Bottom,
            Top,
        }

        public enum Origin90
        {
            BottomLeft,
            TopLeft,
            TopRight,
            BottomRight,
        }

        public enum Origin180
        {
            Bottom,
            Left,
            Top,
            Right,
        }

        public enum Origin360
        {
            Bottom,
            Right,
            Top,
            Left,
        }

        [SerializeField] private Sprite m_Sprite;
        public Sprite sprite
        {
            get { return m_Sprite; }
            set
            {
                if (SetClass(ref m_Sprite, value))
                    SetAllDirty();
            }
        }

        [NonSerialized] private Sprite m_OverrideSprite;
        public Sprite overrideSprite
        {
            get { return activeSprite; }
            set
            {
                if (SetClass(ref m_OverrideSprite, value))
                    SetAllDirty();
            }
        }

        private Sprite activeSprite { get { return m_OverrideSprite != null ? m_OverrideSprite : sprite; } }

        [SerializeField] private FillType m_FillType = FillType.Simple;
        public FillType fillType
        {
            get { return m_FillType; }
            set { if (m_FillType != value) { m_FillType = value; SetVerticesDirty(); } }
        }

        [SerializeField] private FillMethod m_FillMethod = FillMethod.Horizontal;
        public FillMethod fillMethod
        {
            get { return m_FillMethod; }
            set { if (m_FillMethod != value) { m_FillMethod = value; SetVerticesDirty(); } }
        }

        [SerializeField] private int m_FillOrigin;
        public int fillOrigin
        {
            get { return m_FillOrigin; }
            set { if (m_FillOrigin != value) { m_FillOrigin = value; SetVerticesDirty(); } }
        }

        [SerializeField, Range(0f, 1f)] private float m_FillAmount = 1f;
        public float fillAmount
        {
            get { return m_FillAmount; }
            set
            {
                value = Mathf.Clamp01(value);
                if (m_FillAmount != value) { m_FillAmount = value; SetVerticesDirty(); }
            }
        }

        [SerializeField] private bool m_FillClockwise = true;
        public bool fillClockwise
        {
            get { return m_FillClockwise; }
            set { if (m_FillClockwise != value) { m_FillClockwise = value; SetVerticesDirty(); } }
        }

        [SerializeField] private bool m_PreserveAspect;
        public bool preserveAspect
        {
            get { return m_PreserveAspect; }
            set { if (m_PreserveAspect != value) { m_PreserveAspect = value; SetVerticesDirty(); } }
        }

        [SerializeField, Range(1, 100)] private int m_Subdivisions = 10;
        public int subdivisions
        {
            get { return m_Subdivisions; }
            set
            {
                value = Mathf.Clamp(value, 1, 100);
                if (m_Subdivisions != value) { m_Subdivisions = value; SetVerticesDirty(); }
            }
        }

        // Material / texture overrides following UILineRenderer pattern
        static protected Material s_ETC1DefaultUI = null;
        static public Material defaultETC1GraphicMaterial
        {
            get
            {
                if (s_ETC1DefaultUI == null)
                    s_ETC1DefaultUI = Canvas.GetETC1SupportedCanvasMaterial();
                return s_ETC1DefaultUI;
            }
        }

        public override Material material
        {
            get
            {
                if (m_Material != null)
                    return m_Material;
#if UNITY_EDITOR
                if (Application.isPlaying && activeSprite && activeSprite.associatedAlphaSplitTexture != null)
                    return defaultETC1GraphicMaterial;
#else
                if (activeSprite && activeSprite.associatedAlphaSplitTexture != null)
                    return defaultETC1GraphicMaterial;
#endif
                return defaultMaterial;
            }
            set { base.material = value; }
        }

        public override Texture mainTexture
        {
            get
            {
                if (activeSprite == null)
                {
                    if (material != null && material.mainTexture != null)
                        return material.mainTexture;
                    return s_WhiteTexture;
                }
                return activeSprite.texture;
            }
        }

        public float pixelsPerUnit
        {
            get
            {
                float spritePixelsPerUnit = 100;
                if (activeSprite)
                    spritePixelsPerUnit = activeSprite.pixelsPerUnit;

                float referencePixelsPerUnit = 100;
                if (canvas)
                    referencePixelsPerUnit = canvas.referencePixelsPerUnit;

                return spritePixelsPerUnit / referencePixelsPerUnit;
            }
        }

        protected SubdividedImage()
        {
            useLegacyMeshGeneration = false;
        }

        protected override void UpdateMaterial()
        {
            base.UpdateMaterial();

            if (activeSprite == null)
            {
                canvasRenderer.SetAlphaTexture(null);
                return;
            }

            Texture2D alphaTex = activeSprite.associatedAlphaSplitTexture;
            if (alphaTex != null)
                canvasRenderer.SetAlphaTexture(alphaTex);
        }

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();

            if (activeSprite == null && m_Material == null)
            {
                // Still generate mesh with white texture for color tinting
            }

            Rect rect = GetDrawingRect();
            Vector4 uv = GetOuterUV();

            if (m_FillType == FillType.Simple)
                GenerateSimpleMesh(vh, rect, uv);
            else
                GenerateFilledMesh(vh, rect, uv);

            if (vh.currentVertCount > 64000)
            {
                Debug.LogError("SubdividedImage: vertex count [" + vh.currentVertCount + "] exceeds 64000 limit. Clearing mesh.");
                vh.Clear();
            }
        }

        private Rect GetDrawingRect()
        {
            Rect r = GetPixelAdjustedRect();

            if (m_PreserveAspect && activeSprite != null)
            {
                float spriteW = activeSprite.rect.width / pixelsPerUnit;
                float spriteH = activeSprite.rect.height / pixelsPerUnit;
                float spriteAspect = spriteW / spriteH;
                float rectAspect = r.width / r.height;

                if (spriteAspect > rectAspect)
                {
                    // Pillarbox: sprite is wider than rect
                    float newHeight = r.width / spriteAspect;
                    r.y += (r.height - newHeight) * 0.5f;
                    r.height = newHeight;
                }
                else
                {
                    // Letterbox: sprite is taller than rect
                    float newWidth = r.height * spriteAspect;
                    r.x += (r.width - newWidth) * 0.5f;
                    r.width = newWidth;
                }
            }

            return r;
        }

        private Vector4 GetOuterUV()
        {
            if (activeSprite != null)
                return Sprites.DataUtility.GetOuterUV(activeSprite);
            return new Vector4(0, 0, 1, 1);
        }

        #region Simple Mode

        private void GenerateSimpleMesh(VertexHelper vh, Rect rect, Vector4 uv)
        {
            int n = m_Subdivisions;
            int vertCountPerRow = n + 1;
            Color32 col = color;

            for (int y = 0; y <= n; y++)
            {
                float yt = (float)y / n;
                float posY = rect.y + rect.height * yt;
                float uvY = Mathf.Lerp(uv.y, uv.w, yt);

                for (int x = 0; x <= n; x++)
                {
                    float xt = (float)x / n;
                    float posX = rect.x + rect.width * xt;
                    float uvX = Mathf.Lerp(uv.x, uv.z, xt);

                    vh.AddVert(new Vector3(posX, posY, 0), col, new Vector2(uvX, uvY));
                }
            }

            for (int y = 0; y < n; y++)
            {
                for (int x = 0; x < n; x++)
                {
                    int i0 = y * vertCountPerRow + x;
                    int i1 = i0 + 1;
                    int i2 = i0 + vertCountPerRow + 1;
                    int i3 = i0 + vertCountPerRow;

                    vh.AddTriangle(i0, i3, i2);
                    vh.AddTriangle(i0, i2, i1);
                }
            }
        }

        #endregion

        #region Filled Mode

        private void GenerateFilledMesh(VertexHelper vh, Rect rect, Vector4 uv)
        {
            if (m_FillAmount <= 0f)
                return;

            switch (m_FillMethod)
            {
                case FillMethod.Horizontal:
                    GenerateFilledHorizontal(vh, rect, uv);
                    break;
                case FillMethod.Vertical:
                    GenerateFilledVertical(vh, rect, uv);
                    break;
                case FillMethod.Radial90:
                    GenerateFilledRadial(vh, rect, uv, 90f);
                    break;
                case FillMethod.Radial180:
                    GenerateFilledRadial(vh, rect, uv, 180f);
                    break;
                case FillMethod.Radial360:
                    GenerateFilledRadial(vh, rect, uv, 360f);
                    break;
            }
        }

        private void GenerateFilledHorizontal(VertexHelper vh, Rect rect, Vector4 uv)
        {
            float fill = m_FillAmount;
            bool fromRight = m_FillOrigin == (int)OriginHorizontal.Right;

            float clippedWidth = rect.width * fill;
            float clippedUVWidth = (uv.z - uv.x) * fill;

            Rect clippedRect;
            Vector4 clippedUV;

            if (fromRight)
            {
                clippedRect = new Rect(rect.x + rect.width - clippedWidth, rect.y, clippedWidth, rect.height);
                clippedUV = new Vector4(uv.z - clippedUVWidth, uv.y, uv.z, uv.w);
            }
            else
            {
                clippedRect = new Rect(rect.x, rect.y, clippedWidth, rect.height);
                clippedUV = new Vector4(uv.x, uv.y, uv.x + clippedUVWidth, uv.w);
            }

            GenerateSimpleMesh(vh, clippedRect, clippedUV);
        }

        private void GenerateFilledVertical(VertexHelper vh, Rect rect, Vector4 uv)
        {
            float fill = m_FillAmount;
            bool fromTop = m_FillOrigin == (int)OriginVertical.Top;

            float clippedHeight = rect.height * fill;
            float clippedUVHeight = (uv.w - uv.y) * fill;

            Rect clippedRect;
            Vector4 clippedUV;

            if (fromTop)
            {
                clippedRect = new Rect(rect.x, rect.y + rect.height - clippedHeight, rect.width, clippedHeight);
                clippedUV = new Vector4(uv.x, uv.w - clippedUVHeight, uv.z, uv.w);
            }
            else
            {
                clippedRect = new Rect(rect.x, rect.y, rect.width, clippedHeight);
                clippedUV = new Vector4(uv.x, uv.y, uv.z, uv.y + clippedUVHeight);
            }

            GenerateSimpleMesh(vh, clippedRect, clippedUV);
        }

        private void GenerateFilledRadial(VertexHelper vh, Rect rect, Vector4 uv, float maxAngleDeg)
        {
            if (m_FillAmount <= 0f)
                return;

            int n = m_Subdivisions;
            int vertCountPerRow = n + 1;
            Color32 col = color;

            float fillAngle = m_FillAmount * maxAngleDeg;
            float startAngleDeg = GetRadialStartAngle(maxAngleDeg);
            if (!m_FillClockwise)
                startAngleDeg = startAngleDeg + fillAngle;

            float startAngleRad = startAngleDeg * Mathf.Deg2Rad;
            float fillAngleRad = fillAngle * Mathf.Deg2Rad;

            Vector2 center = new Vector2(rect.x + rect.width * 0.5f, rect.y + rect.height * 0.5f);
            Vector2 uvCenter = new Vector2((uv.x + uv.z) * 0.5f, (uv.y + uv.w) * 0.5f);

            // Generate all grid vertices
            Vector3[] positions = new Vector3[(n + 1) * (n + 1)];
            Vector2[] uvs = new Vector2[(n + 1) * (n + 1)];

            for (int y = 0; y <= n; y++)
            {
                float yt = (float)y / n;
                float posY = rect.y + rect.height * yt;
                float uvY = Mathf.Lerp(uv.y, uv.w, yt);

                for (int x = 0; x <= n; x++)
                {
                    float xt = (float)x / n;
                    float posX = rect.x + rect.width * xt;
                    float uvX = Mathf.Lerp(uv.x, uv.z, xt);

                    int idx = y * vertCountPerRow + x;
                    positions[idx] = new Vector3(posX, posY, 0);
                    uvs[idx] = new Vector2(uvX, uvY);
                }
            }

            // For each grid cell, check if it should be included
            // A cell is included if any of its 4 corner vertices falls within the fill angle
            for (int y = 0; y < n; y++)
            {
                for (int x = 0; x < n; x++)
                {
                    int i0 = y * vertCountPerRow + x;
                    int i1 = i0 + 1;
                    int i2 = i0 + vertCountPerRow + 1;
                    int i3 = i0 + vertCountPerRow;

                    bool anyInside = IsVertexInFillAngle(positions[i0], center, startAngleRad, fillAngleRad, m_FillClockwise)
                                  || IsVertexInFillAngle(positions[i1], center, startAngleRad, fillAngleRad, m_FillClockwise)
                                  || IsVertexInFillAngle(positions[i2], center, startAngleRad, fillAngleRad, m_FillClockwise)
                                  || IsVertexInFillAngle(positions[i3], center, startAngleRad, fillAngleRad, m_FillClockwise);

                    if (!anyInside)
                        continue;

                    int baseVert = vh.currentVertCount;
                    vh.AddVert(positions[i0], col, uvs[i0]);
                    vh.AddVert(positions[i1], col, uvs[i1]);
                    vh.AddVert(positions[i2], col, uvs[i2]);
                    vh.AddVert(positions[i3], col, uvs[i3]);

                    vh.AddTriangle(baseVert, baseVert + 3, baseVert + 2);
                    vh.AddTriangle(baseVert, baseVert + 2, baseVert + 1);
                }
            }
        }

        private float GetRadialStartAngle(float maxAngleDeg)
        {
            // Angles measured counter-clockwise from right (+X axis), in degrees
            // Start angle is the beginning of the sweep
            switch (m_FillMethod)
            {
                case FillMethod.Radial360:
                    switch ((Origin360)m_FillOrigin)
                    {
                        case Origin360.Bottom: return 270f;
                        case Origin360.Right:  return 0f;
                        case Origin360.Top:    return 90f;
                        case Origin360.Left:   return 180f;
                    }
                    break;
                case FillMethod.Radial180:
                    switch ((Origin180)m_FillOrigin)
                    {
                        case Origin180.Bottom: return 270f;
                        case Origin180.Left:   return 180f;
                        case Origin180.Top:    return 90f;
                        case Origin180.Right:  return 0f;
                    }
                    break;
                case FillMethod.Radial90:
                    switch ((Origin90)m_FillOrigin)
                    {
                        case Origin90.BottomLeft:  return 180f;
                        case Origin90.TopLeft:     return 90f;
                        case Origin90.TopRight:    return 0f;
                        case Origin90.BottomRight: return 270f;
                    }
                    break;
            }
            return 0f;
        }

        private static bool IsVertexInFillAngle(Vector3 pos, Vector2 center, float startAngleRad, float fillAngleRad, bool clockwise)
        {
            float dx = pos.x - center.x;
            float dy = pos.y - center.y;

            // Consider center vertex always inside
            if (dx * dx + dy * dy < 0.001f)
                return true;

            float vertAngle = Mathf.Atan2(dy, dx);

            float relativeAngle;
            if (clockwise)
            {
                // Clockwise: sweep goes from startAngle downward
                relativeAngle = startAngleRad - vertAngle;
            }
            else
            {
                // Counter-clockwise: sweep goes from startAngle upward
                relativeAngle = vertAngle - startAngleRad;
            }

            // Normalize to [0, 2*PI)
            relativeAngle = relativeAngle % (2f * Mathf.PI);
            if (relativeAngle < 0f)
                relativeAngle += 2f * Mathf.PI;

            return relativeAngle <= fillAngleRad + 0.001f;
        }

        #endregion

        #region ILayoutElement

        public void CalculateLayoutInputHorizontal() { }
        public void CalculateLayoutInputVertical() { }

        public float minWidth { get { return 0; } }
        public float minHeight { get { return 0; } }

        public float preferredWidth
        {
            get
            {
                if (activeSprite == null)
                    return 0;
                return activeSprite.rect.width / pixelsPerUnit;
            }
        }

        public float preferredHeight
        {
            get
            {
                if (activeSprite == null)
                    return 0;
                return activeSprite.rect.height / pixelsPerUnit;
            }
        }

        public float flexibleWidth { get { return -1; } }
        public float flexibleHeight { get { return -1; } }
        public int layoutPriority { get { return 0; } }

        #endregion

        public void SetNativeSize()
        {
            if (activeSprite != null)
            {
                float w = activeSprite.rect.width / pixelsPerUnit;
                float h = activeSprite.rect.height / pixelsPerUnit;
                rectTransform.anchorMax = rectTransform.anchorMin;
                rectTransform.sizeDelta = new Vector2(w, h);
                SetAllDirty();
            }
        }

        private bool SetClass<T>(ref T currentValue, T newValue) where T : class
        {
            if ((currentValue == null && newValue == null) || (currentValue != null && currentValue.Equals(newValue)))
                return false;
            currentValue = newValue;
            return true;
        }
    }
}
