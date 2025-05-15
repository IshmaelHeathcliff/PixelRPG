using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Character
{
    public interface IMoveController : ICharacterControlled
    {
        float Speed { get; set; }
        Vector2 Direction { get; set; }
        Vector2 Position { get; set; }
        void Face(Vector2 direction);
        void Freeze();
        void MoveTo(Vector2 position);
    }

    [RequireComponent(typeof(Animator), typeof(Rigidbody2D))]
    public abstract class MoveController : MonoBehaviour, IMoveController, IController
    {
        public ICharacterController CharacterController { get; set; }

        protected Animator Animator;
        protected Rigidbody2D Rigidbody;

        protected ICharacterModel Model => CharacterController.Model;

        static readonly int Y = Animator.StringToHash("Y");
        static readonly int X = Animator.StringToHash("X");

        public virtual float Speed
        {
            get => Model.Speed;
            set => Model.Speed = value;
        }

        public virtual Vector2 Direction
        {
            get => Model.Direction;
            set => Model.Direction = value;
        }


        public virtual Vector2 Position
        {
            get => Model.Position;
            set => Model.Position = value;
        }
        public async UniTask PlayAnimation(string stateName)
        {
            Animator.Play(stateName);
            await UniTask.WaitUntil(() => Animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f);
        }

        public async UniTask PlayAnimation(int stateNameHash)
        {
            Animator.Play(stateNameHash);
            await UniTask.WaitUntil(() => Animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f, cancellationToken: this.GetCancellationTokenOnDestroy());
        }

        public virtual void Face(Vector2 direction)
        {
            Animator.SetFloat(X, direction.x);
            Animator.SetFloat(Y, direction.y);
            Direction = direction.normalized;
        }

        public virtual void Freeze()
        {
            Rigidbody.linearVelocity = Vector2.zero;
        }

        public virtual void MoveTo(Vector2 position)
        {
            Position = position;
        }

        protected virtual void Awake()
        {
            Animator = GetComponentInChildren<Animator>();
            Rigidbody = GetComponent<Rigidbody2D>();
        }

        public IArchitecture GetArchitecture()
        {
            return PixelRPG.Interface;
        }
    }
}
