using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LocationSceneTrans : LocationRansingle<LocationSceneTrans>
{
    private AsyncOperation sceneasync;

    public void sahneyegit(string sahneismi, List<GameObject> nesneler)
    {
        StartCoroutine(LoadScene(sahneismi,nesneler));
    }

    private IEnumerator LoadScene(string sahneismi,List<GameObject> nesneler)
    {
        SceneManager.LoadSceneAsync(sahneismi);
        SceneManager.sceneLoaded += (yenisahne, mode) => { SceneManager.SetActiveScene(yenisahne);};
        Scene dolscene = SceneManager.GetSceneByName(sahneismi);
        foreach (GameObject nesne in nesneler)
        {
            SceneManager.MoveGameObjectToScene(nesne, dolscene);
        }

        yield return null;
    }
}
