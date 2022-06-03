using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
// Bunu yapmazsak systemdeki random ile UnityEngine'deki random kütüphaneleri karışır. Range fonskiyonunu kullanamayız!
using Random = UnityEngine.Random;

//Location Ransingle'dan türetilen bir sınıf burası, oradaki kodlar temel olarak kullanacağız...
public class LocationItemRandom : LocationRansingle<LocationItemRandom>
{
    //Haritada kaç tane etkileşim nesnesi var? Oyuncumuzun sınıfından etkileşim değişkenleri
    [SerializeField] private Locationitems[] items_inmap;
    private LocationPlayer _player;
    //Spawn'lanma için belirtilen başlangıç parametreleri
    [SerializeField] private float Maxdeger=80.0f;
    [SerializeField] private float Mindeger = 0.5f;
    [SerializeField] private float bekleme = 100f;
    //Bir oturuşta kaç item üreteyim?
    [SerializeField] private int itemstart = 5;
    
    //Kaç tane item aktif ve neler alındı bunları takip etmek gerekiyor.
    private List<Locationitems> ekrandakitemler = new List<Locationitems>();
    private Locationitems itemler;

    public List<Locationitems> Ekrandakitemler
    {
        get => ekrandakitemler;
        set => ekrandakitemler = value;
    }
    public Locationitems Itemler
    {
        get => itemler;
        set => itemler = value;
    }

    private void Awake()
    {
        //Başlarken bu nesneler boş olamaz!!!
        Assert.IsNotNull(items_inmap);
        
    }

    //Instance edeceğimiz ve item etkileşimleri üreteceğimiz noktalar bunlar
    private void itemuretimi()
    {
        //Karakterimizin bulunduğu konumdan belirli noktalara doğru rastgele instantiate yapmaya çalışıyoruz.
        int index = Random.Range(0, items_inmap.Length);
        float x = _player.transform.position.x + mesafeyarat();
        float y = _player.transform.position.y+1.5f; // y yukarı spawn olmasını sağlar, bunu istemiyoruz.
        float z = _player.transform.position.z + mesafeyarat();
        ekrandakitemler.Add(Instantiate(items_inmap[index], new Vector3(x, y, z), Quaternion.identity));
    }

    private float mesafeyarat()
    {
        //Rastgele mesafe yaratmaya çalışıyoruz burada. Eğer değer aralığı 5'ten küçük mü küçükse yani pozitif olsun, ya da negatif...
        float rastgele = Random.Range(Mindeger,Maxdeger);
        bool tamamsa = Random.Range(0, 10) < 5;
        return rastgele *(tamamsa ? 1: -1);
    }

    public void secilitem(Locationitems items)
    {
        itemler = items;
    }
    
    private void Start()
    {
        _player = LocationGameManager.Instance.MevcutPlayer;
        Assert.IsNotNull(_player);
        //Rastgele item üreteceğimiz yer ve sahnede bir oturuşta kaç tane üreteceğimizi gösteriyor.
        for (int i = 0; i < itemstart; i++)
        {
            itemuretimi();
        }

        StartCoroutine(itemyarat());
    }

    private IEnumerator itemyarat()
    {
        while (true)
        {
            itemuretimi();
            yield return new WaitForSeconds(bekleme);
        }
    }
}
