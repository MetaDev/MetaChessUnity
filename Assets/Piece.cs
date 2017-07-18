using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Direction = Directions.Direction;
public class Piece
{
    public float Color { get; private set; }
    public Tile tile;
    private HashSet<Direction> allowedMovement = new HashSet<Direction>();

    public Piece(float color)
    {
        this.Color = color;
        Directions.GetOrthoDirections(allowedMovement);
    }
    #region piece movement
    //find all accesible tiles
    public List<Tile> GetAllAccesibleTiles(int range, LevelBuilder builder, HashSet<Piece> pieces =null)
    {
        
        if (pieces == null)
        {
           pieces = new HashSet<Piece>();
        }
        
        List<Tile> allTiles = new List<Tile>();
        foreach (Direction dir in allowedMovement)
        {
            int i = dir.X * range;
            int j = dir.Y * range;
            var t = FindPath(tile, builder, i, j,pieces);
            allTiles.AddRange(t);
        }
        return allTiles;
    }
    public List<Tile> FindPath(Tile tile, LevelBuilder builder, int i, int j, HashSet<Piece> pieces)
    {
        //use direction coordinates to construct a tile path
        int verMov;
        int horMov;
        int remainingHorMov = i;
        int remainingVerMov = j;

        List<Tile> path = new List<Tile>();
        Tile previousTile = tile;

        //continue while movement left
        while (Mathf.Abs(remainingHorMov) + Mathf.Abs(remainingVerMov) > 0)
        {
            //calculate singel tile directions
            verMov = System.Math.Sign(j);
            horMov = System.Math.Sign(i);
            //decrease remaining movement
            remainingHorMov = remainingHorMov - horMov;
            remainingVerMov = remainingVerMov - verMov;
            //on the last step of the movement hoover is always false
            previousTile = Board.FindTileNeighBour(previousTile,
                        horMov, verMov);
          
            if (previousTile == null )
            {
                return path;
            }
            var pieceOnPath = builder.GetPieceOnTile(previousTile.cube.GetComponent<TileGraphic>());
            if (pieceOnPath != null)
            {
                
                if (pieceOnPath.piece.Color == Color)
                {
                    //only extend path if the piece has not alread been seen
                    if (!pieces.Contains(pieceOnPath.piece))
                    {
                        pieces.Add(pieceOnPath.piece);
                        //extend path
                        path.AddRange(pieceOnPath.piece.GetAllAccesibleTiles(4, builder, pieces));
                    }
                    else
                    {
                        return path;
                    }

                }
                else
                {
                    return path;
                }
            }
           
            
            path.Add(previousTile);

        }
        return path;
    }

    #endregion
    public bool CanPieceKillMe(Piece piece)
    {
        //check if player in piece can be killed
        return piece != null && piece.Color != Color;
    }


}
