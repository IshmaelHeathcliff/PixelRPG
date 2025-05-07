using Core;

namespace Character.Enemy
{
    public enum EnemyStateId
    {
        Idle, Patrol, Chase, Attack, Hurt, Dead
    }
    
    public class EnemyState : AbstractState<EnemyStateId, EnemyController>
    {
        protected EnemyMoveController MoveController;
        public EnemyState(FSM<EnemyStateId> fsm, EnemyController target) : base(fsm, target)
        {
            if (target.MoveController is EnemyMoveController enemyMoveController)
            {
                MoveController = enemyMoveController;
            }
        }
    }
}