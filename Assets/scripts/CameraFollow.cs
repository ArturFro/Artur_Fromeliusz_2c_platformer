using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;

    [Header("Smooth")]
    [SerializeField] private float smoothTime = 0.12f;

    [Header("Offset")]
    [SerializeField] private Vector3 offset = new Vector3(0f, 0f, -10f);

    [Header("Pixel Snap")]
    [SerializeField] private bool pixelSnap = false;
    [SerializeField] private float pixelsPerUnit = 16f;

    private Vector3 velocity;


    private void LateUpdate()
    {
        if (target == null)
        {
            return;
        }


        Vector3 targetPosition = target.position + offset;


        // Smooth camera movement
        Vector3 newPosition = Vector3.SmoothDamp(
            transform.position,
            targetPosition,
            ref velocity,
            smoothTime
        );


        // Opcjonalne przy pixel arcie.
        if (pixelSnap)
        {
            newPosition.x =
                Mathf.Round(newPosition.x * pixelsPerUnit)
                / pixelsPerUnit;

            newPosition.y =
                Mathf.Round(newPosition.y * pixelsPerUnit)
                / pixelsPerUnit;
        }


        transform.position = newPosition;
    }
}