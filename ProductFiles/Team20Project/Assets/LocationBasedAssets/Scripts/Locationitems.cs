using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Locationitems : MonoBehaviour
{
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
   }

   private void OnMouseDown()
   {
      LocationSceneManager[] managers = FindObjectsOfType<LocationSceneManager>();
      foreach (LocationSceneManager locationSceneManager in managers)
      {
         if (locationSceneManager.gameObject.activeSelf)
         {
            locationSceneManager.itemedokun(this.gameObject);
         }
      }
   }
}
