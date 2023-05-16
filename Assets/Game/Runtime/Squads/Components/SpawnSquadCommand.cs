using System;
using UnityEngine;
using CharacterView = Game.Runtime.MonoHell.View.Characters.CharacterView;

namespace Game.Runtime.Squads.Components
{
    [Serializable]
    public struct SpawnSquadCommand
    {
        public CharacterView CharacterPrefab;
        public float CharacterHealth;
        public float CharactersSpeed;
        public float CharactersDamage;
        public float DistanceBetweenUnits;
        public int MinColumnsCount;
        public int MaxColumnsCount;
        public bool HaveRangedAttack;
        public AttackMode AttackMode;
        public int Count;
        public Vector3 Position;
    }
    
    
}