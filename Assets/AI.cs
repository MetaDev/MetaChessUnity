using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx.Triggers;
using UniRx;
public class AI : MonoBehaviour {
    //not working in networking
    //TODO add commands to datasync for moves
    public LevelBuilder builder;
    public float moveTileChance=0.8f;
    public int ms=1000;
    public bool active;
    Player player;
	// Use this for initialization
	void Start () {
        //every 100 ms
        Observable.Interval(System.TimeSpan.FromMilliseconds(ms)).Subscribe(_=> {
            if (player == null)
            {
                return;
            }
            //check if turn
            
            if (player.CanMove())
            {
                //choose between moving on accesible tiles and switching pieces
                bool moveTile = Random.value > (1 - moveTileChance);
                if (moveTile)
                {
                    var allTiles = player.piece.GetAllAccesibleTiles(8, builder,player);
                    if (allTiles.Count > 0)
                    {
                        var randTile = allTiles[Random.Range(0, allTiles.Count - 1)];
                        player.EnterTilePiece(randTile.TileGraphic, null);
                       
                    }
                    else
                    {
                        //if there are no available tiles, move the from piece
                        MovePiece();
                    }
                }
                else
                {
                    MovePiece();
                }
            }
        });

    }
    public void SetPlayer(Player player)
    {
        this.player = player;
    }
	void MovePiece()
    {
        var allPieces = builder.GetAllPiecesFromSide(player.side);
        var randPiece = allPieces[Random.Range(0, allPieces.Count - 1)];
        player.EnterTilePiece(null, randPiece);
    }
	// Update is called once per frame
	void Update () {
		
	}
}
