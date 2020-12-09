using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace DTerrain
{
    /// <summary>
    /// CollidableChunk on each Paint execution deletes data from a list of columns, that is used to create BoxColliders2D.
    /// </summary>
    public class CollidableChunk : PaintableChunk
    {
        public float AlphaTreshold { get; set; } = 0.01f;
        private List<Column> columns;
        private IChunkCollider chunkCollider;
        protected bool colliderChanged = true;

        public override void Init()
        {
            base.Init();
            chunkCollider = GetComponent<IChunkCollider>();
            PrepareColumns();
        }

        public override void Paint(RectInt r, PaintingParameters pp)
        {
            base.Paint(r, pp);
            DeleteFromColumns(r);
        }

        public override void Update()
        {
            base.Update();
            if(colliderChanged)
            {
                chunkCollider?.UpdateColliders(columns, TextureSource);
                colliderChanged = false;
            }
        }

        private void DeleteFromColumns(RectInt rect)
        {
            RectInt common;
            rect.Intersects(new RectInt(0, 0, TextureSource.Texture.width, TextureSource.Texture.height), out common);

            for(int i = 0; i<common.width;i++)
            {
                colliderChanged = columns[common.x + i].DelRange(new Range(common.y, common.y+common.height)) || colliderChanged;
            }
        }



        /// <summary>
        /// Using terrainTexture creates a list of ranges (tiles that are egible for a collider).
        /// </summary>
        private void PrepareColumns()
        {
            columns?.Clear();
            columns = new List<Column>();

            //Iterate texture
            for (int x = 0; x < TextureSource.Texture.width; x++)
            {
                Column c = new Column(x);
                for (int y = 0; y < TextureSource.Texture.height; y++)
                {
                    int potentialMin = y;
                    int potentialMax = y - 1;
                    while (y < TextureSource.Texture.height && TextureSource.Texture.GetPixel(x, y).a > AlphaTreshold)
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
    }
}
