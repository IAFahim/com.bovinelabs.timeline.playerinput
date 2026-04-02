using UnityEngine;
using BovineLabs.Timeline.Authoring;
using System;
using System.ComponentModel;
using PlayerInputs.PlayerInputs.Authoring;
using UnityEngine.Timeline;

namespace BovineLabs.Timeline.PlayerInput.Authoring
{
    [Serializable]
    [TrackClipType(typeof(PlayerInputConditionClip))]
    [TrackColor(0.3f, 0.6f, 0.9f)]
    [TrackBindingType(typeof(PlayerInputAuthoring))]
    [DisplayName("BovineLabs/Timeline/Input/Player Condition")]
    public class PlayerInputConditionTrack : DOTSTrack
    {
    }
}
