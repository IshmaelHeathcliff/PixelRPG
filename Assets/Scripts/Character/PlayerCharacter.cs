using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Character
{
    public class PlayerCharacter : Singleton<PlayerCharacter>
    {
        [HideInInspector]public PlayerController playerController;
        [HideInInspector]public CharacterAttributes characterAttributes;
        [HideInInspector]public Damageable damageable;

        protected override void Awake()
        {
            base.Awake();
            playerController = GetComponent<PlayerController>();
            characterAttributes = GetComponentInChildren<CharacterAttributes>();
            damageable = GetComponentInChildren<Damageable>();
        }
    }
}