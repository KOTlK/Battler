using System;
using UnityEngine;

namespace Game.Runtime.Components.Characters.Movement
{
    [Serializable]
    public struct MoveCommand
    {
        public Vector3 Position;
        public Vector3 LookDirection;
    }
}