using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName="Dialoglar", menuName="DialogData/Dialoglar")]
[System.Serializable]
public class Dialog : ScriptableObject
{
	public List<dialog_metinleri> metinler;
	public Dialog sonraki_dialog;
	
	[System.Serializable]
	public struct dialog_metinleri{
		public string metin;
		public DialogKim konusan;
	}
	
	
	
}
