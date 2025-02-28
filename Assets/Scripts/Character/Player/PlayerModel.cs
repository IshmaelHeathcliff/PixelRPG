using Character.State;
using Character.Stat;
using UnityEngine;

namespace Character
{
    public class PlayerModel : AbstractModel
    {
        #region Position
        Transform _playerTransform;
        public Vector3 Position => _playerTransform.position;
        public Vector2 Direction { get; set; }
        public void BindTransform(Transform playerTransform)
        {
            _playerTransform = playerTransform;
        }
        
        public void SetPosition(Vector3 position)
        {
            _playerTransform.position = position;
        }
        #endregion

        public bool IsDetectable{ get; set; } = true;
        public bool IsAttackable { get; set; } = true;

        public Stats PlayerStats { get; } = new Stats();
        public IStateContainer PlayerState { get; } = new StateContainer();

        protected override void OnInit()
        {
        }
    }
}