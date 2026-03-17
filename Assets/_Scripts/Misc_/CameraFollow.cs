using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset = new Vector3(0f, 1f, -10f);
    [SerializeField] private float smoothSpeed = 5f;
    [SerializeField] private bool followY = false;

    private void LateUpdate()
    {
        if (target == null) return;

        float targetY = followY ? target.position.y + offset.y : transform.position.y;

        Vector3 desiredPosition = new Vector3(
            target.position.x + offset.x,
            targetY,
            offset.z
        );

        transform.position = Vector3.Lerp(
            transform.position,
            desiredPosition,
            smoothSpeed * Time.deltaTime
        );
    }
}