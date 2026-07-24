using System.Collections;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public enum StatType
{
    strength,      //力量：增加物理伤害与暴击伤害
    agility,       //敏捷：增加暴击率与闪避率
    intelegence,   //智力：增加魔法伤害与魔法抗性（拼写：intelligence）
    vitality,      //活力：每点增加5点最大生命值
    damage,        //基础物理伤害
    critChance,    //暴击几率（百分比）
    critPower,     //暴击伤害倍率（百分比，默认150即1.5倍）
    health,        //最大生命值（对应字段maxHealth）
    armor,         //护甲：减免物理伤害
    evasion,       //闪避：概率完全闪避攻击
    magicRes,      //魔法抗性：减免魔法伤害（对应字段magicResistance）
    fireDamage,    //火焰元素伤害，额外附带点燃DoT
    iceDamage,     //冰霜元素伤害，附带减速20%
    lightingDamage //雷电元素伤害，附带触电并连锁闪电（拼写：lightning）
}

public class CharacterStarts : MonoBehaviour
{
    private EntityFX fx;

    [Header("Major stats")]
    public Stat strength;       //力量：物理伤害加成 + 暴击伤害加成
    public Stat agility;        //敏捷：暴击率加成 + 闪避率加成
    public Stat intelligence;   //智力：魔法伤害加成 + 魔法抗性加成（每点+3抗性）
    public Stat vitality;       //活力：每点+5最大生命值

    [Header("Offensive stats")]
    public Stat damage;         //基础物理伤害
    public Stat critChance;     //暴击概率（百分比，100%必暴）
    public Stat critPower;      //暴击伤害倍率（默认150即造成1.5倍伤害）
    

    [Header("Defencive stats")]
    public Stat maxHealth;      //最大生命值
    public Stat armor;          //护甲：直接扣减物理伤害（冰冻状态下仅生效80%）
    public Stat evasion;        //闪避：概率完全免疫一次攻击（百分比）
    public Stat magicResistance;//魔法抗性：直接扣减魔法伤害

    [Header("Magic stats")]
    public Stat fireDamage;     //火焰伤害：叠加点燃持续伤害DoT
    public Stat iceDamage;      //冰霜伤害：附加减速20%（持续ailmentsDuration秒）
    public Stat lightingDamage; //雷电伤害：附加触电+连锁最近敌人的雷击（拼写：lightning）

    public bool isIgnited; //是否处于【点燃】状态：持续每0.3秒造成火焰DoT伤害
    public bool isChilld;  //是否处于【冰冻/寒冷】状态：移速降低20%，护甲减伤仅80%
    public bool isShocked; //是否处于【触电】状态：闪避+20（自身命中下降），下次被雷电攻击触发连锁
    public bool isBleeding; //是否处于【流血】状态：短时间内快速多次流失少量生命值

    [SerializeField] private float ailmentsDuration = 4; //元素异常状态总持续时间（秒）
    private float ignitedTimer;    //点燃状态剩余时间
    private float chilledTimer;    //寒冷状态剩余时间
    private float shockedTimer;    //触电状态剩余时间
    private float bleedingTimer;   //流血状态剩余时间


    private float igniteDamageCoolDown = 0.3f; //点燃DoT的伤害触发间隔（秒）
    private float igniteDamageTimer;           //点燃DoT下一次触发的倒计时
    private int igniteDamage;                   //当前点燃每次触发的伤害值
    private float bleedDamageCoolDown = 0.15f;  //流血DoT的伤害触发间隔（秒，比点燃更快）
    private float bleedDamageTimer;             //流血DoT下一次触发的倒计时
    private int bleedDamage;                    //当前流血每次触发的伤害值
    [SerializeField] private GameObject shockStrikePrefab; //雷电连锁时生成的雷击预制体
    private int shockDamgae;                    //连锁雷击造成的伤害（拼写：shockDamage）
    public int currentHealth;                   //当前生命值

    public System.Action onHealthChanged;       //生命值变化回调事件（UI血量条订阅）
    public bool isDead { get; private set; }    //是否已死亡
    public bool isInvicible { get; private set; }
    private bool isVulnerable;

    protected virtual void Start()
    {
        critPower.SetDefaultValue(150);
        currentHealth = GetMaxHealthValue();

        fx = GetComponent<EntityFX>();

        

    }

    protected virtual void Update()
    {
        ignitedTimer -= Time.deltaTime;
        chilledTimer -= Time.deltaTime;
        shockedTimer -= Time.deltaTime;
        bleedingTimer -= Time.deltaTime;

        igniteDamageTimer -= Time.deltaTime;
        bleedDamageTimer -= Time.deltaTime;



        if (ignitedTimer < 0)
            isIgnited = false;

        if (chilledTimer < 0)
            isChilld = false;

        if (shockedTimer < 0)
            isShocked = false;

        if (bleedingTimer < 0)
            isBleeding = false;

        if(isIgnited)
            ApplyIgniteDamage();

        if(isBleeding)
            ApplyBleedDamage();
    }

    public void MakeVulnerableFor(float _duration) => StartCoroutine(vulnerableCorutine(_duration));
    

    private IEnumerator vulnerableCorutine(float _duration)
    {
        isVulnerable = true;

        yield return new WaitForSeconds(_duration);

        isVulnerable = false;
    }

    public virtual void IncreaseStaBy(int _modifier, float _duration,Stat _statToModify)
    {
        StartCoroutine(StatModCoroutine(_modifier, _duration, _statToModify));
    }

    private IEnumerator StatModCoroutine(int _modifier, float _duration, Stat _statToModify)
    {
        _statToModify.AddModifier(_modifier);

        yield return new WaitForSeconds(_duration);

        _statToModify.RemoveModifier(_modifier);
    }
    

    public virtual void DoDamage(CharacterStarts _targetStats)
    {
        bool criticalStrike = false;

        if (TargetCanAvoidAttack(_targetStats))
            return;

        _targetStats.GetComponent<Entity>().SetupKnockbackDir(transform);

        int totalDamage = damage.GetValue() + strength.GetValue();

        if (CanCrit())
        {
            totalDamage = CalculateCriticalDamage(totalDamage);
            criticalStrike = true;
        }

        fx.CreateHitFX(_targetStats.transform,criticalStrike);

        totalDamage = CheckTargetArmor(_targetStats, totalDamage);
        _targetStats.TakeDamage(totalDamage);

        if (HasMagicalDamage())
            DomagicalDamage(_targetStats);
    }

    private bool HasMagicalDamage()
    {
        return fireDamage.GetValue() > 0 || iceDamage.GetValue() > 0 ||
               lightingDamage.GetValue() > 0 || intelligence.GetValue() > 0;
    }


    #region Magical damage and ailemnts
    public virtual void DomagicalDamage(CharacterStarts _targetStats)
    {
        int _fireDamage = fireDamage.GetValue();
        int _iceDamage = iceDamage.GetValue();
        int _lightingDamage = lightingDamage.GetValue();

        int totalMagicalDamage = _fireDamage + _iceDamage + _lightingDamage + intelligence.GetValue();

        totalMagicalDamage = CheckTargetResistance(_targetStats, totalMagicalDamage);
        _targetStats.TakeDamage(totalMagicalDamage);

        if (Mathf.Max(_fireDamage, _iceDamage, _lightingDamage) <= 0)
            return;

        AttemptyToApplyAilments(_targetStats, _fireDamage, _iceDamage, _lightingDamage);
    }

    private void AttemptyToApplyAilments(CharacterStarts _targetStats, int _fireDamage, int _iceDamage, int _lightingDamage)
    {
        bool canApplyIgnite = _fireDamage > _iceDamage && _fireDamage > _lightingDamage;
        bool canApplyChill = _iceDamage > _fireDamage && _iceDamage > _lightingDamage;
        bool canApplyShock = _lightingDamage > _fireDamage && _lightingDamage > _iceDamage;

        while (!canApplyIgnite && !canApplyChill && !canApplyShock)
        {
            if (Random.value < 0.3f && _fireDamage > 0)
            {
                canApplyIgnite = true;
                _targetStats.ApplyAilments(canApplyIgnite, canApplyChill, canApplyShock);
                
                return;
            }

            if (Random.value < 0.5f && _iceDamage > 0)
            {
                canApplyChill = true;
                _targetStats.ApplyAilments(canApplyIgnite, canApplyChill, canApplyShock);
                
                return;
            }
            if (Random.value < 0.5f && _lightingDamage > 0)
            {
                canApplyShock = true;
                _targetStats.ApplyAilments(canApplyIgnite, canApplyChill, canApplyShock);
                
                return;
            }
        }

        if (canApplyIgnite)
            _targetStats.SetupIgniteDamage(Mathf.RoundToInt(_fireDamage * 0.2f));

        if (canApplyShock)
            _targetStats.SetupShockStrikeDamage(Mathf.RoundToInt(_lightingDamage * 0.1f));

        _targetStats.ApplyAilments(canApplyIgnite, canApplyChill, canApplyShock);
    }

  

    public void ApplyAilments(bool _ignite,bool _chill,bool _shock)
    {
        bool canApplyIgnite = !isIgnited && !isChilld && !isShocked;
        bool canApplyChill = !isIgnited && !isChilld && !isShocked;
        bool canApplyShock = !isIgnited && !isChilld;

        if (_ignite && canApplyIgnite)
        {
            isIgnited = _ignite;
            ignitedTimer = ailmentsDuration;

            fx.IgniteFxFor(ailmentsDuration);
        }
        if (_chill && canApplyChill)
        {
            chilledTimer = ailmentsDuration;
            isChilld = _chill;

            float slowPercentage = 0.2f;

            GetComponent<Entity>().SlowEntityBy(slowPercentage, ailmentsDuration);
            fx.ChillFxFor(ailmentsDuration);
        }

        if (_shock && canApplyShock)
        {
            if (!isShocked)
            {
                ApplyShock(_shock);

            }

            else
            {
                if (GetComponent<Player>() != null)
                    return;
                HitNearestTargetWithShockStrike();

            }

        }
        
    }

    public void ApplyShock(bool _shock)
    {
        if (isShocked)
            return;

        shockedTimer = ailmentsDuration;
        isShocked = _shock;

        fx.ShockFxFor(ailmentsDuration);
    }

    private void HitNearestTargetWithShockStrike()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 25);

        float closesDistance = Mathf.Infinity;
        Transform closesEnemy = null;

        foreach (var hit in colliders)
        {
            if (hit.GetComponent<Enemy>() != null && Vector2.Distance(transform.position, hit.transform.position) > 1)
            {
                float distanceToEnemy = Vector2.Distance(transform.position, hit.transform.position);

                if (distanceToEnemy < closesDistance)
                {
                    closesDistance = distanceToEnemy;
                    closesEnemy = hit.transform;
                }
            }

            if (closesEnemy == null)  //这个功能是用雷电属性的攻击打中敌人后，雷电会寻找最近的敌人进行攻击
                closesEnemy = transform;  //如果你不希望这个效果被实现，可以删除他
        }

        if (closesEnemy != null)
        {
            GameObject newShockStrike = Instantiate(shockStrikePrefab, transform.position, Quaternion.identity);

            newShockStrike.GetComponent<ShockStrike_Controller>().Setup(shockDamgae, closesEnemy.GetComponent<CharacterStarts>());
        }
    }

    private void ApplyIgniteDamage()
    {
        if (igniteDamageTimer < 0)
        {
            DecreaseHealthBy(igniteDamage);

            if (currentHealth < 0 && !isDead)
                Die();

            igniteDamageTimer = igniteDamageCoolDown;
        }
    }

    private void ApplyBleedDamage()
    {
        if (bleedDamageTimer < 0)
        {
            DecreaseHealthBy(bleedDamage);

            if (currentHealth < 0 && !isDead)
                Die();

            bleedDamageTimer = bleedDamageCoolDown;
        }
    }

    public void SetupIgniteDamage(int _damage) => igniteDamage = _damage;
    public void SetupShockStrikeDamage(int _damage) => shockDamgae = _damage;
    public void SetupBleedDamage(int _damage) => bleedDamage = _damage;

    public void ApplyBleed(float _duration)
    {
        isBleeding = true;
        bleedingTimer = _duration;

        fx.BleedFxFor(_duration);
    }

    #endregion

    public virtual void TakeDamage(int _damage)
    {
        if (isInvicible)
            return;

        DecreaseHealthBy(_damage);


        GetComponent<Entity>().DamageImpact();
        fx.StartCoroutine("FlashFX");

        if (currentHealth < 0 && !isDead)
            Die();


    }

    public virtual void IncreaseHealthBy(int _amount)
    {
        currentHealth += _amount;

        if(currentHealth > GetMaxHealthValue())
            currentHealth = GetMaxHealthValue();

        if (onHealthChanged != null)
            onHealthChanged();
    }

    protected virtual void DecreaseHealthBy(int _damage)
    {
        if (isVulnerable)
            _damage = Mathf.RoundToInt(_damage * 1.1f); 

        currentHealth -= _damage;

        if(onHealthChanged != null)
            onHealthChanged();
    }

    protected virtual void Die()
    {
        isDead = true;
    }

    public void KillEntity()
    {
        if (!isDead)
            Die();
    }

    public void MakeInvencible(bool _invincible) => isInvicible = _invincible;

    #region Stat calculations
    protected int CheckTargetArmor(CharacterStarts _targetStats, int totalDamage)
    {
        if(_targetStats.isChilld)
            totalDamage -= Mathf.RoundToInt(_targetStats.armor.GetValue() * 0.8f);
        else
            totalDamage -= _targetStats.armor.GetValue();

        totalDamage = Mathf.Clamp(totalDamage, 0, int.MaxValue);
        return totalDamage;
    }

    private int CheckTargetResistance(CharacterStarts _targetStats, int totalMagicalDamage)
    {
        totalMagicalDamage -= _targetStats.magicResistance.GetValue() + (_targetStats.intelligence.GetValue() * 3);
        totalMagicalDamage = Mathf.Clamp(totalMagicalDamage, 0, int.MaxValue);
        return totalMagicalDamage;
    }

    public virtual void OnEvasion()
    {

    }

    protected bool TargetCanAvoidAttack(CharacterStarts _targetStats)
    {
        int totalEvasion = _targetStats.evasion.GetValue() + _targetStats.agility.GetValue();

        if (isShocked)
            totalEvasion += 20;

        totalEvasion = Mathf.Clamp(totalEvasion, 0, 80);

        if (Random.Range(0, 100) < totalEvasion)
        {
            _targetStats.OnEvasion();
            return true;

        }
        return false;
    }

    protected bool CanCrit()
    {
        int totalCriticalChance = critChance.GetValue() + agility.GetValue();

        if(Random.Range(0, 100) <= totalCriticalChance)
        {
            return true;
        }

        return false;
    }

    protected int CalculateCriticalDamage(int _damage)
    {
        float totalCritPower = (critPower.GetValue() + strength.GetValue()) * 0.01f;
        
        float criDamage = _damage * totalCritPower;
        
        return Mathf.RoundToInt(criDamage);
    }

    public int GetMaxHealthValue()
    {
        return maxHealth.GetValue() + vitality.GetValue() * 5;
    }

    #endregion

    public Stat GetStat(StatType _statType)
    {
        if (_statType == StatType.strength) return strength;
        else if (_statType == StatType.agility) return agility;
        else if (_statType == StatType.intelegence) return intelligence;
        else if (_statType == StatType.vitality) return vitality;
        else if (_statType == StatType.damage) return damage;
        else if (_statType == StatType.critChance) return critChance;
        else if (_statType == StatType.critPower) return critPower;
        else if (_statType == StatType.health) return maxHealth;
        else if (_statType == StatType.armor) return armor;
        else if (_statType == StatType.evasion) return evasion;
        else if (_statType == StatType.magicRes) return magicResistance;
        else if (_statType == StatType.fireDamage) return fireDamage;
        else if (_statType == StatType.iceDamage) return iceDamage;
        else if (_statType == StatType.lightingDamage) return lightingDamage;

        return null;
    }
}
