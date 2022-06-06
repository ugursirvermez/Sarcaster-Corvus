using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WaitCode : MonoBehaviour
{
    [SerializeField]float bekle=5f;
	[SerializeField]int sahne_indexi;
	
    void Start()
    {
        StartCoroutine(wait());	
    }
	
	IEnumerator wait(){
		yield return new WaitForSeconds(bekle);
		SceneManager.LoadSceneAsync(sahne_indexi);
	}
   
}
