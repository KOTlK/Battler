using Game.Runtime.Application;
using Game.Runtime.Components.Characters;
using Scellecs.Morpeh;
using Unity.Burst;
using Unity.Collections;
using Unity.IL2CPP.CompilerServices;
using UnityEngine;
using UnityEngine.Jobs;

namespace Game.Runtime.Systems.Characters
{
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    public class ApplyPreviewPositions : UpdateSystem
    {
        private Filter _filter;
        private Stash<CharacterPreview> _previews;

        private NativeArray<Vector3> _positions;

        public ApplyPreviewPositions(World world) : base(world)
        {
        }

        public override void OnAwake()
        {
            _filter = World.Filter.With<CharacterPreview>().With<EnablePreview>();
            _previews = World.GetStash<CharacterPreview>();
            _positions = new NativeArray<Vector3>(10000, Allocator.Persistent);
        }

        public override void OnUpdate(float deltaTime)
        {
            var index = 0;
            var transformAccess = new TransformAccessArray(10000);

            foreach (var entity in _filter)
            {
                ref var preview = ref _previews.Get(entity);
                _positions[index] = preview.Position;
                transformAccess.Add(preview.Transform);
                index++;
            }

            if (index == 0)
                return;

            var job = new ApplyTransformJob()
            {
                Positions = _positions
            };

            var jobHandle = job.Schedule(transformAccess);
            jobHandle.Complete();
            transformAccess.Dispose();
        }

        public override void Dispose()
        {
            _positions.Dispose();
        }
        
        [BurstCompile]
        private struct ApplyTransformJob : IJobParallelForTransform
        {
            [ReadOnly] public NativeArray<Vector3> Positions;
            
            public void Execute(int index, TransformAccess transform)
            {
                transform.position = Positions[index];
            }
        }
    }
}