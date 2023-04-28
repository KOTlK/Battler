using System;
using Game.Runtime.Components.Characters;
using Scellecs.Morpeh;
using Unity.IL2CPP.CompilerServices;

namespace Game.Runtime.Components.Squads
{
    [Serializable]
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    public struct SpawnSquadCommand : IComponent
    {
        public SpawnCharacterCommand CharacterConfig;
        public int Count;
    }
}