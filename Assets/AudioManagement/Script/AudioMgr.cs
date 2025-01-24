using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

public class AudioMgr : MonoBehaviour
{
    public static AudioMgr Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
            Destroy(gameObject);
    }

    [SerializeField] AudioMixer _audioMixer;
    public AudioMixer AudioMixer => _audioMixer;


    [SerializeField] List<AudioSourceData> _audioSources = new(); //Wasted
    [SerializeField] List<AudioSourceData> _audioSourceDatas = new();
    [SerializeField] List<AudioClipData> _audioClips = new();

    Dictionary<string, AudioSourceData> _audioSourceDataDic = new();
    Dictionary<string, AudioClipData> _audioClipDic = new();

    [SerializeField] float _masterVol;
    [SerializeField] float _musicVol;
    [SerializeField] float _sfxVol;
    [SerializeField] float _genMscVol1;
    [SerializeField] bool _isFading;

    #region Public Method

    /// <summary>
    /// Input volume between 0-1, can be run at Update()
    /// </summary>
    /// <param name="volumeType"></param>
    /// <param name="volume"></param>
    public void SetVolume(VolumeType volumeType, float volume)
    {
        float vol = ConvertNormVolume(volume);
        switch (volumeType)
        {
            case VolumeType.Master:
                _masterVol = vol;
                break;
            case VolumeType.Music:
                _musicVol = vol;
                break;
            case VolumeType.Sfx:
                _sfxVol = vol;
                break;
        }
    }

    /// <summary>
    /// Target volume between 0-1, run once
    /// </summary>
    /// <param name="group"></param>
    /// <param name="targetVol"></param>
    /// <param name="duration"></param>
    public void AudioFade(SourceGroupType group, float targetVol, float duration)
    {
        StartCoroutine(AudioFadeCoroutine(group.ToString(), targetVol, duration));
    }


    public void PlayAudio(SourceGroupType group, float targetVol, float fadeDuration, bool isLoop)
    {
        GetGroupSource(group).loop = isLoop;
        GetGroupSource(group).Play();

        //*Fade in the music clip through mixer
        AudioFade(group, targetVol, fadeDuration);
    }

    public void StopAudio(SourceGroupType group)
        => GetGroupSource(group).Stop();

    public void PauseAudio(SourceGroupType group)
        => GetGroupSource(group).Pause();

    public void LoadAudioCLip(string clipName, SourceGroupType group)
    {
        AudioClipData clipData;
        AudioSource source = GetGroupSource(group);

        //*Try get music clip
        if (!_audioClipDic.ContainsKey(clipName))
            return;

        //*Get source and play the clip
        clipData = _audioClipDic[clipName];
        source.clip = clipData.AudiCilp;
    }

    #region SFX

    public void PlaySfx(SourceGroupType group)
        => GetGroupSource(group).Play();
    public void PlaySfx(SourceGroupType group, float volume)
    {
        SetSourceChannelGroupVolume(group, volume);
        GetGroupSource(group).Play();
    }
    public void PlaySfx(string clipName, SourceGroupType group, float volume)
    {
        LoadAudioCLip(clipName, group);
        SetSourceChannelGroupVolume(group, volume);
        GetGroupSource(group).Play();
    }


    #endregion





    #endregion
    float ConvertNormVolume(float normVolume)
    {
        return normVolume * 80f - 80f;
    }
    float ConvertGroupVolume(float groupVolume)
    {
        return (groupVolume + 80f) / 80f;
    }
    AudioSource GetGroupSource(SourceGroupType group)
    {
        return _audioSourceDataDic[group.ToString()].AudiSource;
    }

    #region Dic Setup
    void SetupAudioClipDic(List<AudioClipData> dataList, Dictionary<string, AudioClipData> dic)
    {
        dataList.ForEach(
            data => dic.Add(data.Name, data)
        );
    }

    void SetupAudioSourceDic(List<AudioSourceData> dataList, Dictionary<string, AudioSourceData> dic)
    {
        SetupAudioSourceDataList(dataList);

        foreach (var item in dataList)
        {
            dic.Add(item.SourceType.ToString(), item);
            item.AudiSource.outputAudioMixerGroup = _audioMixer.FindMatchingGroups(item.SourceType.ToString())[0];
        }
    }

    void SetupAudioSourceDataList(List<AudioSourceData> dataList)
    {
        AudioSource[] sources = this.GetComponentsInChildren<AudioSource>();
        SourceGroupType[] channelTypes = (SourceGroupType[])Enum.GetValues(typeof(SourceGroupType));
        AudioSourceData[] sourceDataCount = new AudioSourceData[channelTypes.Length];

        dataList.AddRange(sourceDataCount);

        for (int i = 0; i < dataList.Count; i++)
        {
            var tempSourceData = dataList[i];
            tempSourceData.AudiSource = sources[i];
            tempSourceData.SourceType = channelTypes[i];
            dataList[i] = tempSourceData;
        }
    }

    #endregion

    #region Fade

    IEnumerator AudioFadeCoroutine(string audioStr, float targetVol, float duration)
    {
        float elipsedTime = 0f;
        _audioMixer.GetFloat(audioStr, out float currentVol);
        float groupVol = ConvertNormVolume(targetVol);

        while (elipsedTime < duration)
        {
            _isFading = true;
            elipsedTime += Time.deltaTime;
            _audioMixer.SetFloat(audioStr, Mathf.Lerp(currentVol, groupVol, elipsedTime / duration));
            yield return null;
        }

        _audioMixer.SetFloat(audioStr, groupVol);
        _isFading = false;
    }
    #endregion


    #region GroupVolume

    float GetSourceChannelGroupNormVolume(SourceGroupType channelType)
    {
        float outVolume;
        _audioMixer.GetFloat(channelType.ToString(), out outVolume);
        return ConvertGroupVolume(outVolume);
    }
    void SetSourceChannelGroupVolume(SourceGroupType channelType, float normVolume)
    {
        _audioMixer.SetFloat(channelType.ToString(), ConvertNormVolume(normVolume));
    }

    void InitGroupVolume()
    {
        SourceGroupType[] channelTypes = (SourceGroupType[])Enum.GetValues(typeof(SourceGroupType));
        channelTypes.ToList().ForEach(
            channelType => SetSourceChannelGroupVolume(channelType, 0f)
        );
    }


    #endregion

    #region Start

    private void Start()
    {
        _audioMixer.GetFloat("MasterVol", out _masterVol);
        _audioMixer.GetFloat("MusicVol", out _musicVol);
        _audioMixer.GetFloat("SfxVol", out _sfxVol);

        SetupAudioSourceDic(_audioSourceDatas, _audioSourceDataDic);
        SetupAudioClipDic(_audioClips, _audioClipDic);

        InitGroupVolume();

    }

    private void Update()
    {
        _audioMixer.SetFloat("MasterVol", _masterVol);
        _audioMixer.SetFloat("MusicVol", _musicVol);
        _audioMixer.SetFloat("SfxVol", _sfxVol);


        //_audioMixer.SetFloat("GenMscVol1", _genMscVol1);


        //! Debug

        if (DebugInput(KeyCode.Space))
        {
            // LoadAudioCLip("BGM1", SourceGroupType.GenMusic1);
        }
        if (DebugInput(KeyCode.C))
        {
            //PlayAudio(SourceGroupType.GenMusic1, 1f, 0f);
        }
        if (DebugInput(KeyCode.V))
        {
            StopAudio(SourceGroupType.GenMusic1);
        }
        if (DebugInput(KeyCode.B))
        {
            PauseAudio(SourceGroupType.GenMusic1);
        }
        if (DebugInput(KeyCode.N))
        {
            LoadAudioCLip("BGM2", SourceGroupType.GenMusic1);
        }






    }

    #endregion


    bool DebugInput(KeyCode keyCode)
    => Input.GetKeyDown(keyCode);




}


public enum VolumeType
{
    Master, Music, Sfx
}
public enum SourceGroupType
{
    Sfx1, Sfx2, Sfx3, Sfx4,
    GenMusic1, GenMusic2,
    GenVertical, Vertical1, Vertical2, Vertical3, Vertical4, Vertical5
}

[Serializable]
public struct AudioClipData
{
    public string Name;
    public AudioClip AudiCilp;
}

[Serializable]
public struct AudioSourceData
{
    public string Name;
    public SourceGroupType SourceType;
    public AudioSource AudiSource;
}


