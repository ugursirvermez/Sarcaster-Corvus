using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeroStateMachine : MonoBehaviour
{
    private BattleStateMachine BSM;
    public BaseHero hero;

    public enum TurnState
    {
        PROCESSING,
        ADDTOLIST,
        WAITING,
        ACTION,
        DEAD
    }

    public TurnState currentState;
    // �lerleme �ubu�u
    private float cur_cooldown = 0f;
    private float max_cooldown = 5f;
    private Image ProgressBar;
    public GameObject Selector;
    // IeNumerator
    public GameObject EnemyToAttack;
    private bool actionStarted = false;
    private Vector3 startPosition;
    private float animSpeed = 10f;
    // Hayatta
    private bool alive = true;
    // Kahraman paneli
    private HeroPanelStats stats;
    public GameObject HeroPanel;
    private Transform HeroPanelSpacer;

    void Start()
    {
        // Spaceri bul
        HeroPanelSpacer = GameObject.Find("BattleCanvas").transform.Find("HeroPanel").transform.Find("HeroPanelSpacer");
        // Panel olu�tur ve i�ini doldur
        CreateHeroPanel();
        
        startPosition = transform.position;
        cur_cooldown = Random.Range(0, 2.5f);
        Selector.SetActive(false);
        currentState = TurnState.PROCESSING;
        BSM = GameObject.Find("BattleManager").GetComponent<BattleStateMachine>();
    }

    
    void Update()
    {
        switch (currentState)
        {
            case (TurnState.PROCESSING):
                UpgradeProgressBar();
                break;
            case (TurnState.ADDTOLIST):
                BSM.HerosToManage.Add(this.gameObject);
                currentState = TurnState.WAITING;
                break;
            case (TurnState.WAITING):
                // idle
                break;
            case (TurnState.ACTION):
                StartCoroutine(TimeForAction());
                break;
            case (TurnState.DEAD):
                if (!alive)
                {
                    return;
                }
                else
                {
                    // Tag� de�i�tir
                    this.gameObject.tag = "DeadHero";
                    // D��manlar taraf�ndan sald�r�lamaz
                    BSM.HerosInBattle.Remove(this.gameObject);
                    // Oyuncu taraf�ndan kontrol edilemez
                    BSM.HerosToManage.Remove(this.gameObject);
                    // Selekt�r� devre d��� b�rak
                    Selector.SetActive(false);
                    // GUI'� s�f�rla
                    BSM.AttackPanel.SetActive(false);
                    BSM.EnemySelectPanel.SetActive(false);
                    // Nesneyi performlist'ten kald�r
                    if (BSM.HerosInBattle.Count > 0)
                    {
                        for (int i = 0; i < BSM.PerformList.Count; i++)
                        {
                            if (i != 0)
                            {
                                if (BSM.PerformList[i].AttackersGameObject == this.gameObject)
                                {
                                    BSM.PerformList.Remove(BSM.PerformList[i]);
                                }
                                if (BSM.PerformList[i].AttackersTarget == this.gameObject)
                                {
                                    BSM.PerformList[i].AttackersTarget = BSM.HerosInBattle[Random.Range(0, BSM.HerosInBattle.Count)];
                                }
                            }
                        }
                    }
                    
                    // Rengi de�i�tir veya �l�m animasyonu oynat
                    this.gameObject.GetComponent<MeshRenderer>().material.color = new Color32(105, 105, 105, 255);
                    // Kahraman inputlar�n� s�f�rla
                    BSM.battleStates = BattleStateMachine.PerformAction.CHECKALIVE;
                    alive = false;
                    // D��man butonlar�n� s�f�rla
                    BSM.EnemyButtons();
                    // Hayatta m� kontrol et
                    BSM.battleStates = BattleStateMachine.PerformAction.CHECKALIVE;
                }
                break;
        }
    }
    void UpgradeProgressBar()
    {
        cur_cooldown = cur_cooldown + Time.deltaTime;
        float calc_cooldown = cur_cooldown / max_cooldown;
        ProgressBar.transform.localScale = new Vector3(Mathf.Clamp(calc_cooldown, 0, 1), ProgressBar.transform.localScale.y, ProgressBar.transform.localScale.z);
        if (cur_cooldown >= max_cooldown)
        {
            currentState = TurnState.ADDTOLIST;
        }
    }

    private IEnumerator TimeForAction()
    {
        if (actionStarted)
        {
            yield break;
        }
        actionStarted = true;

        // Kahraman� d��man�n yak�n�na do�ru ilerlet veya sald�r� animasyonu oynat
        Vector3 enemyPosition = new Vector3(EnemyToAttack.transform.position.x - 1.5f, EnemyToAttack.transform.position.y, EnemyToAttack.transform.position.z);
        while (MoveTowardsEnemy(enemyPosition)) { yield return null; }

        // Biraz bekle
        yield return new WaitForSeconds(0.5f);
        // Hasar ver
        DoDamage();
        // Ba�lang�� pozisyonuna geri d�n veya animasyonu durdur.
        Vector3 firstPosition = startPosition;
        while (MoveTowardsStart(firstPosition)) { yield return null; }

        // Bu performeri BSM'deki listeden kald�r
        BSM.PerformList.RemoveAt(0);
        // reset BSM -> WAIT
        if (BSM.battleStates != BattleStateMachine.PerformAction.WIN && BSM.battleStates != BattleStateMachine.PerformAction.LOSE)
        {
            BSM.battleStates = BattleStateMachine.PerformAction.WAIT;
            // Kahraman durumunu s�f�rla
            cur_cooldown = 0f;
            currentState = TurnState.PROCESSING;
        }
        else
        {
            currentState = TurnState.PROCESSING;
        }
        // Coroutine'yi durdur
        actionStarted = false;
    }

    private bool MoveTowardsEnemy(Vector3 target)
    {
        return target != (transform.position = Vector3.MoveTowards(transform.position, target, animSpeed * Time.deltaTime));
    }
    private bool MoveTowardsStart(Vector3 target)
    {
        return target != (transform.position = Vector3.MoveTowards(transform.position, target, animSpeed * Time.deltaTime));
    }

    public void TakeDamage(float getDamageAmount)
    {
        hero.curHp -= getDamageAmount;
        if (hero.curHp <= 0)
        {
            hero.curHp = 0;
            currentState = TurnState.DEAD;
        }
        UpdateHeroPanel();
    }
    // Hasar hesaplamas�
    void DoDamage()
    {
        float calc_damage = hero.curATK + BSM.PerformList[0].choosenAttack.attackDamage;
        EnemyToAttack.GetComponent<EnemyStateMachine>().TakeDamage(calc_damage);
    }

    // Kahraman paneli olu�tur
    void CreateHeroPanel()
    {
        HeroPanel = Instantiate(HeroPanel) as GameObject;
        stats = HeroPanel.GetComponent<HeroPanelStats>();
        stats.HeroName.text = hero.theName;
        stats.HeroHP.text = "HP: " + hero.curHp;
        stats.HeroMP.text = "MP: " + hero.curMp;

        ProgressBar = stats.ProgressBar;
        HeroPanel.transform.SetParent(HeroPanelSpacer, false);
    }
    // Paneli hasar alma veya iyile�me durumunda g�ncelle
    void UpdateHeroPanel()
    {
        stats.HeroHP.text = "HP: " + hero.curHp;
        stats.HeroMP.text = "MP: " + hero.curMp;
    }
}
