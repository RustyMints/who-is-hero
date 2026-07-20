using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Crystal_Skill : Skill
{
    [SerializeField] private float crystalDurationl;
    [SerializeField] private GameObject crystalPrefab;
    private GameObject currentCrystal;

    [Header("Crystal mirage")]
    [SerializeField] private UI_SkilltreeSlot unlockCloneInstaedButton;
    [SerializeField] private bool cloneInsteadOfCrystal;

    [Header("Crystal simple")]
    [SerializeField] private UI_SkilltreeSlot unlockCrystalButton;
    public bool crystalUnlocked {  get; private set; }

    [Header("Explosive crystal")]
    [SerializeField] private UI_SkilltreeSlot unlockExplosiveButton;
    [SerializeField] private bool canExplode;

    [Header("Moving crystal")]
    [SerializeField] private UI_SkilltreeSlot unlockMovingCrystalButton;
    [SerializeField] private bool canMoveToEnemy;
    [SerializeField] private float moveSpeed;


    [Header("Mult stacking crystal")]
    [SerializeField] private UI_SkilltreeSlot unlockMultiStackButton;
    [SerializeField] private bool canUserMultiStacks;
    [SerializeField] private int amountOfStacks;
    [SerializeField] private float multiStackCooldown;
    [SerializeField] private float useTimeWondow;
    [SerializeField] private List<GameObject> crystalLeft = new List<GameObject>();

    protected override void Start()
    {
        base.Start();

        unlockCrystalButton.GetComponent<Button>().onClick.AddListener(UnlockCrystal);
        unlockCloneInstaedButton.GetComponent<Button>().onClick.AddListener(UnlockCrystalMirage);
        unlockExplosiveButton.GetComponent<Button>().onClick.AddListener(UnlockExplosiveCrystal);
        unlockMovingCrystalButton.GetComponent<Button>().onClick.AddListener(unlockMoveingCrystal);
        unlockMultiStackButton.GetComponent<Button>().onClick.AddListener(unlockMultiStack);

    }

    //解锁技能区域
    #region Unlock skill region

    public override void CheckUnlock()
    {
        UnlockCrystal();
        UnlockCrystalMirage();
        UnlockExplosiveCrystal();
        unlockMoveingCrystal();
        unlockMultiStack();
    }
    private void UnlockCrystal()
    {
        if (unlockCrystalButton.unlocked)
            crystalUnlocked = true;
    }

    private void UnlockCrystalMirage()
    {
        if(unlockCloneInstaedButton.unlocked)
            cloneInsteadOfCrystal = true;
    }

    private void UnlockExplosiveCrystal()
    {
        if (unlockExplosiveButton.unlocked)
            canExplode = true;
    }

    private void unlockMoveingCrystal()
    {
        if(unlockMovingCrystalButton.unlocked)
            canMoveToEnemy = true;
    }

    private void unlockMultiStack()
    {
        if(unlockMultiStackButton.unlocked)
            canUserMultiStacks = true;
    }
    #endregion

    public override void UseSkill()
    {
        base.UseSkill();

        if(CanUseMultiCrystal())
            return;

        if (currentCrystal == null)
        {
            CreateCrystal();
        }
        else
        {
            if (canMoveToEnemy)
                return;

            Vector2 playerPos = player.transform.position;
            player.transform.position = currentCrystal.transform.position;
            currentCrystal.transform.position = playerPos;

            if (cloneInsteadOfCrystal)
            {
                SkillManager.instance.clone.CreateClone(currentCrystal.transform, Vector3.zero);
                Destroy(currentCrystal);
            }

            else
            {

                currentCrystal.GetComponent<Crystal_Skill_Controller>()?.FinishCrystal();
            }

        }
    }

    public void CreateCrystal()
    {
        currentCrystal = Instantiate(crystalPrefab, player.transform.position, Quaternion.identity);
        Crystal_Skill_Controller currentCystalScrit = currentCrystal.GetComponent<Crystal_Skill_Controller>();

        currentCystalScrit.SetupCrystal(crystalDurationl, canExplode, canMoveToEnemy, moveSpeed, FindClosestEnemy(currentCrystal.transform),player);
        
    }

    public void CurrentCrystalChooseRandomTarget() => currentCrystal.GetComponent<Crystal_Skill_Controller>().ChooseRandomEnemy();

    private bool CanUseMultiCrystal()
    {
        if (canUserMultiStacks)
        {
            if(crystalLeft.Count > 0)
            {
                if (crystalLeft.Count == amountOfStacks)
                    Invoke("ResetAbility", useTimeWondow);

                cooldown = 0;
                GameObject crystalTospawn = crystalLeft[crystalLeft.Count - 1];
                GameObject newCrystal = Instantiate(crystalTospawn,player.transform.position,Quaternion.identity); 

                crystalLeft.Remove(crystalTospawn);

                newCrystal.GetComponent<Crystal_Skill_Controller>().
                    SetupCrystal(crystalDurationl, canExplode, canMoveToEnemy, moveSpeed, FindClosestEnemy(newCrystal.transform), player);

                if(crystalLeft.Count <= 0)
                {
                    cooldown = multiStackCooldown;
                    RefilCrystal();
                }

            return true;

            }
        }

        return false;
    }

    private void RefilCrystal()
    {
        int amountToAdd = amountOfStacks - crystalLeft.Count;

        for (int i = 0; i < amountToAdd; i++)
        {
            crystalLeft.Add(crystalPrefab);
        }
    }

    private void ResetAbility()
    {
        if (cooldowmTimer > 0)
            return;

        cooldowmTimer = multiStackCooldown;
        RefilCrystal();
    }
    
}
