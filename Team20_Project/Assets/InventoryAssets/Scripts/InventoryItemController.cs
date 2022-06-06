using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItemController : MonoBehaviour
{
    public Item item;

    public Button RemoveButton;

    public void RemoveItem()
    {
        InventoryManager.Instance.Remove(item);

        Destroy(gameObject);
    }
    public void AddItem(Item newItem)
    {
        item = newItem;
    }

    public void UseItem()
    {
        switch (item.itemType)
        {
            case Item.ItemType.Useless:
                //Eğer itemleri oyun içinde kullanırsak çalışacak kodlar MVP kapsamında yok.
				Debug.Log("Bu itemin bana bir faydasi yok!");
                break;
            case Item.ItemType.Usefull:
               //Eğer itemleri oyun içinde kullanırsak çalışacak kodlar MVP kapsamında yok.
			   Debug.Log("Bu item işe yararmış!");
                break;
        }
        RemoveItem();
    }
}