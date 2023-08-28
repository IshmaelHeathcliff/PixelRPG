using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public float speed = 5;
    public Transform leftBoundary;
    public Transform rightBoundary;
    public Transform upBoundary;
    public Transform downBoundary;

    Camera _camera;
    Transform _transform;

    void Awake()
    {
        _camera = GetComponent<Camera>();
        _transform = GetComponent<Transform>();
    }

    void OnEnable()
    {
        var targetPosition = Character.Character.Instance.transform.position;
        targetPosition.z = _transform.position.z;
        _transform.position = targetPosition;
    }

    void MoveCamera()
    {
        float cameraHalfWidth = _camera.orthographicSize * _camera.aspect;
        float cameraHalfHeight = _camera.orthographicSize;

        var cameraPosition = _transform.position;
        var playerPosition = Character.Character.Instance.transform.position;
        var targetPosition = Vector3.Lerp(cameraPosition, playerPosition, Time.deltaTime * speed);

        if (targetPosition.x - cameraHalfWidth < leftBoundary.transform.position.x)
        {
            targetPosition.x = leftBoundary.transform.position.x + cameraHalfWidth;
        }

        if (targetPosition.x + cameraHalfWidth > rightBoundary.transform.position.x)
        {
            targetPosition.x = rightBoundary.transform.position.x - cameraHalfWidth;
        }

        if (targetPosition.y - cameraHalfHeight < downBoundary.transform.position.y)
        {
            targetPosition.y = downBoundary.transform.position.y + cameraHalfHeight;
        }

        if (targetPosition.y + cameraHalfHeight > upBoundary.transform.position.y)
        {
            targetPosition.y = upBoundary.transform.position.y - cameraHalfHeight;
        }

        targetPosition.z = cameraPosition.z;

        _transform.position = targetPosition;
    }

    void Update()
    {
        MoveCamera();
    }
}