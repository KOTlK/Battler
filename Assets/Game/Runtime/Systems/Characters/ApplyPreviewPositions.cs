using Game.Runtime.Components.Characters;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Unity.Burst;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Jobs;

namespace Game.Runtime.Systems.Characters
{
    public class ApplyPreviewPositions : IEcsRunSystem, IEcsDestroySystem
    {
        private readonly EcsFilterInject<Inc<CharacterPreview, EnablePreview>> _filter = default;
        private readonly EcsPoolInject<CharacterPreview> _previews = default;

        private NativeArray<Vector3> _positions = new NativeArray<Vector3>(10000, Allocator.Persistent);

        public void Run(IEcsSystems systems)
        {
            var index = 0;
            var transformAccess = new TransformAccessArray(10000);

            foreach (var entity in _filter.Value)
            {
                ref var preview = ref _previews.Value.Get(entity);
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

        public void Destroy(IEcsSystems systems)
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