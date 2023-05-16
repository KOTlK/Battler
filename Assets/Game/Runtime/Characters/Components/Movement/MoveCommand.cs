using System;
using UnityEngine;

namespace Game.Runtime.Characters.Components.Movement
{
    [Serializable]
    public struct MoveCommand
    {
        public Vector3 Position;
        public Vector3 LookDirection;
    }
}