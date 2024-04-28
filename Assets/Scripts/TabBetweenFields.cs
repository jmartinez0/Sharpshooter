using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

// Handles tabbing between InputFields (did not seem to be supported natively)
public class TabBetweenFields : MonoBehaviour
{
    // Lists of input fields for login and register screens
    public List<TMP_InputField> loginInputFields = new List<TMP_InputField>();
    public List<TMP_InputField> registerInputFields = new List<TMP_InputField>();

    // Index to track the currently selected input field
    private int currentIndex = 0;

    // Reference to the last active canvas
    private GameObject lastActiveCanvas;

    // Reference to the login and register canvases
    public GameObject loginCanvas;
    public GameObject registerCanvas;

    void Update()
    {
        // Check if the Tab key is pressed
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            // Get the list of active input fields
            List<TMP_InputField> activeInputFields = GetActiveInputFields();

            // Move to the next input field
            currentIndex = (currentIndex + 1) % activeInputFields.Count;
            activeInputFields[currentIndex].Select();
        }

        // Check if the active canvas has changed
        if (lastActiveCanvas != GetActiveCanvas())
        {
            // Update the last active canvas reference
            lastActiveCanvas = GetActiveCanvas();

            // Reset currentIndex when the active canvas changes
            currentIndex = 0;
        }
    }

    // Returns the list of active input fields based on the active canvas
    private List<TMP_InputField> GetActiveInputFields()
    {
        if (LoginCanvasIsActive())
        {
            return loginInputFields;
        }
        else if (RegisterCanvasIsActive())
        {
            return registerInputFields;
        }
        else
        {
            // If no active canvas is found, return an empty list
            Debug.LogWarning("No active canvas found.");
            return new List<TMP_InputField>();
        }
    }

    // Checks if the login canvas is active
    private bool LoginCanvasIsActive()
    {
        return loginCanvas.activeSelf;
    }

    // Checks if the register canvas is active
    private bool RegisterCanvasIsActive()
    {
        return registerCanvas.activeSelf;
    }

    // Returns the reference to the active canvas
    private GameObject GetActiveCanvas()
    {
        if (LoginCanvasIsActive())
        {
            return loginCanvas;
        }
        else if (RegisterCanvasIsActive())
        {
            return registerCanvas;
        }
        else
        {
            // If no active canvas is found, return null
            return null;
        }
    }
}
