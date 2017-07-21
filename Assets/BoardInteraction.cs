using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardInteraction : MonoBehaviour
{
    public LevelBuilder builder;
    void Start()
    {

    }

    private void Update()
    {

        if (builder.LocalPlayer == null)
        {
            return;
        }
        if (Input.GetMouseButtonDown(0))
        {
           
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            // Casts the ray and get the first game object hit
            Physics.Raycast(ray, out hit);
            
            if (hit.collider != null)
            {
                var tileGr = hit.collider.GetComponent<TileGraphic>();
                var pieceGr = hit.collider.GetComponent<PieceGraphic>();
                builder.LocalPlayer.EnterTilePiece(tileGr, pieceGr);
                //because a network command sends both tile and piece, if the piece from the method differs from the piece found in the
                //tile than this means the piece changed position between the action of the player, sending message and the receiving of the command
    
                builder.DataSync.SendPlayerEnterTilePiece(builder.LocalPlayer, tileGr, pieceGr);
            }
        }
    }
}