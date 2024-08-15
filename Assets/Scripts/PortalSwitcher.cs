using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalSwitcher : MonoBehaviour
{
    public GameObject[] portals;  // Array to store all portals

    void Start()
    {
        // Ensure only one portal is active at the start
        for (int i = 0; i < portals.Length; i++)
        {
            portals[i].SetActive(i == 0);  // Activate the first portal by default
        }
    }

    public void SwitchPortal(int index)
    {
        for (int i = 0; i < portals.Length; i++)
        {
            portals[i].SetActive(i == index);  // Activate the selected portal and deactivate others
        }
    }
}
