using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using MoreLinq;
using UniRx.Triggers;
using UniRx;
using TileIJ = System.Collections.Generic.List<UniRx.Tuple<int, int>>;

public class LevelBuilder : MonoBehaviour
{
    public int NrOfPieces = 100;
    //instead of square pass tile prefab
    public GameObject square;
    //piece prefab
    public PieceGraphic piecePrefab;

    //to be sent to next clients
    public Map<PieceGraphic, TileGraphic> pieceTile;
    private List<TileGraphic> tiles;
    public DataSync DataSync;


    public Board board;
    public Dictionary<int, Player> players;

    public ClockScript clock;
    public CameraBehaviour CamBehaviour;
    public BoardInteraction BI;
    public Player PlayerPrefab;

    public Player LocalPlayer { get; private set; }
    public void SetLocalPlayer(Player player)
    {
        LocalPlayer = player;
        clock.Init(player);
        CamBehaviour.Init(player);
        
    }
    //To test
    void resetGame()
    {
        //reverse order of creation
        //first player than pieces and tiles
        foreach (Player pl in players.Values)
        {
            DestroyImmediate(pl.gameObject);
        }
        foreach (PieceGraphic pg in pieceTile.AsEnumerable().Select(p => p.Key))
        {
            DestroyImmediate(pg.gameObject);
        }
        foreach (TileGraphic tileG in tiles)
        {
            DestroyImmediate(tileG.gameObject);
        }
       
        
        LocalPlayer = null;
        board = null;
        players = new Dictionary<int, Player>();
        pieceTile = new Map<PieceGraphic, TileGraphic>();
        tiles = new List<TileGraphic>();

    }
   
   
    public List<Tuple<List<Tuple<int, int>>, int>> InitBoard(List<Tuple<List<Tuple<int, int>>, int>> IJsAndfraction = null)
    {
        if (board != null)
        {
            resetGame();
        }
        players = new Dictionary<int, Player>();
        pieceTile = new Map<PieceGraphic, TileGraphic>();
        tiles = new List<TileGraphic>();
        board = new Board();
        List<Tuple<List<Tuple<int, int>>, int>> res = null;
        if (IJsAndfraction == null)
        {
            res = board.GetRandomFractionBoard(20);
            board.ApplyFraction(res);

        }
        else
        {
            board.ApplyFraction(IJsAndfraction);

        }

        return res;
    }
    public void DrawBoard()
    {
        //draw has to be run before other code that uses the tile cubes
        ProcDraw.DrawBoardRecursive(square, tiles, board.RootTile);
    }
    public List<Tuple<List<Tuple<int, int>>, int>> GeneratePieces()
    {
        var tileSet = new HashSet<TileIJ>();
        var list = new List<Tuple<List<Tuple<int, int>>, int>>();
        foreach (int i in Enumerable.Range(1, NrOfPieces))
        {
            var pTile = GetRandomUnoccupiedTile(tileSet);
            tileSet.Add(pTile.tile.IJs);
            list.Add(Tuple.Create(pTile.tile.IJs, i % 2));

        }

        return list;
    }
    public void DrawInitPieces(List<Tuple<List<Tuple<int, int>>, int>> list)
    {
        foreach (Tuple<List<Tuple<int, int>>, int> tup in list)
        {
            var piece = Instantiate(piecePrefab);
            var tile = board.GetTileByIJs(tup.Item1).TileGraphic;
            piece.init(this, tup.Item2, tile);
            UpdatePiecePosition(piece, tile);
        }
    }
    public int GetLocalScore()
    {
        return pieceTile.AsEnumerable().Where(p => p.Key.piece.Color == LocalPlayer.side).Count();
    }
   public PieceGraphic GetPieceGraphicByName(string Name)
    {
        var t = PhotonNetwork.player.ID;
        return pieceTile.AsEnumerable().Select(p => p.Key).Where(p => p.Name==Name).FirstOrDefault();
    }
 
    public Player AddPlayer(int side,int id,Color32 col, PieceGraphic pg=null)
    {
        var player = Instantiate(PlayerPrefab);
        
        player.Init(this, side, id, col);
        var piece = pg == null ? GetRandomPieceFromSide(side) : pg;
        player.StartOnPiece(piece);
        //if no tile is given, this means that it's being decided by this client, meaning it's the local player
        if (pg == null)
        {
            SetLocalPlayer(player);
        }
        players[id] = player;
        return player;

    }
    public IEnumerable<Player> GetPlayersFromSide(int side)
    {
        return players.Values.Where(player => player.side == side);
    }

    public Player GetPlayerFromPiece(PieceGraphic pieceGr)
    {
        return players.Values.Where(player => player.PieceGraphic == pieceGr).FirstOrDefault();
    }
    public List<PieceGraphic> GetAllPiecesFromSide(int side)
    {
        return pieceTile.AsEnumerable().Select(p => p.Key).Where(p => p.piece.Color == side).ToList();
    }
    public PieceGraphic GetRandomPieceFromSide(int side)
    {
        var pieces = GetAllPiecesFromSide(side);

        return pieces.Skip(Random.Range(0, pieces.Count() - 1)).FirstOrDefault();

    }
    
    TileGraphic GetRandomUnoccupiedTile(HashSet<TileIJ> occupied=null)
    {
        TileGraphic tile = null;
        
        bool ocT = false;
        do
        {
            tile = tiles[Random.Range(0, tiles.Count() - 1)];
            ocT = occupied == null ? pieceTile.Reverse.Contains(tile) : occupied.Contains(tile.tile.IJs);
        } while (ocT);
        return tile;
    }
    // Update is called once per frame
    void Update()
    {
      
    }
   public void UpdatePiecePosition(PieceGraphic pieceG, TileGraphic tileGr)
    {
        if (pieceTile.Reverse.Contains(tileGr))
        {
            Debug.Log("you're trying to place a player on tile that is already occupied");
            return;
        }
        pieceTile.Remove(pieceG);
        pieceTile.Add(pieceG, tileGr);

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
            foreach (Tile tPath in player.piece.GetAllAccesibleTiles(8, this, player))
            {
                tPath.cube.GetComponent<TileGraphic>().SetReachable(player);
            }
        }

    }

}
