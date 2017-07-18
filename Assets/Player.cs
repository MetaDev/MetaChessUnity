using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UnityEngine.UI;
public class Player : MonoBehaviour
{
    private PieceGraphic _pieceGraphic;
    public LevelBuilder builder;
    public Color col;
    public int side;
    public bool MadeMoveInTurn;
  
    private void Start()
    {
        ProcDraw.SetCubeColor(this.gameObject, col);
        StartCoroutine(SubscribeToClock());
    }
    IEnumerator SubscribeToClock()
    {
        yield return new WaitForSeconds(1);
        builder.clock.TurnChanged.Where(turn=>turn).Subscribe(_ => MadeMoveInTurn = false);
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
        return builder.clock.IsPlayerTurn() && !MadeMoveInTurn;
        
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
}
