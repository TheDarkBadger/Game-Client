using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMainMenu : MonoBehaviour
{
    public static UIMainMenu get { get; private set; }

    Transform pnlConnection, pnlCharacterList;

    Text txtConnection;
    InputField ifUsername, ifPassword, ifEmail;
    Toggle tglRememberLogin, tglRegister;
    Image ConnectionStatus;

    #region Prefabs
    [SerializeField]
    private GameObject prefabCharacterSelect;

    #endregion
    // Start is called before the first frame update
    void Start()
    {
        get = this;

        pnlCharacterList = transform.Find("Characters");
        pnlConnection = transform.Find("Connection");
        txtConnection = pnlConnection.Find("TxtConnection").GetComponent<Text>();
        Transform panel = pnlConnection.Find("Panel");
        ifUsername = panel.Find("UserName").GetComponent<InputField>();
        ifPassword = panel.Find("Password").GetComponent<InputField>();
        tglRememberLogin = panel.Find("TglRememberLogin").GetComponent<Toggle>();

        if (Config.GetSetting("RememberLogin") == "True")
        {
            tglRememberLogin.isOn = true;
            ifUsername.text = Config.GetSetting("Username");
            ifPassword.text = Config.GetSetting("Password");

        }

        ifEmail = panel.Find("Email").GetComponent<InputField>();
        ifEmail.gameObject.SetActive(false);

        tglRegister = pnlConnection.Find("TglRegister").GetComponent<Toggle>();
        ConnectionStatus = transform.Find("ConnectionStatus").GetComponent<Image>();
        Network.ConnectToServer();
    }

    public void BtnPlayPressed()
    {
        ShowConnectionPanel(true);
    }

    public void BtnConnectPressed()
    {
        if (tglRegister.isOn)
            Register();
        else
            Login();
        SetConnectionConfig();
    }

    public void TglRegisterPressed()
    {
        ifEmail.gameObject.SetActive(tglRegister.isOn);
        if (tglRegister.isOn)
            txtConnection.text = "Register";
        else
            txtConnection.text = "Login";
    }


    public void TglRememberLoginPressed()
    {
        SetConnectionConfig();
    }

    private void SetConnectionConfig()
    {
        Config.SetSetting("RememberLogin", tglRememberLogin.isOn.ToString());
        if (tglRememberLogin.isOn)
        {
            Config.SetSetting("Username", ifUsername.text);
            Config.SetSetting("Password", ifPassword.text);
        }
        else
        {
            Config.SetSetting("Username", "");
            Config.SetSetting("Password", "");
        }
        Config.UpdateConfigFile();
    }

    public void ShowConnectionPanel(bool toShow)
    {
        pnlConnection.gameObject.SetActive(toShow);
    }

    private void Login()
    {
        Network.Login(ifUsername.text, ifPassword.text);
    }

    private void Register()
    {
        Network.RegisterAccount(ifUsername.text, ifPassword.text, ifEmail.text);
    }
    
    public void UpdateConnectionStatus()
    {
        if (Network.IsConnected())
            ConnectionStatus.color = Color.green;
        else
            ConnectionStatus.color = Color.red;
    }

    private CharacterBasic[] loadedCharacterList;
    public void UpdateCharacterList(CharacterBasic[] list)
    {
        loadedCharacterList = list;
        foreach (CharacterBasic c in loadedCharacterList)
        {
            Transform g = Instantiate(prefabCharacterSelect, pnlCharacterList).transform;
            g.GetChild(0).GetComponent<Text>().text = c.DisplayName;
            g.GetChild(1).GetComponent<Text>().text = $"Lvl {c.Level}";

        }
    }

}
