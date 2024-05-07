using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro; // Ensure you have the TextMesh Pro namespace

public class LoadSceneOnClick : MonoBehaviour
{
    public TMP_Dropdown mapDropdown;

    public void LoadSelectedMap()
    {
        // Check which map is selected and load the respective scene
        switch (mapDropdown.value)
        {
            case 0: // Assuming "Map 1" is at index 0
                SceneManager.LoadScene("Playground");
                break;
            case 1: // Assuming "Map 2" is at index 1
                SceneManager.LoadScene("Playground 1");
                break;
            default:
                Debug.LogError("Selected map index does not match any scenes!");
                break;
        }
    }
}
