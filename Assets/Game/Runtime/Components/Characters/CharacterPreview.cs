using System;
using Game.Runtime.MonoHell.View.Characters;
using Scellecs.Morpeh;
using Unity.IL2CPP.CompilerServices;
using UnityEngine;

namespace Game.Runtime.Components.Characters
{
    [Serializable]
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    public struct CharacterPreview : IComponent
    {
        public Vector3 Position;
        public Transform Transform;
        public UnitPreview Instance;
    }
}