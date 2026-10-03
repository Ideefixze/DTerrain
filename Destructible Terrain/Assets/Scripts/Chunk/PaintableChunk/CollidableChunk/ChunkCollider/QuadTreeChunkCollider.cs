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

            List<PixelRect> pixelRects = new List<PixelRect>();
            ColumnQuadTree.Build(ref pixelData, 0, 0, textureSize.x, textureSize.y, ref pixelRects);
            foreach (PixelRect p in pixelRects)
                rects.Add(new Rect(p.X / PPU, p.Y / PPU, p.W / PPU, p.H / PPU));

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

    }
}
