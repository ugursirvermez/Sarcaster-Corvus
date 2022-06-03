using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class LocationPlayer : MonoBehaviour
{
    //LocationUIManager ile bağlantılı kodlar var.
    // Level atlamamızı sağlayacak xp değerlerimiz.
    [SerializeField] private int xpnow = 0;
    [SerializeField] private int xpnext = 10;
    //Level durumumuz
    private int level = 1;
    //level seviyesi en fazla
    [SerializeField] private int levelmerkez = 100;
    //Haritadaki etkileşimleri çıkaracağımız liste
    [SerializeField] private List<GameObject> itemlocation = new List<GameObject>();
    
    //Verilerin kaydedileceği yol
    private string yol;

    //Burada veriler dışarıya aktarılabilir, önemli noktalar var. O yüzden get set yaptık. Region'un içinde

    #region getset

    public int Xpnext
    {
        get => xpnext;
        set => xpnext = value;
    }

    public int Xpnow
    {
        get => xpnow;
        set => xpnow = value;
    }

    public int Level
    {
        get => level;
        set => level = value;
    }

    public int Levelmerkez
    {
        get => levelmerkez;
        set => levelmerkez = value;
    }

    public List<GameObject> Itemlocation
    {
        get => itemlocation;
        set => itemlocation = value;
    }
    
    #endregion
    
    //XP'nin artacağı metot
    public  void xpartsın(int xp)
    {
        xpnow += xp;
		kaydet();
    }
    
    //Eklenen Itemler rastgele olmadan once buraya atanmalı
    public void itemekle(GameObject item)
    {
        itemlocation.Add(item);
        levelatla();
        //buraya da kaydet atarız ama şuan itemler belli değil...
    }

    //Level atlama fonksiyonu
    private void levelatla()
    {
        level = (xpnow / levelmerkez) + 1;
        xpnext = levelmerkez * level;
        kaydet();
    }

    private void kaydet()
    {
        try
        {
            BinaryFormatter format = new BinaryFormatter();
            FileStream file = File.Create(yol);
            LocationPlayerData dat = new LocationPlayerData(this);
            format.Serialize(file, dat);
            file.Close();
        }
        catch
        {
            Debug.Log("veri kaydediliyor");
        }
    }

    private void veridoldur()
    {
        try
        {
            if (File.Exists(yol))
            {
                BinaryFormatter format = new BinaryFormatter();
                FileStream file = File.Open(yol, FileMode.Open);
                LocationPlayerData dat = (LocationPlayerData) format.Deserialize(file);
                file.Close();
                xpnow = dat.Xpnow;
                xpnext = dat.Xpnext;
                level = dat.Level;
                levelmerkez = dat.Sonrakilevel;
                //ITEMLERİ LOOTA EKLEMEK İÇİN BAŞKA BİR ŞEY YAPACAĞIZ
            }
            else
            {
                levelatla();
            }
        }
        catch
        {
            Debug.Log("veri dolduruldu");
        }
    }
    void Start()
    {
        yol = Application.persistentDataPath + "/player.dat";
        veridoldur();
    }
	
	void Update(){
		levelatla();
	}
	
	void OnApplicationPause(bool isPause){
     if(isPause){
	 kaydet();
	 veridoldur();
	 }
	}
	void OnApplicationQuit(){
		kaydet();
		veridoldur();
	}
}