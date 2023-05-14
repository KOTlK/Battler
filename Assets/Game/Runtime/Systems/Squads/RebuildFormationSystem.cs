using System.Linq;
using Game.Runtime.Application;
using Game.Runtime.Components.Squads.Formations;
using Scellecs.Morpeh;
using Unity.IL2CPP.CompilerServices;
using UnityEngine;

namespace Game.Runtime.Systems.Squads
{
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    public class RebuildFormationSystem : UpdateSystem
    {
        private Filter _filter;
        
        public RebuildFormationSystem(World world) : base(world)
        {
        }

        public override void OnAwake()
        {
            _filter = World.Filter.With<Formation>().With<RebuildFormation>();
        }

        public override void OnUpdate(float deltaTime)
        {
            foreach (var entity in _filter)
            {
                ref var command = ref entity.GetComponent<RebuildFormation>();
                ref var formation = ref entity.GetComponent<Formation>();

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
                entity.RemoveComponent<RebuildFormation>();
            }
        }

        public override void Dispose()
        {

        }
    }
}