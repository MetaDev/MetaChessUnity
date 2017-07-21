using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class ClockScript : MonoBehaviour
{


    // Use this for initialization
    public void Init(Player player)
    {
       
       var subs= Clock.GetTurnUnitObservable()
           .Subscribe(_ =>
           {
               var turn = Clock.GetTileTurnFromTurnUnit(_, player.side, player.tile);
               this.GetComponent<Toggle>().isOn = turn;
           });
        subs.AddTo(player);
    }
    
    // Update is called once per frame
   
}
