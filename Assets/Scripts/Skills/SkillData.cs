using UnityEngine;

public enum SkillType { Melee, Projectile, AreaOfEffect, Buff, Heal }

[CreateAssetMenu(fileName = "NewSkill", menuName = "RPG/Skill Data")]
public class SkillData : ScriptableObject
{
    [Header("Identificacion")]
    public string     skillName    = "Skill";
    public string     description  = "";
    public Sprite     icon;

    [Header("Costo y Cooldown")]
    public float      manaCost     = 10f;
    public float      cooldown     = 3f;       // segundos

    [Header("Tipo")]
    public SkillType  skillType    = SkillType.Melee;

    [Header("Daño (Melee / Projectile / AoE)")]
    public float      damageMultiplier = 1.5f; // multiplicador sobre PhysDamage o MagicDamage
    public bool       isMagicDamage   = false;
    public float      range           = 3f;

    [Header("AoE")]
    public float      aoeRadius    = 4f;

    [Header("Projectile")]
    public GameObject projectilePrefab;
    public float      projectileSpeed = 15f;

    [Header("Buff / Heal")]
    public float      buffDuration = 5f;
    public float      buffValue    = 0f;       // cantidad de heal o valor de stat
    public string     buffStat     = "";       // "STR","VIT","INT","DEX","DEF","SPD"
}
