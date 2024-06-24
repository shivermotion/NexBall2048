using UnityEngine;
using System.Collections;

public class GameOverZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        PolyhedronCollisionHandler polyhedron = other.GetComponent<PolyhedronCollisionHandler>();
        if (polyhedron != null && !polyhedron.recentlyShot)
        {
            Debug.Log($"Polyhedron with value {polyhedron.value} entered Game Over Zone. Starting countdown.");
            StartCoroutine(StartGameOverCountdown(polyhedron));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        PolyhedronCollisionHandler polyhedron = other.GetComponent<PolyhedronCollisionHandler>();
        if (polyhedron != null)
        {
            Debug.Log($"Polyhedron with value {polyhedron.value} exited Game Over Zone. Stopping countdown.");
            StopCoroutine(StartGameOverCountdown(polyhedron));
        }
    }

    private IEnumerator StartGameOverCountdown(PolyhedronCollisionHandler polyhedron)
    {
        yield return new WaitForSeconds(10); // Adjust this value as needed

        // Game over logic
        if (polyhedron != null)
        {
            Debug.Log($"Polyhedron with value {polyhedron.value} triggered Game Over.");
            // Add your game over logic here
        }
    }
}
