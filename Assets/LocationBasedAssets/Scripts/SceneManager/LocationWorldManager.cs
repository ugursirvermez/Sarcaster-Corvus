using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LocationWorldManager : LocationSceneManager
{
    private GameObject item;

    private AsyncOperation loadScene;

    public override void playerdokun(GameObject item)
    {
        Screen.orientation = ScreenOrientation.Portrait;
        //Locationmap'e geridönüş kodu bu...
        LocationSceneTrans.Instance.sahneyegit(LocationItemSabitler.locationmap,new List<GameObject>());
    }

    public override void itemedokun(GameObject item)
    {
        Screen.orientation = ScreenOrientation.Landscape;
        List<GameObject> liste = new List<GameObject>();
        liste.Add(item);
        LocationSceneTrans.Instance.sahneyegit(LocationItemSabitler._2dscene,liste);
    }
    
}
