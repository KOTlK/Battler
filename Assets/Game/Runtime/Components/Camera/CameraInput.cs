using System;
using UnityEngine;

namespace Game.Runtime.Components.Camera
{
    [Serializable]
    public struct CameraInput
    {
        public Vector3 MovementDirection;
        public Vector3 RotationDirection;
    }
}