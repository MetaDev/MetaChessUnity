using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Directions
{

    public class Direction
    {

        public int X { get; private set; }
        public int Y { get; private set; }
       

        public string getName()
        {
            return X + "" + Y;
        }

        public Direction(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }
        
        private string name;
    }

    //for turn;
    private static List<Direction> directionsOrthDiagOrder = new List<Direction>();
    private static Dictionary<string, Direction> allDirections = new Dictionary<string, Direction>();

    public static Direction getDirection(int x, int y)
    {
        string name = x + "" + y;
        if (allDirections.Count==0)
        {
            Init();
        }
        return allDirections[name];
    }
    //untested!!
    //public static Direction turnDirection(Direction direction, int turn)
    //{
    //    return directionsOrthDiagOrder[((
    //        directionsOrthDiagOrder.IndexOf(allDirections[
    //            direction.getName()]) + turn) % directionsOrthDiagOrder.Count)];
    //}

    //public static int getTurnFromDirection(Direction direction)
    //{
    //    return directionsOrthDiagOrder.indexOf(direction);
    //}
    //public static Direction getDirectionFromTurn(int turn)
    //{
    //    return directionsOrthDiagOrder.get(turn);
    //}


    private static void Init()
    {
        Direction d;
        //orhtogonal and diagonal directions
        //all knight directions
        int[] indices;
        //downleft, left, upleft
        indices = new int[] { -1, 0, 1 };
        foreach (int y in indices)
        {
            d = new Direction(-1, y);
            allDirections[d.getName()] =d;
            directionsOrthDiagOrder.Add(d);
        }
        //up
        d = new Direction(0, 1);
        allDirections[d.getName()]= d;
        directionsOrthDiagOrder.Add(d);
        //upright,right,downright
        indices = new int[] { 1, 0, -1 };
        foreach (int y in indices)
        {
            d = new Direction(1, y);
            allDirections[d.getName()] =d;
            directionsOrthDiagOrder.Add(d);
        }
        //down
        d = new Direction(0, -1);
        allDirections[d.getName()] =d;
        directionsOrthDiagOrder.Add(d);

        //all knight directions
        indices = new int[] { -2, -1, 1, 2 };
        foreach (int x in indices)
        {
            foreach (int y in indices)
            {
                if (Mathf.Abs(x) != Mathf.Abs(y))
                {
                    d = new Direction(x, y);
                    allDirections[d.getName()] =d;
                }
            }
        }

    }

    public static void GetKnightDirections(HashSet<Direction> coll)
    {
        //all knight directions
        int[] indices = new int[] { -2, -1, 1, 2 };
        foreach (int x in indices)
        {
            foreach (int y in indices)
            {
                if (Mathf.Abs(x) != Mathf.Abs(y))
                {
                    coll.Add(getDirection(x, y));
                }
            }
        }
    }

    public static void GetOrthoDirections(HashSet<Direction> coll)
    {
        //all knight directions
        int[] indices = new int[] { -1, 0, 1 };
        foreach (int x in indices)
        {
            foreach (int y in indices)
            {
                if (Mathf.Abs(x) != Mathf.Abs(y))
                {
                    coll.Add(getDirection(x, y));
                }
            }
        }
    }

    public static void GetDiagDirections(HashSet<Direction> coll)
    {
        //all knight directions
        int[] indices = new int[] { -1, 1 };
        foreach (int x in indices)
        {
            foreach (int y in indices)
            {

                coll.Add(getDirection(x, y));

            }
        }
    }

}