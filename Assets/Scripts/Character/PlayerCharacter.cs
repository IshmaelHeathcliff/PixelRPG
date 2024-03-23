using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Character
{
    public class PlayerCharacter : MonoBehaviour
    {
        public PlayerController PlayerController { get; private set; }
        public CharacterAttributes CharacterAttributes { get; private set; }
        public Damageable Damageable { get; private set; }

        void Awake()
        {
            PlayerController = GetComponent<PlayerController>();
            CharacterAttributes = GetComponentInChildren<CharacterAttributes>();
            Damageable = GetComponentInChildren<Damageable>();
        }

        void Update()
        {
        }
    }
}