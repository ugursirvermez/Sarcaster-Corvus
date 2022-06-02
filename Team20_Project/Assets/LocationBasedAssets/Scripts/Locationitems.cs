using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Locationitems : MonoBehaviour
{
	//GameManager gamemanage;
   [SerializeField] private float spawnsiklik = 0.10f;
   [SerializeField] private float yakalanmasiklik = 0.10f;
   //Yine verilere dışarıdan ulaşım konusunda okuma görevi verdiğimiz kodlar region içinde
   #region getset

   public float Spawnsiklik
   {
      get => spawnsiklik;
      set => spawnsiklik = value;
   }

   public float Yakalanmasiklik
   {
      get => yakalanmasiklik;
      set => yakalanmasiklik = value;
   }

   #endregion

   private void Start()
   {
     // DontDestroyOnLoad(this);
	 //gamemanage=new GameManager();
   }

   private void OnMouseDown()
   {
     
      LocationSceneManager[] managers = FindObjectsOfType<LocationSceneManager>();
      foreach (LocationSceneManager locationSceneManager in managers)
      {
         if (locationSceneManager.gameObject.activeSelf)
         {
		    GameManager.instance.isWalking=true;
			GameManager.instance.gotAttacked=true;
			GameManager.instance.canGetEncounter=true;
			GameManager.instance.dialogishere=true;
			//GameManager.instance.curRegion = region;
           //locationSceneManager.itemedokun(this.gameObject);
			//gamemanage.StartBattle();
         }
      }
   }
}
