using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System.Linq;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using TileIJ = System.Collections.Generic.List<UniRx.Tuple<int, int>>;
public class DataSync : Photon.PunBehaviour, IPunObservable
{
    //the only thing that can be changed  by other clients
    //public PieceGraphic playerPiece;
    //public bool boardIsDrawn;
    //public List<Tuple<List<Tuple<int, int>>, int>> boardDivisions;
    //public List<Tuple<List<Tuple<int, int>>, int>> piecesOnBoard;
    string boardDivisionsData = "";
    string piecesOnBoardData = "";
    string playerEnterTilePieceData = "";
    string newPlayer = "";

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        //here you can manage how often to send data

        if (stream.isWriting)
        {


            stream.SendNext(boardDivisionsData);
            stream.SendNext(piecesOnBoardData);
            stream.SendNext(playerEnterTilePieceData);
            stream.SendNext(newPlayer);
            //reset, if not done the object keeps sending
            boardDivisionsData = newPlayer=playerEnterTilePieceData = piecesOnBoardData = "";
            //send combo player piece -> int / list<tuple<int,int>>
        }
        else
        {
            ////receive board 
            var boardDivisionsData = (string)stream.ReceiveNext();
            var piecesOnBoardData = (string)stream.ReceiveNext();
            var playerOnPieceData = (string)stream.ReceiveNext();
            var newPlayerData = (string)stream.ReceiveNext();

            
            if (piecesOnBoardData != "" && boardDivisionsData != "")
            {
                initBoardWithData(boardDivisionsData, piecesOnBoardData);
            }
            if (playerOnPieceData != "")
            {
                ReceivePlayerEnterTilePiece(playerOnPieceData);
            }
            if (newPlayerData != "")
            {
                ReceiveNewPlayer(newPlayerData);
            }
           
            boardDivisionsData =newPlayer= piecesOnBoardData = playerOnPieceData = "";
        }
    }
    public void SendPlayerEnterTilePiece(Player player, TileGraphic tileGr, PieceGraphic pg)
    {
        playerEnterTilePieceData = Serialisation.PlayerEnterTilePieceToString(player, tileGr, pg);

    }
    public void ReceivePlayerEnterTilePiece(string playerTilePiece)
    {
       
        var ptpTuple = Serialisation.StringToPlayerEnterTilePiece(builder, playerTilePiece);
        ptpTuple.Item1.EnterTilePiece(ptpTuple.Item2, ptpTuple.Item3);
    }
    void ReceiveNewPlayer(string newPlayer)
    {
        var pt = newPlayer.Split('-');
        
        var id = Convert.ToInt32(pt[0]);
        var col = StringToColor( pt[1]);
        var side = Convert.ToInt32(pt[2]);
        var PieceName = pt[3];
        builder.AddPlayer(side, id, col, builder.GetPieceGraphicByName(PieceName));
    }
    string ColorToString(Color32 col)
    {
        return col.r+ "|"+col.g+"|"+col.b;
    }
    Color StringToColor(string s)
    {
        return new Color32(Convert.ToByte(s.Split('|')[0]), Convert.ToByte(s.Split('|')[1]), Convert.ToByte(s.Split('|')[2]),1);
    }
    void SendNewPlayer(Player player)
    {
        Color32 col = player.col;
        newPlayer = player.ID+"-" + ColorToString(col) + "-" + player.side + '-' + Serialisation.TileIJsToString(player.tile.IJs) ;
    }

    void initBoardWithData(string boardData, string piecesData)
    {
        var boardDivisions = (List<Tuple<TileIJ, int>>)Serialisation.StringToTileIJIntLIst(boardData);
        var piecesOnBoard = (List<Tuple<TileIJ, int>>)Serialisation.StringToTileIJIntLIst(piecesData);
        GameObject.Find("LevelBuilder").GetComponent<LevelBuilder>().InitBoard(boardDivisions);
        GameObject.Find("LevelBuilder").GetComponent<LevelBuilder>().DrawBoard();
        GameObject.Find("LevelBuilder").GetComponent<LevelBuilder>().DrawInitPieces(piecesOnBoard);
    }

    
    public void Join(bool white)
    {
        int side = white ? 1 : 0;
        var player = builder.AddPlayer(side, PhotonNetwork.player.ID, UnityEngine.Random.ColorHSV());
        SendNewPlayer(player);
    }
    LevelBuilder builder;
    public void BuildBoard()
    {
        //init the board variable
        var boardDivisions = GameObject.Find("LevelBuilder").GetComponent<LevelBuilder>().InitBoard();
        GameObject.Find("LevelBuilder").GetComponent<LevelBuilder>().DrawBoard();
        //photonView.RPC("SendMessageRPC", PhotonTargets.All, "Hello there!");


        ////add pieces
        var piecesOnBoard = builder.GeneratePieces();
        builder.DrawInitPieces(piecesOnBoard);
        //TODO change method to sendBoard
        boardDivisionsData = Serialisation.TileIJsIntListToString(boardDivisions);
        piecesOnBoardData = Serialisation.TileIJsIntListToString(piecesOnBoard);


    }
    
    // Use this for initialization
    void Start()
    {
        builder = GameObject.Find("LevelBuilder").GetComponent<LevelBuilder>();
        //if (PhotonNetwork.isMasterClient)
        //{
        //    GameObject.Find("LevelBuilder").GetComponent<LevelBuilder>().InitBoard();
        //}

        //init pieces 
        // we dont' do anything if we are not the local player.
        if (photonView.isMine)
        {
            GameObject.Find("LevelBuilder").GetComponent<LevelBuilder>().DataSync = this;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
