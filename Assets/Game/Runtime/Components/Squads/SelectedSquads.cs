using System.Collections.Generic;

namespace Game.Runtime.Components.Squads
{
    public class SelectedSquads
    {
        public readonly List<Squad> Squads = new();
        public float MinDistance;
    }
}