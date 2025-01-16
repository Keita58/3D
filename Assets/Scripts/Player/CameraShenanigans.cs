using UnityEngine;

public class CameraShenanigans : MonoBehaviour
{
    [SerializeField] private Transform _FollowTarget;
    [Range(2f,10f)][SerializeField] private float _DelayFactor = 5f;

    private void LateUpdate()
    {
        transform.localRotation = Quaternion.Lerp(transform.localRotation, _FollowTarget.localRotation, Time.deltaTime*_DelayFactor);
    }
}
