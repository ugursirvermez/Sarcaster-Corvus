using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Advertisements;

public class AdKing : MonoBehaviour, IUnityAdsListener
{
Button reklambutonu;
#if UNITY_ANDROID
string kod="4771295";
#else
    string kod="4771294";
#endif

    void Awake(){
    reklambutonu=GetComponent<Button>();    
     reklambutonu.gameObject.SetActive(true);
    }
    
    void Start()
    {
        //Android kodumuz var.
        Advertisement.Initialize(kod);
        Advertisement.AddListener(this);
    }

    //INTERSITIAL REKLAM
   public void ShowAd()
    {
        
        if(Advertisement.IsReady("Interstitial_Android")){
         Advertisement.Show("Interstitial_Android");
         reklambutonu.gameObject.SetActive(false);
        }
    }
    
    //Rewarded REKLAM
       public void RewardAd()
        {
            if(Advertisement.IsReady("rewardedVideo")){
             Advertisement.Show("rewardedVideo");
            }else{
             Debug.Log("Nope! Reklam yok ki burada!");
            }
        }
       
       //BANNER REKLAM
       #region banner
       public void ShowBannerAd()
       {
           if (Advertisement.IsReady("banner"))
           {
               Advertisement.Banner.SetPosition(BannerPosition.BOTTOM_CENTER);
               Advertisement.Show("banner");
           }
           else
           {
               StartCoroutine(tekrarlishowbanner());
           }
       }

       IEnumerator tekrarlishowbanner()
       {
           yield return new WaitForSeconds(1);
           ShowBannerAd();
       }

       public void bannersakla()
       {
           Advertisement.Banner.Hide();
       }

       #endregion
       //IMPLEMENTASYONLAR
       public void OnUnityAdsReady(string PlacementId)
       {
           Debug.Log("Reklam hazır!");
       }
        public void OnUnityAdsDidError(string message)
        {
            Debug.Log("Hata: "+message);
        }
        public void OnUnityAdsDidStart(string PlacementId)
        {
            Debug.Log("Reklam basladi");
        }
        public void OnUnityAdsDidFinish(string PlacementId, ShowResult showResult)
        {
            if (PlacementId=="rewardedVideo"&& showResult==ShowResult.Finished)
            {
                Debug.Log("Reklam kapatıldı!");
                reklambutonu.gameObject.SetActive(false);
            }
        }

}