using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Character.Damage
{
    [RequireComponent(typeof(Collider2D), typeof(SpriteRenderer))]
    public class PlayerAttacker : Attacker
    {
        [SerializeField] float _animationTime;
        [SerializeField] float _animationSpeed;

        Collider2D _collider;
        SpriteRenderer _renderer;

        void Awake()
        {
            _collider = GetComponent<Collider2D>();
            _renderer = GetComponent<SpriteRenderer>();
        }


        void Start()
        {
            Attack().Forget();
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            Damageable damageable = other.GetComponent<Damageable>();

            if (damageable == null || damageable.CompareTag("Player"))
            {
                return;
            }

            var keywords = new List<string>()
            {
                "Damage", "Attack", "Physical",
            };


            var damage = new AttackDamage(this, damageable, keywords, DamageType.Physical, 10, 1, 1);
            damage.Apply();
        }

        protected override async UniTask Play()
        {
            float leftTime = _animationTime;
            while (leftTime > 0)
            {
                transform.Translate(Vector3.right * (1 + _animationSpeed) * Time.fixedDeltaTime);
                leftTime -= Time.fixedDeltaTime;
                await UniTask.WaitForFixedUpdate(this.GetCancellationTokenOnDestroy());
            }
        }

        public override async UniTaskVoid Attack()
        {
            await Play().SuppressCancellationThrow();
            AttackerController.RemoveAttacker(this);
            Destroy(gameObject);
        }
    }
}
