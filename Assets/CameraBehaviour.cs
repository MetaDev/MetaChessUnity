using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
public class CameraBehaviour : MonoBehaviour {

    public float height;
    public float zoom = 10f;
    //TODO, add zoom in zoom out with scroll weel
   
    public bool zoomOnPLayer;
    public void Init(Player localPlayer)
    {
        
       
        var subsc = localPlayer.transform.ObserveEveryValueChanged(x => x.TransformPoint(Vector3.zero)).Subscribe(x=> {
            float zoomFactor = zoom / localPlayer.tile.GetAbsFraction();
            this.transform.position = new Vector3(x.x, height*zoomFactor, x.z);
        });
        subsc.AddTo(localPlayer);
    }
    private void Update()
    {
       // Input.mouseScrollDelta()
    }

 

}
