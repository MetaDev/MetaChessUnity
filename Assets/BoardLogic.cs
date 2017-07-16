using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardLogic {
    //return wether the movement is a single tile movement
    public static bool IsSingleTileMovement(int mov)
    {
        return mov == 0 || mov == 1 || mov == -1;
    }


    public static double EuclidianDistance(Tile tile1, Tile tile2)
    {
        //calculate euclididan distance from center of tile
        return Mathf.Sqrt(Mathf.Pow(tile1.GetAbsCenterX() - tile2.GetAbsCenterX(), 2) 
                        + Mathf.Pow(tile1.GetAbsCenterY() - tile2.GetAbsCenterY(), 2));
    }


    public static Tile EnterLowerFractionOfTile(
            Tile tile, int horMov, int verMov)
    {
        int i = 0;
        int j = 0;

        //right
        if (horMov == 1 && verMov == 0)
        {
            i = 0;
            j = 1;
        } //left
        else if (horMov == -1 && verMov == 0)
        {
            i = 1;
            j = 0;
        } //up
        else if (horMov == 0 && verMov == 1)
        {
            i = 0;
            j = 0;
        } //down
        else if (horMov == 0 && verMov == -1)
        {
            i = 1;
            j = 1;
        } //up right
        else if (horMov == 1 && verMov == 1)
        {
            i = 0;
            j = 0;
        } //down right
        else if (horMov == 1 && verMov == -1)
        {
            i = 0;
            j = 1;
        } //down left
        else if (horMov == -1 && verMov == -1)
        {
            i = 1;
            j = 1;
        } //up left
        else if (horMov == -1 && verMov == 1)
        {
            i = 1;
            j = 0;
        }

        return tile.Children[i,j];
    }

  

    //public static bool isInrange(Piece viewer,
    //        Piece subject)
    //{
    //    // calculate viewsquare boundaries
    //    double x = viewer.getTilePosition().getAbsX()
    //            - viewer.getTilePosition().getAbsSize()
    //            * viewer.getNrOfViewTiles();
    //    double y = viewer.getTilePosition().getAbsY()
    //            - viewer.getTilePosition().getAbsSize()
    //            * viewer.getNrOfViewTiles();
    //    double s = viewer.getTilePosition().getAbsSize()
    //            * (2 * viewer.getNrOfViewTiles() + 1);
    //    //the tile of the subject should be completely in the view
    //    if (subject.getTilePosition().getAbsX() >= x
    //            && subject.getTilePosition().getAbsX() + subject.getTilePosition().getAbsSize() <= x + s)
    //    {
    //        if (subject.getTilePosition().getAbsY() >= y
    //                && subject.getTilePosition().getAbsY() + subject.getTilePosition().getAbsSize() <= y + s)
    //        {
    //            return true;
    //        }
    //    }
    //    return false;
    //}
}
