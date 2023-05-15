using System;
using UnityEngine;

namespace Game.Runtime.Components.Squads.Formations
{
    [Serializable]
    public struct Formation
    {
        public int MaxColumns;
        public Vector3 Forward;
        public FormationGraph Graph;
    }
}