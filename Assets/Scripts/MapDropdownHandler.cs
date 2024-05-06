using UnityEngine;
using UnityEngine.UI;
using TMPro; // Namespace for TextMeshPro

public class MapDropdownHandler : MonoBehaviour
{
    public Image mapPreviewImage; // The UI Image component to change
    public Sprite[] mapImages; // Array of images for the maps

    private TMP_Dropdown dropdown; // Private reference to the TMP_Dropdown component

    void Awake()
    {
        dropdown = GetComponent<TMP_Dropdown>(); // Get the TMP_Dropdown component
        if (dropdown != null)
        {
            dropdown.onValueChanged.AddListener(UpdateMapImage); // Add listener
        }
    }

    void Start()
    {
        UpdateMapImage(dropdown.value); // Initialize the image at start
    }

    private void UpdateMapImage(int index)
    {
        if (index >= 0 && index < mapImages.Length)
        {
            mapPreviewImage.sprite = mapImages[index]; // Set the image based on the selected index
        }
    }
}
