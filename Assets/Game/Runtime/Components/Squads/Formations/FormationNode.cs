﻿using System;
using Scellecs.Morpeh;
using UnityEngine;

namespace Game.Runtime.Components.Squads.Formations
{
    public struct FormationNode : IEquatable<FormationNode>
    {
        public Entity Entity;
        public Vector2Int Position;

        public static FormationNode None = default;

        public static bool operator ==(FormationNode first, FormationNode second)
        {
            return first.Equals(second);
        }

        public static bool operator !=(FormationNode first, FormationNode second)
        {
            return !first.Equals(second);
        }

        public override bool Equals(object obj)
        {
            return obj is FormationNode other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Entity, Position);
        }

        public bool Equals(FormationNode other)
        {
            return Equals(Entity, other.Entity) && Position.Equals(other.Position);
        }
    }
}