using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using MoreLinq;
using UniRx.Triggers;
public class LevelBuilder : MonoBehaviour
{
    //instead of square pass tile prefab
    public GameObject square;
    //piece prefab
    public PieceGraphic piecePrefab;
    public Board board;

    public ClockScript clock;
    public BoardInteraction BI;
    public Player player;
    public Player player2;
    private List<TileGraphic> tiles = new List<TileGraphic>();
    public Map<PieceGraphic, TileGraphic> pieceTile = new Map<PieceGraphic, TileGraphic>();
    public int score;
    // Use this for initialization
    void Start()
    {

        board = new Board();
        board.RandomFractionBoard(20);
        //draw has to be run before other code that uses the tile cubes
        ProcDraw.DrawBoardRecursive(square, tiles, board.RootTile);
        foreach (int i in Enumerable.Range(1, 100))
        {
            //init game object
            var piece = Instantiate(piecePrefab);
            piece.init(this, i % 2);
            //add to game
            SetPieceOnTile(piece, GetRandomUnoccupiedTile());
        }

        //assign player to random piece
        //this is seperate because all start methods need to be run before calling this

        player.PieceGraphic = GetRandomPieceFromSide(player.side);
        player2.PieceGraphic = GetRandomPieceFromSide(player2.side);
    }
    
    public void BuildLevel()
    {
        
    }
    public void removePiece(PieceGraphic pieceGr)
    {
        pieceTile.Remove(pieceGr);
        if (pieceGr.Equals(player))
        {
            var piece = GetRandomPieceFromSide(player.side);

            if (piece == null)
            {
                Debug.Log("you lost are your pieces.");
                return;
            }
            player.PieceGraphic = piece;
        }
        if (pieceGr.Equals(player2))
        {
            score++;
            var piece = GetRandomPieceFromSide(player2.side);

            if (piece == null)
            {
                Debug.Log("you took all opponent pieces.");
            }
            player2.PieceGraphic = piece;
        }

        //TODO if the piece contains a player, do something with it
        Destroy(pieceGr);
    }
    PieceGraphic GetRandomPieceFromSide(int side)
    {
        PieceGraphic piece = null;
        foreach (PieceGraphic pieceIt in pieceTile.AsEnumerable().Select(p => p.Key))
        {

            if (pieceIt.piece.Color == side)
            {
                piece = pieceIt;
            }
        }
        return piece;
    }
    TileGraphic GetRandomUnoccupiedTile()
    {
        TileGraphic tile = null;
        do
        {
            tile = tiles[Random.Range(0, tiles.Count() - 1)];
        } while (pieceTile.Reverse.Contains(tile));
        return tile;
    }
    // Update is called once per frame
    void Update()
    {

    }
    private void SetPieceOnTile(PieceGraphic pieceG, TileGraphic tileGr)
    {
        pieceTile.Remove(pieceG);
        pieceTile.Add(pieceG, tileGr);
        pieceG.SetPiecePosition(tileGr);
        UpdatePathVisual();
    }
    public void SetPieceOnTile(Player player, TileGraphic tileGr)
    {
        pieceTile.Remove(player.PieceGraphic);
        pieceTile.Add(player.PieceGraphic, tileGr);
        player.SetPiecePosition(tileGr);
        UpdatePathVisual();
    }
    public PieceGraphic GetPieceOnTile(TileGraphic tile)
    {
        if (pieceTile.Reverse.Contains(tile))
        {
            return pieceTile.Reverse[tile];
        }
        return null;
    }
    public TileGraphic GetTileFromPiece(PieceGraphic piece)
    {
        if (pieceTile.Forward.Contains(piece))
        {
            return pieceTile.Forward[piece];
        }
        return null;
    }
    public void UpdatePathVisual()
    {
        foreach (TileGraphic t in tiles)
        {
            t.SetReachable(false, null);
        }


        if (player != null && player.piece != null)
        {
            foreach (Tile tPath in player.piece.GetAllAccesibleTiles(8, this))
            {
                tPath.cube.GetComponent<TileGraphic>().SetReachable(true, player);
            }
        }

    }

}
