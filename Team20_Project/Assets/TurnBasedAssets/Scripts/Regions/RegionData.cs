using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegionData : MonoBehaviour
{
    // maks düþman sayýsý, hangi sahnenin açýlacaðý, düþmanlarýn hangi türler olacaðý bu scriptin atandýðý objeden belirleniyor.
    public int maxAmountEnemies = 4;
    public string BattleScene;
    public List<GameObject> possibleEnemies = new List<GameObject>();
}
