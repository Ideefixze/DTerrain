using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DTerrain
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class DestructibleTerrainChunk : MonoBehaviour
    {
        public bool updateTerrainOnNextFrame = false;
        [SerializeField]
        private FilterMode filterMode=FilterMode.Point;

        [SerializeField]
        [Range(0f, 1f)]
        private float alphaTreshold=0.05f;


        /*
         * DTerrain uses a list of ranges (list of ranges) to determine which tile is occupied.
         * This makes game run faster than holding every pixel/tile as a separate object.
         * */
        List<Column> columns;

        Texture2D loadedTexture;    //Original texture loaded from spriteRenderer.
        Texture2D terrainTexture;   //Texture ingame. This texture changes itself.
        Texture2D outlineTexture;   //Black pixel outline around solid terrain.
        Texture2D finalTexture;     //Final texture shown to the player.
        Sprite sprite;              // Sprite used by SpriteRenderer.

        void Start()
        {
            InitChunk();
            GetComponent<AutomaticMeshCollider>()?.MakeColliders(columns, 0, 0, terrainTexture.width, terrainTexture.height);
        }

        void Update()
        {
            if (updateTerrainOnNextFrame)
            {
                UpdateTexture();
                GetComponent<AutomaticMeshCollider>()?.MakeColliders(columns, 0, 0, terrainTexture.width, terrainTexture.height);

                updateTerrainOnNextFrame = false;
            }
        }

        // Start is called before the first frame update
        public Texture2D GetTerrainTexture()
        {
            return terrainTexture;
        }
        public Texture2D GetOutlineTexture()
        {
            return outlineTexture;
        }

        /*
         * Prepares textures used in process.
         * finalTexture = terrainTexture(copy of an orginal Texture, that is being changed) + outlineTexture(black outline on destruction)
         */
        public void InitChunk()
        {
            columns = new List<Column>();

            Sprite loadedSprite = GetComponent<SpriteRenderer>().sprite;
            loadedTexture = loadedSprite.texture;

            terrainTexture = new Texture2D(loadedTexture.width, loadedTexture.height);
            terrainTexture.filterMode = filterMode;
            terrainTexture.SetPixels(0, 0, terrainTexture.width, terrainTexture.height, loadedTexture.GetPixels(0, 0, loadedTexture.width, loadedTexture.height));
            terrainTexture.Apply();

            outlineTexture = new Texture2D(loadedTexture.width, loadedTexture.height);
            outlineTexture.filterMode = filterMode;
            Color[] clrs = new Color[(loadedTexture.width * loadedTexture.height)];
            for (int i = 0; i < (loadedTexture.width * loadedTexture.height); i++) clrs[i] = Color.clear;

            outlineTexture.SetPixels(clrs);

            finalTexture = new Texture2D(loadedTexture.width, loadedTexture.height);
            finalTexture.filterMode = filterMode;

            UpdateWorldColumns();
            UpdateTexture();

            sprite = Sprite.Create(finalTexture, new Rect(0, 0, finalTexture.width, finalTexture.height), new Vector2(0.5f, 0.5f), loadedSprite.pixelsPerUnit);
            GetComponent<SpriteRenderer>().sprite = sprite;

            updateTerrainOnNextFrame = true;


        }

        void UpdateWorldColumns()
        {
            if (terrainTexture != null)
            {
                PrepareColumns();

            }

        }


        public Color ColorAt(int x, int y)
        {
            return loadedTexture.GetPixel(x, y);
        }

        /// <summary>
        /// Destroys a single pixel on the bitmap. Warning: use large power. Lower values are not supported and may cause weird looking texture. You can expand on this idea.
        /// </summary>
        /// <param name="x">X coord.</param>
        /// <param name="y">Y coord.</param>
        /// <returns></returns>
        public bool DestroyTexture(int x, int y)
        { 

            terrainTexture.SetPixel(x, y, Color.clear);

            outlineTexture.SetPixel(x, y, Color.clear);

            return true;
        }

        public bool DestroyTexture(int x, int y, int w, int h)
        {
            Color[] c = new Color[w*h];
            for (int i = 0; i < w * h; i++)
                c[i] = Color.clear;

            terrainTexture.SetPixels(x, y, w,h, c);

            outlineTexture.SetPixels(x, y, w,h, c);

            return true;
        }

        public bool DestroyTerrain(int x, int y)
        {
            if (x >= 0 && x < terrainTexture.width && y >= 0 && y < terrainTexture.height)
            {
                DestroyTexture(x,y);

                columns[x].SingleDelRange(y);

                updateTerrainOnNextFrame = true;

                return true;
            }
            return false;
        }
        public bool DestroyTerrain(Vector2Int pos)
        {
            return DestroyTerrain(pos.x, pos.y);
        }

        public void MakeOutline(int x, int y, Color outlineCol)
        {
            if (x >= 0 && x < terrainTexture.width && y >= 0 && y < terrainTexture.height)
            {
                if (terrainTexture.GetPixel(x, y).a > alphaTreshold)
                {
                    outlineTexture.SetPixel(x, y, outlineCol);
                    updateTerrainOnNextFrame = true;
                }
            }
        }

        public void MakeOutline(Vector2Int pos, Color outlineCol)
        {
            MakeOutline(pos.x, pos.y, outlineCol);
        }

        /// <summary>
        /// Cuts a shape in the world. Currently unsupported: World uses DestroyTerrainWithRange to cut with shape.
        /// </summary>
        /// <param name="x">Pos x of shape in world.</param>
        /// <param name="y">Pos y of shape in world.</param>
        /// <param name="s">Shape.</param>
        /// <param name="power">Power of destruction.</param>
        public void DestroyTerrain(int x, int y, Shape s)
        {
            int k = 0;
            bool changed = false;
            foreach (Range r in s.ranges)
            {
                MakeOutline(x + k, y + r.min, Color.black);
                for (int i = r.min + 1; i < r.max - 1; i++)
                {
                    if (k > 0 && k < s.ranges.Count - 1)
                        changed = DestroyTexture(x + k, y + i);
                    else
                        MakeOutline(x + k, y + i, Color.black);

                }
                MakeOutline(x + k, y + r.max - 1, Color.black);
                k++;
            }
        }

        public void DestroyTerrain(Vector2Int pos, Shape s)
        {
            DestroyTerrain(pos.x, pos.y, s);
        }

        /// <summary>
        /// Destroys a terrain using a range at given coordinates.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="r"></param>
        /// <returns></returns>
        public bool DestroyTerrain(int x, int y, Range r)
        {
            int w = terrainTexture.width;
            int h = terrainTexture.height;
            if (x < 0 && x >= w) return false;

            int a = Mathf.Max(0, r.min+y);
            int b = Mathf.Min(h, r.max+y+1);

            if(b>a)
                DestroyTexture(x, a, 1, b - a);

            return columns[x].DelRange(new Range(r.min + y, r.max + y));
        }

        /// <summary>
        /// Recreates final texture that consists of initial texture (that was possibly changed) and outline.
        /// </summary>
        void UpdateTexture()
        {
            outlineTexture.Apply();
            terrainTexture.Apply();

            Color[] clrs = new Color[(loadedTexture.width * loadedTexture.height)];
            Color[] oclrs = outlineTexture.GetPixels();
            Color[] tclrs = terrainTexture.GetPixels();
            int s = terrainTexture.height * terrainTexture.width;
            for (int i = 0; i < s; i++)
            {
                clrs[i] = oclrs[i].a > 0 ? oclrs[i] : tclrs[i];
            }


            finalTexture.SetPixels(clrs);
            finalTexture.Apply();
        }

        //Unused, old method.
        void UpdateOutline()
        {
            //Create new texture and make it transparent.
            outlineTexture = new Texture2D(loadedTexture.width, loadedTexture.height);
            outlineTexture.filterMode = FilterMode.Point;
            Color[] clrs = new Color[(loadedTexture.width * loadedTexture.height)];
            for (int i = 0; i < (loadedTexture.width * loadedTexture.height); i++) clrs[i] = Color.clear;

            outlineTexture.SetPixels(clrs);

            outlineTexture.Apply();
        }

        /// <summary>
        /// Using terrainTexture creates a list of ranges (tiles that are egible for collider).
        /// </summary>
        void PrepareColumns()
        {
            columns.Clear();
            columns = new List<Column>();

            //Iterate texture
            for (int x = 0; x < terrainTexture.width; x++)
            {
                Column c = new Column(x);
                for (int y = 0; y < terrainTexture.height; y++)
                {
                    int potentialMin = y;
                    int potentialMax = y - 1;
                    while (y < terrainTexture.height && terrainTexture.GetPixel(x, y).a > alphaTreshold)
                    {
                        y++;
                        potentialMax++;
                    }
                    if (potentialMin <= potentialMax)
                    {
                        c.AddRange(potentialMin, potentialMax); //Add range to a column...
                    }
                }
                columns.Add(c); //And add the column!
            }
        }

        //Checks if pos in chunk is filled
        public bool FilledAt(int x, int y)
        {
            if (x >= 0 && x < terrainTexture.width && y >= 0 && y < terrainTexture.height)
                return finalTexture.GetPixel(x, y).a > 0.01f;
            else
                return false;
        }

        public bool FilledAt(Vector2Int pos)
        {
            return FilledAt(pos.x, pos.y);
        }
    }
}