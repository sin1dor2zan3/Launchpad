using UnityEngine;

public class CollisionRotate : MonoBehaviour
{
    [Header("Rotation Target")]
    public Transform objectToRotate;

    [Header("Pivot Point")]
    public Transform pivotPoint; // assign an empty GameObject here

    [Header("Rotation Settings")]
    public float rotationAmount = 90f; // degrees
    public float rotationSpeed = 90f;  // degrees per second

    private bool shouldRotate = false;
    private float rotatedSoFar = 0f;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Pickable") && CompareTag("Interactable"))
        {
            shouldRotate = true;
            rotatedSoFar = 0f;
        }
    }

    private void Update()
    {
        if (shouldRotate && objectToRotate != null && pivotPoint != null)
        {
            float step = rotationSpeed * Time.deltaTime;

            // Prevent overshooting
            if (rotatedSoFar + step > rotationAmount)
            {
                step = rotationAmount - rotatedSoFar;
                shouldRotate = false;
            }

            // Rotate around pivot's position using its X-axis
            objectToRotate.RotateAround(pivotPoint.position, pivotPoint.up, step);

            rotatedSoFar += step;
        }
    }
}