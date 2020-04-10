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
}
public class RLEColumn
{
    int x;
    List<Range> ranges;
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

    public void DelRange(Range delr)
    {
        int a = delr.min;
        int b = delr.max;
        
        for(int i = 0; i<ranges.Count;i++)
        {

            if(ranges[i].min<a && ranges[i].max>b)
            {
                
                ranges.Add(new Range(ranges[i].min, a));
                ranges.Add(new Range(b, ranges[i].max));
                ranges.Remove(ranges[i]);
                break;
            }

            if(ranges[i].min>=a && ranges[i].max<=b)
            {
                ranges.Remove(ranges[i]);
                i--;
                continue;
            }

            if(ranges[i].min<a && ranges[i].max>a)
            {
                
                ranges.Add(new Range(ranges[i].min, a));
                ranges.Remove(ranges[i]);
                i--;
                continue;
            }

            if(ranges[i].max>b && ranges[i].min<b)
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
    Texture2D terrainImage;
    Texture2D loadedTexture;
    Sprite sprite;
    // Start is called before the first frame update
    void Start()
    {
        columns = new List<RLEColumn>();

        Sprite loadedSprite = GetComponent<SpriteRenderer>().sprite;
        loadedTexture = loadedSprite.texture;
        

        terrainImage = new Texture2D(loadedTexture.width,loadedTexture.height);
        terrainImage.filterMode = FilterMode.Point;
        terrainImage.SetPixels(0,0,terrainImage.width,terrainImage.height,loadedTexture.GetPixels(0,0,loadedTexture.width,loadedTexture.height));
        terrainImage.Apply();

        sprite = Sprite.Create(terrainImage,new Rect(0,0,terrainImage.width,terrainImage.height), new Vector2(0.5f,0.5f), loadedSprite.pixelsPerUnit);

        GetComponent<SpriteRenderer>().sprite = sprite;
        

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
        
    }

    void UpdateWorld()
    {
        if(terrainImage!=null)
            PrepareColumns();

        //GetComponent<AutomaticMeshCollider>().MakeCollider(columns, 0,0,terrainImage.width,terrainImage.height);
    }

    // Update is called once per frame
    void Update()
    {
        MouseDestruction();
    }

    public void MouseDestruction()
    {
        
        if(Input.GetMouseButton(0))
        {
            Vector3 mPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            int xPos = terrainImage.width/2-(int)((transform.position.x-mPos.x )*sprite.pixelsPerUnit);
            int yPos = terrainImage.height/2+(int)(-(transform.position.y-mPos.y )*sprite.pixelsPerUnit);
            //Debug.Log(mPos);
            //Debug.Log(xPos+"      "+yPos);

            int r = 3;
            for(int i = xPos-r;i<=xPos+r;i++)
            {
                for(int j = yPos-r;j<=yPos+r;j++)
                {
                    if(i>=0 && i<terrainImage.width && j>=0 && j<terrainImage.height)
                    {
                        terrainImage.SetPixel(i,j,Color.clear);
                        //columns[i].DelRange(new Range(yPos, yPos));
                    }  
                    
                } 
            }
            //columns[xPos].DelRange(new Range(yPos-2, yPos+2));
            terrainImage.Apply(); 
            UpdateWorld();
        }
    }

    void UpdateTexture()
    {
        for(int x=0; x<loadedTexture.width;x++)
        {
            for(int y=0;y<loadedTexture.height;y++)
            {
                if(columns[x].isWithin(y)==false)
                {
                    terrainImage.SetPixel(x,y,Color.clear);
                    
                }
            }
        }
        terrainImage.Apply();
    }

    void PrepareColumns()
    {
        Color[] colorMap = new Color[terrainImage.width*terrainImage.height];
        colorMap = terrainImage.GetPixels(0,0,terrainImage.width,terrainImage.height,0);
        columns.Clear();
        columns = new List<RLEColumn>();

        for(int x = 0; x<terrainImage.width; x++)
        {
            RLEColumn c = new RLEColumn(x);
            for(int y = 0; y<terrainImage.height;y++)
            {
                int potentialMin=y;
                int potentialMax=y-1;
                while(y<terrainImage.height && terrainImage.GetPixel(x,y).a>0.01f)
                {
                    y++;
                    potentialMax++;    
                }
                if(potentialMin<=potentialMax)
                {
                    c.AddRange(potentialMin, potentialMax);
                    //Debug.Log("New range:  "+potentialMin+"      "+potentialMax);
                    //terrainImage.SetPixel(x,potentialMin, new Color(1.0f,0.0f,0.0f,1.0f));
                    //terrainImage.SetPixel(x,potentialMax, new Color(1.0f,0.0f,0.0f,1.0f));
                    //terrainImage.Apply();
                }
            }
            columns.Add(c);
        } 
    }
}
