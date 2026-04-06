using System;
using System.ComponentModel;
using BovineLabs.Timeline.Authoring;
using PlayerInputs.Data;
using Unity.Entities;
using UnityEngine.Timeline;

namespace BovineLabs.Timeline.PlayerInput.Authoring
{
    [Serializable]
    [TrackClipType(typeof(InputRecordClip))]
    [TrackColor(0.8f, 0.6f, 0.2f)]
    [TrackBindingType(typeof(UnityEngine.GameObject))]
    [DisplayName("BovineLabs/Player Inputs/Input Buffer Record")]
    public class InputRecordTrack : DOTSTrack { }
}
