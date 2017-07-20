using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UnityEngine.UI;
using System;

public class Player : MonoBehaviour
{
    private PieceGraphic _pieceGraphic;
    public LevelBuilder builder;
    public Color col;
    public int side;
    public bool MadeMoveInTurn;
    public bool turn;
    private void Start()
    {
     
        ProcDraw.SetCubeColor(this.gameObject, col);
        //subscribe to clock and reset the mademoveinturn once the tile is available to calculate clock turn reset
            
        StartCoroutine(SubscribeToClock());
    }
    bool previousTurn;
    IEnumerator SubscribeToClock()
    {
        //add some delay to make sure the piecegraphic is 
        yield return new WaitForSeconds(1);
       
        Clock.GetAbsoluteTimeObservable()
           .Subscribe(_ => 
           {
               turn = Clock.GetTurnFromAbsTime(_, side, tile);
               //This is a hack, better is to get the time of a turn on a tile
               if (turn != previousTurn)
               {
                   MadeMoveInTurn = turn ? false : MadeMoveInTurn;
                   previousTurn = turn;
               }
              
           });
       
    }
    public void SetPiecePosition(TileGraphic tileGr)
    {
        PieceGraphic.SetPiecePosition(tileGr);
        MadeMoveInTurn = true;
    }
    public Tile tile { get { return PieceGraphic.piece.tile; } }
    public Piece piece
    {
        get
        {
            return PieceGraphic != null
               ? PieceGraphic.piece : null;
        }
    }
    public bool CanMove()
    {
        return turn && !MadeMoveInTurn;

    }
    public PieceGraphic PieceGraphic
    {
        get { return _pieceGraphic; }
        set
        {
            _pieceGraphic = value;
            transform.SetParent(PieceGraphic.transform);
            MadeMoveInTurn = true;
            transform.localPosition = new Vector3(0, transform.localScale.y, 0);
            transform.localScale = new Vector3(1.0f / 2, 1, 1.0f / 2);
            //update tile in clock
            builder.UpdatePathVisual(this);

        }
    }
    
    public void EnterTilePiece(TileGraphic tileGr, PieceGraphic pieceGr)
    {
        if (tileGr != null)
        {
            pieceGr = builder.GetPieceOnTile(tileGr);
        }
        else if (pieceGr != null)
        {
            tileGr = builder.GetTileFromPiece(pieceGr);
        }
        if (tileGr != null)
        {
            var tile = tileGr.tile;
            //check if tile is occupied
            

            if (pieceGr != null)
            {
                //first move piece
                //because if a reachable tile with piece on is click from your colour you switch pieces
                if (pieceGr.piece.Color == this.piece.Color)
                {
                    //change piece of player
                    this.PieceGraphic = pieceGr;
                }
                else
                {
                    //take piece if player is in it
                    if (tileGr.Reachable==this)
                    {
                        //move player piece
                        var otherPLayer = builder.GetPlayerFromPiece(pieceGr);
                        if (otherPLayer != null)
                        {
                            TakePlayer(otherPLayer);
                        }
                        else
                        {
                            TakePiece(pieceGr);
                        }
                       
                    }
                }
            }
            //if not same color and no piece move on tile if reachable
            else
            {
                if (tileGr.Reachable==this)
                {
                    //move player piece
                    builder.SetPieceOnTile(this, tileGr);
                }
            }

        }
    }
    //TODO do something awesome when you're able to push out the player
    void TakePlayer(Player player)
    {
        Debug.Log("Awesome you took the player.");
        var pieceGr = player.PieceGraphic;
        pieceGr.SetColor(side);
        //the player moves to piece taken
        PieceGraphic = pieceGr;
        //assign new piece to hit player
        var newPiece = builder.GetRandomPieceFromSide(player.side);
        if (newPiece == null)
        {
            Debug.Log("player " + player.side + " lost all pieces.");
            return;
        }
        player.PieceGraphic = newPiece;
    }
    //
    public void TakePiece(PieceGraphic pieceGr)
    {

        pieceGr.SetColor(side);
        //the player moves to piece taken
        PieceGraphic = pieceGr;
       
    }
}
