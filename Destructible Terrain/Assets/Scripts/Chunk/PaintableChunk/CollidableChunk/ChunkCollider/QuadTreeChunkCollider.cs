using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace DTerrain
{
    public class QuadTreeChunkCollider : MonoBehaviour, IChunkCollider
    {
        private List<Rect> rects;
        private float PPU;

        /// <summary>
        /// Prepares all colliders. Deletes previous BoxColliders2D and adds new by using Quadtree. Also fits them correctly with texture.
        /// 
        /// <para>Thanks for /u/idbrii for pointing out overkill in deletion/addition of BoxColliders2D.</para>
        /// </summary>
        /// <param name="pixelData">List of columns (chunk data) to find potential colliding pixels</param>
        public void UpdateColliders(List<Column> pixelData, ITextureSource textureSource)
        {
            PPU = textureSource.PPU;
            Vector2Int textureSize = new Vector2Int(textureSource.Texture.width, textureSource.Texture.height);

            if (rects != null) rects.Clear();

            List<BoxCollider2D> colls = new List<BoxCollider2D>(gameObject.GetComponents<BoxCollider2D>());

            rects = new List<Rect>();

            QuadTreeToRect(pixelData, 0, 0, textureSize.x, textureSize.y);

            //Assume all colliders would be deleted. We use enabled for that.
            foreach (BoxCollider2D b in colls)
            {
                b.enabled = false;
            }

            //For each Rect found from Columns (Quadtree in QuadTreeToRect)...
            foreach (Rect r in rects)
            {
                //Newly created collider will have an offset equeal to that.
                Vector2 rColliderOffset = new Vector2(r.x + r.size.x / 2, r.y + r.size.y / 2f);

                //Find already existing BoxCollider2D that would fit newly created BoxCollider2D.
                BoxCollider2D boxC = colls.Find(coll => coll.offset == rColliderOffset && coll.size == r.size);
                if (!boxC)
                {
                    //Not found? Create new one.
                    BoxCollider2D b = gameObject.AddComponent<BoxCollider2D>();
                    b.offset = rColliderOffset;
                    b.size = r.size;
                }
                else
                {
                    //Found. It won't be deleted!
                    boxC.enabled = true;
                }

            }

            //All BoxColliders2D that were modified and haven't been found are deleted.
            foreach (BoxCollider2D b in colls)
            {
                if (b.enabled == false)
                    Destroy(b);
            }
        }

        /// <summary>
        /// Generates Rects using QuadTree algorithm.
        /// </summary>
        /// <param name="chunk">Chunk data</param>
        /// <param name="x">Offset x</param>
        /// <param name="y">Offset y.</param>
        /// <param name="sizeX">Width of this step</param>
        /// <param name="sizeY">Height of this step</param>
        private void QuadTreeToRect(List<Column> chunk, int x, int y, int sizeX, int sizeY)
        {
            bool hasAnyAir = false;
            bool hasAnyGround = false;

            for (int i = x; i < x + sizeX; i++)
            {
                for (int j = y; j < y + sizeY; j++)
                {

                    if (chunk[i].isWithin(j))
                        hasAnyGround = true;
                    else
                        hasAnyAir = true;


                    if (hasAnyAir && hasAnyGround)
                    {
                        QuadTreeToRect(chunk, x, y, sizeX / 2, sizeY / 2);
                        QuadTreeToRect(chunk, x + sizeX / 2, y, sizeX / 2, sizeY / 2);
                        QuadTreeToRect(chunk, x, y + sizeY / 2, sizeX / 2, sizeY / 2);
                        QuadTreeToRect(chunk, x + sizeX / 2, y + sizeY / 2, sizeX / 2, sizeY / 2);
                        return;
                    }
                }

            }

            if (hasAnyGround && !hasAnyAir)
                rects.Add(new Rect(x / PPU, y / PPU, sizeX / PPU, sizeY / PPU));

            return;
        }

    }
}
