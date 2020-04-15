using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Range
{
    public int min;
    public int max;

    public bool isWithin(int point)
    {   
        return point<=max && point>=min;
    }

    public Range(int a, int b)
    {
        min = a;
        max = b;
    }

    public void PrintRange()
    {
        Debug.Log("Range: [ "+min+" , "+max+" ]");
    }
}
public class RLEColumn
{
    public int x;
    public List<Range> ranges;
    public RLEColumn(int x){this.x=x; ranges=new List<Range>();}
    public void AddRange(int mini, int maxi){ ranges.Add(new Range(mini,maxi));}

    public bool isWithin(int point)
    {
        foreach(Range r in ranges)
        {
            if(r.isWithin(point)==true) return true;
        }
        return false;
    }

    public void PrintColumn()
    {
        foreach(Range r in ranges)
        {
            r.PrintRange();
        }
    }

    public void DelRange(Range delr)
    {
        int a = delr.min;
        int b = delr.max;
        for(int i = 0; i<ranges.Count;i++)
        {  
            //Debug.Log(i);
            if(ranges[i].min<a && ranges[i].max>b) ///0---a-----b----1
            {
                if(Mathf.Abs(a-b)==0)
                {
                    ranges.Add(new Range(ranges[i].min, a-1));
                    ranges.Add(new Range(b+1, ranges[i].max));
                    ranges.Remove(ranges[i]);
                    break;
                }
                ranges.Add(new Range(ranges[i].min, a));
                ranges.Add(new Range(b, ranges[i].max));
                ranges.Remove(ranges[i]);
                break;
            }

            if(ranges[i].min>=a && ranges[i].max<=b) ///-------a-0---1--b
            {
                ranges.Remove(ranges[i]);
                i--;
                continue;
            }

            if(ranges[i].min<a && ranges[i].max<=b && ranges[i].max>a) ///-------0--a---1---b
            {
                ranges.Add(new Range(ranges[i].min, a));
                ranges.Remove(ranges[i]);
                i--;
                continue;
            }

            if(ranges[i].min>=a && ranges[i].max>b && ranges[i].min<b) ///--a-0----b---1--
            {
                ranges.Add(new Range(b, ranges[i].max));
                ranges.Remove(ranges[i]);
                i--;
                continue;
            }
        }
    }
    

}

public class DestructibleTerrain : MonoBehaviour
{
    List<RLEColumn> columns;
    Texture2D terrainTexture;
    Texture2D columnsImage;

    Texture2D outlineTexture;

    Texture2D finalTexture;
    Texture2D loadedTexture;
    Sprite sprite;

    bool showColumns;
    // Start is called before the first frame update
    void Start()
    {
        columns = new List<RLEColumn>();

        Sprite loadedSprite = GetComponent<SpriteRenderer>().sprite;
        loadedTexture = loadedSprite.texture;
        

        terrainTexture = new Texture2D(loadedTexture.width,loadedTexture.height);
        terrainTexture.filterMode = FilterMode.Point;
        terrainTexture.SetPixels(0,0,terrainTexture.width,terrainTexture.height,loadedTexture.GetPixels(0,0,loadedTexture.width,loadedTexture.height));
        terrainTexture.Apply();

        sprite = Sprite.Create(terrainTexture,new Rect(0,0,terrainTexture.width,terrainTexture.height), new Vector2(0.5f,0.5f), loadedSprite.pixelsPerUnit);

        GetComponent<SpriteRenderer>().sprite = sprite;
        

        RLEColumn testRLE = new RLEColumn(0);
        testRLE.AddRange(5,20);
        testRLE.AddRange(25,30);
        testRLE.AddRange(35,40);
        testRLE.AddRange(45,60);
        testRLE.PrintColumn();
        Debug.Log("=---------=");
        testRLE.DelRange(new Range(15, 37));
        testRLE.PrintColumn();
        /*
        Range a = new Range(12,40);
        Range b = new Range(3,4);
        Range c = new Range(5,8);
        Range d = new Range(1,2);
        Range e = new Range(13,50);
        Range f = new Range(60,70);
        RangeTree rt = new RangeTree(a);
        rt.Insert(b);
        rt.Insert(c);
        rt.Insert(d);
        rt.Insert(e);
        rt.Insert(f);
        rt.root.Inorder(rt.root);
        Range w = new Range(65,69);
        RNode found = rt.root.OverlapSearch(rt.root, w);
        found.Print();
        */

        UpdateWorld();
        UpdateTexture();
        
    }

    void UpdateWorld()
    {
        if(terrainTexture!=null)
            PrepareColumns();

        //GetComponent<AutomaticMeshCollider>().MakeCollider(columns, 0,0,terrainTexture.width,terrainTexture.height);
    }

    // Update is called once per frame
    void Update()
    {
        MouseDestruction();
        if(Input.GetKeyDown(KeyCode.C))
        {
            CreateColumnsImage();
            sprite = Sprite.Create(columnsImage,new Rect(0,0,terrainTexture.width,terrainTexture.height), new Vector2(0.5f,0.5f), sprite.pixelsPerUnit);
            GetComponent<SpriteRenderer>().sprite = sprite;
        }
    }   

    public void CreateColumnsImage()
    {
        
        columnsImage = new Texture2D(loadedTexture.width,loadedTexture.height);
        columnsImage.filterMode = FilterMode.Point;
        columnsImage.SetPixels(0,0,terrainTexture.width,terrainTexture.height,terrainTexture.GetPixels(0,0,terrainTexture.width,terrainTexture.height));
        foreach(RLEColumn c in columns)
        {
            foreach(Range r in c.ranges)
            {
                columnsImage.SetPixel(c.x, r.min, Color.red);
                columnsImage.SetPixel(c.x, r.max, Color.blue);
                if(r.min==r.max) columnsImage.SetPixel(c.x, r.max, Color.cyan);
            }
        }
        columnsImage.Apply();
    }

    public void MouseDestruction()
    {
        if(Input.GetMouseButton(0))
        {
            Vector3 mPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            int xPos = terrainTexture.width/2-(int)((transform.position.x-mPos.x )*sprite.pixelsPerUnit);
            int yPos = terrainTexture.height/2+(int)(-(transform.position.y-mPos.y )*sprite.pixelsPerUnit);
            //Debug.Log(mPos);
            //Debug.Log(xPos+"      "+yPos);
            
            int r = 3;
            for(int i = xPos-r;i<=xPos+r;i++)
            {
                for(int j = yPos-r;j<=yPos+r;j++)
                {
                    if(i>=0 && i<terrainTexture.width && j>=0 && j<terrainTexture.height)
                    {
                        terrainTexture.SetPixel(i,j,terrainTexture.GetPixel(i,j) + new Color(-.05f,-.05f,-.05f,-0.25f));
                    }  
                    
                } 
                columns[i].DelRange(new Range(yPos-r, yPos+r));
            }

            //terrainTexture.Apply(); 

            /*terrainTexture.SetPixel(xPos,yPos-1,Color.clear);
            terrainTexture.SetPixel(xPos,yPos,Color.clear);
            terrainTexture.SetPixel(xPos,yPos+1,Color.clear);
            columns[xPos].DelRange(new Range(yPos-1, yPos+1));

            //columns[xPos].DelRange(new Range(yPos-2, yPos+2));
            terrainTexture.Apply(); 
            */
            UpdateTexture();
        }
    }

    void UpdateTexture()
    {
        terrainTexture.Apply();
        UpdateOutline();

        finalTexture = new Texture2D(loadedTexture.width,loadedTexture.height);
        finalTexture.filterMode = FilterMode.Point;
        Color[] clrs = new Color[(loadedTexture.width*loadedTexture.height)];
        Color[] oclrs = outlineTexture.GetPixels();
        Color[] tclrs = terrainTexture.GetPixels();
        for(int i = 0; i<terrainTexture.height*terrainTexture.width;i++)
        {
            clrs[i] = oclrs[i].a>0?oclrs[i]:tclrs[i];
            //clrs[i] = oclrs[i];
        }

        finalTexture.SetPixels(clrs);
        finalTexture.Apply();

        sprite = Sprite.Create(finalTexture,new Rect(0,0,finalTexture.width,finalTexture.height), new Vector2(0.5f,0.5f), sprite.pixelsPerUnit);

        GetComponent<SpriteRenderer>().sprite = sprite;
    }

    void UpdateOutline()
    {
        //Create new texture and make it transparent.
        outlineTexture = new Texture2D(loadedTexture.width,loadedTexture.height);
        outlineTexture.filterMode = FilterMode.Point;
        Color[] clrs = new Color[(loadedTexture.width*loadedTexture.height)];
        for(int i = 0; i<(loadedTexture.width*loadedTexture.height);i++) clrs[i] = Color.clear;

        outlineTexture.SetPixels(clrs);
        
        foreach(RLEColumn c in columns)
        {
            foreach(Range r in c.ranges)
            {
                outlineTexture.SetPixel(c.x, r.min, Color.black);
                outlineTexture.SetPixel(c.x, r.max, Color.black);
                
                for(int i=r.min; i<r.max;i++)
                {
                    
                    if(c.x>1)
                        if(!columns[c.x-1].isWithin(i)) outlineTexture.SetPixel(c.x-1, i, Color.black);

                    if(c.x<columns.Count-1)
                        if(!columns[c.x+1].isWithin(i)) outlineTexture.SetPixel(c.x+1, i, Color.black);
                        
                }
            }
        }

        outlineTexture.Apply();
    }

    void PrepareColumns()
    {
        Color[] colorMap = new Color[terrainTexture.width*terrainTexture.height];
        colorMap = terrainTexture.GetPixels(0,0,terrainTexture.width,terrainTexture.height,0);
        columns.Clear();
        columns = new List<RLEColumn>();

        for(int x = 0; x<terrainTexture.width; x++)
        {
            RLEColumn c = new RLEColumn(x);
            for(int y = 0; y<terrainTexture.height;y++)
            {
                int potentialMin=y;
                int potentialMax=y-1;
                while(y<terrainTexture.height && terrainTexture.GetPixel(x,y).a>0.01f)
                {
                    y++;
                    potentialMax++;    
                }
                if(potentialMin<=potentialMax)
                {
                    c.AddRange(potentialMin, potentialMax);
                    //Debug.Log("New range:  "+potentialMin+"      "+potentialMax);
                    //terrainTexture.SetPixel(x,potentialMin, new Color(1.0f,0.0f,0.0f,1.0f));
                    //terrainTexture.SetPixel(x,potentialMax, new Color(1.0f,0.0f,0.0f,1.0f));
                    //terrainTexture.Apply();
                }
            }
            columns.Add(c);
        } 
    }
}
