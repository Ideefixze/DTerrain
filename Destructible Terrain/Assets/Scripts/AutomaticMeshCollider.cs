using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace DTerrain
{
    //Uses a Quadtree algorithm to generate a box colliders using List of RLEColumns of a given chunk.
    //Also uses PPU and positions of chunk to make every collider fit pixels in our World.
    public class AutomaticMeshCollider : MonoBehaviour
    {
        List<Rect> rects;
        List<BoxCollider2D> colliders;
        public float ppu = 1;

        public void MakeColliders(List<Column> world, int x, int y, int sizeX, int sizeY)
        {
            ppu = GetComponent<SpriteRenderer>().sprite.pixelsPerUnit;
            float time1 = Time.realtimeSinceStartup;
            if (rects != null) rects.Clear();

            foreach (Component b in gameObject.GetComponents<Component>())
            {
                if (b is BoxCollider2D)
                {
                    Destroy(b);
                }
            }

            rects = new List<Rect>();

            PrepareMesh(world, x, y, sizeX, sizeY);


            foreach (Rect r in rects)
            {
                BoxCollider2D b = gameObject.AddComponent<BoxCollider2D>();
                b.offset = new Vector2(-sizeX / ppu / 2f + r.x + r.size.x / 2, -sizeY / ppu / 2f + r.y + r.size.y / 2f);
                b.size = r.size;
            }

            float time2 = Time.realtimeSinceStartup;

            //Debug.Log("Created Collider in: " + (time2 - time1));
        }

        //Simple quadtree algortihm
        public void PrepareMesh(List<Column> world, int x, int y, int sizeX, int sizeY)
        {
            bool hasAnyAir = false;
            bool hasAnyGround = false;
            
            for (int i = x; i < x + sizeX; i++)
            {
                for (int j = y; j < y + sizeY; j++)
                {

                    if (world[i].isWithin(j))
                        hasAnyGround = true;
                    else
                        hasAnyAir = true;


                    if (hasAnyAir && hasAnyGround)
                    {
                        PrepareMesh(world, x, y, sizeX / 2, sizeY / 2);
                        PrepareMesh(world, x + sizeX / 2, y, sizeX / 2, sizeY / 2);
                        PrepareMesh(world, x, y + sizeY / 2, sizeX / 2, sizeY / 2);
                        PrepareMesh(world, x + sizeX / 2, y + sizeY / 2, sizeX / 2, sizeY / 2);
                        return;
                    }
                }

            }

            if (hasAnyGround && !hasAnyAir)
                rects.Add(new Rect(x / ppu, y / ppu, sizeX / ppu, sizeY / ppu));

            return;
        }

    }
}