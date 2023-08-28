using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Character
{
    public class Character : Singleton<Character>
    {
        [HideInInspector]public PlayerController playerController;
        [HideInInspector]public CharacterAttributes characterAttributes;
        [HideInInspector]public Damageable damageable;

        protected void Start()
        {
            playerController = GetComponent<PlayerController>();
            characterAttributes = GetComponentInChildren<CharacterAttributes>();
            damageable = GetComponentInChildren<Damageable>();
        }
    }
}