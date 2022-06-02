using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class LocationSceneManager : MonoBehaviour
{
	//Bunlar etkileşim için gerekli static nesneler
    public abstract void playerdokun(GameObject item);
    public abstract void itemedokun(GameObject item);
}
