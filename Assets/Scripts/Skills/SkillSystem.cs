using System.Collections;
using UnityEngine;

[RequireComponent(typeof(PlayerStats))]
[RequireComponent(typeof(PlayerCombat))]
public class SkillSystem : MonoBehaviour
{
    [Header("Skill Slots (F1-F8)")]
    public SkillData[] skills = new SkillData[8];

    private float[]          cooldownTimers = new float[8];
    private bool[]           onCooldown     = new bool[8];
    private PlayerStats      stats;
    private PlayerCombat     combat;
    private SkillBarUI       skillBarUI;

    private static readonly KeyCode[] SkillKeys =
    {
        KeyCode.F1, KeyCode.F2, KeyCode.F3, KeyCode.F4,
        KeyCode.F5, KeyCode.F6, KeyCode.F7, KeyCode.F8
    };

    void Awake()
    {
        stats  = GetComponent<PlayerStats>();
        combat = GetComponent<PlayerCombat>();
    }

    void Start()
    {
        skillBarUI = FindFirstObjectByType<SkillBarUI>();
        SyncIconsToBar();
    }

    void Update()
    {
        TickCooldowns();

        for (int i = 0; i < 8; i++)
        {
            if (Input.GetKeyDown(SkillKeys[i]))
                TryActivate(i);
        }
    }

    void TickCooldowns()
    {
        for (int i = 0; i < 8; i++)
        {
            if (!onCooldown[i]) continue;
            cooldownTimers[i] -= Time.deltaTime;
            if (cooldownTimers[i] <= 0f)
            {
                cooldownTimers[i] = 0f;
                onCooldown[i]     = false;
            }
        }
    }

    void TryActivate(int idx)
    {
        var skill = skills[idx];
        if (skill == null)  { Debug.Log($"[Skills] F{idx+1} vacío"); return; }
        if (onCooldown[idx]) { Debug.Log($"[Skills] {skill.skillName} en cooldown ({cooldownTimers[idx]:F1}s)"); return; }
        if (stats.currentMana < skill.manaCost)
        {
            Debug.Log($"[Skills] Sin mana para {skill.skillName}");
            FloatingDamageSpawner.Spawn(transform.position + Vector3.up * 2f, 0f, DamageType.Miss);
            return;
        }

        stats.UseMana(skill.manaCost);
        StartCooldown(idx, skill.cooldown);
        ExecuteSkill(skill);
    }

    void ExecuteSkill(SkillData skill)
    {
        switch (skill.skillType)
        {
            case SkillType.Melee:      ExecuteMelee(skill);      break;
            case SkillType.Projectile: ExecuteProjectile(skill);  break;
            case SkillType.AreaOfEffect: ExecuteAoE(skill);      break;
            case SkillType.Buff:       StartCoroutine(ExecuteBuff(skill)); break;
            case SkillType.Heal:       ExecuteHeal(skill);        break;
        }
    }

    void ExecuteMelee(SkillData skill)
    {
        float damage = CalcDamage(skill);
        int mask = LayerMask.GetMask("Enemy");
        var hits = Physics.OverlapSphere(transform.position, skill.range, mask);
        foreach (var col in hits)
        {
            var enemy = col.GetComponentInParent<EnemyController>();
            if (enemy != null && !enemy.IsDead)
                enemy.TakeDamage(damage);
        }
        Debug.Log($"[Skills] {skill.skillName}: golpe melee {damage:F0} dmg a {hits.Length} enemigos");
    }

    void ExecuteProjectile(SkillData skill)
    {
        if (skill.projectilePrefab == null)
        {
            Debug.LogWarning($"[Skills] {skill.skillName}: falta projectilePrefab");
            return;
        }

        float damage = CalcDamage(skill);
        var origin = transform.position + Vector3.up * 1.2f;
        var dir = GetAimDirection();

        var go = Instantiate(skill.projectilePrefab, origin, Quaternion.LookRotation(dir));
        var proj = go.GetComponent<Projectile>();
        proj?.Initialize(damage, skill.projectileSpeed, LayerMask.GetMask("Enemy"), skill.isMagicDamage);
    }

    void ExecuteAoE(SkillData skill)
    {
        float damage = CalcDamage(skill);
        var center = combat.LockedTarget != null
            ? combat.LockedTarget.transform.position
            : transform.position;

        int mask = LayerMask.GetMask("Enemy");
        var hits = Physics.OverlapSphere(center, skill.aoeRadius, mask);
        foreach (var col in hits)
        {
            var enemy = col.GetComponentInParent<EnemyController>();
            if (enemy != null && !enemy.IsDead)
                enemy.TakeDamage(damage);
        }
        Debug.Log($"[Skills] {skill.skillName}: AoE {damage:F0} dmg a {hits.Length} enemigos en radio {skill.aoeRadius}");
    }

    IEnumerator ExecuteBuff(SkillData skill)
    {
        ApplyBuff(skill, true);
        Debug.Log($"[Skills] {skill.skillName}: buff '{skill.buffStat}' activado por {skill.buffDuration}s");
        yield return new WaitForSeconds(skill.buffDuration);
        ApplyBuff(skill, false);
        Debug.Log($"[Skills] {skill.skillName}: buff '{skill.buffStat}' expirado");
    }

    void ApplyBuff(SkillData skill, bool apply)
    {
        int val = apply ? Mathf.RoundToInt(skill.buffValue) : -Mathf.RoundToInt(skill.buffValue);
        switch (skill.buffStat)
        {
            case "STR": stats.strength     += val; break;
            case "VIT": stats.vitality     += val; break;
            case "INT": stats.intelligence += val; break;
            case "DEX": stats.dexterity    += val; break;
        }
        stats.NotifyStatsChanged();
    }

    void ExecuteHeal(SkillData skill)
    {
        stats.HealHP(skill.buffValue);
        Debug.Log($"[Skills] {skill.skillName}: cura {skill.buffValue} HP");
    }

    void StartCooldown(int idx, float duration)
    {
        onCooldown[idx]     = true;
        cooldownTimers[idx] = duration;
        skillBarUI?.StartCooldown(idx, duration);
    }

    float CalcDamage(SkillData skill)
    {
        float baseDmg = skill.isMagicDamage ? stats.MagicDamage : stats.PhysDamage;
        return baseDmg * skill.damageMultiplier;
    }

    Vector3 GetAimDirection()
    {
        if (combat.LockedTarget != null)
        {
            var dir = (combat.LockedTarget.transform.position - transform.position).normalized;
            dir.y = 0f;
            return dir == Vector3.zero ? transform.forward : dir;
        }

        // Apuntar en la dirección que mira el player
        var forward = transform.forward;
        forward.y = 0f;
        return forward == Vector3.zero ? Vector3.forward : forward;
    }

    void SyncIconsToBar()
    {
        if (skillBarUI == null) return;
        for (int i = 0; i < skills.Length; i++)
        {
            if (skills[i] != null)
                skillBarUI.SetSlotIcon(i, skills[i].icon);
        }
    }

    public void SetSkill(int slot, SkillData skill)
    {
        if (slot < 0 || slot >= 8) return;
        skills[slot] = skill;
        skillBarUI?.SetSlotIcon(slot, skill?.icon);
    }
}
