using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIskillTooltip : UI_ToolTip
{
    [SerializeField] private TextMeshProUGUI skillText;
    [SerializeField] private TextMeshProUGUI skillName;
    [SerializeField] private TextMeshProUGUI skillCost;
    [SerializeField] private float defaultNameFontSize;

   public void ShowToolTip(string _skillDescription,string _skillName,int _price)
    {
        skillName.text = _skillName;
        skillText.text = _skillDescription;
        skillCost.text = "∑—”√: " + _price;

        AdjustPosition();

        AdjiusFontSize(skillName);

        gameObject.SetActive(true);
    }

    public void HideToolTip()
    {
        skillName.fontSize = defaultNameFontSize;
        gameObject.SetActive(false);
    } 
}
