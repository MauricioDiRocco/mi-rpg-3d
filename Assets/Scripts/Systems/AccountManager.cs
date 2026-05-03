using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public class AccountManager : MonoBehaviour
{
    public static AccountManager Instance { get; private set; }

    public AccountData   CurrentAccount   { get; private set; }
    public CharacterData SelectedCharacter { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // ── Paths ────────────────────────────────────────────────────────────────
    static string SaveDir  => Path.Combine(Application.persistentDataPath, "accounts");
    static string FilePath(string user) => Path.Combine(SaveDir, user.ToLowerInvariant() + ".json");

    // ── Public API ───────────────────────────────────────────────────────────
    public bool AccountExists(string username)
    {
        if (string.IsNullOrWhiteSpace(username)) return false;
        return File.Exists(FilePath(username));
    }

    public bool TryLogin(string username, string password, out string error)
    {
        error = null;
        if (!Validate(username, password, out error)) return false;

        if (!AccountExists(username)) { error = "La cuenta no existe."; return false; }

        var data = Load(username);
        if (data == null) { error = "Error al leer la cuenta."; return false; }
        if (data.passwordHash != Hash(password)) { error = "Contraseña incorrecta."; return false; }

        CurrentAccount = data;
        return true;
    }

    public bool TryRegister(string username, string password, out string error)
    {
        error = null;
        if (!Validate(username, password, out error)) return false;
        if (username.Length < 3)  { error = "El usuario necesita al menos 3 caracteres."; return false; }
        if (password.Length < 4)  { error = "La contraseña necesita al menos 4 caracteres."; return false; }
        if (AccountExists(username)) { error = "Ya existe una cuenta con ese nombre."; return false; }

        var data = new AccountData
        {
            username     = username,
            passwordHash = Hash(password),
            characters   = new List<CharacterData>()
        };
        Save(data);
        CurrentAccount = data;
        return true;
    }

    public void SelectCharacter(CharacterData ch) => SelectedCharacter = ch;

    public void AddCharacter(CharacterData ch)
    {
        if (CurrentAccount == null) return;
        CurrentAccount.characters.Add(ch);
        Save(CurrentAccount);
    }

    public void DeleteCharacter(int index)
    {
        if (CurrentAccount == null || index < 0 || index >= CurrentAccount.characters.Count) return;
        CurrentAccount.characters.RemoveAt(index);
        Save(CurrentAccount);
    }

    public void SaveCurrentAccount()
    {
        if (CurrentAccount != null) Save(CurrentAccount);
    }

    public void SaveCharacterProgress(PlayerStats stats)
    {
        if (SelectedCharacter == null || stats == null) return;
        SelectedCharacter.level       = stats.level;
        SelectedCharacter.statPoints  = stats.statPoints;
        SelectedCharacter.strength    = stats.strength;
        SelectedCharacter.vitality    = stats.vitality;
        SelectedCharacter.intelligence = stats.intelligence;
        SelectedCharacter.dexterity   = stats.dexterity;
        SaveCurrentAccount();
    }

    public void Logout()
    {
        CurrentAccount    = null;
        SelectedCharacter = null;
    }

    // ── Internal ─────────────────────────────────────────────────────────────
    static bool Validate(string user, string pass, out string error)
    {
        error = null;
        if (string.IsNullOrWhiteSpace(user)) { error = "Ingresá un nombre de usuario."; return false; }
        if (string.IsNullOrWhiteSpace(pass)) { error = "Ingresá una contraseña."; return false; }
        return true;
    }

    void Save(AccountData data)
    {
        if (!Directory.Exists(SaveDir)) Directory.CreateDirectory(SaveDir);
        File.WriteAllText(FilePath(data.username), JsonUtility.ToJson(data, true));
    }

    AccountData Load(string username)
    {
        string path = FilePath(username);
        if (!File.Exists(path)) return null;
        try { return JsonUtility.FromJson<AccountData>(File.ReadAllText(path)); }
        catch { return null; }
    }

    static string Hash(string input)
    {
        using var sha = SHA256.Create();
        var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(input));
        var sb = new StringBuilder();
        foreach (var b in bytes) sb.Append(b.ToString("x2"));
        return sb.ToString();
    }
}
