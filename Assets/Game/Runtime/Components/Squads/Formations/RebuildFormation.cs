using System;

namespace Game.Runtime.Components.Squads.Formations
{
    [Serializable]
    public struct RebuildFormation
    {
        public int Columns;
        public FormationType FormationType;
    }
}