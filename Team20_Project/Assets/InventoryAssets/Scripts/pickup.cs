using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pickup : MonoBehaviour
{
    public Item Item;

   public void Pickup()
    {
        InventoryManager.Instance.Add(Item);
    }
}
