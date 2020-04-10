using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
public struct Range
{
    public int min;
    public int max;
    public Range(int a, int b)
    {
        min=a;
        max=b;
    }

}

public class RNode
{
    public Range r;
    public int max;
    public RNode left,right,parent;

    public RNode(Range r)
    {
        this.r = r;
        this.max = r.max;
        left = null;
        right = null;
        parent = null;
    }

    public void Print()
    {
         Debug.Log("[ "+r.min+", "+r.max+" ] : ( "+max+" )");
    }

    public void Insert(RNode root, Range i)
    {
        if(root==null) return;
        int l = root.r.min;

        //root.Print();

        if(l>i.min)
        {
            if(left!=null)
            {
                left.Insert(left,i);
                return;
            }
            else
            {
                left = new RNode(i);
                left.parent = this;
                if(left.max>this.max)
                    this.max=left.max;
            }
        }
        else
        {
            if(right!=null)
            {
                right.Insert(right,i);
                return;
            }
            else
            {
                right = new RNode(i);
                right.parent = this;
                if(right.max>this.max)
                    this.max=right.max;
            }
        }
    }

    public bool OverlapRange(Range r)
    {
        return (r.min>=this.r.min && r.max<=this.r.max);
    }

    public bool isWithin(int point)
    {   
        return point<=r.max && point>=r.min;
    }

    public RNode OverlapSearch(RNode root, Range r)
    {
        if(root==null) return null;

        if(root.OverlapRange(r)) return root;

        if(root.left!=null && root.left.isWithin(r.min)) return OverlapSearch(root.left,r);
        else return OverlapSearch(root.right,r);

    }

    public void Inorder(RNode root) 
    { 
        if (root==null) return; 
    
        Inorder(root.left); 
    
        root.Print();
    
        Inorder(root.right); 
    } 

}

public class RangeTree 
{
    public RNode root;
    public RangeTree(Range rootRange)
    {
        root = new RNode(rootRange);
    }

    public RangeTree()
    {
    }

    public void Insert(Range r)
    {
        if(root==null) root = new RNode(r);
        else
        {
            root.Insert(root,r);
        }

    }

    public void Insert(int a, int b)
    {
        Range r = new Range(a,b);
        this.Insert(r);

    }

}
*/
