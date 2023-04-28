using System;
using Scellecs.Morpeh;
using Unity.IL2CPP.CompilerServices;
using UnityEngine;

namespace Game.Runtime.Components.Characters
{
    [Serializable]
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    public struct SpawnCharacterCommand : IComponent
    {
        public Entity TargetEntity;
        public Character Config;
        public Vector3 Position;
        public MonoHell.View.Characters.CharacterView Prefab;
    }
}