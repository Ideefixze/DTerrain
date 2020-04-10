using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class AutomaticMeshCollider : MonoBehaviour
{
    List<Rect> rects;
    List<BoxCollider2D> colliders;
    public int ppu=1;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void MakeCollider(List<RLEColumn> world, int x, int y, int sizeX, int sizeY)
    {
        float time1 = Time.realtimeSinceStartup;
        if(rects!=null)rects.Clear();
        if(rects!=null)colliders.Clear();
        rects = new List<Rect>();
        colliders = new List<BoxCollider2D>();
        PrepareMesh(world, x, y, sizeX, sizeY);
        

        foreach(Rect r in rects)
        {
           BoxCollider2D b = gameObject.AddComponent<BoxCollider2D>();
           b.offset = new Vector2(-(float)sizeX/ppu/2 + r.x + r.size.x/2, -(float)sizeY/ppu/2 + r.y + r.size.y/2);
           b.size = r.size;
           colliders.Add(b);
        }

        float time2 = Time.realtimeSinceStartup;

        Debug.Log("Created Collider in: "+(time2-time1));
    }

    public void PrepareMesh(List<RLEColumn> world, int x, int y, int sizeX, int sizeY)
    {
        bool hasAnyAir=false;
        bool hasAnyGround=false;
       // Debug.Log("Starting step: " + x.ToString() + ", "+ y.ToString() + "  :  "+sizeX.ToString()+"   "+sizeY.ToString());

        for(int i = x; i<x+sizeX;i++)
        {
            for(int j = y; j<y+sizeY;j++)
            {
                
                if(world[i].isWithin(j))
                    hasAnyGround=true;
                else
                    hasAnyAir=true;


                if(hasAnyAir&&hasAnyGround)
                {
                    PrepareMesh(world,x,y,sizeX/2,sizeY/2);
                    PrepareMesh(world,x+sizeX/2,y,sizeX/2,sizeY/2);
                    PrepareMesh(world,x,y+sizeY/2,sizeX/2,sizeY/2);
                    PrepareMesh(world,x+sizeX/2,y+sizeY/2,sizeX/2,sizeY/2);
                    //Debug.Log("Return on check: "+rects.Count.ToString());
                    return;
                }
            }
            
        }

        if(hasAnyGround&&!hasAnyAir)
            rects.Add(new Rect((float)x/ppu,(float)y/ppu,(float)sizeX/ppu,(float)sizeY/ppu));

        return;
    }
    

    public void OnGUI()
    {
    }
}
