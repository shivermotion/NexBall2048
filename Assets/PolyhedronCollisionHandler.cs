using UnityEngine;
using TMPro;
using System.Collections;

public class PolyhedronCollisionHandler : MonoBehaviour
{
    public int value;
    public Color color;
    [HideInInspector]
    public PolyhedronShooter shooter;
    public TextMeshProUGUI frontFace;
    public TextMeshProUGUI backFace;
    public float magnetismStrength = 1f; // Strength of the magnetism force
    public float stickinessStrength = 0.5f; // Strength of the stickiness force
    public float stickinessRange = 5f; // Range within which stickiness is applied
    public float strikeForce = 10f; // Force applied on collision to mimic a striking effect
    public float outwardForceStrength = 5f; // Strength of the outward force applied to new polyhedrons
    [HideInInspector]
    public float stayTime = 0f; // Time the polyhedron has stayed in the game over zone
    public float recognitionDelay = 2f; // Time window after being shot before it can trigger game over

    public bool recentlyShot = true; // Flag to indicate if the polyhedron has been recently shot
    public float shotTimer = 0f; // Timer to track time since it was shot

    private bool isMerging = false;
    private Rigidbody rb;

    void Start()
    {
        if (shooter == null)
        {
            shooter = FindObjectOfType<PolyhedronShooter>();
        }

        rb = GetComponent<Rigidbody>();

        // Apply initial color
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            Material material = new Material(renderer.material);
            material.color = color;
            renderer.material = material;
        }

        // Update the displayed value
        if (frontFace != null) frontFace.text = value.ToString();
        if (backFace != null) backFace.text = value.ToString();
    }

    void Update()
    {
        if (recentlyShot)
        {
            shotTimer += Time.deltaTime;
            if (shotTimer >= recognitionDelay)
            {
                recentlyShot = false;
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (isMerging) return;

        PolyhedronCollisionHandler otherPolyhedron = collision.gameObject.GetComponent<PolyhedronCollisionHandler>();

        if (otherPolyhedron != null && otherPolyhedron.value == value && !otherPolyhedron.isMerging)
        {
            isMerging = true;
            otherPolyhedron.isMerging = true;

            // Calculate the new value
            int newValue = value * 2;

            // Log the collision and new value
            Debug.Log($"Collision detected between two polyhedrons with value: {value}. Creating new polyhedron with value: {newValue}");

            // Apply striking force to both polyhedrons
            ApplyStrikingForce(collision, otherPolyhedron);

            // Create the new polyhedron
            Vector3 collisionPoint = collision.contacts[0].point;
            otherPolyhedron.GetComponent<Collider>().enabled = false;
            GetComponent<Collider>().enabled = false;

            StartCoroutine(MergePolyhedrons(collisionPoint, newValue, otherPolyhedron));
        }
    }

    void ApplyStrikingForce(Collision collision, PolyhedronCollisionHandler otherPolyhedron)
    {
        Vector3 collisionNormal = collision.contacts[0].normal;
        rb.AddForce(-collisionNormal * strikeForce, ForceMode.Impulse);
        otherPolyhedron.GetComponent<Rigidbody>().AddForce(collisionNormal * strikeForce, ForceMode.Impulse);
    }

    IEnumerator MergePolyhedrons(Vector3 collisionPoint, int newValue, PolyhedronCollisionHandler otherPolyhedron)
    {
        // Add visual effect for merging (optional)
        yield return new WaitForSeconds(0.1f); // Short delay to ensure proper merging

        // Add score
        ScoreManager.instance.AddScore(newValue);

        // Create the new polyhedron
        GameObject newPolyhedron = shooter.CreatePolyhedron(collisionPoint, newValue);

        // Apply outward force to the new polyhedron
        ApplyOutwardForce(newPolyhedron);

        // Destroy the two original polyhedrons
        Destroy(gameObject);
        Destroy(otherPolyhedron.gameObject);

        // Log the creation of the new polyhedron
        Debug.Log($"New polyhedron with value {newValue} created at position {collisionPoint}");
    }

    void ApplyOutwardForce(GameObject polyhedron)
    {
        Rigidbody rb = polyhedron.GetComponent<Rigidbody>();
        if (rb != null)
        {
            Vector3 outwardForce = Random.onUnitSphere * shooter.explosionForce; // Use explosionForce from shooter
            rb.AddForce(outwardForce, ForceMode.Impulse);
            Debug.Log($"Applying outward force: {outwardForce}");
        }
    }

    void FixedUpdate()
    {
        if (rb.velocity.magnitude > 0.1f)
        {
            ApplyMagnetism();
        }

        ApplyStickiness();
    }

    void ApplyMagnetism()
    {
        PolyhedronCollisionHandler[] polyhedrons = FindObjectsOfType<PolyhedronCollisionHandler>();

        foreach (PolyhedronCollisionHandler otherPolyhedron in polyhedrons)
        {
            if (otherPolyhedron != this && otherPolyhedron.value == value)
            {
                Vector3 direction = otherPolyhedron.transform.position - transform.position;
                float distance = direction.magnitude;

                if (distance > 0.1f && distance < 5f) // Apply force within a certain range
                {
                    Vector3 force = direction.normalized * (magnetismStrength / distance);
                    rb.AddForce(force);
                }
            }
        }
    }

    void ApplyStickiness()
    {
        PolyhedronCollisionHandler[] polyhedrons = FindObjectsOfType<PolyhedronCollisionHandler>();

        foreach (PolyhedronCollisionHandler otherPolyhedron in polyhedrons)
        {
            if (otherPolyhedron != this)
            {
                Vector3 direction = otherPolyhedron.transform.position - transform.position;
                float distance = direction.magnitude;

                if (distance > 0.1f && distance < stickinessRange) // Apply force within a certain range
                {
                    Vector3 force = direction.normalized * (stickinessStrength / distance);
                    rb.AddForce(force);
                }
            }
        }
    }
}
