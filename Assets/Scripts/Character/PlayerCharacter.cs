using System;
using Character.Entry;
using UnityEngine;
using UnityEngine.Serialization;

namespace Character
{
    public class PlayerCharacter : MonoBehaviour
    {
        public PlayerController PlayerController { get; private set; }

        protected void Awake()
        {
            PlayerController = GetComponent<PlayerController>();
        }
    }
}