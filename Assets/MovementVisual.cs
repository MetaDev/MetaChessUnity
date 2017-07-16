using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementVisual : MonoBehaviour {

    public Tile tile;
    public Board board;
    bool flip = false;
    void OnMouseDown()
    {
        var piece = new Piece(Piece.PieceType.pawn, board, 1);
        
        piece.SetTilePos(tile);
        flip = !flip;
        foreach (Tile tPath in piece.GetAllAccesibleTiles(8))
        {
            Color col;
            if (flip)
            {
                col = Color.cyan;
            }
            else
            {
                col = new Color(tPath.Color, tPath.Color, tPath.Color);
            }
            print(tPath.cube);
            ProcDraw.SetCubeColor(tPath.cube, col);
        }
    }


    // Update is called once per frame
    void Update () {
		
	}
    void Start()
    {

    }
}
