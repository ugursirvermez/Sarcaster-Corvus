using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fire1Spell : BaseAttack
{
    public Fire1Spell()
    {
        attackName = "Fire 1";
        attackDescription = "Basic Fire Spell which burns foes";
        attackDamage = 20f;
        attackCost = 10f;
    }
}
