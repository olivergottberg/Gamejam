using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform target;

    private float startingCameraX;
    private float startingTargetX;

    private void Start()
    {
        startingCameraX = transform.position.x;
        startingTargetX = target.position.x;
    }

    private void LateUpdate()
    {
        Vector3 cameraPosition = transform.position;
        cameraPosition.x =
            startingCameraX + (target.position.x - startingTargetX);

        transform.position = cameraPosition;
    }
}