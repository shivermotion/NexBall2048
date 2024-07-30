using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorTileCollider : MonoBehaviour
{
    [SerializeField] private float timeInSecondsBeforeFalling = 3f;
    private HashSet<PolyhedronCollisionHandler> touchingPolys = new();
    private bool dying = false;

    private bool behindFoulLine => GameOverZone.FoulLinePosition.z > transform.position.z + 2;
    
    private void OnDrawGizmos()
    {
        Gizmos.color = behindFoulLine ? Color.red : Color.green;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.up * 10);
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Polyhedron"))
        {
            var poly = other.gameObject.GetComponent<PolyhedronCollisionHandler>();

            if (poly)
            {
                touchingPolys.Add(poly);
            }
        }
    }

    private void OnCollisionExit(Collision other)
    {
        if (other.gameObject.CompareTag("Polyhedron"))
        {
            var poly = other.gameObject.GetComponent<PolyhedronCollisionHandler>();

            if (poly)
            {
                touchingPolys.Remove(poly);
            }
        }
    }

    private void LateUpdate()
    {
        if (touchingPolys.Count <= 0 || !behindFoulLine)
        {
            if (dying)
            {
                dying = false;
                StopAllCoroutines();
            }
            return;
        }
        
        Debug.DrawLine(transform.position, transform.position + (Vector3.up * 10));

        bool touchingGameEndingPoly = false;

        foreach (var poly in touchingPolys)
        {
            if (!poly)
            {
                touchingPolys.Remove(poly);
                return;
            }
            
            if (poly.canTriggerGameOver)
            {
                touchingGameEndingPoly = true;
                break;
            }
        }

        if (touchingGameEndingPoly)
        {
            if (!dying)
            {
                dying = true;
                StartCoroutine(DeathRoutine());
            }
        }else if (dying)
        {
            dying = false;
            StopAllCoroutines();
        }
    }

    IEnumerator DeathRoutine()
    {
        print("Starting death");
        yield return new WaitForSeconds(timeInSecondsBeforeFalling);
        
        Destroy(gameObject);
    }
}
