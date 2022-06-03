using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
	public DialogRun dialogrun;

    public RegionData curRegion;

    // Spawn points
    public string nextSpawnPoint;

    // Kahraman
    public GameObject heroCharacter;
	
	//Dialog paneli
	public GameObject DialogPanel;

    // Pozisyonlar
    public Vector3 nextHeroPosition;
    public Vector3 lastHeroPosition; // Sava�

    // Sahneler
    public string sceneToLoad;
    public string lastScene; // Sava�

    // Bools
    public bool isWalking = false;
    public bool canGetEncounter = false;
    public bool gotAttacked = false;
	public bool dialogishere=false;

    // Enum
    public enum GameStates
    {
        WORLD_STATE,
		DIALOG_STATE,
        TOWN_STATE,
        BATTLE_STATE,
        IDLE
    }

    // Battle
    public int enemyAmount;
    public List<GameObject> enemyToBattle = new List<GameObject>();

    public GameStates gameState;

    private void Awake()
    {
        // Instance var m� kontrol et
        if (instance == null)
        {
            // Yoksa instance'yi buna ata
            instance = this;
        }
        // Varsa ama bu instance de�ilse
        else if (instance != this)
        {
            // Yok et
            Destroy(gameObject);
        }
        // Bunu yok edilemez olarak ayarla
        DontDestroyOnLoad(gameObject);
        if (!GameObject.Find("HeroChar"))
        {
            GameObject Hero = Instantiate(heroCharacter, nextHeroPosition, Quaternion.identity) as GameObject;
            Hero.name = "HeroChar";
        }
    }

    private void Update()
    {
        switch (gameState)
        {
            case (GameStates.WORLD_STATE):
                if (isWalking)
                {
                    RandomEncounter();
                }
			
                if (gotAttacked)
                {
				
                    gameState = GameStates.DIALOG_STATE;

                }
                break;
            case (GameStates.TOWN_STATE):
                break;
				
			case (GameStates.DIALOG_STATE):
			if(dialogishere==true){
			DialogPanel.SetActive(true);
			}
			if(dialogishere==false){
				DialogPanel.SetActive(false);
				gameState = GameStates.BATTLE_STATE;
			}
			
			break;
			
            case (GameStates.BATTLE_STATE):
                // Sava� sahnesini y�kle

               StartBattle();
                // �dle'ye git
                gameState = GameStates.IDLE;
                break;
            case (GameStates.IDLE):
                break;
        }
    }

    public void LoadNextScene()
    {
		 Screen.orientation = ScreenOrientation.Landscape;
         SceneManager.LoadSceneAsync(sceneToLoad);
    }

    public void LoadSceneAfterBattle()
    {
		 Screen.orientation = ScreenOrientation.Portrait;
         SceneManager.LoadSceneAsync(lastScene);
    }
	
    void RandomEncounter()
    {
        if (isWalking && canGetEncounter)
        {
            if (Random.Range(0,10000)<10)
            {
                gotAttacked = true;
            }
        }
    }

   public void StartBattle()
    {
        // D��manlar�n say�s�
        enemyAmount = Random.Range(1, curRegion.maxAmountEnemies + 1);
        // Hangi d��manlar
        for (int i = 0; i < enemyAmount; i++)
        {
            enemyToBattle.Add(curRegion.possibleEnemies[Random.Range(0, curRegion.possibleEnemies.Count)]);
        }
        // Kahraman
        lastHeroPosition = GameObject.Find("HeroChar").gameObject.transform.position;
        nextHeroPosition = lastHeroPosition;
        lastScene = SceneManager.GetActiveScene().name;
		
		
         Screen.orientation = ScreenOrientation.Landscape;
        SceneManager.LoadSceneAsync(curRegion.BattleScene);
        // Kahraman� s�f�rla
        isWalking = false;
        gotAttacked = false;
        canGetEncounter = false;
		dialogishere=false;
    }
}
