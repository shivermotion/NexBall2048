using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PolyTypes;
using Property_Attributes;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class PolyhedronShooter : MonoBehaviour
{
    [SerializeField] private int nextPolyIndex = -1;
    [SerializeField, ReadOnly] private string nextPolySize;
    [Space(20)]
    
    public GameObject polyhedronPrefab;
    public Transform shootPoint;
    public GameObject arrowPrefab; // Reference to the arrow prefab
    
    public GameObject bombPrefab; // Reference to the bomb prefab
    private bool shotCooldown = false;
    private GameObject previewPolyhedron;
    private GameObject arrowInstance;
    
    public float planeWidth = 14f; // Width of the plane

    [FormerlySerializedAs("polyColorData")] public PolyDataSO polyData;

    private int currentMaxPolyIndex => polyData.initialMaxPolyValue;

    private void OnValidate()
    {
        nextPolySize = nextPolyIndex < 0 ? "Random" : ((int)Mathf.Pow(2, nextPolyIndex+1)).ToString();
    }
    

    void Start()
    {
        // Apply the gravity scale globally
        Physics.gravity = new Vector3(0, -9.81f * polyData.gravityScale, 0);

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
            //Debug.Log("Spacebar pressed");
            ShootPolyhedron();
        }

        // Update the horizontal position of the shoot point
        float horizontal = Input.GetAxis("Horizontal");
        Vector3 position = shootPoint.position;
        position.x += horizontal * Time.deltaTime * polyData.horizontalSpeed; // Use the adjustable speed

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
        yield return new WaitForSeconds(polyData.instantiationDelay);

        if (previewPolyhedron != null)
        {
            Destroy(previewPolyhedron);
        }

        // Select a random value
        int value = nextPolyIndex >= 0 ? nextPolyIndex : Random.Range(0,currentMaxPolyIndex);

        // Adjust the shoot point position to ensure it spawns above the plane
        shootPoint.position = new Vector3(shootPoint.position.x, polyData.GetSize(value) + 0.2f, shootPoint.position.z);

        // Instantiate the polyhedron
        Vector3 offsetPosition = shootPoint.position - new Vector3(0, 0, 1); // Slightly behind the shoot point
        previewPolyhedron = CreateNewPoly(value,false, offsetPosition).gameObject;
    }

    public PolyhedronCollisionHandler CreateNewPoly(int value, bool rbEnabled, Vector3 position)
    {
        Color color = polyData.GetColor(value);
        float baseSize = 0.8f;
        float scaleFactor = polyData.GetSize(value);
        
        GameObject newPolyhedron = Instantiate(polyhedronPrefab, position, Quaternion.identity);

        Rigidbody rb = newPolyhedron.GetComponent<Rigidbody>();
        Collider collider = newPolyhedron.GetComponent<Collider>();
        
        rb.isKinematic = !rbEnabled;
        
        rb.mass = polyData.polyhedronMass; // Set mass
        rb.drag = polyData.polyhedronDrag; // Set drag
        rb.angularDrag = polyData.polyhedronAngularDrag; // Set angular drag
        
        PhysicMaterial bouncyMaterial = new PhysicMaterial();
        bouncyMaterial.bounciness = polyData.polyhedronBounciness; // Set bounciness
        bouncyMaterial.frictionCombine = PhysicMaterialCombine.Minimum;
        bouncyMaterial.bounceCombine = PhysicMaterialCombine.Maximum;
        collider.material = bouncyMaterial;
        
        // Set the color
        Renderer renderer = newPolyhedron.GetComponent<Renderer>();
        if (renderer != null)
        {
            Material material = new Material(renderer.material);
            material.color = color;
            renderer.material = material;
        }

        // Set the size based on the value
        newPolyhedron.transform.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);

        // Set the value and color in the PolyhedronCollisionHandler
        PolyhedronCollisionHandler handler = newPolyhedron.GetComponent<PolyhedronCollisionHandler>();
        handler.index = value;
        handler.color = color;

        return newPolyhedron.GetComponent<PolyhedronCollisionHandler>();
    }

    public GameObject CreateMergedPolyhedron(Vector3 position, int value)
    {
        GameObject newPolyhedron = CreateNewPoly(value, true, position).gameObject;

        // Apply wiggle effect
        StartCoroutine(WiggleEffect(newPolyhedron, polyData.GetSize(value)));

        // Get the PolyhedronCollisionHandler component 
        var collisionHandler = newPolyhedron.GetComponent<PolyhedronCollisionHandler>();

        // Set the value on the front and back faces
        collisionHandler.frontFace.text = value.ToString();
        collisionHandler.backFace.text = value.ToString();
        
        // Check if the value is 2048 or higher
        if (value >= 2048)
        {
            GameManager.instance.IncrementBombCounter();
            //Debug.Log($"Polyhedron with value {value} created. Incrementing bomb counter.");
        }

        collisionHandler.canTriggerGameOver = true;

        return newPolyhedron;
    }

    void ShootPolyhedron()
    {
        if (shotCooldown)
        {
            //Debug.Log("Shot on cooldown");
            return;
        }
        if (previewPolyhedron != null)
        {
            //Debug.Log("Shooting Polyhedron at position: " + previewPolyhedron.transform.position);
            Rigidbody rb = previewPolyhedron.GetComponent<Rigidbody>();
            if (rb != null)
            {
                previewPolyhedron.GetComponent<MeshCollider>().enabled = true;
                rb.isKinematic = false; // Reactivate physics
                Vector3 force = Vector3.forward * polyData.shootForce;
                rb.velocity = Vector3.zero; // Reset velocity
                rb.AddForce(force, ForceMode.Impulse);
                //Debug.Log("Force applied to Polyhedron: " + force);

                // Reset the recently shot flag and timer
                PolyhedronCollisionHandler handler = previewPolyhedron.GetComponent<PolyhedronCollisionHandler>();
                handler.OnShot();
            }
            StartCoroutine(CoolDown());
            previewPolyhedron = null; // Reset the preview polyhedron
        }

        // Load the next polyhedron to be shot
        LoadNextPolyhedron();
    }

    // =======================================
    // Coroutines
    // =======================================

    IEnumerator CoolDown()
    {
        shotCooldown = true;
        yield return new WaitForSeconds(polyData.instantiationDelay);
        shotCooldown = false;
    }

    IEnumerator WiggleEffect(GameObject polyhedron, float originalScale)
    {
        Vector3 originalScaleVector = new Vector3(originalScale, originalScale, originalScale);
        float elapsedTime = 0f;

        while (elapsedTime < polyData.wiggleDuration)
        {
            if (!polyhedron)
            {
                yield break;
            }
            float scale = originalScale + Mathf.Sin(elapsedTime * Mathf.PI * 4) * polyData.wiggleMagnitude;
            polyhedron.transform.localScale = new Vector3(scale, scale, scale);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        polyhedron.transform.localScale = originalScaleVector; // Reset to original scale
    }

    public void SpawnBomb(GameObject bombPrefab)
    {
        if (previewPolyhedron != null)
        {
            Destroy(previewPolyhedron);
        }

        previewPolyhedron = Instantiate(bombPrefab, shootPoint.position, Quaternion.identity);
        //Debug.Log("Bomb spawned on the shoot point.");
    }
}
