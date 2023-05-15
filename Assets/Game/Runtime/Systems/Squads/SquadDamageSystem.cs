using Game.Runtime.Components.Characters;
using Game.Runtime.Components.Squads;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;

namespace Game.Runtime.Systems.Squads
{
    public class SquadDamageSystem : IEcsRunSystem
    {
        private readonly EcsFilterInject<Inc<Squad, DamageBuffer>, Exc<Dead>> _filter = default;
        private readonly EcsPoolInject<Squad> _squads = default;
        private readonly EcsPoolInject<DamageBuffer> _damageBuffers = default;
        private readonly EcsPoolInject<Dead> _dead = default;
        private readonly EcsPoolInject<Health> _healths = default;
        private readonly EcsPoolInject<CharacterView> _characterViews = default;

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _filter.Value)
            {
                ref var squad = ref _squads.Value.Get(entity);
                ref var buffer = ref _damageBuffers.Value.Get(entity);

                if (buffer.Buffer.Count == 0)
                {
                    continue;
                }

                var aliveCount = squad.AliveMembers.Count;

                if (aliveCount == 0)
                {
                    _dead.Value.Add(entity);
                    continue;
                }

                if (aliveCount == 1)
                {
                    var characterEntity = squad.AliveMembers[0];

                    ref var health = ref _healths.Value.Get(characterEntity);
                    
                    while (health.Current > 0 && buffer.Buffer.Count > 0)
                    {
                        health.Current -= buffer.Buffer.Dequeue();
                    }
                    
                    if (health.Current <= 0)
                    {
                        ref var view = ref _characterViews.Value.Get(characterEntity);
                        view.Instance.PlayDeathAnimation();
                        squad.DeadMembers.Add(characterEntity);
                        squad.AliveMembers.RemoveAt(0);
                    }
                }

                for (var i = 0; i < aliveCount - 1; i++)
                {
                    var characterEntity = squad.AliveMembers[i];
                    
                    if (buffer.Buffer.Count == 0)
                    {
                        break;
                    }

                    ref var health = ref _healths.Value.Get(characterEntity);

                    while (health.Current > 0 && buffer.Buffer.Count > 0)
                    {
                        health.Current -= buffer.Buffer.Dequeue();
                    }
                    
                    if (health.Current <= 0)
                    {
                        ref var view = ref _characterViews.Value.Get(characterEntity);
                        view.Instance.PlayDeathAnimation();
                        squad.DeadMembers.Add(characterEntity);
                        squad.AliveMembers.RemoveAt(i);
                    }
                }
            }
        }
    }
}