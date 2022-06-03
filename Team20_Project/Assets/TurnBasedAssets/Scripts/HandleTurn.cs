using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HandleTurn 
{
    public string Attacker; // Saldýranýn ismi
    public string Type;
    public GameObject AttackersGameObject; // Kim saldýrýyor
    public GameObject AttackersTarget; // Kim saldýrýya uðruyor

    // Hangi saldýrý gerçekleþtirildi
    public BaseAttack choosenAttack;
}
