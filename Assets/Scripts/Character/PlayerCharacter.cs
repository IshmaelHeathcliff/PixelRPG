using System;
using Character.Entry;
using UnityEngine;
using UnityEngine.Serialization;

namespace Character
{
    public class PlayerCharacter : Character
    {
        public PlayerController PlayerController { get; private set; }
        public CharacterAttributes CharacterAttributes { get; private set; }
        public Damageable Damageable { get; private set; }
        public IEntryFactory EntryFactory { get; private set; }

        protected void Awake()
        {
            PlayerController = GetComponent<PlayerController>();
            CharacterAttributes = GetComponentInChildren<CharacterAttributes>();
            Damageable = GetComponentInChildren<Damageable>();
            // EntryFactory = new IEntryFactory(CharacterAttributes);
        }
    }
}