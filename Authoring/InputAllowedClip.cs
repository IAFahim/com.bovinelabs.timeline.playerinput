// using BovineLabs.Timeline.Authoring;
// using BovineLabs.Reaction.Authoring.Conditions;
// using BovineLabs.Reaction.Data.Conditions;
// using PlayerInputs.Data;
// using Unity.Entities;
// using UnityEngine.Timeline;
//
// namespace BovineLabs.Timeline.PlayerInput.Authoring
// {
//     public class InputAllowedClip : DOTSClip, ITimelineClipAsset
//     {
//         public string ActionName = "Jump";
//         public InputPressType PressType = InputPressType.Down;
//         public ConditionEventObject TriggerEvent;
//
//         public ClipCaps clipCaps => ClipCaps.None;
//
//         public override void Bake(Entity clipEntity, BakingContext context)
//         {
//             context.Baker.AddComponent(clipEntity, new InputAllowedComponent
//             {
//                 ActionID = InputUtility.GetActionID(ActionName),
//                 PressType = PressType,
//                 EventKey = TriggerEvent ? TriggerEvent.Key : ConditionKey.Null
//             });
//
//             base.Bake(clipEntity, context);
//         }
//     }
// }
