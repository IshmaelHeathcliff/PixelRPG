using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

namespace Character.Enemy
{
    public class EnemyMoveController : MoveController
    {
        [SerializeField] float _speed;
        [SerializeField] float _attackSpeed;
        [SerializeField] float _detectRadius;
        [SerializeField] float _attackRadius;
        [SerializeField] float _idleTime;

        public float IdleTime => _idleTime;
        public float AttackRadius => _attackRadius;

        public static readonly int Idle = Animator.StringToHash("Idle");
        public static readonly int Chase = Animator.StringToHash("Chase");
        public static readonly int Patrol = Animator.StringToHash("Patrol");
        public static readonly int Attack = Animator.StringToHash("Attack");
        public static readonly int Hurt = Animator.StringToHash("Hurt");
        public static readonly int Dead = Animator.StringToHash("Dead");


        public void Move()
        {
            Rigidbody.linearVelocity = Direction * Speed;
        }

        public Vector2 DirectionToPlayer()
        {
            Vector3 playerPos = this.SendQuery(new PlayerPositionQuery());
            Vector2 direction = playerPos - transform.position;
            return direction;
        }

        public float SqrDistanceToPlayer()
        {
            return DirectionToPlayer().sqrMagnitude;
        }

        public bool FindPlayer()
        {
            Vector2 direction = DirectionToPlayer();
            if (direction.sqrMagnitude > _detectRadius * _detectRadius)
            {
                return false;
            }
            else
            {
                Face(direction);
                return true;
            }
        }

        public async UniTask AttackPlayer(CancellationToken ct)
        {
            CancellationToken combinedToken = CancellationTokenSource.CreateLinkedTokenSource(ct, this.GetCancellationTokenOnDestroy()).Token;
            Vector3 initialPosition = transform.position;
            Vector3 playerPosition = this.SendQuery(new PlayerPositionQuery());
            while (Vector2.Distance(playerPosition, transform.position) > 0.1f)
            {
                Rigidbody.linearVelocity = (playerPosition - transform.position).normalized * _attackSpeed;
                await UniTask.WaitForFixedUpdate(combinedToken);
            }


            while (Vector2.Distance(initialPosition, transform.position) > 0.1f)
            {
                Rigidbody.MovePosition(Vector2.Lerp(transform.position, initialPosition, _attackSpeed * Time.fixedDeltaTime));
                await UniTask.WaitForFixedUpdate(combinedToken);
            }

            Rigidbody.MovePosition(initialPosition);
        }

        public bool LosePlayer()
        {
            return SqrDistanceToPlayer() > _detectRadius * _detectRadius;
        }

        void Start()
        {
            Model.BindTransform(transform);
            Speed = _speed;
        }
    }
}
