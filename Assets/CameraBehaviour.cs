using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
public class CameraBehaviour : MonoBehaviour {

    float height;
    public float zoom = 10f;
    //TODO
    public bool followPlayer;
    public bool zoomOnPLayer;
    public void Init(Player localPlayer)
    {
        height = this.transform.position.y;
       
        localPlayer.transform.ObserveEveryValueChanged(x => x.TransformPoint(Vector3.zero)).Subscribe(x=> {
            float zoomFactor = zoom / localPlayer.tile.GetAbsFraction();
            this.transform.position = new Vector3(x.x, height*zoomFactor, x.z);
        });
    }
    private void Update()
    {
        
    }

 

}
