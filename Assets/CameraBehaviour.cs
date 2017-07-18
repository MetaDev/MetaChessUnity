using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
public class CameraBehaviour : MonoBehaviour {
    public LevelBuilder builder;
    float height;
    public float zoom = 10f;
    //TODO
    public bool followPlayer;
    public bool zoomOnPLayer;
    private void Start()
    {
        height = this.transform.position.y;
       
        builder.player.transform.ObserveEveryValueChanged(x => x.TransformPoint(Vector3.zero)).Subscribe(x=> {
            float zoomFactor = zoom / builder.player.tile.GetAbsFraction();
            this.transform.position = new Vector3(x.x, height*zoomFactor, x.z);
        });
    }
    private void Update()
    {
        
    }

 

}
