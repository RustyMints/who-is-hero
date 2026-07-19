using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_CraftWindow : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI itemName;
    [SerializeField] private TextMeshProUGUI itemDescription;
    [SerializeField] private Image itemIcon;
    [SerializeField] private Button craftButton;


    [SerializeField] private Image[] materialImage;

    public void SetupCraftWindow(ItemData_Equipment _data)
    {
        if (_data == null)
            return;

        craftButton.onClick.RemoveAllListeners();

        for (int i = 0; i < materialImage.Length; i++)
        {
            materialImage[i].color = Color.clear;
            materialImage[i].GetComponentInChildren<TextMeshProUGUI>().color = Color.clear;
        }

        if (_data.craftingMaterials != null)
        {
            int count = Mathf.Min(_data.craftingMaterials.Count, materialImage.Length);
            for (int i = 0; i < count; i++)
            {
                InventoryItem matItem = _data.craftingMaterials[i];
                if (matItem != null && matItem.data != null && matItem.data.itemicon != null)
                {
                    materialImage[i].sprite = matItem.data.itemicon;
                    materialImage[i].color = Color.white;

                    TextMeshProUGUI materialSlotText = materialImage[i].GetComponentInChildren<TextMeshProUGUI>();
                    if (materialSlotText != null)
                    {
                        materialSlotText.text = matItem.stackSize.ToString();
                        materialSlotText.color = Color.white;
                    }
                }
            }
        }

        if (_data.itemicon != null)
            itemIcon.sprite = _data.itemicon;
        itemName.text = _data.itemName;
        itemDescription.text = _data.GetDescription();

        craftButton.onClick.AddListener(() => Inventory.instance.CanCraft(_data, _data.craftingMaterials));
    }
}
