using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Character
{
    public class PlayerCharacter : MonoBehaviour
    {
        [HideInInspector]public PlayerController playerController;
        [HideInInspector]public CharacterAttributes characterAttributes;
        [HideInInspector]public Damageable damageable;

        void Awake()
        {
            playerController = GetComponent<PlayerController>();
            characterAttributes = GetComponentInChildren<CharacterAttributes>();
            damageable = GetComponentInChildren<Damageable>();
        }

        void Update()
        {
        }
    }
}