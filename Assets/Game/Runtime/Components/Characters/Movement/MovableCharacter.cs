﻿using System;
using UnityEngine;
using UnityEngine.AI;

namespace Game.Runtime.Components.Characters.Movement
{
    [Serializable]
    public struct MovableCharacter
    {
        public float Speed;
        public Vector3 Position => Agent.transform.position;
        public NavMeshAgent Agent;
    }
}