using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
public class UI : MonoBehaviour {
    public Button BuildButton;
    public Button JoinButton;
    public Toggle WhiteToggle;
    public Text text;
	// Use this for initialization
	void Start () {
        BuildButton.OnClickAsObservable().Subscribe(_ =>
        {
            GameObject.Find("DataSyncObject(Clone)").GetComponent<DataSync>().BuildBoard();
            
        });
        JoinButton.OnClickAsObservable().Subscribe(_ => {
            GameObject.Find("DataSyncObject(Clone)").GetComponent<DataSync>().Join(WhiteToggle.isOn);
            
        });
       
        //TODO join button
    }
    
	public void ShowMessage(string textchars)
    {
        text.enabled = true;
        text.text = textchars;
    }
    public void CloseMessage()
    {
        text.enabled = false;
    }
	// Update is called once per frame
	void Update () {
		
	}
}
