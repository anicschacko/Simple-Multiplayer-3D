using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform _target;

    [SerializeField] private float _smoothSpeed = 0.125f;
    [SerializeField] private Vector3 _offset;

    private void FixedUpdate()
    {
        var position = transform.position;
        Vector3 desiredPosition = _target.position + _offset;
        Vector3 smoothedPosition = Vector3.Lerp(position, desiredPosition, _smoothSpeed);
        position = smoothedPosition;
        transform.position = position;

        transform.LookAt(_target);
    }
}
