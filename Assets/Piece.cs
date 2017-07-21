using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Direction = Directions.Direction;
public class Piece
{
    public int Color { get; set; }
    public Tile tile;
    private HashSet<Direction> allowedMovement = new HashSet<Direction>();

    public Piece(int color)
    {
        this.Color = color;
        Directions.GetOrthoDirections(allowedMovement);
    }
    #region piece movement
    //find all accesible tiles
    public List<Tile> GetAllAccesibleTiles(int range, LevelBuilder builder, Player player, HashSet<Piece> pieces = null)
    {

        if (pieces == null)
        {
            pieces = new HashSet<Piece>();
            //the tile of the starting piece should not be added
            pieces.Add(this);
        }

        List<Tile> allTiles = new List<Tile>();
        foreach (Direction dir in allowedMovement)
        {
            int i = dir.X * range;
            int j = dir.Y * range;
            var t = FindPath(tile, builder, player, i, j, pieces);
            allTiles.AddRange(t);
        }
        return allTiles;
    }
    public List<Tile> FindPath(Tile tile, LevelBuilder builder, Player player, int i, int j, HashSet<Piece> pieces)
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

            if (previousTile == null)
            {
                return path;
            }
            //if the tile is reachable by player of opposite side, stop
            if (previousTile.TileGraphic.Reachable!=null && previousTile.TileGraphic.Reachable.side==((player.side + 1) % 2))
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
                        //add tile of piece
                        path.Add(previousTile);
                        //extend path
                        path.AddRange(pieceOnPath.piece.GetAllAccesibleTiles(4, builder, player, pieces));
                        return path;
                    }
                    else
                    {
                        return path;
                    }

                }
                else
                {
                    //if there's a piece from the opposite path, you can catch it
                    path.Add(previousTile);

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
