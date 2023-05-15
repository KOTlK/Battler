using System;
using System.Collections.Generic;

namespace Game.Runtime.Components.Squads
{
    [Serializable]
    public struct Squad
    {
        public List<int> DeadMembers;
        public List<int> AliveMembers;
        public int[] AllMembers;
        public int TotalCount;
        public float DistanceBetweenUnits;
        public int MinColumnsCount;
        public int MaxColumnsCount;
        public bool HaveRangedAttack;
        public AttackMode AttackMode;
    }
}