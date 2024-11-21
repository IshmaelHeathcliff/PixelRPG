using Character.Buff;
using Character.Stat;
using UnityEngine;

namespace Character
{
    public class PlayerModel : AbstractModel
    {
        Transform _playerTransform;

        public bool IsDetectable{ get; set; } = true;
        public bool IsAttackable { get; set; } = true;

        public Vector3 Position => _playerTransform.position;
        public Vector2 Direction { get; set; }

        public Stats PlayerStats { get; } = new Stats();
        public IBuffContainer PlayerBuff { get; } = new BuffContainer();


        public void BindTransform(Transform playerTransform)
        {
            _playerTransform = playerTransform;
        }
        
        
        public void SetPosition(Vector3 position)
        {
            _playerTransform.position = position;
        }
        
        protected override void OnInit()
        {
        }
    }
}