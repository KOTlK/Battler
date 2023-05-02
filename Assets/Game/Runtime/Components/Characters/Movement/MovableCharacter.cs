﻿using System;
using Scellecs.Morpeh;
using Unity.IL2CPP.CompilerServices;
using UnityEngine;
using UnityEngine.AI;

namespace Game.Runtime.Components.Characters.Movement
{
    [Serializable]
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    public struct MovableCharacter : IComponent
    {
        public float Speed;
        public Vector3 Position => Agent.transform.position;
        public NavMeshAgent Agent;
    }
}