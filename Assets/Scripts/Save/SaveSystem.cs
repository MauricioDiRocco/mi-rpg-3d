using System;
using System.IO;
using UnityEngine;

// Guarda y carga toda la partida en JSON (Application.persistentDataPath/save.json).
// Agregar al Player o a un Manager GameObject en la escena.
public class SaveSystem : MonoBehaviour
{
    [Serializable]
    public class SaveData
    {
        public int   level;
        public int   statPoints;
        public int   strength;
        public int   vitality;
        public int   intelligence;
        public int   dexterity;
        public float currentHP;
        public float currentMana;
        public long  currentExp;
        public int   gold;
        public string selectedClassName;
    }

    private static string SavePath => Path.Combine(Application.persistentDataPath, "save.json");

    public void Save()
    {
        var stats = GetComponent<PlayerStats>();
        var exp   = GetComponent<ExperienceSystem>();
        var inv   = GetComponent<Inventory>();
        var cls   = GetComponent<ClassManager>();

        if (stats == null) return;

        var data = new SaveData
        {
            level           = stats.level,
            statPoints      = stats.statPoints,
            strength        = stats.strength,
            vitality        = stats.vitality,
            intelligence    = stats.intelligence,
            dexterity       = stats.dexterity,
            currentHP       = stats.currentHP,
            currentMana     = stats.currentMana,
            currentExp      = exp != null ? exp.currentExp : 0,
            gold            = inv != null ? inv.gold : 0,
            selectedClassName = cls?.currentClass?.name ?? ""
        };

        string json = JsonUtility.ToJson(data, prettyPrint: true);
        File.WriteAllText(SavePath, json);
        Debug.Log($"[Save] Partida guardada en {SavePath}");
    }

    public bool Load()
    {
        if (!File.Exists(SavePath))
        {
            Debug.Log("[Save] No existe partida guardada");
            return false;
        }

        string json = File.ReadAllText(SavePath);
        var data    = JsonUtility.FromJson<SaveData>(json);

        var stats = GetComponent<PlayerStats>();
        var exp   = GetComponent<ExperienceSystem>();
        var inv   = GetComponent<Inventory>();

        if (stats == null) return false;

        stats.level        = data.level;
        stats.statPoints   = data.statPoints;
        stats.strength     = data.strength;
        stats.vitality     = data.vitality;
        stats.intelligence = data.intelligence;
        stats.dexterity    = data.dexterity;
        stats.currentHP    = data.currentHP;
        stats.currentMana  = data.currentMana;
        stats.NotifyStatsChanged();

        if (exp != null) exp.currentExp = data.currentExp;
        if (inv != null) inv.AddGold(data.gold);

        Debug.Log($"[Save] Partida cargada. Nivel {data.level}");
        return true;
    }

    public void DeleteSave()
    {
        if (File.Exists(SavePath))
            File.Delete(SavePath);
        Debug.Log("[Save] Partida eliminada");
    }

    public bool HasSave() => File.Exists(SavePath);

    void Update()
    {
        // Auto-save con F5, load con F9
        if (Input.GetKeyDown(KeyCode.F5)) Save();
        if (Input.GetKeyDown(KeyCode.F9)) Load();
    }
}
