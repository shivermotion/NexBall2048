using UnityEngine;

public class ArrowController : MonoBehaviour
{
    public Transform shootPoint; // Reference to the shoot point
    public float arrowLength = 10f; // Length of the arrow

    void Update()
    {
        if (shootPoint != null)
        {
            // Position the arrow at the shoot point
            transform.position = shootPoint.position;

            // Rotate the arrow to match the shoot point's rotation and adjust to extend along the Z-axis
            transform.rotation = shootPoint.rotation;
            transform.Rotate(90, 0, 0); // Adjust rotation to point along the Z-axis

            // Adjust the length of the arrow
            Vector3 scale = transform.localScale;
            scale.z = arrowLength; // Use the Z-axis for length
            transform.localScale = scale;
        }
    }
}
