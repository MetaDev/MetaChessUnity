using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class PieceGraphic : MonoBehaviour {
    
    public Piece piece;

    //the first position of the pieces is unique
    public string Name { get; private set; }
   public void SetColor(int Color)
    {
        piece.Color = Color;
        ProcDraw.SetCubeColor(this.gameObject, new Color(piece.Color, piece.Color, piece.Color));
    }
    public Tile Tile { get { return piece.tile; } }
    public LevelBuilder builder;
	// Use this for initialization
	void Start () {
       
    }
	public void init(LevelBuilder builder, int color, TileGraphic newTilePos)
    {
        piece = new Piece( color);
        Name = Serialisation.TileIJsToString(newTilePos.tile.IJs);
        ProcDraw.SetCubeColor(this.gameObject, new Color(piece.Color, piece.Color, piece.Color));
        SetPiecePosition(newTilePos);
    }
    public void SetPiecePosition(TileGraphic newTilePos)
    {
        if (newTilePos == null)
        {
            Debug.Log("Tile is null");
            return;
        }
        var tile = newTilePos.tile;
        piece.tile = tile;
        transform.localScale= new Vector3(tile.GetAbsSize()/4, tile.GetAbsSize() / 4, tile.GetAbsSize() / 4);
        this.transform.position = new Vector3(tile.GetDrawCenterX(), tile.GetAbsSize() *(3.0f/4), tile.GetDrawCenterY());
    }
  
    // Update is called once per frame
    void Update () {
		//update position

	}
}
