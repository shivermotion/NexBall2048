using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameOverZone : MonoBehaviour
{
    private static GameOverZone instance;
    
    private bool prepareToGameOver = false;
    private Coroutine gameOverCountdownCoroutine;
    private HashSet<PolyhedronCollisionHandler> polyhedronsInZone = new HashSet<PolyhedronCollisionHandler>();
    [SerializeField] private GameObject foulLinePosition;

    public static Vector3 FoulLinePosition => instance?.foulLinePosition?.transform.position ?? Vector3.zero;
}
