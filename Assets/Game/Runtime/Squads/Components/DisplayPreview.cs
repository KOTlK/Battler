using System;
using Game.Runtime.Squads.Components.Formations;
using UnityEngine;

namespace Game.Runtime.Squads.Components
{
    [Serializable]
    public struct DisplayPreview
    {
        public Vector3 Forward;
        public Vector3 StartPosition;
        public FormationType FormationType;
        public int MaxColumns;
    }
}