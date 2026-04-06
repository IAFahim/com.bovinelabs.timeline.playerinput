using System;
using System.ComponentModel;
using BovineLabs.Timeline.Authoring;
using UnityEngine.Timeline;

namespace BovineLabs.Timeline.PlayerInput.Authoring
{
    [Serializable]
    [TrackClipType(typeof(InputClearClip))]
    [TrackColor(0.8f, 0.2f, 0.2f)]
    [TrackBindingType(typeof(UnityEngine.GameObject))]
    [DisplayName("BovineLabs/Player Inputs/Input Buffer Clear")]
    public class InputClearTrack : DOTSTrack { }
}
