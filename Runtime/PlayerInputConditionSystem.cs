// using BovineLabs.Reaction.Conditions;
// using BovineLabs.Timeline.Data;
// using BovineLabs.Timeline.Tracks.Data.PlayerInputs;
// using PlayerInputs.Data;
// using Unity.Burst;
// using Unity.Collections;
// using Unity.Entities;
//
// namespace BovineLabs.Timeline.PlayerInput
// {
//     [UpdateInGroup(typeof(TimelineComponentAnimationGroup))]
//     public partial struct PlayerInputConditionSystem : ISystem
//     {
//         [BurstCompile]
//         public void OnCreate(ref SystemState state)
//         {
//         }
//
//         [BurstCompile]
//         public void OnUpdate(ref SystemState state)
//         {
//             var eventWriter = SystemAPI.GetSingleton<ConditionEventWriter>();
//
//             var activeClipsMap = new NativeParallelMultiHashMap<Entity, PlayerInputConditionValue>(64, Allocator.TempJob);
//
//             state.Dependency = new GatherClipsJob
//             {
//                 ActiveClipsMap = activeClipsMap.AsParallelWriter()
//             }.ScheduleParallel(state.Dependency);
//
//             state.Dependency = new EvaluateInputEventsJob
//             {
//                 EventWriter = eventWriter,
//                 ActiveClipsMap = activeClipsMap.AsReadOnly()
//             }.ScheduleParallel(state.Dependency);
//
//             activeClipsMap.Dispose(state.Dependency);
//         }
//
//         [BurstCompile]
//         [WithAll(typeof(ClipActive))]
//         private partial struct GatherClipsJob : IJobEntity
//         {
//             public NativeParallelMultiHashMap<Entity, PlayerInputConditionValue>.ParallelWriter ActiveClipsMap;
//
//             private void Execute(in TrackBinding binding, in PlayerInputConditionValue clipData)
//             {
//                 ActiveClipsMap.Add(binding.Value, clipData);
//             }
//         }
//
//         [BurstCompile]
//         private partial struct EvaluateInputEventsJob : IJobEntity
//         {
//             public ConditionEventWriter EventWriter;
//             [ReadOnly] public NativeParallelMultiHashMap<Entity, PlayerInputConditionValue>.ReadOnly ActiveClipsMap;
//
//             private void Execute(Entity entity, in DynamicBuffer<InputButtonDownBuffer> downs)
//             {
//                 if (!ActiveClipsMap.TryGetFirstValue(entity, out var clipData, out var iterator)) return;
//
//                 do
//                 {
//                     if (clipData.ConditionKey == BovineLabs.Reaction.Data.Conditions.ConditionKey.Null) continue;
//
//                     // O(n) search over a tiny array. Highly cache-friendly.
//                     for (int i = 0; i < downs.Length; i++)
//                     {
//                         if (downs[i].ActionID == clipData.RequiredActionID)
//                         {
//                             EventWriter.Trigger(clipData.ConditionKey, 1);
//                             break;
//                         }
//                     }
//                 } 
//                 while (ActiveClipsMap.TryGetNextValue(out clipData, ref iterator));
//             }
//         }
//     }
// }