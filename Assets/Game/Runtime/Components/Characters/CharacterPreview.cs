using System;
using Game.Runtime.MonoHell.View.Characters;
using UnityEngine;

namespace Game.Runtime.Components.Characters
{
    [Serializable]
    public struct CharacterPreview
    {
        public Vector3 Position;
        public Transform Transform;
        public UnitPreview Instance;
    }
}