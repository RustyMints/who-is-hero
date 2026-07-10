using TMPro;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIitemSlot : MonoBehaviour , IPointerDownHandler,IPointerEnterHandler,IPointerExitHandler
{
    [SerializeField] private Image itemImage;
    [SerializeField] private TextMeshProUGUI itemText;

    private UI ui;
    public InventoryItem item;


    private void Start()
    {
        ui = GetComponentInParent<UI>();
    }

    private void Awake()
    {
        if (itemImage == null)
            itemImage = GetComponent<Image>();

        if (itemText == null)
            itemText = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void UpdateSlot(InventoryItem _newitem)
    {
        if (itemImage == null || itemText == null)
            return;

        item = _newitem;

        itemImage.color = Color.white;

        if (item != null && item.data != null)
        {
            itemImage.sprite = item.data.icon;

            if (item.stackSize > 1)
            {
                itemText.text = item.stackSize.ToString();
            }
            else
            {
                itemText.text = "";
            }
        }
        else
        {
            itemImage.sprite = null;
            itemText.text = "";
        }
    }

    public void CleanUpSlot()
    {
        item = null;

        itemImage.sprite = null;
        itemImage.color = Color.clear;
        itemText.text = "";

    }

    public virtual void OnPointerDown(PointerEventData eventData)
    {
        if(item == null)
            return;

        if (Input.GetKey(KeyCode.LeftControl))
        {
            Inventory.instance.RemoveItem(item.data);
            return;
        }

        if (item != null && item.data != null && item.data.itemType == ItemType.Equipment)
            Inventory.GetInstance().EquipItem(item.data);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(item == null)
            return;

        ui.itemToolTip.ShowToolTip(item.data as ItemData_Equipment);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (item == null)
            return;

        ui.itemToolTip.HideToolTip();
    }
}
