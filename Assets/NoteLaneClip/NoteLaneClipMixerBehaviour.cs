using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using Object = UnityEngine.Object;

public class NoteLaneClipMixerBehaviour : PlayableBehaviour
{
    public List<TimelineClip> clips;
    public PlayableDirector director;
    public NoteLaneClipTrack track;
    public GameManager gameManager;

    //private Stack<GameObject> knobs = new Stack<GameObject>();
    //public Dictionary<TimelineClip, GameObject> knobs = new Dictionary<TimelineClip, GameObject>();
    
    public void CreateKnob(NoteLane lane)
    {
        var knob = Object.Instantiate(lane.knobPrefab, lane.transform);
        lane.knobs.Add(knob);
    }
    
    public void DestroyKnob(NoteLane lane)
    {
        var knob = lane.knobs.Last();
        lane.knobs.Remove(knob);
        //Debug.Log($"Destroying {knob}");
        Object.DestroyImmediate(knob);
    }
    
    // NOTE: This function is called at runtime and edit time.  Keep that in mind when setting the values of properties.
    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        var noteLane = playerData as NoteLane;

        if (!noteLane)
            return;

        var inputCount = playable.GetInputCount ();
      
        //Debug.Log($"Knobs: {knobs.Count}\nInputs: {inputCount}");
        
        for (var i = noteLane.knobs.Count; i < clips.Count; i++)
        {
            CreateKnob(noteLane);
        }
    
        for (var i = 0; i < clips.Count && i < inputCount; i++)
        {
            var inputWeight = playable.GetInputWeight(i);
            var inputPlayable = (ScriptPlayable<NoteLaneClipBehaviour>)playable.GetInput(i);
            var input = inputPlayable.GetBehaviour ();

            var knob = noteLane.knobs.ElementAt(i);

            if (knob != null)
                knob.transform.localPosition = new Vector3(0f, 0f, (float) ((clips[i].start + gameManager.interval/2f)  - director.time) * gameManager.distanceBetweenBeats);
        }
    }
}
