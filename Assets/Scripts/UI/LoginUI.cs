using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoginUI : MonoBehaviour
{
    [Header("Campos")]
    public TMP_InputField      usernameField;
    public TMP_InputField      passwordField;

    [Header("Botón y feedback")]
    public Button              confirmBtn;
    public TextMeshProUGUI     confirmBtnText;
    public TextMeshProUGUI     accountHintText;
    public TextMeshProUGUI     statusText;

    void Start()
    {
        EnsureAccountManager();
        usernameField.onValueChanged.AddListener(_ => RefreshHint());
        confirmBtn.onClick.AddListener(OnConfirm);
        statusText.text     = "";
        accountHintText.text = "";
        RefreshHint();
    }

    void RefreshHint()
    {
        string user   = usernameField.text.Trim();
        bool   exists = user.Length >= 3 && AccountManager.Instance.AccountExists(user);

        confirmBtnText.text = exists ? "ENTRAR" : "CREAR CUENTA";

        if (user.Length < 3)
            accountHintText.text = "";
        else if (exists)
        {
            accountHintText.text  = "Cuenta existente";
            accountHintText.color = new Color(0.5f, 0.85f, 0.5f);
        }
        else
        {
            accountHintText.text  = "Cuenta nueva — se creará al confirmar";
            accountHintText.color = new Color(0.85f, 0.75f, 0.35f);
        }
    }

    void OnConfirm()
    {
        string user = usernameField.text.Trim();
        string pass = passwordField.text;
        statusText.text = "";

        bool ok;
        if (AccountManager.Instance.AccountExists(user))
            ok = AccountManager.Instance.TryLogin(user, pass, out string err1)
                 || SetStatus(err1);
        else
            ok = AccountManager.Instance.TryRegister(user, pass, out string err2)
                 || SetStatus(err2);

        if (ok) SceneManager.LoadScene("CharSelectScene");
    }

    bool SetStatus(string msg) { statusText.text = msg; return false; }

    static void EnsureAccountManager()
    {
        if (AccountManager.Instance == null)
            new GameObject("AccountManager").AddComponent<AccountManager>();
    }
}
