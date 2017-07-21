using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileGraphic : MonoBehaviour
{

    public Tile tile;
    //null
    public Color captured;

    public void SetReachable( Player player)
    {

        Reachable = player;
        if (player != null)
        {
            ProcDraw.SetCubeColor(gameObject, player.col);
        }
        else
        {
            ProcDraw.SetCubeColor(gameObject, new Color(tile.Color, tile.Color, tile.Color));
        }


    }
    public Player Reachable { get; private set; }

 
}
