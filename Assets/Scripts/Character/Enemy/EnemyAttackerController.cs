using Character.Damage;
using UnityEngine;
using Core;

namespace Character.Enemy
{
    public class EnemyAttackerController : AttackerController
    {
        public FSM<EnemyStateId> FSM => (CharacterController as IHasFSM<EnemyStateId>).FSM;
        protected override IAttacker GetOrCreateAttackerInternal()
        {
            EnemyAttacker attacker = GetComponentInChildren<EnemyAttacker>();
            attacker.SetStats(CharacterController.Stats);
            return attacker;
        }

        void Start()
        {
            GetOrCreateAttacker();
        }
    }
}
