using BovineLabs.Timeline.Authoring;
using PlayerInputs.Data;
using Unity.Entities;
using UnityEngine.Timeline;

namespace BovineLabs.Timeline.PlayerInput.Authoring
{
    public class InputRecordClip : DOTSClip, ITimelineClipAsset
    {
        public ClipCaps clipCaps => ClipCaps.None;

        public override void Bake(Entity clipEntity, BakingContext context)
        {
            context.Baker.AddComponent<InputRecordComponent>(clipEntity);
            base.Bake(clipEntity, context);
        }
    }
}
