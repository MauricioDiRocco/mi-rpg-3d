using UnityEngine;
using TMPro;

// Prefab de item en el suelo. Necesita un mesh visible y un TextMeshPro 3D para el nombre.
public class DroppedItem : MonoBehaviour
{
    [Header("Visual")]
    public TextMeshPro nameLabel;
    public float pickupRange = 2f;

    public ItemData item   { get; private set; }
    public int      amount { get; private set; }
    public bool     isGold { get; private set; }
    public int      goldAmount { get; private set; }

    private Transform   player;
    private Inventory   playerInventory;

    void Start()
    {
        var playerGO    = GameObject.FindGameObjectWithTag("Player");
        player          = playerGO?.transform;
        playerInventory = playerGO?.GetComponent<Inventory>();
    }

    void Update()
    {
        // Label siempre mira a la cámara
        if (nameLabel != null && Camera.main != null)
        {
            nameLabel.transform.rotation = Camera.main.transform.rotation;
        }
    }

    public void Initialize(ItemData data, int qty)
    {
        item   = data;
        amount = qty;
        isGold = false;
        RefreshLabel();
    }

    public void InitializeGold(int qty)
    {
        isGold     = true;
        goldAmount = qty;
        RefreshLabel();
    }

    void RefreshLabel()
    {
        if (nameLabel == null) return;

        if (isGold)
        {
            nameLabel.text  = $"{goldAmount} Yang";
            nameLabel.color = new Color(1f, 0.85f, 0.1f);
        }
        else if (item != null)
        {
            nameLabel.text  = amount > 1 ? $"{item.itemName} x{amount}" : item.itemName;
            nameLabel.color = item.RarityColor();
        }
    }

    void OnMouseDown()
    {
        if (player == null) return;
        if (Vector3.Distance(transform.position, player.position) <= pickupRange)
            TryPickup();
    }

    public void TryPickup()
    {
        if (isGold)
        {
            playerInventory?.AddGold(goldAmount);
            Destroy(gameObject);
            return;
        }

        if (item == null) return;

        if (item.type == ItemType.Potion)
        {
            // Auto-usar pociones
            var pStats = player?.GetComponent<PlayerStats>();
            if (pStats != null)
            {
                pStats.HealHP(item.healHP);
                pStats.currentMana = Mathf.Min(pStats.currentMana + item.healMana, pStats.MaxMana);
            }
            Destroy(gameObject);
            return;
        }

        if (playerInventory != null && playerInventory.AddItem(item, amount))
            Destroy(gameObject);
        else
            Debug.Log("Inventario lleno!");
    }
}
