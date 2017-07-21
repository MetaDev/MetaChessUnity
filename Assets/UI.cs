using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
public class UI : MonoBehaviour {
    public Button BuildButton;
    public Button JoinButton;
    public Toggle WhiteToggle;
	// Use this for initialization
	void Start () {
        BuildButton.OnClickAsObservable().Subscribe(_=> {
            GameObject.Find("DataSyncObject(Clone)").GetComponent<DataSync>().BuildBoard();
            BuildButton.enabled = false;
        });
        JoinButton.OnClickAsObservable().Subscribe(_ => {
            GameObject.Find("DataSyncObject(Clone)").GetComponent<DataSync>().Join(WhiteToggle.isOn);
            JoinButton.enabled = false;
            WhiteToggle.enabled = false;
        });
        //TODO join button
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
