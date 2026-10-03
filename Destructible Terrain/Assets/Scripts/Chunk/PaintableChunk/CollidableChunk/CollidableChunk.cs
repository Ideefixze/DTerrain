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
        protected List<Column> columns;
        protected IChunkCollider chunkCollider;
        protected bool colliderChanged = true;

        /// <summary>The chunk's columns (read only by convention): neighbors read them along the border.</summary>
        public List<Column> Columns { get { return columns; } }

        /// <summary>The chunks next to this one, set by the layer (null at the edge of the layer).
        /// A collider that follows the ground across a chunk border reads them, and a change next to a
        /// border makes the neighbor rebuild.</summary>
        public CollidableChunk LeftNeighbor;
        public CollidableChunk RightNeighbor;
        public CollidableChunk DownNeighbor;
        public CollidableChunk UpNeighbor;

        public override void Init()
        {
            base.Init();
            chunkCollider = GetComponent<IChunkCollider>();
            PrepareColumns();
        }

        public override bool Paint(RectInt r, PaintingParameters pp)
        {
            bool b = base.Paint(r, pp);

            bool changed = false;
            if(pp.DestructionMode==DestructionMode.DESTROY)
                changed = DeleteFromColumns(r);
            if (pp.DestructionMode == DestructionMode.BUILD)
                changed = AddToColumns(r);

            if (changed)
                MarkNeighborsChanged(r);

            return b;
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

        public virtual bool IsOccupiedAt(Vector2Int at)
        {
            if (at.x >= 0 && at.x < columns.Count)
            {
                return columns[at.x].isWithin(at.y);
            }
            return false;
        }

        /// <summary>
        /// A change at a border changes the neighbor's boundary there: it rebuilds too.
        /// </summary>
        private void MarkNeighborsChanged(RectInt r)
        {
            int w = TextureSource.Texture.width;
            int h = TextureSource.Texture.height;
            if (LeftNeighbor != null && r.x <= 0) LeftNeighbor.colliderChanged = true;
            if (RightNeighbor != null && r.x + r.width >= w - 1) RightNeighbor.colliderChanged = true;
            if (DownNeighbor != null && r.y <= 0) DownNeighbor.colliderChanged = true;
            if (UpNeighbor != null && r.y + r.height >= h - 1) UpNeighbor.colliderChanged = true;
        }

        private bool DeleteFromColumns(RectInt rect)
        {
            bool any = false;
            RectInt common;
            rect.Intersects(new RectInt(0, 0, TextureSource.Texture.width, TextureSource.Texture.height), out common);

            for(int i = 0; i<common.width;i++)
            {
                bool c = columns[common.x + i].DelRange(new Range(common.y-1, common.y+common.height));
                any = any || c;
                colliderChanged = c || colliderChanged;
            }
            return any;
        }

        private bool AddToColumns(RectInt rect)
        {
            bool any = false;
            RectInt common;
            rect.Intersects(new RectInt(0, 0, TextureSource.Texture.width, TextureSource.Texture.height), out common);

            for (int i = 0; i < common.width; i++)
            {
                bool c = columns[common.x + i].SumRange(new Range(common.y, common.y + common.height-1));
                any = any || c;
                colliderChanged = c || colliderChanged;
            }
            return any;
        }



        /// <summary>
        /// Using terrainTexture creates a list of ranges (tiles that are egible for a collider).
        /// </summary>
        protected void PrepareColumns()
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
