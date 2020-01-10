using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
public class NoteLaneClip : PlayableAsset, ITimelineClipAsset
{
    public NoteLaneClipBehaviour template = new NoteLaneClipBehaviour ();

    public ClipCaps clipCaps => ClipCaps.None;

    public override Playable CreatePlayable (PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<NoteLaneClipBehaviour>.Create (graph, template);
        //var clone = playable.GetBehaviour ();
        
        return playable;
    }

    public Action onDestroy;

    private void OnDestroy()
    {
        //Debug.Log("Destroyed");
        onDestroy?.Invoke();
    }
}
