using System;
using UnityEngine;

public class PolyKillZone : MonoBehaviour
{
    public static Action onPolyKilled = default;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Polyhedron"))
        {
            Destroy(other.gameObject);
            onPolyKilled?.Invoke();
            
            GameManager.instance.GameOver();
            
            return;
        }

        if (other.gameObject.CompareTag("FloorTile"))
        {
            Destroy(other.gameObject);
        }
    }
}
