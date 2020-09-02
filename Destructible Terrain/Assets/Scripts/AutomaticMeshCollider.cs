using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace DTerrain
{
    ///<summary>
    ///Uses a Quadtree algorithm to generate a box colliders using List of Columns of a given chunk.
    ///Also uses PPU and positions of chunk to make every collider fit pixels in our World.
    ///</summary>
    public class AutomaticMeshCollider : MonoBehaviour
    {
        private List<Rect> rects;
        private float PPU = 1;


        /// <summary>
        /// Prepares all colliders. Deletes previous BoxColliders2D and adds new by using Quadtree. Also fits them correctly with texture.
        /// 
        /// <para>Thanks for /u/idbrii for pointing out overkill in deletion/addition of BoxColliders2D.</para>
        /// </summary>
        /// <param name="world">List of columns (chunk data) to find potential colliding pixels</param>
        /// <param name="x">Chunk offset X</param>
        /// <param name="y">Chunk offset Y</param>
        /// <param name="sizeX">Size of an chunk X</param>
        /// <param name="sizeY">Size of an chunk Y</param>
        public void MakeColliders(List<Column> world, int x, int y, int sizeX, int sizeY)
        {
            PPU = GetComponent<SpriteRenderer>().sprite.pixelsPerUnit;

            float time1 = Time.realtimeSinceStartup;
            if (rects != null) rects.Clear();

            List<BoxCollider2D> colls = new List<BoxCollider2D>(gameObject.GetComponents<BoxCollider2D>());

            rects = new List<Rect>();

            PrepareMesh(world, x, y, sizeX, sizeY);

            //Assume all colliders would be deleted. We use enabled for that.
            foreach (BoxCollider2D b in colls)
            {
                b.enabled = false;
            }

            //For each Rect found from Columns (Quadtree in PrepareMesh)...
            foreach (Rect r in rects)
            { 
                //Newly created collider will have an offset equeal to that.
                Vector2 rColliderOffset = new Vector2(-sizeX / PPU / 2f + r.x + r.size.x / 2, -sizeY / PPU / 2f + r.y + r.size.y / 2f);
                
                //Find already existing BoxCollider2D that would fit newly created BoxCollider2D.
                BoxCollider2D boxC = colls.Find(coll => coll.offset == rColliderOffset && coll.size == r.size);
                if(!boxC)
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

            float time2 = Time.realtimeSinceStartup;

            //For testing. How long it takes to make a colliders.
            //Debug.Log("#Created Collider in: " + (time2 - time1));
        }
      

        //Simple quadtree algortihm
        public void PrepareMesh(List<Column> chunk, int x, int y, int sizeX, int sizeY)
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
                        PrepareMesh(chunk, x, y, sizeX / 2, sizeY / 2);
                        PrepareMesh(chunk, x + sizeX / 2, y, sizeX / 2, sizeY / 2);
                        PrepareMesh(chunk, x, y + sizeY / 2, sizeX / 2, sizeY / 2);
                        PrepareMesh(chunk, x + sizeX / 2, y + sizeY / 2, sizeX / 2, sizeY / 2);
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