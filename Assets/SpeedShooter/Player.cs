using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    public GameController gameController;
    private Camera cam;

    void Start()
    {
        cam = Camera.main;
        Cursor.lockState = CursorLockMode.Locked; // Optional: Lock the cursor to the center of the screen
        Cursor.visible = false; // Optional: Hide the cursor
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Left mouse click
        {
            gameController.RecordShot(false);  // Record a shot attempt

            Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100.0f)) // Adjust the distance as needed
            {
                if (hit.collider.gameObject.CompareTag("Target")) // Ensure your target has the tag "Target"
                {
                    Destroy(hit.collider.gameObject);
                    gameController.RecordShot(true);  // Record a successful hit
                }
            }
        }
    }

}
