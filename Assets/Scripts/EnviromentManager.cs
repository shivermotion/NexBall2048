using UnityEngine;

public class EnvironmentManager : MonoBehaviour
{
    public GameObject[] environments; // Array to hold the environment GameObjects
    public Material defaultSkyboxMaterial; // Default Unity Skybox
    public Material environment3SkyboxMaterial; // Custom rotating skybox

    public void ChangeEnvironment(int index)
    {
        // Deactivate all environments first
        foreach (GameObject environment in environments)
        {
            environment.SetActive(false);
        }

        // Activate the selected environment
        if (index >= 0 && index < environments.Length)
        {
            environments[index].SetActive(true);

            if (index == 2) // If environment 3 (index 2) is selected
            {
                RenderSettings.skybox = environment3SkyboxMaterial;
                // Enable rotation for the skybox
                StartSkyboxRotation();
            }
            else
            {
                RenderSettings.skybox = defaultSkyboxMaterial;
                // Disable rotation for the skybox
                StopSkyboxRotation();
            }

            Debug.Log("Activated Environment: " + environments[index].name);
        }
        else
        {
            Debug.LogWarning("Invalid environment index");
        }
    }

    private void StartSkyboxRotation()
    {
        // Add your skybox rotation code here if needed
        enabled = true; // Assuming your script is rotating the skybox in Update()
    }

    private void StopSkyboxRotation()
    {
        enabled = false; // Stop skybox rotation
    }

    void Update()
    {
        // Rotate the skybox if enabled
        if (enabled && RenderSettings.skybox == environment3SkyboxMaterial)
        {
            RenderSettings.skybox.SetFloat("_Rotation", Time.time * 2f); // Adjust speed as needed
        }
    }
}
