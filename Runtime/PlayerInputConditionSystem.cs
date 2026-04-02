using BovineLabs.Reaction.Conditions;
using BovineLabs.Reaction.Data.Conditions;
using BovineLabs.Timeline.Data;
using BovineLabs.Timeline.PlayerInput;
using BovineLabs.Timeline.Tracks.Data.PlayerInputs;
using PlayerInputs.PlayerInputs.Data;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;

namespace BovineLabs.Timeline.PlayerInput
{
    [UpdateInGroup(typeof(TimelineComponentAnimationGroup))]
    public partial struct PlayerInputConditionSystem : ISystem
    {
        private InputCurrentFacet.TypeHandle currentHandle;
        private InputPreviousFacet.TypeHandle previousHandle;
        private ConditionEventWriter.TypeHandle eventWriterHandle;
        private EntityTypeHandle entityHandle;
        private EntityQuery targetQuery;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            currentHandle.Create(ref state);
            previousHandle.Create(ref state);
            eventWriterHandle.Create(ref state);
            entityHandle = state.GetEntityTypeHandle();

            targetQuery = SystemAPI.QueryBuilder()
                .WithAll<ConditionEvent, InputJump>()
                .Build();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            currentHandle.Update(ref state);
            previousHandle.Update(ref state);
            eventWriterHandle.Update(ref state);
            entityHandle.Update(ref state);

            var activeClipsMap =
                new NativeParallelMultiHashMap<Entity, PlayerInputConditionValue>(64, Allocator.TempJob);

            var gatherJob = new GatherClipsJob
            {
                ActiveClipsMap = activeClipsMap.AsParallelWriter()
            };
            state.Dependency = gatherJob.ScheduleParallel(state.Dependency);

            var evaluateJob = new EvaluateInputEventsJob
            {
                CurrentHandle = currentHandle,
                PreviousHandle = previousHandle,
                EventWriterHandle = eventWriterHandle,
                EntityHandle = entityHandle,
                ActiveClipsMap = activeClipsMap.AsReadOnly()
            };

            state.Dependency = evaluateJob.ScheduleParallel(targetQuery, state.Dependency);

            activeClipsMap.Dispose(state.Dependency);
        }

        [BurstCompile]
        [WithAll(typeof(ClipActive))]
        private partial struct GatherClipsJob : IJobEntity
        {
            public NativeParallelMultiHashMap<Entity, PlayerInputConditionValue>.ParallelWriter ActiveClipsMap;

            private void Execute(in TrackBinding binding, in PlayerInputConditionValue clipData)
            {
                ActiveClipsMap.Add(binding.Value, clipData);
            }
        }

        [BurstCompile]
        private struct EvaluateInputEventsJob : IJobChunk
        {
            public InputCurrentFacet.TypeHandle CurrentHandle;
            public InputPreviousFacet.TypeHandle PreviousHandle;
            public ConditionEventWriter.TypeHandle EventWriterHandle;
            [ReadOnly] public EntityTypeHandle EntityHandle;
            [ReadOnly] public NativeParallelMultiHashMap<Entity, PlayerInputConditionValue>.ReadOnly ActiveClipsMap;

            public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask,
                in v128 chunkEnabledMask)
            {
                var currentResolved = CurrentHandle.Resolve(chunk);
                var previousResolved = PreviousHandle.Resolve(chunk);
                var eventWriters = EventWriterHandle.Resolve(chunk);
                var entities = chunk.GetNativeArray(EntityHandle);

                for (var i = 0; i < chunk.Count; i++)
                {
                    var entity = entities[i];

                    if (!ActiveClipsMap.TryGetFirstValue(entity, out var clipData, out var iterator))
                        continue;

                    var current = currentResolved[i];
                    var previous = previousResolved[i];
                    var writer = eventWriters[i];

                    do
                    {
                        if (clipData.ConditionKey == ConditionKey.Null)
                            continue;

                        var triggered = false;
                        switch (clipData.PlayerInputType)
                        {
                            case PlayerInputType.Attack:
                                triggered = current.Attack.ValueRO && !previous.Attack.ValueRO;
                                break;
                            case PlayerInputType.Interact:
                                triggered = current.Interact.ValueRO && !previous.Interact.ValueRO;
                                break;
                            case PlayerInputType.Crouch:
                                triggered = current.Crouch.ValueRO && !previous.Crouch.ValueRO;
                                break;
                            case PlayerInputType.Jump:
                                triggered = current.Jump.ValueRO && !previous.Jump.ValueRO;
                                break;
                            case PlayerInputType.Previous:
                                triggered = current.Prev.ValueRO && !previous.Prev.ValueRO;
                                break;
                            case PlayerInputType.Next:
                                triggered = current.Next.ValueRO && !previous.Next.ValueRO;
                                break;
                            case PlayerInputType.Sprint:
                                triggered = current.Sprint.ValueRO && !previous.Sprint.ValueRO;
                                break;
                            case PlayerInputType.Move:
                                triggered = current.MoveActive.ValueRO && !previous.MoveActive.ValueRO;
                                break;
                            case PlayerInputType.Look:
                                triggered = current.LookActive.ValueRO && !previous.LookActive.ValueRO;
                                break;
                        }

                        if (triggered) writer.Trigger(clipData.ConditionKey, 1);
                    } while (ActiveClipsMap.TryGetNextValue(out clipData, ref iterator));
                }
            }
        }
    }
}
