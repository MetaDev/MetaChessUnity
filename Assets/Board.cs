
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board
{

    public static int maxTurnBasedTileFraction = 64;
    private Dictionary<Tile, Piece> tileToPiece = new Dictionary<Tile, Piece>();
    private Dictionary<Piece, Tile> pieceToTile = new Dictionary<Piece, Tile>();
    private HashSet<Player> playersOnBoard = new HashSet<Player>();

    public Tile RootTile { get; private set; }
    public static int rootSize = 8 * 64;
    public static bool IsOverMaxFractionFromTileSize(float tilesSize)
    {
        return tilesSize < rootSize / maxTurnBasedTileFraction;
    }

    public Board()
    {
        this.RootTile = new Tile(0, rootSize);
        RootTile.Divide(8);

    }
    public void ChangePiecePosition(Piece piece, Tile tile)
    {
        //update lookup table and tiles

       

        //remove previous tile of piece from map
        if (pieceToTile.ContainsKey(piece))
        {
            //get previous tile from piece
            Tile previousTile = pieceToTile[piece];
            tileToPiece.Remove(previousTile);
        }


        //put new tile of piece in map
        if (tile != null)
        {
            tileToPiece[tile] =piece;
        }
        pieceToTile[piece]= tile;

    }

    public void RandomFractionBoard()
    {

        int fractionUnderMaxFraction = 32;
        int maxAbsFraction = maxTurnBasedTileFraction
                + fractionUnderMaxFraction;
        int iterations = 10;

        // the more iterations the more fractioned tiles

        int randFraction;
        int randAbsFraction;
        for (int i = 0; i < iterations; i++)
        {

            // pick random fraction
            randFraction = (int)System.Math.Pow(2, RangeIncl(1, 3));
            // pick a random max level of depth

            randAbsFraction = RangeIncl(8, maxAbsFraction);

            // now choose random tile
            GetRandomTile(true).Divide(randFraction);

        }

    }
    #region Tile arithmic 
    public Tile FindTileNeighBour(Tile tile, int horMov, int verMov, bool hoover, int startFraction)
    {
        Tile neighbour = tile;
        if (neighbour.GetParent() == null || !BoardLogic.IsSingleTileMovement(horMov) || !BoardLogic.IsSingleTileMovement(verMov))
        {
            return null;
        }
        if (tile.I + horMov > tile.GetParent().GetChildFraction() - 1 || 
            tile.I + horMov < 0 || 
            tile.J + verMov > tile.GetParent().GetChildFraction() - 1 || 
            neighbour.J + verMov < 0)
        {
            
            neighbour = GetTileFromAbsPosition(tile.GetAbsCenterX() + horMov * tile.GetAbsSize(), 
                                        tile.GetAbsCenterY() + verMov * tile.GetAbsSize(), startFraction);
        }
        else
        {
            neighbour = neighbour.GetParent().Children[neighbour.I + horMov, neighbour.J + verMov];
        }
        if (neighbour == null)
        {
            return neighbour;
        }
        if (neighbour.Children != null && !hoover)
        {
            neighbour = GetClosestLargestFractionTileFromNeighbour(tile, neighbour, verMov, horMov);
        }
        return neighbour;
    }
    public Tile GetTileFromAbsPosition(float x, float y, int maxFraction)
    {
        if (x < 0 || y < 0 || x > rootSize || y > rootSize)
        {
            return null;
        }
        Tile it = RootTile;
        int i;
        int j;
        float childSize;
        while (it.Children != null && it.GetAbsFraction() * it.GetChildFraction() <= maxFraction)
        {
            childSize = it.GetAbsSize() / it.GetChildFraction();
            i = (int)Mathf.Floor(x / childSize);
            j = (int)Mathf.Floor(y / childSize);
            it = it.Children[i,j];
            x -= i * childSize;
            y -= j * childSize;
            x = Mathf.Max(0, x);
            y = Mathf.Max(0, y);
        }
        return it;
    }

    public Tile GetClosestLargestFractionTileFromNeighbour(Tile tile, Tile neighbour, int verMov, int horMov)
    {
        double minDist;
        Tile closestChild;
        Tile it = BoardLogic.EnterLowerFractionOfTile(neighbour, horMov, verMov);
        while (it.Children != null)
        {
            closestChild = it.Children[1, 1];
            minDist = BoardLogic.EuclidianDistance(closestChild, tile);
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    if (BoardLogic.EuclidianDistance(it.Children[i, j], tile) < minDist)
                    {
                        closestChild = it.Children[i, j];
                        minDist = BoardLogic.EuclidianDistance(closestChild, tile);
                    }
                }
            }
            it = closestChild;
        }
        return it;
    }
    // return a tile with an abs fraction smaller then the one given
    public Tile GetRandomTile(bool canBeOccupied)
    {
        Tile tileIt;
        do
        {
            tileIt = RootTile;
            // now choose random tile
            int randCol;
            int randRow;
            while (tileIt.Children != null)
            {
                randCol = Random.Range(0, tileIt.Children.GetLength(0));
                randRow = Random.Range(0, tileIt.Children.GetLength(1));

                tileIt = tileIt.Children[randCol, randRow];
            }
        } while (!canBeOccupied && tileToPiece[tileIt] != null);
        return tileIt;
    }

    #endregion

    public Tile GetTileFromPiece(Piece piece)
    {
        return pieceToTile[piece];
    }
    public Piece GetPieceFromTile(Tile tile)
    {
        return tileToPiece[tile];
    }
    int RangeIncl(int min, int max)
    {
        return Random.Range(min, max + 1);
    }

    public void PieceTaken(Piece pieceTaken, Piece pieceTaker)
    {
        Player playerTaken = getPlayerByPiece(pieceTaken);
        Player playerTaker = getPlayerByPiece(pieceTaker);
        //decrease team lives
        if (playerTaken != null)
        {
            //decreaseSideLives(playerTaken.getSide(),
            //        pieceTaken.getLives());
            //adapt player lives lost and taken
            //playerTaken.increaseLivesLost(pieceTaken.getLives());

            //lives not added to your own score if a bischop step on burning tail
            //if (!playerTaken.equals(playerTaker))
            //{
            //    playerTaker.increaseLivesTaken(pieceTaken.getLives());
            //}
        }
        //put piecetaken on a random tile
        pieceTaken.SetTilePos(GetRandomTile(false));

    }
    public Player getPlayerByPiece(Piece piece)
    {
        foreach (Player player in playersOnBoard)
        {
            
            if (piece.Equals(player.Piece))
            {
                return player;
            }
        }
        return null;
    }

   
}
