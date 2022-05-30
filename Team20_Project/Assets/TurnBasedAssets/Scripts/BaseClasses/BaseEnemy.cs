using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BaseEnemy : BaseClass
{
    // Baseclassdan �zellikleri al�p ek olarak d��man tipi ve nadirli�ini atayabiliriz -- mevcut oyunda t�r ve nadirlik bir�eyi de�i�tirmeyecek
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
