using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoginUIManager : MonoBehaviour
{

    public GameObject login;
    public GameObject register;
    // Start is called before the first frame update
    void Start()
    {
        ShowLogin();
    }

    public void ShowLogin()
    {
        login.SetActive(true);
        register.SetActive(false);
    }

    public void ShowRegister()
    {
        login.SetActive(false);
        register.SetActive(true);
    }

}
