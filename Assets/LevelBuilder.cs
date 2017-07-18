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
    public List<PieceGraphic> GetAllPiecesFromSide(int side)
    {
        return pieceTile.AsEnumerable().Select(p => p.Key).Where(p => p.piece.Color == side).ToList();
    }
    PieceGraphic GetRandomPieceFromSide(int side)
    {

        return pieceTile.AsEnumerable().Select(p => p.Key)
            .Where(p => p.piece.Color == side).Skip(Random.Range(0, pieceTile.Count() - 1))
            .First();
   
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

    }
    public void SetPieceOnTile(Player player, TileGraphic tileGr)
    {
        if (pieceTile.Reverse.Contains(tileGr))
        {
            Debug.Log("you're trying to place a player on tile that is already occupied");
            return;
        }
       
        pieceTile.Remove(player.PieceGraphic);
       
        pieceTile.Add(player.PieceGraphic, tileGr);
        player.SetPiecePosition(tileGr);
        UpdatePathVisual(player);
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
    public void UpdatePathVisual(Player player)
    {
        foreach (TileGraphic t in tiles)
        {
            if (t.Reachable == player)
            {
                t.SetReachable(null);
            }

        }


        if (player != null && player.piece != null)
        {
            foreach (Tile tPath in player.piece.GetAllAccesibleTiles(8, this))
            {
                tPath.cube.GetComponent<TileGraphic>().SetReachable(player);
            }
        }

    }

}
