using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoginUIManager : MonoBehaviour
{
    public static LoginUIManager instance;
    public GameObject login;
    public GameObject register;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != null)
        {
            Debug.Log("Instance already exists, destroying object!");
            Destroy(this);
        }
    }

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
