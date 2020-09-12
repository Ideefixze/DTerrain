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

        /// <summary>
        /// Initializes a single chunk.
        /// </summary>
        public void InitChunk()
        {
            //Prepares a list of columns
            columns = new List<Column>();

            //Sets loaded texture from sprite
            Sprite loadedSprite = GetComponent<SpriteRenderer>().sprite;
            loadedTexture = loadedSprite.texture;

            //Creates a terrain texture (the one that will be destroyed, so original texture remains unchanged)
            //We might use original texture for something else
            terrainTexture = new Texture2D(loadedTexture.width, loadedTexture.height);
            terrainTexture.filterMode = filterMode;
            terrainTexture.SetPixels(0, 0, terrainTexture.width, terrainTexture.height, loadedTexture.GetPixels(0, 0, loadedTexture.width, loadedTexture.height));
            terrainTexture.Apply();

            //Create an outline layer for a destroyed terrain
            outlineTexture = new Texture2D(loadedTexture.width, loadedTexture.height);
            outlineTexture.filterMode = filterMode;
            Color[] clrs = new Color[(loadedTexture.width * loadedTexture.height)];
            for (int i = 0; i < (loadedTexture.width * loadedTexture.height); i++) clrs[i] = Color.clear;

            outlineTexture.SetPixels(clrs);

            //Final texture: will be combination of an outline+terrain textures and is displayed with SpriteRenderer
            finalTexture = new Texture2D(loadedTexture.width, loadedTexture.height);
            finalTexture.filterMode = filterMode;

            UpdateWorldColumns();
            UpdateTexture();

            sprite = Sprite.Create(finalTexture, new Rect(0, 0, finalTexture.width, finalTexture.height), new Vector2(0.5f, 0.5f), loadedSprite.pixelsPerUnit);
            GetComponent<SpriteRenderer>().sprite = sprite;

            updateTerrainOnNextFrame = true;

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




        void UpdateWorldColumns()
        {
            if (terrainTexture != null)
            {
                PrepareColumns();

            }

        }

        /*
         * Prepares textures used in process.
         * finalTexture = terrainTexture(copy of an orginal Texture, that is being changed) + outlineTexture(black outline on destruction)
         */
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
        /// Using terrainTexture creates a list of ranges (tiles that are egible for a collider).
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

        public Texture2D GetTerrainTexture()
        {
            return terrainTexture;
        }
        public Texture2D GetOutlineTexture()
        {
            return outlineTexture;
        }

        /* 
         * Methods below are single-pixel methods (for destruction etc.)
         * rather than chunk oriented.
         */

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
            Color[] c = new Color[w * h];
            for (int i = 0; i < w * h; i++)
                c[i] = Color.clear;

            terrainTexture.SetPixels(x, y, w, h, c);
            outlineTexture.SetPixels(x, y, w, h, c);

            return true;
        }

        public bool DestroyTerrain(int x, int y)
        {
            if (x >= 0 && x < terrainTexture.width && y >= 0 && y < terrainTexture.height)
            {
                DestroyTexture(x, y);

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

            int a = Mathf.Max(0, r.min + y);
            int b = Mathf.Min(h, r.max + y + 1);

            if (b > a)
                DestroyTexture(x, a, 1, b - a);

            return columns[x].DelRange(new Range(r.min + y, r.max + y));
        }
    }
}