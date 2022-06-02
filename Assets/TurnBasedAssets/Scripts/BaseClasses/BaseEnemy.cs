using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BaseEnemy : BaseClass
{
    // Baseclassdan özellikleri alýp ek olarak düþman tipi ve nadirliðini atayabiliriz -- mevcut oyunda tür ve nadirlik birþeyi deðiþtirmeyecek
    public enum Type
    {
        GRASS,
        FIRE,
        WATER,
        ELECTRIC
    }
    public enum Rarity
    {
        COMMON,
        UNCOMMON,
        RARE,
        SUPERRARE
    }

    public Type EnemyType;
    public Rarity rarity;
}
