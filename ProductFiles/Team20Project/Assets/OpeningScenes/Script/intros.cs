using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class intros : MonoBehaviour
{
   
   //Start videonun 1 olması lazım!
   public void basla_ekrani()
   {
      SceneManager.LoadScene(1);
   }

   //Start Video gelince 3 olacak.
   public void locationa_gecis()
   {
      SceneManager.LoadScene(2);
   }
}
