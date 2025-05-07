using System.Collections.Generic;
using UnityEngine;

namespace Character.Damage
{
    public interface IAttackerController : ICharacterControlled
    {
        IAttacker GetOrCreateAttacker();
        void RemoveAttacker(IAttacker attacker);
        void ClearAttacker();
        bool CanAttack { get; set; }
    }

    public abstract class AttackerController : MonoBehaviour, IController, IAttackerController
    {
        public ICharacterController Controller { get; set; }
        protected ICharacterModel Model => Controller.Model;

        public ICharacterController CharacterController { get; set; }

        public bool CanAttack { get; set; } = true;

        protected List<IAttacker> Attackers = new();

        public IAttacker GetOrCreateAttacker()
        {
            IAttacker attacker = GetOrCreateAttackerInternal();
            attacker.AttackerController = this;
            if (!Attackers.Contains(attacker))
            {
                Attackers.Add(attacker);
            }

            return attacker;
        }

        public void RemoveAttacker(IAttacker attacker)
        {
            Attackers.Remove(attacker);
        }

        protected abstract IAttacker GetOrCreateAttackerInternal();

        public virtual void ClearAttacker()
        {
            Attackers.Clear();
        }


        public IArchitecture GetArchitecture()
        {
            return PixelRPG.Interface;
        }
    }

}
