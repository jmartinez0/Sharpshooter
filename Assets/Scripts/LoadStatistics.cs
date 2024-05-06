using Firebase.Auth;
using Firebase.Firestore;
using Firebase.Extensions;
using UnityEngine;
using TMPro;
using System.Collections.Generic;
public class LoadStatistics : MonoBehaviour
{
    public TMP_Text totalScoreTMP;
    public TMP_Text accuracyTMP;
    public TMP_Text gamesPlayedTMP;
    public TMP_Text totalShotsTMP;
    public TMP_Text totalHitsTMP;
    public TMP_Text highScoreTMP;
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

                    // Get all the statistics from the document
                    string totalScore = userData["total_score"].ToString();
                    string gamesPlayed = userData["games_played"].ToString();
                    string totalShots = userData["total_shots"].ToString();
                    string totalHits = userData["total_hits"].ToString();
                    string highScore = userData["high_score"].ToString();

                    // Format the accuracy
                    double accuracyValue = (double)userData["overall_accuracy"];
                    string accuracy = (accuracyValue * 100).ToString("0.00") + "%";

                    // Update the text on screen with the statistics data we pulled
                    totalScoreTMP.text = totalScore;
                    accuracyTMP.text = accuracy;
                    gamesPlayedTMP.text = gamesPlayed;
                    totalShotsTMP.text = totalShots;
                    totalHitsTMP.text = totalHits;
                    highScoreTMP.text = highScore;
        } else {
            Debug.LogWarning("User document does not exist.");
        }
        });
    }
}
