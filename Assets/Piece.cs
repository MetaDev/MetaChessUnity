using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Direction = Directions.Direction;
public class Piece
{
    public float Color { get; private set; }
    public Board Board { get; private set; }
    public PieceType Type { get; private set; }
    private HashSet<Direction> allowedMovement = new HashSet<Direction>();
    public enum PieceType
    {
        pawn, rook, knight, bischop, king, queen
    }
    public Piece(PieceType type, Board board, float color)
    {
        this.Board = board;
        this.Color = color;
        this.Type = type;
      
        Directions.GetDiagDirections(allowedMovement);
        //allowedMovement.Add(Directions.getDirection(1,-1));

    }
    public Tile GetTilePos()
    {
        return Board.GetTileFromPiece(this);
    }
    public void SetTilePos(Tile newPos)
    {
        Board.ChangePiecePosition(this, newPos);
    }
    //find all accesible tiles
    public List<Tile> GetAllAccesibleTiles(int range)
    {
        List<Tile> allTiles = new List<Tile>();
        foreach (Direction dir in allowedMovement)
        {
            int i = dir.X * range;
            int j = dir.Y * range;
            var t = FindPath(i, j);
            allTiles.AddRange(t);
        }
        return allTiles;
    }

    //public bool HandleMovement(Direction direction, int range)
    //{
    //    int i = direction.X * range;
    //    int j = direction.Y * range;
    //    List<Tile> path = FindPath( i, j,false);
    //    if (path.Count < range)
    //    {
    //        return false;
    //    }
    //    return MoveWithPath(path);
    //}
    public List<Tile> FindPath(Tile tile,int i, int j)
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
            path.Add(previousTile);

        }
        return path;
    }
    #region piece movement
    public List<Tile> FindPath(int i, int j)
    {
        return FindPath(Board.GetTileFromPiece(this), i, j);
    }
    public bool MoveWithPath( List<Tile> path)
    {
        //chech if path and last tile are valid

        //first check last tile for move to be made, than check path
        if (CheckPath(path))
        {
            HandleLastTileInPath(path[path.Count - 1]);
            return true;
        }
        return false;
    }

    //handle movement on last tile, and set position
    private bool HandleLastTileInPath(Tile lastTileInPath)
    {
        Piece pieceOnnewTile = Board
                .GetPieceFromTile(lastTileInPath);
        if (CanPieceKillMe(pieceOnnewTile))
        {
            Board.PieceTaken(this, pieceOnnewTile);
        }
        SetTilePos(lastTileInPath);
        return true;
    }

    //check if path can be taken
    private  bool CheckPath(List<Tile> path)
    {
        Piece pieceOnnewTile = Board
                .GetPieceFromTile(path[path.Count - 1]);
        //first check last tile for move to be made, than check path
        if (pieceOnnewTile != null && !CanPieceKillMe(pieceOnnewTile))
        {
            return false;
        }
        //if not occupied continue
        Piece pieceOnPath;
        //check if path doesn't contain any other pieces that are in the way or burning
        for (int i = path.Count - 1; i >= 0; i--)
        {
            pieceOnPath = Board.GetPieceFromTile(path[i]);
            //if path occupied, bad path
            if (pieceOnPath != null)
            {
                //no movement made
                return false;
            }
        }

        return true;
    }

    #endregion
    public bool CanPieceKillMe(Piece piece)
    {
        //check if player in piece can be killed
        return piece != null && piece.Color != Color;
    }


}
