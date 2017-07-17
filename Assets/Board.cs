
using System.Collections;
using System.Collections.Generic;
using UniRx;
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

    public void RandomFractionBoard(int iterations=20)
    {

        int fractionUnderMaxFraction = 32;
        int maxAbsFraction = maxTurnBasedTileFraction
                + fractionUnderMaxFraction;
        

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
    private bool TileMovOutParent(Tile tile, int horMov, int verMov )
    {
        
        return tile.I + horMov >= 2 ||
            tile.I + horMov < 0 ||
            tile.J + verMov >= 2 ||
            tile.J + verMov < 0;
    }
    #region Tile arithmic 
    public Tile FindTileNeighBour(Tile tile, int horMov, int verMov)
    {
        Debug.Log(tile.I + " test " + tile.J);
        Tile neighbour=null;
        if (tile.GetParent() == null || !BoardLogic.IsSingleTileMovement(horMov) || !BoardLogic.IsSingleTileMovement(verMov))
        {
            return null;
        }
        Stack<Tuple<int, int>> indices = new Stack<Tuple<int, int>>();
        
        if (TileMovOutParent(tile,horMov,verMov))
        {
            //The piece is at the border
            //find piece without children that is closest

            //move into parents until one has neighbours
            Tile it = tile;
            Tile prevTile = it;
            
            while (prevTile.GetParent() != null && TileMovOutParent(prevTile, horMov, verMov))
            {
                prevTile = it;
                //safe indices from ascending process in hierarchy
                Debug.Log("en "+ it.I + " " + it.J + " " + it.Level);
                indices.Push(Tuple.Create(it.I, it.J));
                it = it.GetParent();
                
            }
            Debug.Log("sibling I" + prevTile.I + " move"+ + horMov + " J " + (prevTile.J+ "mov " + verMov));
            if (prevTile.GetParent()!=null && !TileMovOutParent(prevTile, horMov, verMov))
            {
                Debug.Log("sibling"+(it.I + horMov) + " " + (it.J + verMov));
                //neighbour is sibling
                neighbour = it.Children[prevTile.I + horMov, prevTile.J + verMov];
            }
            //if movement not possible and top level is reached try seperate
            if (neighbour == null && Mathf.Abs(horMov)+Mathf.Abs(verMov)==2)
            {
                //first hor than ver

                var tempNeighbour = FindTileNeighBour(tile, horMov, 0);
                if (tempNeighbour != null)
                {
                    tempNeighbour = FindTileNeighBour(tempNeighbour, 0, verMov);
                }
                //Than ver and hor
                if (tempNeighbour == null)
                {
                    tempNeighbour = FindTileNeighBour(tile, 0, verMov);
                    if (tempNeighbour != null)
                    {
                        tempNeighbour = FindTileNeighBour(tempNeighbour, horMov, 0);
                    }
                }
                neighbour = tempNeighbour;
            }
            
        }
        else
        {
            
            neighbour = tile.GetParent().Children[tile.I + horMov, tile.J + verMov];
        }
        
        if (neighbour == null)
        {
            return neighbour;
        }
        //go deeper into the children
        if (neighbour.Children != null)
        {
            var it = neighbour;
            //init values should not be used
            //first I
            
            int newI= (neighbour.I + horMov + 2)%2;
            int newJ= (neighbour.J + verMov + 2)%2;
           
            while (it.Children != null && indices.Count >0)
            {
                newI = (indices.Peek().Item1 + horMov + 2) % 2;
                newJ = (indices.Peek().Item2 + verMov + 2) % 2;
                Debug.Log("de "+ indices.Peek().Item1 + " " + horMov + " " + indices.Peek().Item2 + " " + verMov);
                indices.Pop();
                it = it.Children[newI,newJ];
            }
            
            neighbour = it;
            if (neighbour.Children != null)
            {
                it = neighbour.Children[(newI - verMov + 2) % 2, (newJ - horMov + 2) % 2];
                //newI = (newI - verMov + 2)%2;
                //newJ = (newJ - horMov +2)%2;
                while (it.Children != null)
                {  
                    it = it.Children[newI, newJ];
                }
                neighbour = it;
            }
            
        }
        
        return neighbour;
    }
    //public Tile GetTileFromAbsPosition(float x, float y, int maxFraction)
    //{
    //    if (x < 0 || y < 0 || x > rootSize || y > rootSize)
    //    {
    //        return null;
    //    }
    //    Tile it = RootTile;
    //    int i;
    //    int j;
    //    float childSize;
    //    while (it.Children != null && it.GetAbsFraction() * it.GetChildFraction() <= maxFraction)
    //    {
    //        childSize = it.GetAbsSize() / it.GetChildFraction();
    //        i = (int)Mathf.Floor(x / childSize);
    //        j = (int)Mathf.Floor(y / childSize);
    //        it = it.Children[i, j];
    //        x -= i * childSize;
    //        y -= j * childSize;
    //        x = Mathf.Max(0, x);
    //        y = Mathf.Max(0, y);
    //    }
    //    return it;
    //}

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
