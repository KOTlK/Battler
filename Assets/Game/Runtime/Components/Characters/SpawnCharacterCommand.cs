using System;
using UnityEngine;

namespace Game.Runtime.Components.Characters
{
    [Serializable]
    public struct SpawnCharacterCommand
    {
        public float Speed;
        public float Damage;
        public float MaxHealth;
        public Vector3 Position;
        public MonoHell.View.Characters.CharacterView Prefab;
    }
}