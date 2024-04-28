using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using TMPro;
using System.Threading.Tasks;
using Firebase.Extensions;
using UnityEngine.SceneManagement;

public class AuthManager : MonoBehaviour
{
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

    void Start()
    {
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
        });

        SceneManager.LoadScene("Menu");

    }
    
    public void RegisterButton()
    {
        // Get user input
        string email = emailLoginField.text;
        string password = passwordLoginField.text;

        // Attempt to create a new user
        auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWith(task => {
        if (task.IsCanceled) {
            Debug.LogError("CreateUserWithEmailAndPasswordAsync was canceled.");
            return;
        }
        if (task.IsFaulted) {
            Debug.LogError("CreateUserWithEmailAndPasswordAsync encountered an error: " + task.Exception);
            return;
        }

        // Firebase user has been created.
        Firebase.Auth.AuthResult result = task.Result;
        Debug.LogFormat("Firebase user created successfully: {0} ({1})",
            result.User.DisplayName, result.User.UserId);
        });

    }

}
