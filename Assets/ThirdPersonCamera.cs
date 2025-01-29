using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    [SerializeField] private Transform _Pivot;
    [SerializeField] private LayerMask _CameraCollisionMask;

    public void UpdateCamera(Vector2 rotationXY, float distance)
    {
        transform.position = _Pivot.transform.position;
        transform.localRotation = Quaternion.Euler(rotationXY.y, rotationXY.x, 0);
        if(Physics.Raycast(transform.position, -transform.forward, out RaycastHit hit, distance, _CameraCollisionMask))
        {
            transform.position = hit.point + transform.forward * 0.1f;
        }
        else
        {
            transform.position -= transform.forward * distance;
        }
    }
}
