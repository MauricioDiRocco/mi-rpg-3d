using UnityEngine;

// Agregar al Player. Aplica los stats y skills de la clase elegida.
[RequireComponent(typeof(PlayerStats))]
[RequireComponent(typeof(SkillSystem))]
public class ClassManager : MonoBehaviour
{
    public CharacterClass currentClass;

    private PlayerStats  stats;
    private SkillSystem  skillSystem;

    void Awake()
    {
        stats       = GetComponent<PlayerStats>();
        skillSystem = GetComponent<SkillSystem>();
    }

    void Start()
    {
        if (currentClass != null)
            ApplyClass(currentClass);
    }

    public void ApplyClass(CharacterClass cls)
    {
        currentClass = cls;

        stats.strength     = cls.startSTR;
        stats.vitality     = cls.startVIT;
        stats.intelligence = cls.startINT;
        stats.dexterity    = cls.startDEX;
        stats.currentHP    = stats.MaxHP;
        stats.currentMana  = stats.MaxMana;
        stats.NotifyStatsChanged();

        for (int i = 0; i < cls.classSkills.Length && i < 8; i++)
            skillSystem.SetSkill(i, cls.classSkills[i]);

        Debug.Log($"[Class] Clase aplicada: {cls.className}");
    }

    // Llamar cuando sube de nivel para dar bonus automáticos de clase
    public void OnLevelUp()
    {
        if (currentClass == null) return;
        stats.strength     += currentClass.bonusSTRPerLevel;
        stats.vitality     += currentClass.bonusVITPerLevel;
        stats.intelligence += currentClass.bonusINTPerLevel;
        stats.dexterity    += currentClass.bonusDEXPerLevel;
    }
}
