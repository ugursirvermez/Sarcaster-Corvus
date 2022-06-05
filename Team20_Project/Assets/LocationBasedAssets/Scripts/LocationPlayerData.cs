using System;
using System.Collections;
using System.Collections.Generic;
using Mapbox.VectorTile;
using UnityEngine;

//BU SINIFIN AYNISI ITEMLERIN OZELLİKLERİ TANIMLANDIĞINDA TEKRARDAN LocationItemData olarak tanımlanacak.
[Serializable]
public class LocationPlayerData
{
   private int xpnow;
   private int xpnext;
   private int level;
   private int sonrakilevel;
   private List<GameObject> itemler; //MVP'de bunu yapmak zaman alacak...

   #region vericek
   
   public int Xpnow
   {
      get { return xpnow; }
   }
   public int Xpnext
   {
      get { return xpnext; }
   }
   public int Level
   {
      get { return level; }
   }
   public int Sonrakilevel
   {
      get { return sonrakilevel; }
   }
   public List<GameObject> Itemler
   {
      get { return itemler; }
   }
   #endregion

   public LocationPlayerData(LocationPlayer player)
   {
      xpnow = player.Xpnow;
      xpnext = player.Xpnext;
      level = player.Level;
      sonrakilevel = player.Levelmerkez;
     // itemler = player.Itemlocation;
   }
}
