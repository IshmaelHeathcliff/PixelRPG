using System;
using UnityEngine;

namespace Character
{
    public class CameraFollow : MonoBehaviour
    {
        [SerializeField] float speed = 5;
        [SerializeField] Transform leftBoundary;
        [SerializeField] Transform rightBoundary;
        [SerializeField] Transform upBoundary;
        [SerializeField] Transform downBoundary;

        Camera _camera;
        Transform _transform;
        Vector2 _cameraHalfSize;

        void Awake()
        {
            _camera = GetComponent<Camera>();
            _transform = GetComponent<Transform>();
            _cameraHalfSize = new Vector2(_camera.orthographicSize * _camera.aspect, _camera.orthographicSize);
        }

        void MoveCamera()
        {
            var playerPosition = GameManager.Instance.Player.transform.position;
            var targetPosition = Vector3.Lerp(_transform.position, playerPosition, Time.deltaTime * speed);

            if (targetPosition.x - _cameraHalfSize.x < leftBoundary.position.x)
            {
                targetPosition.x = leftBoundary.position.x + _cameraHalfSize.x;
            }

            if (targetPosition.x + _cameraHalfSize.x > rightBoundary.position.x)
            {
                targetPosition.x = rightBoundary.position.x - _cameraHalfSize.x;
            }

            if (targetPosition.y - _cameraHalfSize.y < downBoundary.position.y)
            {
                targetPosition.y = downBoundary.position.y + _cameraHalfSize.y;
            }

            if (targetPosition.y + _cameraHalfSize.y > upBoundary.position.y)
            {
                targetPosition.y = upBoundary.position.y - _cameraHalfSize.y;
            }

            targetPosition.z = _transform.position.z;
            _transform.position = targetPosition;
        }

        void Start()
        {
        }

        void Update()
        {
            MoveCamera();
        }
    }
}