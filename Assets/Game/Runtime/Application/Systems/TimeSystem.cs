using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;

namespace Game.Runtime.Application.Systems
{
    public class TimeSystem : IEcsRunSystem
    {
        private readonly EcsCustomInject<Time> _time = default;
        
        public void Run(IEcsSystems systems)
        {
            _time.Value.DeltaTime = UnityEngine.Time.deltaTime;
        }
    }
}