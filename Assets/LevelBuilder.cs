using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelBuilder : MonoBehaviour {
    public GameObject square;
    // Use this for initialization
    void Start () {
        
        var board = new Board();
        board.RandomFractionBoard();
        ProcDraw.DrawBoardRecursive(square, board, board.RootTile);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
