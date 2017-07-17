using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelBuilder : MonoBehaviour {
    public GameObject square;
    public Board board;
    // Use this for initialization
    void Start () {
        
        board = new Board();
        board.RandomFractionBoard(20);
        ProcDraw.DrawBoardRecursive(square, board, board.RootTile);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
