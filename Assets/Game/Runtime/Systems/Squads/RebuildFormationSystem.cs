using System.Linq;
using Game.Runtime.Components.Squads.Formations;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Game.Runtime.Systems.Squads
{
    public class RebuildFormationSystem : IEcsRunSystem
    {
        private readonly EcsFilterInject<Inc<Formation, RebuildFormation>> _filter = default;
        private readonly EcsPoolInject<RebuildFormation> _rebuildFormationCommands = default;
        private readonly EcsPoolInject<Formation> _formations = default;

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _filter.Value)
            {
                ref var command = ref _rebuildFormationCommands.Value.Get(entity);
                ref var formation = ref _formations.Value.Get(entity);

                if (command.FormationType == FormationType.Rectangle)
                {
                    var localPosition = Vector2Int.zero;
                    var maxColumns = command.Columns;
                    formation.MaxColumns = maxColumns;

                    for (var i = 0; i < formation.Graph.AllNodes.Count; i++)
                    {
                        var node = formation.Graph.AllNodes.ElementAt(i);

                        node.Position = localPosition;
                        formation.Graph.AllNodes[i] = node;

                        localPosition.x++;
                        if (localPosition.x >= maxColumns)
                        {
                            localPosition.x = 0;
                            localPosition.y--;
                        }
                    }
                    
                }

                formation.Graph.RecalculateMap();
                _rebuildFormationCommands.Value.Del(entity);
            }
        }
    }
}