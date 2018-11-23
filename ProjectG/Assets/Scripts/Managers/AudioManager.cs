using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Audio;
using System;

public class AudioManager : Singleton<AudioManager>
{
    //- VARIABLES -----------------------------------------------------------

    public enum ChannelType
    {
        MUSIC,
        AMBIENCE,
        AMBIENTTWO,
        FX,
        VO
    }
    // Narator is a different category, has only one available track

    public GameObject GlobalSourceObj { get; private set; }
    public AudioMixer MainMixer { private get; set; }

    // 2D Audio sources
    private List<AudioSource> m_MusicTracks = new List<AudioSource>();
    private List<AudioSource> m_AmbienceTracks = new List<AudioSource>();
    private List<AudioSource> m_AmbientTwo = new List<AudioSource>();
    private List<AudioSource> m_FXTracks = new List<AudioSource>();
    private List<AudioSource> m_VOTracks = new List<AudioSource>();

    // 3D Audio sources
    private List<AudioSource> m_Scene3DTracks = new List<AudioSource>();
    private List<GameObject> m_3DTracks = new List<GameObject>();

    private AudioSource m_NaratorVOTrack;

    // Narator Variables
    private bool m_IsPlayingNaratorVo;
    private float m_NaratorVoTimer;
    private float m_NaratorVoTrigger;
    private Action OnVoComplete = delegate { };

    private Queue<NaratorVOClip> m_QueuedNaratorVO = new Queue<NaratorVOClip>();
    private NaratorVOClip m_CurrentVO = null;

    // Fade Variables
    private class FadeInfo
    {
        public AudioSource[] FadeTracks;
        public float[] OriginalVolume;
        public float TimeToFade;
        public float ToValue;
        public float Progress;
        public Action OnFadeComplete;
    }

    private List<FadeInfo> m_FadeList = new List<FadeInfo>();

    //- SETUP / UPDATE ----------------------------------------------------------

    /// <summary>
    /// Initialise the Audio manager. It is required to be called once at the beginning function properly.
    /// </summary>
    public void Init()
    {
        if(GlobalSourceObj == null)
        {
            GlobalSourceObj = new GameObject("GlobalAudioManager");

            //Init Narator
            m_NaratorVOTrack = GlobalSourceObj.AddComponent<AudioSource>();
            if(MainMixer != null)
            {
                m_NaratorVOTrack.outputAudioMixerGroup = MainMixer.outputAudioMixerGroup;
            }
        }

        //m_AData = Resources.Load<AudioData>("ScriptableObjects/AudioData");
    }

    /// <summary>
    /// Update function for the manager. Make sure to update it for all the audio features to work properly.
    /// </summary>
    public void Update()
    {
        if(m_IsPlayingNaratorVo)
        {
            m_NaratorVoTimer += Time.deltaTime;
            if(m_NaratorVoTimer > m_NaratorVoTrigger)
            {
                m_IsPlayingNaratorVo = false;
                OnVoComplete();
                if(m_QueuedNaratorVO.Count > 0)
                {
                    PlayNaratorVO(m_QueuedNaratorVO.Dequeue(), delegate { });
                }
            }
        }

        if(m_FadeList.Count > 0)
        {
            for(int i = 0; i < m_FadeList.Count; i++)
            {
                FadeInfo info = m_FadeList[i];
                FadeTrack(ref info);
            }
        }
    }

    //- PLAYBACK FUNCTIONS ----------------------------------------------------

    // - 2D Audio Generic Play
    /// <summary>
    /// Play a 2D audio at the specified channel.
    /// </summary>
    /// <param name="clip"> Clip to play</param>
    /// <param name="type"> Channel type</param>
    /// <param name="loop"> Should the clip loop when it ends</param>
    public void Play2DAudio(AudioClip clip, ChannelType type, bool loop = false)
    {
        AudioSource trackToPlay = GetAvailable2DTrack(type);
        if(trackToPlay == null) return;
        trackToPlay.pitch = 1;
        trackToPlay.clip = clip;
        trackToPlay.loop = loop;
        trackToPlay.Play();
    }

    /// <summary>
    /// Play a 2D audio at the specified channel.
    /// </summary>
    /// <param name="clip"> Clip to play</param>
    /// <param name="type"> Channel type</param>
    /// <param name="loop"> Should the clip loop when it ends</param>
    /// <param name="source"> Returns the audio source in case the reference needs to be kept by the calling class</param>
    public void Play2DAudio(AudioClip clip, ChannelType type, bool loop, out AudioSource source)
    {
        AudioSource trackToPlay = GetAvailable2DTrack(type);
        source = trackToPlay;
        if(trackToPlay == null) return;
        trackToPlay.pitch = 1;
        trackToPlay.clip = clip;
        trackToPlay.loop = loop;
        trackToPlay.Play();
    }

    public void Play2DAudioGroup(ChannelType type, bool loop, params AudioClip[] clips)
    { }

    // - 3D Audio Generic Play
    /// <summary>
    /// Play a 3D audio at the specified channel and location.
    /// </summary>
    /// <param name="clip"> Clip to play</param>
    /// <param name="position"> Position of the new 3D source</param>
    /// <param name="minDistance"> Minumum distance that the listener can hear this source</param>
    /// <param name="maxDistance"> Maximum distance that the listener can hear this source</param>
    /// <param name="loop"> Should the clip loop when it ends</param>
    public void Play3DAudio(AudioClip clip, Vector3 position, float minDistance, float maxDistance, bool loop = false)
    {
        GameObject audioPoint = GetAvailable3DTrack();
        AudioSource trackToPlay = audioPoint.GetComponent<AudioSource>();
        if(trackToPlay == null) return;

        audioPoint.transform.position = position;
        trackToPlay.minDistance = minDistance;
        trackToPlay.maxDistance = maxDistance;

        trackToPlay.pitch = 1;
        trackToPlay.clip = clip;
        trackToPlay.loop = loop;
        trackToPlay.Play();
    }

    /// <summary>
    /// Play a 3D audio at the specified channel and location.
    /// </summary>
    /// <param name="clip"> Clip to play</param>
    /// <param name="position"> Position of the new 3D source</param>
    /// <param name="minDistance"> Minumum distance that the listener can hear this source</param>
    /// <param name="maxDistance"> Maximum distance that the listener can hear this source</param>
    /// <param name="loop"> Should the clip loop when it ends</param>
    /// <param name="source"> Returns the audio source in case the reference needs to be kept by the calling class</param>
    public void Play3DAudio(AudioClip clip, Vector3 position, float minDistance, float maxDistance, bool loop, out AudioSource source)
    {
        GameObject audioPoint = GetAvailable3DTrack();
        AudioSource trackToPlay = audioPoint.GetComponent<AudioSource>();
        source = trackToPlay;
        if(trackToPlay == null) return;

        audioPoint.transform.position = position;
        trackToPlay.minDistance = minDistance;
        trackToPlay.maxDistance = maxDistance;

        trackToPlay.pitch = 1;
        trackToPlay.clip = clip;
        trackToPlay.loop = loop;
        trackToPlay.Play();
    }


    // - Generic Pause / Resume
    /// <summary>
    /// Pause the specified channel.
    /// </summary>
    /// <param name="type">Channel to be paused</param>
    public void PauseChannel(ChannelType type)
    {
        List<AudioSource> trackList = GetTrackList(type);
        for(int i = 0; i < trackList.Count; i++)
        {
            trackList[i].Pause();
        }
    }

    /// <summary>
    /// Resume the specified channel.
    /// </summary>
    /// <param name="type">Channel to be resumed</param>
    public void ResumeChannel(ChannelType type)
    {
        List<AudioSource> trackList = GetTrackList(type);
        for(int i = 0; i < trackList.Count; i++)
        {
            trackList[i].UnPause();
        }
    }

    // - Generic Stop / Clear tracks
    /// <summary>
    /// Stops the specified channel.
    /// </summary>
    /// <param name="type">Channel to be stopped</param>
    public void StopChannel(ChannelType type)
    {
        List<AudioSource> trackList = GetTrackList(type);
        for(int i = 0; i < trackList.Count; i++)
        {
            trackList[i].Stop();
        }
    }

    /// <summary>
    /// Stop all channels and clears any naration queue.
    /// </summary>
    public void StopAll()
    {
        StopChannel(ChannelType.MUSIC);
        StopChannel(ChannelType.AMBIENCE);
        StopChannel(ChannelType.FX);
        StopChannel(ChannelType.VO);
        m_NaratorVOTrack.Stop();
        m_QueuedNaratorVO.Clear();
        m_IsPlayingNaratorVo = false;
    }

    // - Shortcut SFX Functions since it's the most common used play
    /// <summary>
    /// Shortcut to the Play2DAudio that automatically plays on the SFX Channel.
    /// </summary>
    /// <param name="clip"> Clip to play</param>
    /// <param name="loop"> Should the clip loop when it ends</param>
    public void PlaySFX(AudioClip clip, bool loop = false)
    {
        AudioSource source = GetAvailable2DTrack(ChannelType.FX);
        source.pitch = 1;
        source.loop = loop;
        source.clip = clip;
        source.Play();
    }

    /// <summary>
    /// Shortcut to the Play2DAudio that automatically plays on the SFX Channel.
    /// </summary>
    /// <param name="clip"> Clip to play</param>
    /// <param name="loop"> Should the clip loop when it ends</param>
    /// <param name="source"> Returns the audio source in case the reference needs to be kept by the calling class</param>
    public void PlaySFX(AudioClip clip, bool loop, out AudioSource fxSource)
    {
        AudioSource source = GetAvailable2DTrack(ChannelType.FX);
        fxSource = source;
        source.pitch = 1;
        source.loop = loop;
        source.clip = clip;
        source.Play();
    }

    // - Narator Play
    /// <summary>
    /// Play a narator VO clip. The narator channel is separate from the channel list. Only ONE naration voice can play.
    /// If the narator channel is ALREADY playing a clip, the specified clip will be queued.
    /// </summary>
    /// <param name="voClip"> Narator VO clip to be played/queued</param>
    /// <param name="OnComplete">OnComplete action for when this specified clip finishes playing</param>
    public void PlayNaratorVO(NaratorVOClip voClip, Action OnComplete)
    {
        AudioClip clip = voClip.Clip;
        if(clip == null)
        {
            Debug.LogError("Missing Sound Clip: " + voClip.ToString());
            OnComplete();
            return;
        }

        if(m_IsPlayingNaratorVo)
        {
            if(m_CurrentVO.ShouldCut)
            {
                m_NaratorVOTrack.Stop();
                m_IsPlayingNaratorVo = false;
                OnVoComplete();
            }
            else
            {
                m_QueuedNaratorVO.Enqueue(voClip);
                return;
            }
        }

        OnVoComplete = OnComplete;
        m_NaratorVoTimer = 0;
        m_NaratorVoTrigger = clip.length;
        m_IsPlayingNaratorVo = true;
        m_NaratorVOTrack.clip = clip;
        m_NaratorVOTrack.Play();
        m_CurrentVO = voClip;
        Debug.Log("PLAYING NARATOR VO: [ " + voClip.Clip.name.ToString() + " ]");
    }

    /// <summary>
    /// Pause the narator channel.
    /// </summary>
    public void NaratorPause()
    {
        m_NaratorVOTrack.Pause();
        m_IsPlayingNaratorVo = false;
    }

    /// <summary>
    /// Resume the narator channel.
    /// </summary>
    public void NaratorResume()
    {
        m_NaratorVOTrack.UnPause();
        m_IsPlayingNaratorVo = true;
    }

    /// <summary>
    /// Stop the narator Channel.
    /// </summary>
    public void NaratorStop()
    {
        m_NaratorVOTrack.Stop();
        m_IsPlayingNaratorVo = false;
    }

    //- TRACK OPTIONS -----------------------------------------------------

    // - Sound Effects
    /// <summary>
    /// Switch to the specified snapshot, if there is an available mixer (MainMixer) and a configured snapshop.
    /// </summary>
    /// <param name="snapshotName"> The name of the snapshot (case sensitive)</param>
    /// <param name="blendTime"> Time to blend to the new snapshot</param>
    public void EnableSnapshot(string snapshotName, float blendTime)
    {
        if(MainMixer == null)
        {
            Debug.LogError("There is no available mixer or it is not assigned correctly.");
            return;
        }
        AudioMixerSnapshot snap = MainMixer.FindSnapshot(snapshotName);
        snap.TransitionTo(blendTime);
    }

    /// <summary>
    /// Set the pitch of the given AudioSource.
    /// </summary>
    /// <param name="source"> Audiosource to use</param>
    /// <param name="value"> Value of the pitch</param>
    public void SetPitch(AudioSource source, float value)
    {
        source.pitch = value;
    }

    // - Fade
    /// <summary>
    /// Fade to a specified value for a specific channel. Can be used to equialise all the available tracks in the channel uniformly.
    /// </summary>
    /// <param name="channel"> The type of the channel to fade</param>
    /// <param name="toValue"> The value for the channel to fade to (from 0.0f to 1.0f)</param>
    /// <param name="timeToFade"> Time to reach the fade value</param>
    /// <param name="onFadeComplete"> OnComplete action for when the reached the fade value</param>
    public void FadeChannel(ChannelType channel, float toValue, float timeToFade, Action onFadeComplete = null)
    {
        FadeInfo info = new FadeInfo();
        info.FadeTracks = GetTrackList(channel).ToArray();
        info.OriginalVolume = new float[info.FadeTracks.Length];
        for(int i = 0; i < info.OriginalVolume.Length; i++)
        {
            info.OriginalVolume[i] = info.FadeTracks[i].volume;
        }
        info.ToValue = toValue;
        info.TimeToFade = timeToFade;
        info.OnFadeComplete = onFadeComplete;
        m_FadeList.Add(info);
    }

    /// <summary>
    /// Cross fade to a new clip on specific channel.
    /// </summary>
    /// <param name="channel"> The type of the channel to fade</param>
    /// <param name="toClip"> The new clip to fade into</param>
    /// <param name="timeToCrossfade"> The time to take to perform the cross fade. (time halfed for the fade out/in)</param>
    /// <param name="newClipToLoop"> Should the new clip loop</param>
    public void CrossfadeTo(ChannelType channel, AudioClip toClip, float timeToCrossfade, float crossfadeValue = 0, bool newClipToLoop = false)
    {
        FadeChannel(channel, crossfadeValue, timeToCrossfade * 0.5f,
            delegate
            {
                StopChannel(channel);
                Play2DAudio(toClip, channel, newClipToLoop);
                FadeChannel(channel, 1, timeToCrossfade * 0.5f);
            });
    }

    // - Sounds
    /// <summary>
    /// Set the volume of all the available tracks directly to the specified channel. (Can also be set from the mixer)
    /// </summary>
    /// <param name="channel"> Channel to change</param>
    /// <param name="value"> Percentage of the new volume (0.0f to 1.0f) (</param>
    public void SetChannelVolume(ChannelType channel, float value)
    {
        AudioSource[] tracks = GetTrackList(channel).ToArray();
        for(int i = 0; i < tracks.Length; i++)
        {
            tracks[i].volume = value;
        }
    }

    //- POOL SYSTEM -------------------------------------------------------

    // - 2D Pool
    private AudioSource GetAvailable2DTrack(ChannelType type)
    {
        AudioSource track = null;
        List<AudioSource> trackList = GetTrackList(type);

        for(int i = 0; i < trackList.Count; i++)
        {
            if(!trackList[i].isPlaying)
            {
                return trackList[i];
            }
        }
        if(track == null)
        {
            track = GlobalSourceObj.AddComponent<AudioSource>();
            if(MainMixer != null)
            {
                track.outputAudioMixerGroup = MainMixer.outputAudioMixerGroup;
            }
            trackList.Add(track);
        }
        return track;
    }

    /// <summary>
    /// Clear and delete all the tracks from the specified channel. Should only be used when memory need to be cleared.
    /// </summary>
    /// <param name="type"> Channel to clear</param>
    public void Clear2DTracks(ChannelType type)
    {
        List<AudioSource> trackList = GetTrackList(type);
        StopChannel(type);
        for(int i = 0; i < trackList.Count; i++)
        {
            GameObject.Destroy(trackList[i]);
        }
        trackList.Clear();
    }

    /// <summary>
    /// Clear and delete all the 2D channels. Should only be used when memory need to be cleared.
    /// </summary>
    public void ClearAll2DTracks()
    {
        Clear2DTracks(ChannelType.MUSIC);
        Clear2DTracks(ChannelType.AMBIENCE);
        Clear2DTracks(ChannelType.FX);
        Clear2DTracks(ChannelType.VO);
    }

    // - 3D Pool
    private GameObject GetAvailable3DTrack()
    {
        for(int i = 0; i < m_3DTracks.Count; i++)
        {
            if(!m_3DTracks[i].GetComponent<AudioSource>().isPlaying)
            {
                return m_3DTracks[i];
            }
        }
        GameObject go = new GameObject("3DAudioPoint");
        AudioSource track = go.AddComponent<AudioSource>();
        if(MainMixer != null)
        {
            track.outputAudioMixerGroup = MainMixer.outputAudioMixerGroup;
        }

        m_3DTracks.Add(go);
        return go;
    }

    /// <summary>
    /// Clear and delete all the 3D channels. Should only be used when memory need to be cleared.
    /// </summary>
    public void Clear3DTracks()
    {
        for(int i = 0; i < m_3DTracks.Count; i++)
        {
            GameObject.Destroy(m_3DTracks[i]);
        }
        m_3DTracks.Clear();
    }

    //- 3D AUDIO SOURCE FUNCTIONS ------------------------------------------

    /// <summary>
    /// Find all the available 3D AudioSources currently in the scene.
    /// </summary>
    /// <returns> A list with all the available 3D AudioSources</returns>
    public List<AudioSource> FindAll3DAudioSourcesInScene()
    {
        AudioSource[] allSources = GameObject.FindObjectsOfType<AudioSource>();
        for(int i = 0; i < allSources.Length; i++)
        {
            if(allSources[i].gameObject.name == "GlobalAudioManager") continue;
            m_Scene3DTracks.Add(allSources[i]);
        }
        return m_Scene3DTracks;
    }

    public AudioSource Get3DAudioSourceInScene(string sourceName)
    {
        //TO-DO
        return null;
    }

    /// <summary>
    /// Clears (but not deletes) all the 3D Audiosources found by the <cref>FindAll3DAudioSourcesInScene</cref>
    /// </summary>
    public void Clear3DAudioSources()
    {
        m_Scene3DTracks.Clear();
    }

    //- UTILITY / HELPERS --------------------------------------------------

    private List<AudioSource> GetTrackList(ChannelType type)
    {
        List<AudioSource> trackList = null;
        switch(type)
        {
            case ChannelType.MUSIC:
                trackList = m_MusicTracks;
                break;
            case ChannelType.AMBIENCE:
                trackList = m_AmbienceTracks;
                break;
            case ChannelType.AMBIENTTWO:
                trackList = m_AmbientTwo;
                break;
            case ChannelType.FX:
                trackList = m_FXTracks;
                break;
            case ChannelType.VO:
                trackList = m_VOTracks;
                break;
        }
        return trackList;
    }

    // TO - DO Exporting / Importing settings for the audio
    /// <summary>
    /// Export all the audio settings into a AudioSettings Class to be handled by any saving technique.
    /// </summary>
    /// <returns> A class with data to be exported and saved</returns>
    public AudioSettings ExportAudioSettings()
    {
        AudioSettings settings = new AudioSettings();
        if(GetTrackList(ChannelType.MUSIC).Count > 0)
        {
            settings.MusicChannelVolume = GetTrackList(ChannelType.MUSIC)[0].volume;
        }
        if(GetTrackList(ChannelType.AMBIENCE).Count > 0)
        {
            settings.AmbienceChannelVolume = GetTrackList(ChannelType.AMBIENCE)[0].volume;
        }
        if(GetTrackList(ChannelType.FX).Count > 0)
        {
            settings.SFXChannelVolume = GetTrackList(ChannelType.FX)[0].volume;
        }
        if(GetTrackList(ChannelType.VO).Count > 0)
        {
            settings.VOChannelVolume = GetTrackList(ChannelType.VO)[0].volume;
        }
        settings.NaratorChannelVolume = m_NaratorVOTrack.volume;
        return settings;
    }

    /// <summary>
    /// Import any AudioSettings and apply them to the current AudioManager.
    /// </summary>
    /// <param name="settings"> Settings to be imported</param>
    public void ImportAudioSettings(AudioSettings settings)
    {
        SetChannelVolume(ChannelType.MUSIC, settings.MusicChannelVolume);
        SetChannelVolume(ChannelType.AMBIENCE, settings.AmbienceChannelVolume);
        SetChannelVolume(ChannelType.FX, settings.SFXChannelVolume);
        SetChannelVolume(ChannelType.VO, settings.VOChannelVolume);
        m_NaratorVOTrack.volume = settings.NaratorChannelVolume;
    }

    // Fade Helpers
    private void FadeTrack(ref FadeInfo info)
    {
        info.Progress += Time.deltaTime / info.TimeToFade;

        for(int i = 0; i < info.FadeTracks.Length; i++)
        {
            float rate = Mathf.Lerp(info.OriginalVolume[i], info.ToValue, info.Progress);
            info.FadeTracks[i].volume = rate;
        }
        if(info.Progress >= 1.0f)
        {
            m_FadeList.Remove(info);
            if(info.OnFadeComplete != null)
            {
                info.OnFadeComplete();
            }
            info = null;
        }
    }

    //--------------------------------------------------------------------
}

[Serializable]
public class NaratorVOClip
{
    public AudioClip Clip;
    public bool ShouldCut;
    [HideInInspector]
    public Action OnComplete;
}

[Serializable]
public class AudioSettings
{
    public float MusicChannelVolume;
    public float AmbienceChannelVolume;
    public float SFXChannelVolume;
    public float VOChannelVolume;
    public float NaratorChannelVolume;

    public AudioSettings()
    {
        MusicChannelVolume = 0.5f;
        AmbienceChannelVolume = 0.5f;
        SFXChannelVolume = 0.5f;
        VOChannelVolume = 0.5f;
        NaratorChannelVolume = 0.5f;
    }

    public AudioSettings(float musicVol, float ambienceVol, float sfxVol, float VOVol, float naratorVol)
    {
        MusicChannelVolume = musicVol;
        AmbienceChannelVolume = ambienceVol;
        SFXChannelVolume = sfxVol;
        VOChannelVolume = VOVol;
        NaratorChannelVolume = naratorVol;
    }
}