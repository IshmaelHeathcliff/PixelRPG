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
        Vector2 _cameraHalfSize;

        void Awake()
        {
            _camera = GetComponent<Camera>();
            _cameraHalfSize = new Vector2(_camera.orthographicSize * _camera.aspect, _camera.orthographicSize);
        }

        void MoveCamera()
        {
            var playerPosition = this.SendQuery(new PlayerPositionQuery());
            var targetPosition = Vector3.Lerp(transform.position, playerPosition, Time.fixedDeltaTime * _speed);

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

            targetPosition.z = transform.position.z;
            transform.position = targetPosition;
        }

        void Start()
        {
        }

        void FixedUpdate()
        {
            MoveCamera();
        }

        public IArchitecture GetArchitecture()
        {
            return PixelRPG.Interface;
        }
    }
}