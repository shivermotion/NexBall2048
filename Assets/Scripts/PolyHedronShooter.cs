using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class PolyhedronShooter : MonoBehaviour
{
    public GameObject polyhedronPrefab;
    public Transform shootPoint;
    public GameObject arrowPrefab; // Reference to the arrow prefab
    public GameObject shatteredGlassPrefab;
    public float shootForce = 30f;
    public float horizontalSpeed = 20f; // Adjustable horizontal speed
    public float instantiationDelay = 0.2f; // Delay before instantiating the next polyhedron
    public float planeWidth = 14f; // Width of the plane
    public float planeHeight = 10f; // Height of the plane
    public float popUpVelocity = 2f; // Velocity for the new polyhedron to pop up
    public float wiggleDuration = 0.5f; // Duration of the wiggle effect
    public float wiggleMagnitude = 0.1f; // Magnitude of the wiggle effect
    public float spinTorque = 5f; // Torque for the spin effect
    public float gravityScale = 2f; // Gravity scale to make objects fall faster
    public float polyhedronMass = 0.5f; // Mass of the polyhedrons
    public float polyhedronDrag = 0.1f; // Drag of the polyhedrons
    public float polyhedronAngularDrag = 0.05f; // Angular drag of the polyhedrons
    public float polyhedronBounciness = 0.9f; // Bounciness of the polyhedrons
    public float explosionForce = 5f; // Force of the explosion applied to new polyhedrons
    public GameObject bombPrefab; // Reference to the bomb prefab
    private bool shotCooldown = false;
    private GameObject previewPolyhedron;
    private GameObject arrowInstance;

    private Dictionary<int, Color> valueColorMap = new Dictionary<int, Color>()
    {
        { 2, Color.red },
        { 4, Color.green },
        { 8, Color.blue },
        { 16, Color.yellow },
        { 32, Color.magenta },
        { 64, Color.cyan },
        { 128, new Color(1f, 0.5f, 0f) }, // Orange
        { 256, new Color(0.5f, 0f, 1f) }, // Purple
        { 512, new Color(0.75f, 1f, 0f) }, // Lime
        { 1024, new Color(0f, 0.75f, 1f) }, // Sky Blue
        { 2048, new Color(1f, 0f, 0.75f) },  // Pink
    };

    private List<int> possibleValues = new List<int>()
    {
        2, 4, 8, 16, 32, 64,
        128, 256, 512, 1024, 2048,
    };

    // =======================================
    // Unity Standard Methods
    // =======================================

    void Start()
    {
        // Apply the gravity scale globally
        Physics.gravity = new Vector3(0, -9.81f * gravityScale, 0);

        // Instantiate the arrow
        arrowInstance = Instantiate(arrowPrefab);
        arrowInstance.GetComponent<ArrowController>().shootPoint = shootPoint;

        LoadNextPolyhedron();
    }

    void Update()
    {
        // Check for spacebar press to shoot the polyhedron
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Spacebar pressed");
            ShootPolyhedron();
        }

        // Update the horizontal position of the shoot point
        float horizontal = Input.GetAxis("Horizontal");
        Vector3 position = shootPoint.position;
        position.x += horizontal * Time.deltaTime * horizontalSpeed; // Use the adjustable speed

        // Clamp the position to the width of the plane
        float halfPlaneWidth = planeWidth / 2;
        position.x = Mathf.Clamp(position.x, -halfPlaneWidth, halfPlaneWidth);

        shootPoint.position = position;

        // Update the position of the preview polyhedron to follow the shoot point
        if (previewPolyhedron != null)
        {
            previewPolyhedron.transform.position = shootPoint.position;
        }

        // For dev purposes: Add a bomb with the "B" key
        if (Input.GetKeyDown(KeyCode.B))
        {
            GameManager.instance.IncrementBombCounter();
        }
    }

    // =======================================
    // Polyhedron Handling Methods
    // =======================================

    void LoadNextPolyhedron()
    {
        StartCoroutine(InstantiateNextPolyhedron());
    }

    IEnumerator InstantiateNextPolyhedron()
    {
        yield return new WaitForSeconds(instantiationDelay);

        if (previewPolyhedron != null)
        {
            Destroy(previewPolyhedron);
        }

        // Select a random value
        int randomValue = possibleValues[Random.Range(0, possibleValues.Count)];

        // Get the color for the selected value
        Color color = valueColorMap[randomValue];

        // Calculate the scale factor based on the value, with each increment by 0.1
        float baseSize = 0.8f;
        int index = possibleValues.IndexOf(randomValue);
        float scaleFactor = baseSize + (index * 0.1f);

        // Adjust the shoot point position to ensure it spawns above the plane
        shootPoint.position = new Vector3(shootPoint.position.x, scaleFactor + 0.2f, shootPoint.position.z);

        // Instantiate the polyhedron
        Vector3 offsetPosition = shootPoint.position - new Vector3(0, 0, 1); // Slightly behind the shoot point
        previewPolyhedron = Instantiate(polyhedronPrefab, offsetPosition, Quaternion.identity);

        // Apply physics properties to the preview polyhedron
        Rigidbody rb = previewPolyhedron.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.mass = polyhedronMass; // Set mass
            rb.drag = polyhedronDrag; // Set drag
            rb.angularDrag = polyhedronAngularDrag; // Set angular drag

            // Add bounciness
            Collider collider = previewPolyhedron.GetComponent<Collider>();
            if (collider != null)
            {
                PhysicMaterial bouncyMaterial = new PhysicMaterial();
                bouncyMaterial.bounciness = polyhedronBounciness; // Set bounciness
                bouncyMaterial.frictionCombine = PhysicMaterialCombine.Minimum;
                bouncyMaterial.bounceCombine = PhysicMaterialCombine.Maximum;
                collider.material = bouncyMaterial;
            }
        }

        // Get the PolyhedronCollisionHandler component
        var collisionHandler = previewPolyhedron.GetComponent<PolyhedronCollisionHandler>();

        // Set the value on the front and back faces
        collisionHandler.frontFace.text = randomValue.ToString();
        collisionHandler.backFace.text = randomValue.ToString();

        // Set the polyhedron's value, color, and size
        SetPolyhedronAttributes(previewPolyhedron, randomValue, color, scaleFactor);

        Debug.Log("Preview Polyhedron instantiated at: " + previewPolyhedron.transform.position + " with value: " + randomValue);
    }

    public GameObject CreatePolyhedron(Vector3 position, int value)
    {
        Color color = valueColorMap[value];
        float baseSize = 0.8f;
        int index = possibleValues.IndexOf(value);
        float scaleFactor = baseSize + (index * 0.1f);

        GameObject newPolyhedron = Instantiate(polyhedronPrefab, position, Quaternion.identity);
        SetPolyhedronAttributes(newPolyhedron, value, color, scaleFactor);

        // Apply random direction pop-up effect
        Rigidbody rb = newPolyhedron.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.mass = polyhedronMass; // Set mass
            rb.drag = polyhedronDrag; // Set drag
            rb.angularDrag = polyhedronAngularDrag; // Set angular drag

            Vector3 randomDirection = Random.onUnitSphere;
            randomDirection.y = Mathf.Abs(randomDirection.y); // Ensure the direction is upwards
            rb.AddForce(randomDirection * popUpVelocity, ForceMode.Impulse);
            Debug.Log($"Applying pop-up force: {randomDirection * popUpVelocity}");

            // Apply random spin
            Vector3 randomTorque = Random.insideUnitSphere * spinTorque;
            rb.AddTorque(randomTorque, ForceMode.Impulse);
            Debug.Log($"Applying spin torque: {randomTorque}");

            // Add bounciness
            Collider collider = newPolyhedron.GetComponent<Collider>();
            if (collider != null)
            {
                PhysicMaterial bouncyMaterial = new PhysicMaterial();
                bouncyMaterial.bounciness = polyhedronBounciness; // Set bounciness
                bouncyMaterial.frictionCombine = PhysicMaterialCombine.Minimum;
                bouncyMaterial.bounceCombine = PhysicMaterialCombine.Maximum;
                collider.material = bouncyMaterial;
            }
        }

        // Apply wiggle effect
        StartCoroutine(WiggleEffect(newPolyhedron, scaleFactor));

        // Get the PolyhedronCollisionHandler component 
        var collisionHandler = newPolyhedron.GetComponent<PolyhedronCollisionHandler>();

        // Set the value on the front and back faces
        collisionHandler.frontFace.text = value.ToString();
        collisionHandler.backFace.text = value.ToString();
        collisionHandler.recentlyShot = true; // Set as recently shot
        collisionHandler.shotTimer = 0f; // Reset shot timer

        // Check if the value is 2048 or higher
        if (value >= 2048)
        {
            GameManager.instance.IncrementBombCounter();
            Debug.Log($"Polyhedron with value {value} created. Incrementing bomb counter.");
        }

        return newPolyhedron;
    }

    void SetPolyhedronAttributes(GameObject polyhedron, int value, Color color, float scaleFactor)
    {
        // Set the color
        Renderer renderer = polyhedron.GetComponent<Renderer>();
        if (renderer != null)
        {
            Material material = new Material(renderer.material);
            material.color = color;
            renderer.material = material;
        }

        // Set the size based on the value
        polyhedron.transform.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);

        // Set the value and color in the PolyhedronCollisionHandler
        PolyhedronCollisionHandler handler = polyhedron.GetComponent<PolyhedronCollisionHandler>();
        handler.value = value;
        handler.color = color;
        handler.recentlyShot = true; // Set as recently shot
        handler.shotTimer = 0f; // Reset shot timer
    }

    void ShootPolyhedron()
    {
        if (shotCooldown)
        {
            Debug.Log("Shot on cooldown");
            return;
        }
        if (previewPolyhedron != null)
        {
            Debug.Log("Shooting Polyhedron at position: " + previewPolyhedron.transform.position);
            Rigidbody rb = previewPolyhedron.GetComponent<Rigidbody>();
            if (rb != null)
            {
                previewPolyhedron.GetComponent<MeshCollider>().enabled = true;
                rb.isKinematic = false; // Reactivate physics
                Vector3 force = Vector3.forward * shootForce;
                rb.velocity = Vector3.zero; // Reset velocity
                rb.AddForce(force, ForceMode.Impulse);
                Debug.Log("Force applied to Polyhedron: " + force);

                // Reset the recently shot flag and timer
                PolyhedronCollisionHandler handler = previewPolyhedron.GetComponent<PolyhedronCollisionHandler>();
                handler.recentlyShot = true;
                handler.shotTimer = 0f;
            }
            StartCoroutine(CoolDown());
            previewPolyhedron = null; // Reset the preview polyhedron
        }

        // Load the next polyhedron to be shot
        LoadNextPolyhedron();
    }

    // =======================================
    // Bomb Handling Methods
    // =======================================

    public void SpawnBomb(GameObject bombPrefab)
    {
        if (previewPolyhedron != null)
        {
            Destroy(previewPolyhedron);
        }

        previewPolyhedron = Instantiate(bombPrefab, shootPoint.position, Quaternion.identity);
        Debug.Log("Bomb spawned on the shoot point.");
    }

    // =======================================
    // Coroutines
    // =======================================

    IEnumerator CoolDown()
    {
        shotCooldown = true;
        yield return new WaitForSeconds(instantiationDelay);
        shotCooldown = false;
    }

    IEnumerator WiggleEffect(GameObject polyhedron, float originalScale)
    {
        Vector3 originalScaleVector = new Vector3(originalScale, originalScale, originalScale);
        float elapsedTime = 0f;

        while (elapsedTime < wiggleDuration)
        {
            float scale = originalScale + Mathf.Sin(elapsedTime * Mathf.PI * 4) * wiggleMagnitude;
            polyhedron.transform.localScale = new Vector3(scale, scale, scale);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        polyhedron.transform.localScale = originalScaleVector; // Reset to original scale
    }
}
