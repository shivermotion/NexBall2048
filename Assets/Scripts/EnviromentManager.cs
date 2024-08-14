using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentManager : MonoBehaviour
{
    public GameObject[] environments; // Array to hold the environment GameObjects

    // Method to switch environments based on the button pressed
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
        }
        else
        {
            Debug.LogWarning("Invalid environment index");
        }
    }
}
