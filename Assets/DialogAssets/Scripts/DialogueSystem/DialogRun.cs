using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class DialogRun : MonoBehaviour
{
	private int metinindex=-1;
	private int word_index=0;
	public Dialog getir;
	public Image avatar;
	public TextMeshProUGUI name;
	public TextMeshProUGUI words;
	public DialogSTATE state=DialogSTATE.ended;
	
	void Awake(){
		metinindex=-1;
		DontDestroyOnLoad(gameObject);
	}
	
	public enum DialogSTATE{
		playing, 
		ended
	}
	
	public void metinkontrol(){
		
		if(metinindex+1== getir.metinler.Count){
			GameManager.instance.dialogishere=false;
			getir=getir.sonraki_dialog;
			metinindex=-1;
		}
		else{
			degistir();
		}
	}
	
	private void Start(){
		degistir();
	}
	
	public void degistir(){
		metinindex++;
		//name.color=getir.metinler[metinindex].konusan.ismin_color;
		StartCoroutine(metintext(getir.metinler[metinindex].metin));
		name.text=getir.metinler[metinindex].konusan.ismi;
		avatar.sprite=getir.metinler[metinindex].konusan.avatar;
	}
	
	private IEnumerator metintext(string yazi){
		words.text="";
		word_index=0;
		state=DialogSTATE.playing;
		while(state != DialogSTATE.ended){
			words.text += yazi[word_index];
			yield return new WaitForSeconds(0.01f);
			if(++word_index==yazi.Length){
				state=DialogSTATE.ended;
				break;
			}
		}
	}
	
}