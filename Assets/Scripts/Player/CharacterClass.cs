using UnityEngine;

public enum ClassType { Warrior, Mage, Archer }

[CreateAssetMenu(fileName = "NewClass", menuName = "RPG/Character Class")]
public class CharacterClass : ScriptableObject
{
    [Header("Identificacion")]
    public ClassType  classType;
    public string     className    = "Guerrero";
    public string     description  = "";
    public Sprite     icon;
    public Color      classColor   = Color.white;

    [Header("Stats Iniciales")]
    public int startSTR = 10;
    public int startVIT = 10;
    public int startINT = 10;
    public int startDEX = 10;

    [Header("Skills de Clase (slots F1-F4 pre-asignados)")]
    public SkillData[] classSkills = new SkillData[4];

    [Header("Multiplicadores de Stat por nivel")]
    [Tooltip("Cuántos puntos de stat sube automáticamente cada stat por nivel")]
    public int bonusSTRPerLevel = 0;
    public int bonusVITPerLevel = 0;
    public int bonusINTPerLevel = 0;
    public int bonusDEXPerLevel = 0;
}
