using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HandleTurn 
{
    public string Attacker; // name of attacker
    public string Type;
    public GameObject AttackersGameObject; // who attacks
    public GameObject AttackersTarget; // who is going to be attacked

    // which attack performed
    public BaseAttack choosenAttack;
}
