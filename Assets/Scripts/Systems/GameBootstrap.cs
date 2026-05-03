using UnityEngine;

// Aplicar los stats del personaje seleccionado cuando carga RPG_Main.
// Colocar en el mismo GameObject que PlayerStats.
public class GameBootstrap : MonoBehaviour
{
    void Start()
    {
        var ch = AccountManager.Instance?.SelectedCharacter;
        if (ch == null) return;

        var stats = GetComponent<PlayerStats>();
        if (stats == null) return;

        stats.level        = ch.level;
        stats.statPoints   = ch.statPoints;
        stats.strength     = ch.strength;
        stats.vitality     = ch.vitality;
        stats.intelligence = ch.intelligence;
        stats.dexterity    = ch.dexterity;

        // Reiniciar HP/Mana con los stats del personaje cargado
        stats.currentHP   = stats.MaxHP;
        stats.currentMana = stats.MaxMana;
    }

    void OnDestroy()
    {
        // Guardar progreso al salir de la escena
        var stats = GetComponent<PlayerStats>();
        AccountManager.Instance?.SaveCharacterProgress(stats);
    }
}
