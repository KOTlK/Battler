using System;
using Scellecs.Morpeh;
using Unity.IL2CPP.CompilerServices;

namespace Game.Runtime.Components.Squads
{
    [Serializable]
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    public struct Squad : IComponent
    {
        public Entity[] Members;
        public float DistanceBetweenUnits;
        public int MinColumnsCount;
        public int MaxColumnsCount;
        public bool HaveRangedAttack;
        public AttackMode AttackMode;
    }
}