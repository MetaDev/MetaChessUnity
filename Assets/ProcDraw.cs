using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProcDraw : MonoBehaviour {

   
    public static void DrawTile(GameObject cube,Tile tile,List<TileGraphic> tiles)
    {
        var newsquare = Instantiate(cube);
        newsquare.GetComponent<TileGraphic>().tile = tile;
        tiles.Add(newsquare.GetComponent<TileGraphic>());
        tile.cube = newsquare;
 
        newsquare.GetComponent<Renderer>().material.color = new Color(tile.Color, tile.Color, tile.Color);
        newsquare.transform.localScale = new Vector3(tile.GetAbsSize(), tile.GetAbsSize(), tile.GetAbsSize());
        newsquare.transform.position = new Vector3(tile.GetDrawCenterX(), 0, tile.GetDrawCenterY());
        
    }
    public static void SetCubeColor(GameObject cube, Color col)
    {
        cube.GetComponent<Renderer>().material.color = col;
    }
    public static  void DrawBoardRecursive(GameObject cube,List<TileGraphic> tiles, Tile tile)
    {
        // if no children anymore draw square
        if (tile.Children == null)
        {
            DrawTile(cube, tile,tiles);
        } // recursive render
        else
        {
            for (int i = 0; i < tile.GetChildFraction(); i++)
            {
                for (int j = 0; j < tile.GetChildFraction(); j++)
                {
                    if (tile.Children[i, j] != null)
                    {
                        DrawBoardRecursive(cube,tiles, tile.Children[i, j]);
                    }

                }
            }
        }

    }
 
}
