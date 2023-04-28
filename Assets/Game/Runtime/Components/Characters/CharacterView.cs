using System;
using Scellecs.Morpeh;
using Unity.IL2CPP.CompilerServices;

namespace Game.Runtime.Components.Characters
{
    [Serializable]
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    public struct CharacterView : IComponent
    {
        public MonoHell.View.Characters.CharacterView Prefab;
        public MonoHell.View.Characters.CharacterView Instance;
    }
}