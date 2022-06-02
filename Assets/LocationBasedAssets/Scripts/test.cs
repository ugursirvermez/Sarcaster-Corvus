using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    
    void Awake()
    {
        StartCoroutine(bekle());
        Screen.orientation = ScreenOrientation.Portrait;
                LocationSceneTrans.Instance.sahneyegit(LocationItemSabitler.locationmap,new List<GameObject>());
        
    }

    IEnumerator bekle()
    {
        
        yield return new WaitForSeconds(10);
    }
}
