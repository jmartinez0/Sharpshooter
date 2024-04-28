using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using TMPro;
using System.Threading.Tasks;
using Firebase.Extensions;
using UnityEngine.SceneManagement;

// Handles authentication actions and switching between log in and register screens
public class LoginUIManager : MonoBehaviour
{
    public static LoginUIManager instance;
    public GameObject login;
    public GameObject register;

    //Firebase variables
    [Header("Firebase")]
    public DependencyStatus dependencyStatus;
    public FirebaseAuth auth;    
    public FirebaseUser User;

    //Login variables
    [Header("Login")]
    public TMP_InputField emailLoginField;
    public TMP_InputField passwordLoginField;
    public TMP_Text errorLoginText;
    public Firebase.FirebaseApp app;

    //Register variables
    [Header("Register")]
    public TMP_InputField usernameRegisterField;
    public TMP_InputField emailRegisterField;
    public TMP_InputField passwordRegisterField;
    public TMP_Text errorRegisterText;

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

        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task => {
        var dependencyStatus = task.Result;
        if (dependencyStatus == Firebase.DependencyStatus.Available) {
        // Create and hold a reference to your FirebaseApp,
        // where app is a Firebase.FirebaseApp property of your application class.
        app = Firebase.FirebaseApp.DefaultInstance;

        // Set a flag here to indicate whether Firebase is ready to use by your app.
        } else {
            UnityEngine.Debug.LogError(System.String.Format(
            "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
            // Firebase Unity SDK is not safe to use here.
        }
        });
        InitializeFirebase();
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
        ClearInputFields();
        SetFocusToInputField(emailLoginField);
    }

    public void ShowRegister()
    {
        login.SetActive(false);
        register.SetActive(true);
        ClearInputFields();
        SetFocusToInputField(emailRegisterField);
    }


    private void InitializeFirebase()
    {
        Debug.Log("Setting up Firebase Auth");
        //Set the authentication instance object
        auth = FirebaseAuth.DefaultInstance;
    }

    public void LoginButton()
    {
        // Get user input
        string email = emailLoginField.text;
        string password = passwordLoginField.text;
        auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWith(task => {
        if (task.IsCanceled) {
            Debug.LogError("SignInWithEmailAndPasswordAsync was canceled.");
            return;
        }
        if (task.IsFaulted) {
            Debug.LogError("SignInWithEmailAndPasswordAsync encountered an error: " + task.Exception);
            return;
        }

        Firebase.Auth.AuthResult result = task.Result;
        Debug.LogFormat("User signed in successfully: {0} ({1})",
            result.User.DisplayName, result.User.UserId);
            // Load the main menu on success
            SceneManager.LoadScene("Menu");
        });

    }
    
    public void RegisterButton()
    {
        // Get user input
        string email = emailRegisterField.text;
        string password = passwordRegisterField.text;
        string username = usernameRegisterField.text;

        // Check if any of the input fields are empty
        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(username))
        {
            errorRegisterText.text = "All fields must be filled.";
            errorRegisterText.enabled = true;
            Debug.LogError("Fields cannot be empty.");
            return;
        }

        auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("CreateUserWithEmailAndPasswordAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("CreateUserWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                return;
            }

            // Firebase user has been created.
            Firebase.Auth.AuthResult result = task.Result;
            Debug.LogFormat("Firebase user created successfully: {0} ({1})", result.User.DisplayName, result.User.UserId);

            // Load the login screen on success
            Debug.Log("Showing login screen...");
            ShowLogin();
        });
    }

    // Clears all input fields for use when switching the canvas
    public void ClearInputFields()
    {
        emailLoginField.text = "";
        passwordLoginField.text = "";
        emailRegisterField.text = "";
        passwordRegisterField.text = "";
        usernameRegisterField.text = "";
    }

    // Pulls focus to first input field in a canvas, used when switching the canvas
    public void SetFocusToInputField(TMP_InputField inputField)
    {
        inputField.Select();
        inputField.ActivateInputField();
    }



}
