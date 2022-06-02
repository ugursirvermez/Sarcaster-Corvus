using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStateMachine : MonoBehaviour
{
    private BattleStateMachine BSM;
    public BaseEnemy enemy;

    public enum TurnState
    {
        PROCESSING,
        CHOOSEACTION,
        WAITING,
        ACTION,
        DEAD
    }

    public TurnState currentState;
    // �lerleme �ubu�u
    private float cur_cooldown = 0f;
    private float max_cooldown = 10f;
    // Bu nesneyle alakal�
    private Vector3 startposition;
    public GameObject Selector;
    // timeforaction ile alakal�
    private bool actionStarted = false;
    public GameObject HeroToAttack;
    private float animSpeed = 10f;

    // Hayatta
    private bool alive = true;

    void Start()
    {
        currentState = TurnState.PROCESSING;
        Selector.SetActive(false);
        BSM = GameObject.Find("BattleManager").GetComponent<BattleStateMachine>();
        startposition = transform.position;
    }

    
    void Update()
    {
        switch (currentState)
        {
            case (TurnState.PROCESSING):
                UpgradeProgressBar();
                break;
            case (TurnState.CHOOSEACTION):
                ChooseAction();
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
                    // D��man tag�n� de�i�tir
                    this.gameObject.tag = "DeadEnemy";
                    // Kahramanlar taraf�ndan sald�r�lamaz durumda
                    BSM.EnemiesInBattle.Remove(this.gameObject);
                    // Selekt�r� devre d��� b�rak
                    Selector.SetActive(false);
                    // D��man sald�r�lar�ndan t�m inputlar� ��kar
                    if (BSM.EnemiesInBattle.Count > 0)
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
                                    BSM.PerformList[i].AttackersTarget = BSM.EnemiesInBattle[Random.Range(0, BSM.EnemiesInBattle.Count)];
                                }
                            }  
                        }
                    }
                    
                    // Karakterin rengini de�i�tir veya �lme animasyonu oynat
                    this.gameObject.GetComponent<MeshRenderer>().material.color = new Color32(105, 105, 105, 255);
                    // Hayatta durumunu de�i�tir
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
        
        if (cur_cooldown >= max_cooldown)
        {
            currentState = TurnState.CHOOSEACTION;

        }
    }
    
    void ChooseAction()
    {
        HandleTurn myAttack = new HandleTurn();
        myAttack.Attacker = enemy.theName;
        myAttack.Type = "Enemy";
        myAttack.AttackersGameObject = this.gameObject;
        myAttack.AttackersTarget = BSM.HerosInBattle[Random.Range(0, BSM.HerosInBattle.Count)];

        int num = Random.Range(0, enemy.attacks.Count);
        myAttack.choosenAttack = enemy.attacks[num];
        Debug.Log(this.gameObject.name+" has choosen" + myAttack.choosenAttack.attackName
            + " and do" + myAttack.choosenAttack.attackDamage + " damage!");
        BSM.CollectActions(myAttack);
    }

    private IEnumerator TimeForAction()
    {
        if (actionStarted )
        {
            yield break;
        }
        actionStarted = true;

        // D��man karakteri kahraman karakterin yan�na do�ru ilerlet veya animasyon oynat
        Vector3 heroPosition = new Vector3(HeroToAttack.transform.position.x + 1.5f, HeroToAttack.transform.position.y, HeroToAttack.transform.position.z);
        while (MoveTowardsEnemy(heroPosition)) { yield return null; }

        // Biraz bekle
        yield return new WaitForSeconds(0.5f);
        // Hasar ver
        DoDamage();
        // Ba�lang�� pozisyonuna geri d�n veya animasyon oynatmay� durdur
        Vector3 firstPosition = startposition;
        while (MoveTowardsStart(firstPosition)) { yield return null; }

        // Bu performeri BSM'deki listeden kald�r
        BSM.PerformList.RemoveAt(0);
        // reset BSM -> WAIT
        BSM.battleStates = BattleStateMachine.PerformAction.WAIT;
        // Coroutine'yi durdur
        actionStarted = false;
        // D��man durumunu s�f�rla
        cur_cooldown = 0f;
        currentState = TurnState.PROCESSING;
    }

    private bool MoveTowardsEnemy(Vector3 target)
    {
        return target != (transform.position = Vector3.MoveTowards(transform.position, target, animSpeed * Time.deltaTime));
    }
    private bool MoveTowardsStart(Vector3 target)
    {
        return target != (transform.position = Vector3.MoveTowards(transform.position, target, animSpeed * Time.deltaTime));
    }
    
    void DoDamage()
    {
        float calc_damage = enemy.curATK + BSM.PerformList[0].choosenAttack.attackDamage;
        HeroToAttack.GetComponent<HeroStateMachine>().TakeDamage(calc_damage);
    }

    public void TakeDamage(float getDamageAmount)
    {
        enemy.curHp -= getDamageAmount;
        if (enemy.curHp <= 0)
        {
            enemy.curHp = 0;
            currentState = TurnState.DEAD;
        }
    }
}
