using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Dash_Skill : Skill
{
    [Header("Dash")]
    public bool dashUnlocked;
    [SerializeField] private UI_SkilltreeSlot dashUnlockButton;

    [Header("Clone on dash")]
    public bool cloneOnDashUlocked;
    [SerializeField] private UI_SkilltreeSlot cloneOnDashUnlockButton;

    [Header("Clone on arrival")]
    public bool cloneOnArrivalUlocked;
    [SerializeField] private UI_SkilltreeSlot cloneOnArrivalUnlockButton;

    

    public override void UseSkill()
    {
        base.UseSkill();
    }

    protected override void Start()
    {
        base.Start();

        dashUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockDash);
        cloneOnDashUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockCloneOnDash);
        cloneOnArrivalUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockCloneOnArrival);
    }

    private void UnlockDash()
    {
       

        if (dashUnlockButton.unlocked)
        {
            
            dashUnlocked = true;
        }
    }
    private void UnlockCloneOnDash()
    {
        if(cloneOnDashUnlockButton.unlocked) 
            cloneOnDashUlocked = true;
    }

    private void UnlockCloneOnArrival()
    {
        if(cloneOnArrivalUnlockButton.unlocked)
            cloneOnArrivalUlocked = true;
    }

    public void CloneOnDash()
    {
        if (cloneOnDashUlocked)
           SkillManager.instance.clone.CreateClone(player.transform, Vector3.zero);
    }

    public void CloneOnArrival()
    {
        if (cloneOnArrivalUlocked)
            SkillManager.instance.clone.CreateClone(player.transform, Vector3.zero);
    }

   
}
