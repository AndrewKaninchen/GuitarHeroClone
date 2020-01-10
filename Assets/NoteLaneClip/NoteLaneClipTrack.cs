using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[TrackColor(0.855f, 0.8623f, 0.87f)]
[TrackClipType(typeof(NoteLaneClip))]
[TrackBindingType(typeof(NoteLane))]
public class NoteLaneClipTrack : TrackAsset
{
    public List<TimelineClip> previousClips = new List<TimelineClip>();

    private Action onClipMoved;
    private Action onClipDestroyed;
    private GameManager gameManager;

    private void OnEnable()
    {
        EditorApplication.update += UpdateClips;
        previousClips.Clear();
        previousClips.AddRange(m_Clips);
        gameManager = FindObjectOfType<GameManager>();
    }

    private void OnDisable()
    {
        UpdateClips();
        
        // ReSharper disable once DelegateSubtraction
        EditorApplication.update -= UpdateClips;
    }

    private void UpdateClips()
    {
        //Debug.Log($"Current: {m_Clips.Count} Previous: {previousClips.Count}");
        
        
        var clipsToRemove = previousClips.Where((x) => !m_Clips.Contains(x)).ToList();
        
        
        foreach (var x in clipsToRemove)
        {
            Debug.Log(x);
            previousClips.Remove(x);
            onClipDestroyed();
        }
        
        var clipsToAdd = m_Clips.Where((x) => !previousClips.Contains(x)).ToList();
        clipsToAdd.ForEach(previousClips.Add);

        for (var i = 0; i < m_Clips.Count; i++)
        {
            var clip = m_Clips[i];
            if (clip == null)
            {
                m_Clips.RemoveAt(i);
                return;
            }

            if (!previousClips.Contains(clip)) previousClips.Add(clip);

//            var dif = (clip.start % gameManager.interval);
//            if (Mathf.Abs((float) (dif - (gameManager.interval / 2))) > Mathf.Epsilon)
//                clip.start -= dif;
//            else
//                clip.start += gameManager.interval - dif;
            
            
//            if (!Mathf.Approximately((float) (clip.start % gameManager.interval), 0f))
//                clip.start -= (clip.start % gameManager.interval);
            //clip.start = Mathf.Floor((float) clip.start);
            clip.duration = gameManager.tolerance;
        }
    }
    

    public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
    {
        var mixer = ScriptPlayable<NoteLaneClipMixerBehaviour>.Create (graph, inputCount);
        var mixerBehaviour = mixer.GetBehaviour();
        mixerBehaviour.clips = m_Clips;
        var director = FindObjectOfType<PlayableDirector>();
        mixerBehaviour.director = director;
        mixerBehaviour.gameManager = gameManager;
        var noteLane = director.GetGenericBinding(outputs.ElementAt(0).sourceObject) as NoteLane;
        //var bind = outputs.ElementAt(0);
        //Debug.Log(bind);
        //Debug.Log(huh);
        //onClipMoved += () => mixerBehaviour.DestroyKnob(noteLane);
        onClipDestroyed = () => mixerBehaviour.DestroyKnob(noteLane);
        return mixer;
    }
    
    protected override void OnCreateClip(TimelineClip clip)
    {
        base.OnCreateClip(clip);
        clip.duration = gameManager.tolerance;
        previousClips.Add(clip);
        
//        var asset = clip.asset as NoteLaneClip;
//        if (asset != null)
//        {
//            //asset.onDestroy += onClipDestroyed;
//            Debug.Log("Asset");
//        }
    }
}
