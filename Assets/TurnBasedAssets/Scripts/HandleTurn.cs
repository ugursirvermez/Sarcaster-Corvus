using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HandleTurn 
{
    public string Attacker; // Sald�ran�n ismi
    public string Type;
    public GameObject AttackersGameObject; // Kim sald�r�yor
    public GameObject AttackersTarget; // Kim sald�r�ya u�ruyor

    // Hangi sald�r� ger�ekle�tirildi
    public BaseAttack choosenAttack;
}
