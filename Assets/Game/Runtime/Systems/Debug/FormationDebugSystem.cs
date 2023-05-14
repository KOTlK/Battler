using System.Text;
using Game.Runtime.Application;
using Game.Runtime.Components.Characters;
using Game.Runtime.Components.Squads;
using Game.Runtime.Components.Squads.Formations;
using Game.Runtime.Extensions;
using Scellecs.Morpeh;
using TMPro;
using Unity.IL2CPP.CompilerServices;
using UnityEngine;

namespace Game.Runtime.Systems.Debug
{
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    public class FormationDebugSystem : UpdateSystem
    {
        private readonly TMP_Text _debugText;
        private readonly StringBuilder _stringBuilder = new();
        
        private Filter _filter;

        public FormationDebugSystem(World world, TMP_Text debugText) : base(world)
        {
            _debugText = debugText;
        }

        public override void OnAwake()
        {
            _filter = World.Filter.With<Squad>().With<Formation>().With<Selected>();
        }

        public override void OnUpdate(float deltaTime)
        {
            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                _stringBuilder.Clear();
                
                foreach (var entity in _filter)
                {
                    ref var formation = ref entity.GetComponent<Formation>();

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
            
            foreach (var entity in _filter)
            {
                ref var formation = ref entity.GetComponent<Formation>();

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

        public override void Dispose()
        {

        }
    }
}