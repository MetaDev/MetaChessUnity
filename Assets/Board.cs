
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
            tileToPiece[tile] = piece;
        }
        pieceToTile[piece] = tile;

    }

    public void RandomFractionBoard(int iterations = 20)
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
    private bool TileMovOutParent(Tile tile, int horMov, int verMov)
    {

        return tile.I + horMov >= 2 ||
            tile.I + horMov < 0 ||
            tile.J + verMov >= 2 ||
            tile.J + verMov < 0;
    }
    #region Tile arithmic 
    public Tile FindTileNeighBour(Tile tile, int horMov, int verMov)
    {

        //find all close cubs
        Collider[] hitColliders = Physics.OverlapSphere(new Vector3(tile.GetDrawX(), (tile.GetAbsSize() / 20), tile.GetDrawY())
            , tile.GetAbsSize());
        int i = 0;
        var angle = Mathf.Atan2(horMov, -verMov) * Mathf.Rad2Deg;
        float diffAngle = 360;
        Tile neighbour = null;
        ProcDraw.SetCubeColor(tile.cube, Color.red);
        var threshold = 10;
        var tilePos = new Vector3(tile.cube.transform.position.x, 0, tile.cube.transform.position.z);

        var dist = float.MaxValue;

        while (i < hitColliders.Length)
        {
            var posHit = new Vector3(hitColliders[i].transform.position.x, 0, hitColliders[i].transform.position.z);
            var hitAngle = Mathf.Atan2((tilePos - posHit).x, (tilePos - posHit).z) * Mathf.Rad2Deg;
            var tile2 = hitColliders[i].GetComponent<TileGraphic>().tile;

            //var factor = (angle - hitAngle) / 45 + (tilePos - posHit).magnitude / tile.GetAbsSize();
            // go the cube which the angle direction is closest to movement, favour positive difference
            Debug.Log(Mathf.Abs(angle - hitAngle));

            
            if (hitColliders[i].gameObject != tile.cube &&
                Mathf.Abs(angle - hitAngle) < 45.0f / 2 &&
                 Mathf.Abs(angle - hitAngle) < diffAngle
                )
            {

                
                diffAngle = Mathf.Abs(angle - hitAngle);
                neighbour = tile2;

            }
            i++;
        }
        
            

        Debug.Log("final anlge" + diffAngle);
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
