using System;
using Game.Runtime.Components.Squads.Formations;
using UnityEngine;

namespace Game.Runtime.Components.Squads
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