using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(BoxCollider))]
public class FloorTileCollider : MonoBehaviour
{
    [SerializeField] private float timeInSecondsBeforeFalling = 3f;
    [SerializeField] private GameObject renderer;
    private HashSet<PolyhedronCollisionHandler> touchingPolys = new();
    private bool dying = false;

    [SerializeField] float wobbleSpeed = .2f;
    [SerializeField] private float wobbleStrength = 5f;

    [SerializeField] new Rigidbody rigidbody;
    [SerializeField] private BoxCollider boxCollider;
    
    
    private bool behindFoulLine => GameOverZone.FoulLinePosition.z >= transform.position.z + 1.98f;
    
    private void OnDrawGizmos()
    {
        Gizmos.color = behindFoulLine ? Color.red : Color.green;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.up * 10);
    }

    private void OnValidate()
    {
        boxCollider = GetComponent<BoxCollider>();
        rigidbody = GetComponent<Rigidbody>();
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
        void ResetDying()
        {
            dying = false;
            StopAllCoroutines();
            renderer.transform.localEulerAngles = Vector3.zero;
        }
        
        if (touchingPolys.Count <= 0 || !behindFoulLine)  
        {
            if (dying)
            {
                ResetDying();
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
            ResetDying();
        }
    }

    IEnumerator DeathRoutine()
    {
        float fallEndTime = Time.time + timeInSecondsBeforeFalling;
        print("Starting death");

        while (Time.time < fallEndTime)
        {
            float percent = 1 - ((fallEndTime - Time.time) / timeInSecondsBeforeFalling);

            float t = Time.time * wobbleSpeed;

            float wobbleX = Mathf.PerlinNoise(0f, t) - .5f;
            float wobbley = Mathf.PerlinNoise(356.8f, t) - .5f;
            float wobblez = Mathf.PerlinNoise(t, 103.2f) - .5f;

            Vector3 wobble = new Vector3(wobbleX, wobbley, wobblez) * (wobbleStrength * 2f * percent);

            renderer.transform.localEulerAngles = wobble;
            
            yield return null;
        }

        rigidbody.isKinematic = false;
        boxCollider.size = new Vector3(boxCollider.size.x * .75f, boxCollider.size.y, boxCollider.size.z * .75f);
        
        Destroy(this);
    }
}
