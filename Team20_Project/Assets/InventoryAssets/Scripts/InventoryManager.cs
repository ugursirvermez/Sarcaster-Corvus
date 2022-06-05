using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;
    public List<Item> Items = new List<Item>();
	public Item itemim;
    public Transform ItemContent;
    public GameObject InventoryItem;

    public Toggle EnableRemove;

    public InventoryItemController[] InventoryItems;

    private void Awake()
    {
		DontDestroyOnLoad(gameObject);
        Instance = this;
    }

    public void Add(Item item)
    {
        Items.Add(item);
    }

    public void Remove(Item item)
    {
        Items.Remove(item);

    }

    public void ListItems()
    {
        //Ýçeriði açmadan önce temizleyin.
        foreach (Transform item in ItemContent)
        {
            Destroy(item.gameObject);
        }
            foreach (var item in Items)
        {
            GameObject obj = Instantiate(InventoryItem, ItemContent);
            var itemName = obj.transform.Find("ItemName").GetComponent<Text>();
            var itemIcon = obj.transform.Find("ItemIcon").GetComponent<Image>();
            var removeButton = obj.transform.Find("RemoveButton").GetComponent<Button>();

            itemName.text = item.itemName;
            itemIcon.sprite = item.icon;

            if (EnableRemove.isOn)
                removeButton.gameObject.SetActive(true);

        }

        SetInventoryItems();
    }

    public void EnableItemsRemove()
    {
        if (EnableRemove.isOn)
        {
            foreach(Transform item in ItemContent)
            {
                item.Find("RemoveButton").gameObject.SetActive(true);
            }
        }
        else
        {
            foreach (Transform item in ItemContent)
            {
                item.Find("RemoveButton").gameObject.SetActive(false);
            }
        }
    }

    public void SetInventoryItems()
    {
        InventoryItems = ItemContent.GetComponentsInChildren<InventoryItemController>();

        for(int i = 0; i < Items.Count; i++)
        {
            InventoryItems[i].AddItem(Items[i]);
        }
    }
}
