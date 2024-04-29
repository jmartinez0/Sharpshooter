using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using TMPro;
using System.Threading.Tasks;
using Firebase.Extensions;
using UnityEngine.SceneManagement;
using Firebase.Firestore;

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

        // Check if any of the input fields are empty
        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            // Display error message for empty fields
            UpdateUIOnError("All fields must be filled.", errorLoginText);
            return;
        }

        auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task => {
        if (task.IsCanceled)
        {
            Debug.LogError("SignInWithEmailAndPasswordAsync was canceled.");
            return;
        }
        if (task.IsFaulted)
        {
            Debug.LogError("SignInWithEmailAndPasswordAsync encountered an error: " + task.Exception);
            // Display error message
            UpdateUIOnError("Incorrect username or password.", errorLoginText);
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
            // Display error message
            UpdateUIOnError("All fields must be filled.", errorRegisterText);
            return;
        }

        auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("CreateUserWithEmailAndPasswordAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Firebase.FirebaseException firebaseEx = null;
                // Loop to find the first Firebase exception,
                // since there are multiple possible exceptions,
                // like bad email formatting, weak password, etc.
                foreach (var exception in task.Exception.Flatten().InnerExceptions)
                {
                    firebaseEx = exception as Firebase.FirebaseException;
                    if (firebaseEx != null)
                    {
                        Debug.LogError("CreateUserWithEmailAndPasswordAsync encountered an error: " + firebaseEx.Message);
                        break; // Exit loop after finding the first Firebase exception
                    }
                }
                if (firebaseEx != null)
                {
                    HandleFirebaseAuthErrors(firebaseEx.ErrorCode, errorRegisterText);
                }
                else
                {
                    UpdateUIOnError("Failed to register. Please try again.", errorRegisterText);
                }
                return;
            }

            // Firebase user has been created.
            Firebase.Auth.AuthResult result = task.Result;
            Debug.LogFormat("Firebase user created successfully: {0} ({1})", result.User.DisplayName, result.User.UserId);

            // Create a new document in Firestore under 'Users' collection with the email as the document ID
            FirebaseFirestore db = FirebaseFirestore.DefaultInstance;
            DocumentReference docRef = db.Collection("Users").Document(email);
            Dictionary<string, object> user = new Dictionary<string, object>
            {
                { "email", email },
                { "username", username },
                { "total_score", 0 },
                { "overall_accuracy", 0.0 },
                { "total_shots", 0 },
                { "total_hits", 0 },
                { "total_misses", 0 },
                { "games_played", 0 },
                { "sensitivity", 0.0 },
                { "pistol_skin", "default"},
            };
            docRef.SetAsync(user).ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.LogError("Error adding document: " + task.Exception);
                    UpdateUIOnError("Error creating user profile, please try again.", errorRegisterText);
                }
                else
                {
                    Debug.Log("Document added successfully with ID: " + email);
                    // Optionally, load the login screen on success or any other appropriate action
                    ShowLogin();
                }
            });

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
        errorLoginText.text = "";
        errorRegisterText.text = "";
    }

    // Pulls focus to first input field in a canvas, used when switching the canvas
    public void SetFocusToInputField(TMP_InputField inputField)
    {
        inputField.Select();
        inputField.ActivateInputField();
    }

    // Updates the error text on screen. There are two separate errorText GameObjects.
    private void UpdateUIOnError(string message, TMP_Text errorText)
    {
        errorText.text = message;
        errorText.gameObject.SetActive(true);
    }

    // Handles Firebase Auth Errors based on the error code provided by Firebase,
    // This is used for the register function to account for all possible registration errors.
    private void HandleFirebaseAuthErrors(int errorCode, TMP_Text errorText)
    {
        switch (errorCode)
        {
            case (int)Firebase.Auth.AuthError.InvalidEmail:
                UpdateUIOnError("Email must follow the format email@email.com.", errorText);
                break;
            case (int)Firebase.Auth.AuthError.EmailAlreadyInUse:
                UpdateUIOnError("The email address is already in use by another account.", errorText);
                break;
            case (int)Firebase.Auth.AuthError.WeakPassword:
                UpdateUIOnError("Passwords must be at least 6 characters long.", errorText);
                break;
            default:
                UpdateUIOnError($"An error occurred: {errorCode}", errorText);
                break;
        }
    }

}
