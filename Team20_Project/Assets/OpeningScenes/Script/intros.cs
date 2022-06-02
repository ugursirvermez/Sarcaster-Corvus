using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class intros : MonoBehaviour
{
    [SerializeField] GameObject loading_panel;
    [SerializeField] Slider load_slider;
    [SerializeField] TextMeshProUGUI yuzdelik;
   //Start videonun 1 olması lazım!
   public void basla_ekrani()
   {
     StartCoroutine(LocationSenkron(1));
   }

   //Start Video gelince 3 olacak.
   public void locationa_gecis()
   {
     // await Task.Delay(1000);
      StartCoroutine(LocationSenkron(2));
   }
   
   //SAHNE DOLUMU ICIN YAPTIGIMIZ BIR YUKLEME EKRANI
   //LOCATION BASED KASMAMALI EN TEMEL SORUNLARIMIZDAN BIRISI YADA OYLE GORUNMEMELI
   IEnumerator LocationSenkron(int index){
   AsyncOperation durum = SceneManager.LoadSceneAsync(index);
   durum.allowSceneActivation = false;
   loading_panel.SetActive(true);
   while(!durum.isDone){
   float dolumsuresi= Mathf.Clamp01(durum.progress/0.9f);
   load_slider.value=dolumsuresi;
   yuzdelik.text="%"+ dolumsuresi * 100;
   durum.allowSceneActivation = true;
   yield return null;
   } 
   }
}
