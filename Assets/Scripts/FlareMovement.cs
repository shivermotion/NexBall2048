using UnityEngine;

public class FlareMovement : MonoBehaviour
{
    public Transform startPoint; // Starting point of the flare
    public Transform endPoint; // Ending point of the flare
    public float speed = 2f; // Speed of the flare
    public Transform gameOverZoneScaler; // Reference to the game over zone scaler

    private bool movingToEnd = true;

    void Start()
    {
        UpdatePositions();
        transform.position = startPoint.position;
    }

    void Update()
    {
        UpdatePositions();

        float step = speed * Time.deltaTime;
        if (movingToEnd)
        {
            transform.position = Vector3.MoveTowards(transform.position, endPoint.position, step);
             transform.position = new Vector3(transform.position.x, transform.position.y, endPoint.position.z);
            if (Vector3.Distance(transform.position, endPoint.position) < 0.1f)
                movingToEnd = false;
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, startPoint.position, step);
            transform.position = new Vector3(transform.position.x, transform.position.y, startPoint.position.z);
            if (Vector3.Distance(transform.position, startPoint.position) < 0.1f)
                movingToEnd = true;
        }
    }

    void UpdatePositions()
    {
        if (gameOverZoneScaler == null) return;

        // Get the bounds of the GameOverZoneScaler
        Collider collider = gameOverZoneScaler.GetComponent<Collider>();
        if (collider == null) return;

        Bounds bounds = collider.bounds;

        // Calculate the top left and top right vertices
        Vector3 topLeft = new Vector3(bounds.min.x, bounds.max.y, bounds.max.z);
        Vector3 topRight = new Vector3(bounds.max.x, bounds.max.y, bounds.max.z);

        startPoint.position = topLeft;
        endPoint.position = topRight;

      
    }
}
