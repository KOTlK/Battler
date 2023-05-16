using System;

namespace Game.Runtime.Squads.Components.Formations
{
    [Serializable]
    public struct RebuildFormation
    {
        public int Columns;
        public FormationType FormationType;
    }
}