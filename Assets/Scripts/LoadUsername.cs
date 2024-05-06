using Firebase.Auth;
using Firebase.Firestore;
using Firebase.Extensions;
using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class LoadUsername : MonoBehaviour
{
    public TMP_Text usernameTMP;
    void Start()
    {
        // Get the current user
        FirebaseUser user = FirebaseAuth.DefaultInstance.CurrentUser;

        // Get Firestore instance
        FirebaseFirestore db = FirebaseFirestore.DefaultInstance;

        // Get email address of current user
        string email = user.Email;

        // Get a reference to the current user's document
        DocumentReference docRef = db.Collection("Users").Document(email);
        
        // Fetch the document snapshot asynchronously
        docRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
        DocumentSnapshot snapshot = task.Result;
        if (snapshot.Exists) {
            // Get the data dictionary from the document
                    Dictionary<string, object> userData = snapshot.ToDictionary();

                    // Check if the username field exists in the data
                    if (userData.ContainsKey("username"))
                    {
                        // Get the username value from the data
                        string username = userData["username"].ToString();

                        // Update the TextMeshPro component with the username
                        print(username);
                        usernameTMP.text = username;
                    }
        } else {
            Debug.LogWarning("User document does not exist.");
        }
        });
    }
}
