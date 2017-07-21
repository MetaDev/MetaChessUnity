using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TileIJ = System.Collections.Generic.List<UniRx.Tuple<int, int>>;
using UniRx;
using System.Linq;
public class Serialisation {
    //both the tile and the piece can be null
    public static string PlayerEnterTilePieceToString(Player player, TileGraphic tileGr, PieceGraphic pg)
    {
        var pgName = pg == null ? "" : pg.Name;
        var tileIJs = tileGr == null ? "" : TileIJsToString(tileGr.tile.IJs);
        return player.ID + "-" + tileIJs + "-" + pgName;
    }
    
   
    public static Tuple<Player,TileGraphic,PieceGraphic> StringToPlayerEnterTilePiece(LevelBuilder builder, string playerTilePiece)
    {
        string[] ptp = playerTilePiece.Split('-');

        var player = builder.players[Convert.ToInt32(ptp[0])];
        var tile = ptp[1]==""? null: builder.board.GetTileByIJs(StringToTileIJs(ptp[1])).TileGraphic;
        var piece = ptp[2] == ""? null: builder.GetPieceGraphicByName(ptp[2]);

        return Tuple.Create(player, tile, piece);
    }
    public static List<Tuple<TileIJ, int>> StringToTileIJIntLIst(string list)
    {
        return list.Split('T').Select(tIJA =>
        Tuple.Create(StringToTileIJs(tIJA.Split('-')[0]),
        Convert.ToInt32(tIJA.Split('-')[1])))
        .ToList();
    }
    public static  TileIJ StringToTileIJs(string IJs)
    {
        return IJs.Split(';').Select(tupstr =>
      Tuple.Create(Convert.ToInt32(tupstr.Split(',')[0]),
      Convert.ToInt32(tupstr.Split(',')[1])))
        .ToList();
    }
    public static string TileIJsToString(TileIJ tIJ)
    {
        return String.Join(";", tIJ.Select(IJ => IJ.Item1 + "," + IJ.Item2).ToArray());
    }

    public static string TileIJsIntListToString(List<Tuple<TileIJ, int>> list)
    {
        return String.Join("T", list.Select(tIJA => TileIJsToString(tIJA.Item1) + "-" + tIJA.Item2).ToArray());
    }
}
