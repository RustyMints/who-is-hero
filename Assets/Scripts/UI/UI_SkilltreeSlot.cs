using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_SkilltreeSlot : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler,IsaveManager
{
    private UI ui;
    private Image skillImage;

    [SerializeField] private int skillCost; 
    [SerializeField] private string skillName;
    [TextArea]
    [SerializeField] private string skillDescription;
    [SerializeField] private Color lockedSkillColor;

    public bool unlocked;

    [SerializeField] private UI_SkilltreeSlot[] shouldBeUnlocked;
    [SerializeField] private UI_SkilltreeSlot[] shouldBeLocked;


    private void OnValidate()
    {
        gameObject.name = "SkillTreeSlot_UI - " + skillName;
    }


    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(() => unlockSkillSlot());
        
    }

    private void Start()
    {
        skillImage = GetComponent<Image>();
        ui = GetComponentInParent<UI>();

        skillImage.color = lockedSkillColor;

        if(unlocked)
            skillImage.color = Color.white;
    }

    public void unlockSkillSlot()
    {
        if (unlocked)
            return;

        for (int i = 0; i < shouldBeUnlocked.Length; i++)
        {
            if (shouldBeUnlocked[i].unlocked == false)
            {
                Debug.Log("无法解锁：前置技能未解锁");
                return;
            }
        }

        for (int i = 0; i < shouldBeLocked.Length; i++)
        {
            if (shouldBeLocked[i].unlocked == true)
            {
                Debug.Log("无法解锁：前置技能未解锁");
                return;
            }
        }

        if (PlayerManager.instance.HaveEnoughMoney(skillCost) == false)
            return;

        unlocked = true;
        skillImage.color = Color.white;
        if (SkillManager.instance != null)
            SkillManager.instance.RefreshAllSkills();
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        ui.skillTooltip.ShowToolTip(skillDescription,skillName,skillCost);

        
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ui.skillTooltip.HideToolTip();
    }

    public void LoadData(GameData _data)
    {
        if(_data.skillTree.TryGetValue(skillName,out bool value))
        {
            unlocked = value;
            if (unlocked && skillImage != null)
                skillImage.color = Color.white;
        }
    }

    public void SaveData(ref GameData _data)
    {
        if(_data.skillTree.TryGetValue(skillName,out bool value))
        {
            _data.skillTree.Remove(skillName);
            _data.skillTree.Add(skillName, unlocked);
        }

        else
            _data.skillTree.Add(skillName,unlocked);
    }
}
