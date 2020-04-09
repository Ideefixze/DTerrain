using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Range
{
    public int min;
    public int max;

    public Range(int a, int b){min=a;max=b;}
    public bool isWithin(int point)
    {   
        //Debug.Log("P: "+point+" in ["+min+" , "+max+"]");
        return point<=max && point>=min;
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

    public void AddRange(Range r)
    {
        
    }
    

}
public class DestructibleTerrain : MonoBehaviour
{
    List<RLEColumn> columns;
    Texture2D terrainImage;
    // Start is called before the first frame update
    void Start()
    {
        columns = new List<RLEColumn>();
        terrainImage = GetComponent<SpriteRenderer>().sprite.texture;

        if(terrainImage!=null)
            PrepareColumns();

        GetComponent<AutomaticMeshCollider>().MakeCollider(columns, 0,0,terrainImage.width,terrainImage.height);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void PrepareColumns()
    {
        Color[] colorMap = new Color[terrainImage.width*terrainImage.height];
        colorMap = terrainImage.GetPixels(0,0,terrainImage.width,terrainImage.height,0);

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
                    Debug.Log("New range:  "+potentialMin+"      "+potentialMax);
                    //terrainImage.SetPixel(x,potentialMin, new Color(1.0f,0.0f,0.0f,1.0f));
                    //terrainImage.SetPixel(x,potentialMax, new Color(1.0f,0.0f,0.0f,1.0f));
                    //terrainImage.Apply();
                }
            }
            columns.Add(c);
        } 
    }
}
