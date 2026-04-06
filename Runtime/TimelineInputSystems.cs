// using BovineLabs.Timeline.Data;
// using BovineLabs.Timeline;
// using BovineLabs.Reaction.Conditions;
// using BovineLabs.Reaction.Data.Conditions;
// using PlayerInputs.Data;
// using Unity.Burst;
// using Unity.Collections;
// using Unity.Entities;
//
// namespace BovineLabs.Timeline.PlayerInput
// {
//     [UpdateInGroup(typeof(TimelineComponentAnimationGroup))]
//     public partial struct TimelineInputSystems : ISystem
//     {
//         private ConditionEventWriter.Lookup _eventWriterLookup;
//
//         [BurstCompile]
//         public void OnCreate(ref SystemState state)
//         {
//             _eventWriterLookup.Create(ref state);
//         }
//
//         [BurstCompile]
//         public void OnDestroy(ref SystemState state) { }
//
//         public void OnUpdate(ref SystemState state)
//         {
//             _eventWriterLookup.Update(ref state);
//
//             state.Dependency = new RecordInputJob
//             {
//                 HistoryBufferLookup = SystemAPI.GetBufferLookup<InputHistoryBuffer>(false),
//                 DownBufferLookup = SystemAPI.GetBufferLookup<InputButtonDownBuffer>(true)
//             }.ScheduleParallel(state.Dependency);
//
//             ProcessConsumes(ref state);
//             ProcessAllowed(ref state);
//
//             state.Dependency = new ClearHistoryJob
//             {
//                 HistoryBufferLookup = SystemAPI.GetBufferLookup<InputHistoryBuffer>(false)
//             }.ScheduleParallel(state.Dependency);
//         }
//
//         private void ProcessConsumes(ref SystemState state)
//         {
//             var em = state.EntityManager;
//             using var entities = state.GetEntityQuery(
//                 typeof(TrackBinding),
//                 typeof(InputConsumeComponent),
//                 typeof(ClipActive)).ToEntityArray(Allocator.Temp);
//
//             for (int i = 0; i < entities.Length; i++)
//             {
//                 var entity = entities[i];
//                 var target = em.GetComponentData<TrackBinding>(entity).Value;
//                 var config = em.GetComponentData<InputConsumeComponent>(entity);
//
//                 if (!em.HasBuffer<InputHistoryBuffer>(target)) continue;
//
//                 var history = em.GetBuffer<InputHistoryBuffer>(target);
//                 ref var sequence = ref config.Combo.Value.Sequence;
//
//                 if (history.Length < sequence.Length) continue;
//
//                 bool matched = true;
//                 int start = history.Length - sequence.Length;
//                 for (int j = 0; j < sequence.Length; j++)
//                 {
//                     if (history[start + j].ActionID != sequence[j])
//                     {
//                         matched = false;
//                         break;
//                     }
//                 }
//
//                 if (matched)
//                 {
//                     if (config.EventKey != ConditionKey.Null)
//                     {
//                         if (_eventWriterLookup.TryGet(target, out var writer))
//                             writer.Trigger(config.EventKey, 1);
//                     }
//
//                     if (config.ClearOnConsume)
//                         history.Clear();
//                 }
//             }
//         }
//
//         private void ProcessAllowed(ref SystemState state)
//         {
//             var em = state.EntityManager;
//             using var entities = state.GetEntityQuery(
//                 typeof(TrackBinding),
//                 typeof(InputAllowedComponent),
//                 typeof(ClipActive)).ToEntityArray(Allocator.Temp);
//
//             for (int i = 0; i < entities.Length; i++)
//             {
//                 var entity = entities[i];
//                 var target = em.GetComponentData<TrackBinding>(entity).Value;
//                 var config = em.GetComponentData<InputAllowedComponent>(entity);
//
//                 if (!CheckInput(ref state, target, config.ActionID, config.PressType)) continue;
//
//                 if (config.EventKey != ConditionKey.Null)
//                 {
//                     if (_eventWriterLookup.TryGet(target, out var writer))
//                         writer.Trigger(config.EventKey, 1);
//                 }
//             }
//         }
//
//         private static bool CheckInput(ref SystemState state, Entity target, int actionId, InputPressType pressType)
//         {
//             if (pressType == InputPressType.Down)
//             {
//                 var lookup = state.GetBufferLookup<InputButtonDownBuffer>(true);
//                 if (lookup.TryGetBuffer(target, out var buf))
//                     for (int i = 0; i < buf.Length; i++)
//                         if (buf[i].ActionID == actionId) return true;
//             }
//             else if (pressType == InputPressType.Held)
//             {
//                 var lookup = state.GetBufferLookup<InputButtonHeldBuffer>(true);
//                 if (lookup.TryGetBuffer(target, out var buf))
//                     for (int i = 0; i < buf.Length; i++)
//                         if (buf[i].ActionID == actionId) return true;
//             }
//             else if (pressType == InputPressType.Up)
//             {
//                 var lookup = state.GetBufferLookup<InputButtonUpBuffer>(true);
//                 if (lookup.TryGetBuffer(target, out var buf))
//                     for (int i = 0; i < buf.Length; i++)
//                         if (buf[i].ActionID == actionId) return true;
//             }
//             return false;
//         }
//
//         [BurstCompile]
//         [WithAll(typeof(ClipActive))]
//         private partial struct RecordInputJob : IJobEntity
//         {
//             [NativeDisableParallelForRestriction]
//             public BufferLookup<InputHistoryBuffer> HistoryBufferLookup;
//
//             [ReadOnly] public BufferLookup<InputButtonDownBuffer> DownBufferLookup;
//
//             private void Execute(in TrackBinding binding)
//             {
//                 if (!HistoryBufferLookup.TryGetBuffer(binding.Value, out var history)) return;
//                 if (!DownBufferLookup.TryGetBuffer(binding.Value, out var downs)) return;
//
//                 for (int i = 0; i < downs.Length; i++)
//                     history.Add(new InputHistoryBuffer { ActionID = downs[i].ActionID });
//             }
//         }
//
//         [BurstCompile]
//         [WithAll(typeof(ClipActive))]
//         private partial struct ClearHistoryJob : IJobEntity
//         {
//             [NativeDisableParallelForRestriction]
//             public BufferLookup<InputHistoryBuffer> HistoryBufferLookup;
//
//             private void Execute(in TrackBinding binding)
//             {
//                 if (HistoryBufferLookup.TryGetBuffer(binding.Value, out var history))
//                     history.Clear();
//             }
//         }
//     }
// }
