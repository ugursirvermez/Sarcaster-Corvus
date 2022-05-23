using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.AdverGame;

public class MoneyScript : MonoBehaviour
{
    InterstitialAdExample reklam;
	
	void Awake(){
		reklam= new InterstitialAdExample();
	}
	
    void Start()
    {
	 reklam.InitServices();	
       reklam.SetupAd();
    }
	
	public void reklamgoster(){
	 reklam.ShowAd();
}
   
}
