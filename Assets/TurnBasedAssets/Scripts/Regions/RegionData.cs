using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegionData : MonoBehaviour
{
    // maks d��man say�s�, hangi sahnenin a��laca��, d��manlar�n hangi t�rler olaca�� bu scriptin atand��� objeden belirleniyor.
    public int maxAmountEnemies = 4;
    public string BattleScene;
    public List<GameObject> possibleEnemies = new List<GameObject>();
}
