using QFramework;
using UnityEngine;

namespace Character
{
    public class CameraFollow : MonoBehaviour, IController
    {
        [SerializeField] float _speed = 5;
        [SerializeField] Transform _leftBoundary;
        [SerializeField] Transform _rightBoundary;
        [SerializeField] Transform _upBoundary;
        [SerializeField] Transform _downBoundary;

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
            var playerPosition = this.SendQuery(new PlayerPositionQuery());
            var targetPosition = Vector3.Lerp(_transform.position, playerPosition, Time.deltaTime * _speed);

            if (targetPosition.x - _cameraHalfSize.x < _leftBoundary.position.x)
            {
                targetPosition.x = _leftBoundary.position.x + _cameraHalfSize.x;
            }

            if (targetPosition.x + _cameraHalfSize.x > _rightBoundary.position.x)
            {
                targetPosition.x = _rightBoundary.position.x - _cameraHalfSize.x;
            }

            if (targetPosition.y - _cameraHalfSize.y < _downBoundary.position.y)
            {
                targetPosition.y = _downBoundary.position.y + _cameraHalfSize.y;
            }

            if (targetPosition.y + _cameraHalfSize.y > _upBoundary.position.y)
            {
                targetPosition.y = _upBoundary.position.y - _cameraHalfSize.y;
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

        public IArchitecture GetArchitecture()
        {
            return PixelRPG.Interface;
        }
    }
}