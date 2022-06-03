using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocationSaveManager : MonoBehaviour
{
	public static LocationSaveManager instance{set; get;}
    //public LocationPlayerData data;
	public LocationPlayer player;
	
	private void Awake(){
		DontDestroyOnLoad(gameObject);
		instance=this;
		load_et();
		Debug.Log(FileSaver.Serialize<LocationPlayer>(player));
	}
	
	public void save_et(){
		PlayerPrefs.SetString("save",FileSaver.Serialize<LocationPlayer>(player));
	}
	
	public void load_et(){
		if(PlayerPrefs.HasKey("save")){//data
			player=FileSaver.Deserialize<LocationPlayer>(PlayerPrefs.GetString("save"));
		}
		else{//data
			player=new LocationPlayer();
			save_et();
		}
	}
}
