using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using UnityEngine.Android;

public class menu : MonoBehaviour
{
    // OYUNA BAŞLA BUTONU KODU INTROS'UN İÇİNDEDİR OPENİNG SCENES BAĞLAMINDA BÜTÜN KODLAR ORADA
    
    public AudioMixer base_audio;
    private int menuscene=3;
    private int options=6;
    
    #region menuden ayarlara gecis
    //MENU'DE BULUNAN AYARLAR BUTONUNUN İŞLEMİ
  public void menu_ayarlarbuton(){
    SceneManager.LoadSceneAsync(options);
    }
    
   //AYARLARDAN MENUYE DON 
   public void geri_butonu(){
  SceneManager.LoadScene(menuscene);
   }
   
   //NASIL OYNANIR RESMİ ÇIKACAK
   public void howtoplay(){
   Debug.Log("Nasıl?");
   }
   
   //ÇIKIŞ BUTONU
    public void quit(){
    Application.Quit();
    }
    #endregion
    
    // AYARLAR MENÜSÜNÜN ÖZELLİKLERİNDEN BAZILARINI BURAYA EKLEDİK
    #region ayarlar penceresi
    
    //SES DÜZEYİNİ AYARLIYORUZ
    public void sesduzeyi(float duzey){
    base_audio.SetFloat("duzey",duzey);
    }
    
    public void grafikkalitesi(int index){
    QualitySettings.SetQualityLevel(index);
    }
    
    #endregion

    //BURADA İZİNLERİ YÖNETECEĞİZ. OYUNDA LOCATİONBASEDE GELMEDEN YAPMALIYIZ YOKSA MAP CACHE ETMİYOR
    void Awake()
    {
        if(!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
        {
            Permission.RequestUserPermission(Permission.FineLocation);
            Permission.RequestUserPermission(Permission.FineLocation);
        }
    }
}
