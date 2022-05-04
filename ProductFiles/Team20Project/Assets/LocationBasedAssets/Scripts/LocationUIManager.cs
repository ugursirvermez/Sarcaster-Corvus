using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Kodlarda değişmesini istemediğimiz unsurları oluşturmaya yarayan bir kütüphane
using UnityEngine.Assertions;
using UnityEngine.UI;
using TMPro;

public class LocationUIManager : MonoBehaviour
{
    // 3 temel öğe var. Birincisi menü panelinin menü butonuna basılınca aktif olması
    //Diğer ikisi loot kastıkça level atlayan sistemin UI'da yazımı
   [SerializeField] private TextMeshProUGUI itemxp;
   [SerializeField] private TextMeshProUGUI leveltext;
   [SerializeField] private GameObject _Menu;
   [SerializeField] private AudioClip menusoundbuton;
   private AudioSource kaynak;
   void Awake()
   { 
       kaynak = GetComponent<AudioSource>();
       Assert.IsNotNull(kaynak); 
   //ASLA menu,item ve itemxp'si boş geçmesin.
   Assert.IsNotNull(_Menu);
   Assert.IsNotNull(itemxp);
   Assert.IsNotNull(leveltext);
   Assert.IsNotNull(menusoundbuton);
   }

   //Level xp arttıkça atla.
   public void levelatla()
   {
       leveltext.text = LocationGameManager.Instance.MevcutPlayer.Level.ToString();
   }

   //loot yapıldıkça xp arttır.
   public void itemxpkas()
   {
       itemxp.text = LocationGameManager.Instance.MevcutPlayer.Xpnow.ToString() + "/" +
                     LocationGameManager.Instance.MevcutPlayer.Xpnext.ToString();
   }
   
   //Menu aktifliği ve pasifliği buradan
   private void menugoster()
   {
       _Menu.SetActive(!_Menu.activeSelf);
   }

   public void Menubtnclick()
   {
       kaynak.PlayOneShot(menusoundbuton);
       menugoster();
   }

   private void Update()
   {
       levelatla();
       itemxpkas();
   }
   
   public void exit()
   {
       Application.Quit();
   }

   public void loot()
   {
      
   }
}
