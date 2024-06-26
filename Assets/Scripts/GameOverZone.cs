using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameOverZone : MonoBehaviour
{


  private bool prepareToGameOver = false;
  private Coroutine gameOverCountdownCoroutine;
  private HashSet<PolyhedronCollisionHandler> polyhedronsInZone = new HashSet<PolyhedronCollisionHandler>();
  private void OnTriggerEnter(Collider other)
  {
    Debug.Log("Game Over Zone Triggered");
    PolyhedronCollisionHandler polyhedron = other.GetComponent<PolyhedronCollisionHandler>();
    if (polyhedron != null)
    {
      polyhedronsInZone.Add(polyhedron);
      Debug.Log($"Polyhedron with value {polyhedron.value} entered Game Over Zone. Starting countdown.");
    }
  }


  void Update()
  {

    if (polyhedronsInZone.Count > 0 && !prepareToGameOver)
    {

      foreach (PolyhedronCollisionHandler polyhedron in polyhedronsInZone)
      {
        if (polyhedron.recentlyShot)
        {
          continue;
        }
        if (gameOverCountdownCoroutine == null)
        {

          prepareToGameOver = true;
          gameOverCountdownCoroutine = StartCoroutine(StartGameOverCountdown(polyhedron));

        }
        break;
      }

    }
    else if (polyhedronsInZone.Count == 0 && prepareToGameOver)
    {
      if (gameOverCountdownCoroutine != null)
      {
        StopCoroutine(gameOverCountdownCoroutine);
        prepareToGameOver = false;
        gameOverCountdownCoroutine = null;
      }
    }
  }
  private void OnTriggerExit(Collider other)
  {
    PolyhedronCollisionHandler polyhedron = other.GetComponent<PolyhedronCollisionHandler>();
    if (polyhedron != null)
    {
      polyhedronsInZone.Remove(polyhedron);
      Debug.Log($"Polyhedron with value {polyhedron.value} exited Game Over Zone. Stopping countdown.");
      foreach (PolyhedronCollisionHandler poly in polyhedronsInZone)
      {
        if (!poly.recentlyShot)
        {
          return;
        }

      }
      if (gameOverCountdownCoroutine != null)
      {
        StopCoroutine(gameOverCountdownCoroutine);
        prepareToGameOver = false;
        gameOverCountdownCoroutine = null;
      }

    }

  }

  private IEnumerator StartGameOverCountdown(PolyhedronCollisionHandler polyhedron)
  {
    Debug.Log($"Starting Game Over countdown for polyhedron with value {polyhedron.value}");
    for (int i = 10; i > 0; i--)
    {
      Debug.Log($"Game Over countdown for polyhedron with value {polyhedron.value}: {i}");
      yield return new WaitForSeconds(1);
    }

    // Game over logic
    if (polyhedron != null)
    {
      GameManager.instance.GameOver();
      Debug.Log($"Polyhedron with value {polyhedron.value} triggered Game Over.");
      // Add your game over logic here
    }
  }
}
