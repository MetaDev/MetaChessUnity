using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementVisual : MonoBehaviour
{

    private void Update()
    {


        if (Input.GetMouseButtonDown(0))
        {
            click();
        }
    }
        
    public LevelBuilder builder;
    public Board board;
    bool flip = false;
    void click()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        // Casts the ray and get the first game object hit
        Physics.Raycast(ray, out hit);
        
        if (hit.collider != null)
        {


            var piece = new Piece(Piece.PieceType.pawn, builder.board, 1);
            var tile = hit.collider.GetComponent<TileGraphic>().tile;
            piece.SetTilePos(tile);
            flip = !flip;
            foreach (Tile tPath in piece.GetAllAccesibleTiles(1))
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
              
                ProcDraw.SetCubeColor(tPath.cube, col);
            }

        }


    }
}
