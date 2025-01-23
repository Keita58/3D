using UnityEngine;

public class CameraShenanigans : MonoBehaviour
{
    [SerializeField] private Transform _FollowTarget;
    [SerializeField] private Transform _Camera;
    [Range(2f,10f)][SerializeField] private float _DelayFactor = 5f;

    private void LateUpdate()
    {
        if ( _FollowTarget.gameObject.activeInHierarchy)
        {
            transform.localRotation = Quaternion.Lerp(transform.localRotation, _FollowTarget.localRotation, Time.deltaTime * _DelayFactor);
        }
        else
        {
            //transform.localRotation = Quaternion.Lerp(transform.localRotation, _Camera.localRotation, Time.deltaTime * _DelayFactor);
        }
    }
}
