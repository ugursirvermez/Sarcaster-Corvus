using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EnemyStateMachine : MonoBehaviour
{
    private BattleStateMachine BSM;
    public BaseEnemy enemy;
    public Animator anim;

    public TMP_Text enemyHP;

    public enum TurnState
    {
        PROCESSING,
        CHOOSEACTION,
        WAITING,
        ACTION,
        DEAD
    }

    public TurnState currentState;
    // Ýlerleme çubuðu
    private float cur_cooldown = 0f;
    public float max_cooldown = 10f;
    // Bu nesneyle alakalý
    private Vector3 startposition;
    public GameObject Selector;
    // timeforaction ile alakalý
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
        enemyHP.text = "HP: " + enemy.curHp;
    }

    
    void Update()
    {
        switch (currentState)
        {
            case (TurnState.PROCESSING):
                UpgradeProgressBar();
                anim.SetBool("Walk", false);
                anim.SetBool("Attacking", false);
                anim.SetBool("takingHit", false);
                break;
            case (TurnState.CHOOSEACTION):
                ChooseAction();
                currentState = TurnState.WAITING;
                break;
            case (TurnState.WAITING):
                // idle 
                anim.SetBool("Walk", false);
                anim.SetBool("Attacking", false);
                anim.SetBool("takingHit", false);
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
                    // Düþman tagýný deðiþtir
                    this.gameObject.tag = "DeadEnemy";
                    // Kahramanlar tarafýndan saldýrýlamaz durumda
                    BSM.EnemiesInBattle.Remove(this.gameObject);
                    // Selektörü devre dýþý býrak
                    Selector.SetActive(false);
                    // Düþman saldýrýlarýndan tüm inputlarý çýkar
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
                    enemyHP.text = " ";
                    // Karakterin rengini deðiþtir veya Ölme animasyonu oynat
                    this.gameObject.GetComponent<MeshRenderer>().material.color = new Color32(105, 105, 105, 255);
                    anim.SetBool("dead", true);
                    // Hayatta durumunu deðiþtir
                    alive = false;
                    // Düþman butonlarýný sýfýrla
                    BSM.EnemyButtons();
                    // Hayatta mý kontrol et
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

        // Düþman karakteri kahraman karakterin yanýna doðru ilerlet veya animasyon oynat
        Vector3 heroPosition = new Vector3(HeroToAttack.transform.position.x + 1.5f, HeroToAttack.transform.position.y, HeroToAttack.transform.position.z);
        while (MoveTowardsEnemy(heroPosition))
        {
            anim.SetBool("Walk", true);
            yield return null; 
        }
        anim.SetBool("Walk", false);
        // Biraz bekle
        anim.SetBool("Attacking", true);
        yield return new WaitForSeconds(1f);
        // Hasar ver
        DoDamage();
        // Baþlangýç pozisyonuna geri dön veya animasyon oynatmayý durdur
        Vector3 firstPosition = startposition;
        while (MoveTowardsStart(firstPosition))
        {
            anim.SetBool("Walk", true);
            yield return null;
        }
        anim.SetBool("Walk", false);
        // Bu performeri BSM'deki listeden kaldýr
        BSM.PerformList.RemoveAt(0);
        // reset BSM -> WAIT
        BSM.battleStates = BattleStateMachine.PerformAction.WAIT;
        // Coroutine'yi durdur
        actionStarted = false;
        // Düþman durumunu sýfýrla
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
        anim.SetBool("takingHit", true);
        if (enemy.curHp <= 0)
        {
            enemy.curHp = 0;
            currentState = TurnState.DEAD;
        }
        UpdateEnemyHp();
    }

    public void UpdateEnemyHp()
    {
        enemyHP.text = "HP: " + enemy.curHp;
    }
}
