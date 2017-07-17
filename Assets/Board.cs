
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
        
        return tile.I + horMov >=2 ||
            tile.I + horMov < 0 ||
            tile.J + verMov >=2 ||
            tile.J + verMov < 0;
    }
    #region Tile arithmic 
    public Tile FindTileNeighBour(Tile tile, int horMov, int verMov)
    {
        Debug.Log("tile: "+ tile.I + " , "+ tile.J);
        Debug.Log("mov: " + horMov + " " + verMov);
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
            Tile prevTile = tile;
            
            while (it.GetParent() != null && TileMovOutParent(prevTile, horMov, verMov))
            {
                prevTile = it;
                //safe indices from ascending process in hierarchy

                indices.Push(Tuple.Create(it.I, it.J));
                Debug.Log("en " + it.I + " " + it.J + " " + it.Level);
                it = it.GetParent();
                
               

            }
            Debug.Log("sibling I" + prevTile.I + " move"+ + horMov + " J " + (prevTile.J+ "mov " + verMov));
            if (prevTile.GetParent()!=null && !TileMovOutParent(prevTile, horMov, verMov))
            {
                //Debug.Log("sibling"+(it.I + horMov) + " " + (it.J + verMov));
                //neighbour is sibling
                neighbour = it.Children[prevTile.I + horMov, prevTile.J + verMov];
                indices.Pop();
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
            //first 
            int newI= (neighbour.I + Mathf.Abs(horMov) + 2)%2;
            int newJ= (neighbour.J + Mathf.Abs(verMov) + 2)%2;
           
            while (it.Children != null && indices.Count >0)
            {
                newI = (indices.Peek().Item1 + Mathf.Abs(horMov) + 2) % 2;
                newJ = (indices.Peek().Item2 + Mathf.Abs(verMov) + 2) % 2;
                Debug.Log("de "+ indices.Peek().Item1+  " " + indices.Peek().Item2 );
                indices.Pop();
                it = it.Children[newI,newJ];
            }
            
            neighbour = it;
            if (neighbour.Children != null)
            {
                it = neighbour.Children[(newI + Mathf.Abs(verMov) + 2) % 2, (newJ + Mathf.Abs(horMov) + 2) % 2];
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
