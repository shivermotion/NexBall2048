using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorTileCollider : MonoBehaviour
{
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Polyhedron"))
        {
            Debug.Log("Poly Entered");

            var poly = other.gameObject.GetComponent<PolyhedronCollisionHandler>();
            
            if (poly)
            {
                
            }
        }
    }
}
