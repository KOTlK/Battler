using System.Collections.Generic;
using UnityEngine;

namespace Game.Runtime.Squads.Components.Formations
{
    public struct FormationGraph
    {
        public List<FormationNode> AllNodes;
        public Dictionary<FormationNode, IEnumerable<FormationNode>> NeighboursMap;

        private static readonly Vector2Int[] Directions = new Vector2Int[4]
        {
            Vector2Int.left,
            Vector2Int.right,
            Vector2Int.down,
            Vector2Int.up,
        };

        public IEnumerable<FormationNode> Neighbours(FormationNode node) => NeighboursMap[node];

        public void RecalculateMap()
        {
            NeighboursMap.Clear();

            foreach (var node in AllNodes)
            {
                NeighboursMap[node] = CalculateNeighbours(node);
            }
        }

        private IEnumerable<FormationNode> CalculateNeighbours(FormationNode node)
        {
            var results = new List<FormationNode>();

            foreach (var direction in Directions)
            {
                foreach (var formationNode in AllNodes)
                {
                    if (formationNode.Position == node.Position + direction)
                    {
                        results.Add(formationNode);
                    }
                }
            }

            return results;
        }
    }
}