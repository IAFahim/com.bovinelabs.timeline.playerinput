// using BovineLabs.Timeline.Authoring;
// using BovineLabs.Reaction.Authoring.Conditions;
// using BovineLabs.Reaction.Data.Conditions;
// using PlayerInputs.Data;
// using Unity.Collections;
// using Unity.Entities;
// using UnityEngine.Timeline;
//
// namespace BovineLabs.Timeline.PlayerInput.Authoring
// {
//     public class InputConsumeClip : DOTSClip, ITimelineClipAsset
//     {
//         public string[] ComboSequence = { "PrimaryAttack", "PrimaryAttack" };
//         public ConditionEventObject TriggerEvent;
//         public bool ClearBufferOnConsume = true;
//
//         public ClipCaps clipCaps => ClipCaps.None;
//
//         public override void Bake(Entity clipEntity, BakingContext context)
//         {
//             var builder = new BlobBuilder(Allocator.Temp);
//             ref var root = ref builder.ConstructRoot<ComboBlob>();
//             var sequence = builder.Allocate(ref root.Sequence, ComboSequence.Length);
//
//             for (int i = 0; i < ComboSequence.Length; i++)
//             {
//                 sequence[i] = InputUtility.GetActionID(ComboSequence[i]);
//             }
//
//             var blob = builder.CreateBlobAssetReference<ComboBlob>(Allocator.Persistent);
//             context.Baker.AddBlobAsset(ref blob, out _);
//             builder.Dispose();
//
//             context.Baker.AddComponent(clipEntity, new InputConsumeComponent
//             {
//                 Combo = blob,
//                 EventKey = TriggerEvent ? TriggerEvent.Key : ConditionKey.Null,
//                 ClearOnConsume = ClearBufferOnConsume
//             });
//
//             base.Bake(clipEntity, context);
//         }
//     }
// }
