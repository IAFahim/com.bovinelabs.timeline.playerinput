using BovineLabs.Timeline.Authoring;
using BovineLabs.Reaction.Data.Conditions;
using BovineLabs.Reaction;
using BovineLabs.Reaction.Authoring.Conditions;
using BovineLabs.Timeline.PlayerInput;
using BovineLabs.Timeline.Tracks.Data.PlayerInputs;
using PlayerInputs.PlayerInputs.Authoring;
using Unity.Entities;
using UnityEngine.Timeline;
namespace BovineLabs.Timeline.PlayerInput.Authoring
{
    public class PlayerInputConditionClip : DOTSClip, ITimelineClipAsset
    {
        public ConditionEventObject ConditionEvent;
        public InputIDSettings Settings;

        public ClipCaps clipCaps => ClipCaps.None;

        public override void Bake(Entity clipEntity, BakingContext context)
        {
            ConditionKey conditionEventKey = ConditionEvent ? ConditionEvent.Key : ConditionKey.Null;
            var playerInputType = PlayerInputType.Attack;
            for (var i = 0; i < Settings.inputEvents.Length; i++)
            {
                var settingsInputEvent = Settings.inputEvents[i];
                if (settingsInputEvent.conditionEventObject == ConditionEvent)
                {
                    playerInputType = settingsInputEvent.id;
                    break;
                }
            }

            context.Baker.AddComponent(clipEntity, new PlayerInputConditionValue
                {
                    PlayerInputType = playerInputType,
                    ConditionKey = conditionEventKey
                }
            );

            base.Bake(clipEntity, context);
        }
    }
}
