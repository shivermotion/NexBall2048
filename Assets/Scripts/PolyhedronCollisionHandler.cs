using UnityEngine;
using TMPro;
using System.Collections;
using Extension_Methods;

public class PolyhedronCollisionHandler : MonoBehaviour
{
    [SerializeField] private GameObject mergeParticlesPrefab;
    public int index;
    public int value => index.IndexToPolyValue();
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
    public float explosionForce = 20f; // Force of the explosion applied to nearby polyhedrons
    public float explosionRange = 5f; // Range of the explosion effect
    [HideInInspector]
    public float stayTime = 0f; // Time the polyhedron has stayed in the game over zone
    public float recognitionDelay = 2f; // Time window after being shot before it can trigger game over

    //public bool recentlyShot = true; // Flag to indicate if the polyhedron has been recently shot
    public bool hasBeenShot = false;
    public bool canTriggerGameOver = false;

    private bool isMerging = false;
    private Rigidbody rb;

    public System.Action OnCanTriggerGameOver = default;

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
        if (frontFace != null) frontFace.text = index.IndexToPolyValue().ToString();
        if (backFace != null) backFace.text = index.IndexToPolyValue().ToString();
    }

    public void OnShot()
    {
        if (hasBeenShot) return;
        hasBeenShot = true;
        StartCoroutine(ShotTimer());
    }

    IEnumerator ShotTimer()
    {
        yield return new WaitForSeconds(recognitionDelay);
        canTriggerGameOver = true;
        OnCanTriggerGameOver?.Invoke();
    }

    void OnCollisionEnter(Collision collision)
    {
        // Check if the collision is with a bomb
        if (collision.gameObject.CompareTag("Bomb"))
        {
            // Apply explosion effects
            Explode(collision.contacts[0].point);

            // Destroy the bomb
            Destroy(collision.gameObject);

            return;
        }

        if (isMerging) return;

        PolyhedronCollisionHandler otherPolyhedron = collision.gameObject.GetComponent<PolyhedronCollisionHandler>();

        if (otherPolyhedron != null && otherPolyhedron.index == index && !otherPolyhedron.isMerging)
        {
            isMerging = true;
            otherPolyhedron.isMerging = true;

            // Calculate the new value
            int newValue = index + 1;

            // Log the collision and new value
            //Debug.Log($"Collision detected between two polyhedrons with value: {value}. Creating new polyhedron with value: {newValue}");

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

        // Instantiate the shattered glass effect
        GameObject shatteredGlass = Instantiate(mergeParticlesPrefab, collisionPoint, Quaternion.identity);
        ParticleSystem particleSystem = shatteredGlass.GetComponent<ParticleSystem>();
        if (particleSystem != null)
        {
            particleSystem.Play();
        }

        // Add score
        ScoreManager.instance.AddScore(newValue.IndexToPolyValue(), collisionPoint);

        // Create the new polyhedron
        GameObject newPolyhedron = shooter.CreateMergedPolyhedron(collisionPoint, newValue);

        // Enable the mesh collider for the new polyhedron
        newPolyhedron.GetComponent<MeshCollider>().enabled = true;

        // Apply outward force to the new polyhedron
        ApplyOutwardForce(newPolyhedron);

        // Notify the GameManager about the new polyhedron value
        GameManager.instance.IncrementPolyhedronCount(newValue);

        // Destroy the two original polyhedrons
        Destroy(gameObject);
        Destroy(otherPolyhedron.gameObject);

        // Log the creation of the new polyhedron
        //Debug.Log($"New polyhedron with value {newValue} created at position {collisionPoint}");

        // Destroy the particle system after it finishes
        Destroy(shatteredGlass, particleSystem.main.duration);
    }

    void ApplyOutwardForce(GameObject polyhedron)
    {
        Rigidbody rb = polyhedron.GetComponent<Rigidbody>();
        if (rb != null)
        {
            Vector3 outwardForce = Random.onUnitSphere * shooter.polyData.explosionForce; // Use explosionForce from shooter
            rb.AddForce(outwardForce, ForceMode.Impulse);
            //Debug.Log($"Applying outward force: {outwardForce}");
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
            if (otherPolyhedron != this && otherPolyhedron.index == index)
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

    void Explode(Vector3 explosionPoint)
{
    Collider[] colliders = Physics.OverlapSphere(explosionPoint, explosionRange);
    foreach (Collider hit in colliders)
    {
        Rigidbody rb = hit.GetComponent<Rigidbody>();
        if (rb != null)
        {
            Vector3 explosionDirection = hit.transform.position - explosionPoint;
            float explosionDistance = explosionDirection.magnitude;
            float explosionEffect = 1 - (explosionDistance / explosionRange);
            rb.AddForce(explosionDirection.normalized * explosionForce * explosionEffect, ForceMode.Impulse);
            //Debug.Log($"Applying explosion force to {hit.name}");
        }
    }

    // Add visual effect for the explosion (optional)
    // Example: Instantiate explosion particles
    // GameObject explosionEffect = Instantiate(shooter.explosionEffectPrefab, explosionPoint, Quaternion.identity);
    // Destroy(explosionEffect, 2f); // Destroy the explosion effect after 2 seconds
}

}
