using System;
using System.Collections;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Character
{
    public class EnemyMoveController : MonoBehaviour, IController
    {
        [SerializeField] float _speed;
        [SerializeField] float _detectRadius;
        [SerializeField] float _lostRadius;
        [SerializeField] float _freezeTime;

        Vector2 _direction;
        Animator _animator;
        Rigidbody2D _rigidbody;

        bool _isPlayerFound;
        bool _isFrozen;
        bool _isDisabled;

        public bool IsAttacking { get; set; }


        static readonly int X = Animator.StringToHash("X");
        static readonly int Y = Animator.StringToHash("Y");
        static readonly int Walking = Animator.StringToHash("Walking");

        void Face(Vector2 direction)
        {
            _animator.SetFloat(X, direction.x);
            _animator.SetFloat(Y, direction.y);
            _direction = direction.normalized;
        }

        public async void Freeze()
        {
            _isFrozen = true;
            await UniTask.Delay((int) (_freezeTime * 1000));
            _isFrozen = false;
        }

        static Vector2 RandomDirection()
        {
            float x = Random.Range(-1f, 1f);
            float y = Random.Range(-1f, 1f);
            return new Vector2(x, y);
        }

        async UniTask ChangeDirection()
        {
            while (true)
            {
                if (_isDisabled)
                {
                    break;
                }
                
                await UniTask.Delay((int) (Random.Range(1f, 3f) * 1000));
                
                if (_isPlayerFound)
                {
                    continue;
                }

                Face(RandomDirection());
            }
        }

        void Move()
        {
            if (IsAttacking)
            {
                return;
            }
            
            if (_isFrozen)
            {
                _animator.SetBool(Walking, false);
                _rigidbody.linearVelocity = Vector2.zero;
                return;
            }
            
            _animator.SetBool(Walking, true);
            if (_isPlayerFound)
            {
                Face(((Vector2) (this.SendQuery(new PlayerPositionQuery()) - transform.position)).normalized);
            }

            _rigidbody.linearVelocity = _direction * _speed;
        }

        void FindPlayer()
        {
            var playerPos = this.SendQuery(new PlayerPositionQuery());
            if (Vector2.Distance(playerPos, transform.position) < _detectRadius)
            {
                _isPlayerFound = true;
            }
            
            if(Vector2.Distance(playerPos, transform.position) > _lostRadius)
            {
                _isPlayerFound = false;
            }
        }

        void OnValidate()
        {
            if (_detectRadius > _lostRadius)
            {
                _lostRadius = _detectRadius;
            }
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        void FixedUpdate()
        {
            FindPlayer();
            Move();
        }

        async void OnEnable()
        {
            _animator = GetComponent<Animator>();
            _rigidbody = GetComponent<Rigidbody2D>();
            
            _isDisabled = false;
            await ChangeDirection();
        }

        void OnDisable()
        {
            _isDisabled = true;
        }

        public IArchitecture GetArchitecture()
        {
            return PixelRPG.Interface;
        }
    }
}