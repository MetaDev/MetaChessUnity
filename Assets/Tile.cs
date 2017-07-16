using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile
{
    public GameObject cube;
    private float absSize;
    private float _absX;
    private float _absY;

    private float size;
    public int Color { get; set; }
    

    public Tile[,] Children { get; private set; }

   
    private Tile parent;
    public Tile GetParent()
    {
        return parent;
    }

    public int Level { get; set; }
    

    private int childFraction = 1;
    // relative position in parent
    public int I { get; private set; }
   
    
    public int J { get; private set; }




    // mothertile aka "The Board"
    public Tile(int color, float size)
    {
        this.Color = color;
        this.size = size;

    }

    // any child tile
    public Tile(int color, int i, int j, int level,
            Tile parent)
    {
        this.Color = color;
        this.parent = parent;
        this.Level = level;
        this.I = i;
        this.J = j;
        this.absSize = GetAbsSize();

    }

    

    public void Undivide()
    {
        Children = null;
    }

    public void Divide(int fraction)
    {
        if (fraction != 2 && fraction != 4 && fraction != 8)
        {
            return;
        }
        //divide one for sure
        Divide();
        //if fraction higher than 2, further divide children equally
        if (fraction == 4)
        {
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    Children[i,j].Divide();
                }
            }
        }
        else if (fraction == 8)
        {
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    Children[i,j].Divide(4);
                }
            }
        }

    }

    // don't allow empty (null) children
    public void Divide()
    {
        childFraction = 2;
        Children = new Tile[2,2];
        for (int i = 0; i < 2; i++)
        {
            for (int j = 0; j < 2; j++)
            {
                Children[i,j] = new Tile(
                        ((i + j) % 2) % 2, i, j, Level + 1, this);
            }
        }
    }

    public int GetChildFraction()
    {
        return childFraction;
    }

   
    public void RemoveChild(int i, int j)
    {
        Children[i,j] = null;
    }

   

   
    // implement recursive positioning and size
    private float GetRelX()
    {
        if (parent != null)
        {
            return I * GetAbsSize();
        }
        else
        {
            return 0;
        }
    }
    //the Y axis is opposite direction of J axis
    private float GetRelY()
    {
        if (parent != null)
        {
            return J * GetAbsSize();
        }
        else
        {
            return 0;
        }
    }

    private float GetAbsX()
    {
        if (_absX == 0 && parent != null)
        {
            _absX = GetRelX() + parent.GetAbsX();
        }
        return _absX;
       

    }
    //TODO
    public float GetDrawX()
    {
        return GetAbsX() + GetAbsSize() / 2;

    }
    //TODO
    public float GetDrawY()
    {
        return -GetAbsY() - GetAbsSize() / 2;
    }
        public float GetAbsCenterX()
    {
        return GetAbsX() + GetAbsSize() / 2;
    }
    public float GetAbsCenterY()
    {
        return GetAbsY() + GetAbsSize() / 2;
    }

    private float GetAbsY()
    {
        if (_absY == 0 && parent != null)
        {
            _absY = GetRelY() + parent.GetAbsY();
        }
        return _absY;

    }

    public int GetAbsFraction()
    {
        if (parent != null)
        {
            return parent.childFraction * parent.GetAbsFraction();
        }
        else
        {
            return 1;
        }
    }

    // get size relative to container
    public float GetAbsSize()
    {
        if (absSize == 0)
        {
            if (parent != null)
            {
                return (parent.GetAbsSize() / parent.GetChildFraction());
            }
            else
            {
                return size;
            }
        }
        else
        {
            return absSize;
        }

    }

    

   
}