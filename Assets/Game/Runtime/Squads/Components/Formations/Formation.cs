using System;
using UnityEngine;

namespace Game.Runtime.Squads.Components.Formations
{
    [Serializable]
    public struct Formation
    {
        public int MaxColumns;
        public Vector3 Forward;
        public FormationGraph Graph;
    }
}