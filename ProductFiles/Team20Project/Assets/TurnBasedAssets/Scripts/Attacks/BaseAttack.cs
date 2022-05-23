using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BaseAttack : MonoBehaviour
{
    public string attackName;
    public string attackDescription;
    public float attackDamage; // base damage burada hasarý istediðimiz gibi belirleyeceðiz örn -> base 15 lvl 10 dexterity 25 hasar = 15 + 10 + 25 = 50 
    public float attackCost; // mana cost
}
