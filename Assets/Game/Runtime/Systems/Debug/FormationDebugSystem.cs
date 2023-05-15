using System.Text;
using Game.Runtime.Components.Characters;
using Game.Runtime.Components.Squads;
using Game.Runtime.Components.Squads.Formations;
using Game.Runtime.Extensions;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using TMPro;
using UnityEngine;

namespace Game.Runtime.Systems.Debug
{
    public class FormationDebugSystem : IEcsRunSystem
    {
        private readonly TMP_Text _debugText;
        private readonly StringBuilder _stringBuilder = new();

        private readonly EcsFilterInject<Inc<Squad, Formation, Selected>> _filter = default;
        private readonly EcsPoolInject<Formation> _formations = default;

        public FormationDebugSystem(TMP_Text debugText)
        {
            _debugText = debugText;
        }

        public void Run(IEcsSystems systems)
        {
            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                _stringBuilder.Clear();
                
                foreach (var entity in _filter.Value)
                {
                    ref var formation = ref _formations.Value.Get(entity);

                    foreach (var node in formation.Graph.AllNodes)
                    {
                        _stringBuilder.Append(node.Position.ToString());
                        _stringBuilder.Append(":");
                        _stringBuilder.Append(node.Position.ToString());
                        _stringBuilder.Append("\n");
                    }
                }

                _debugText.text = _stringBuilder.ToString();
            }
            
            foreach (var entity in _filter.Value)
            {
                ref var formation = ref _formations.Value.Get(entity);

                foreach (var node in formation.Graph.AllNodes)
                {
                    UnityEngine.Debug.DrawRay(node.Position.FromXZ(), Vector3.up, Color.blue);

                    foreach (var neighbour in formation.Graph.Neighbours(node))
                    {
                        if(neighbour == FormationNode.None)
                            continue;
                        
                        UnityEngine.Debug.DrawLine(node.Position.FromXZ(), neighbour.Position.FromXZ(), Color.red);
                    }
                }
            }
        }
    }
}