using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BaseHero : BaseClass
{
    // Baseclassdan �zellikleri al�p ek olarak stat de�erleri olu�turur -- mevcut oyunda etkisi olmayacak

    public int stamina;
    public int intellect;
    public int dexterity;
    public int agility;

    public List<BaseAttack> MagicAttacks = new List<BaseAttack>();
}
