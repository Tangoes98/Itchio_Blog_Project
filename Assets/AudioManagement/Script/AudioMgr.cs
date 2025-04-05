using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
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


        SetupAudioSourceDic(_audioSourceDatas, _audioSourceDataDic);
        SetupAudioClipDic(_audioClips, _audioClipDic);
        InitGroupVolume();
        SFX_Init();

    }
    #region Variables


    [Header("Mixer")]
    [SerializeField] AudioMixer _audioMixer;
    public AudioMixer AudioMixer => _audioMixer;

    [Header("AudioData")]
    [SerializeField] List<AudioClipData> _audioClips = new();


    [Header("SfxData")]
    [SerializeField] List<BatchedAudioClipData> _sfxClipDatas = new();

    [Header("Volume")]
    [SerializeField] float _masterVol;
    [SerializeField] float _musicVol;
    [SerializeField] float _sfxVol;
    [SerializeField] float _ambient;
    [SerializeField] public float InitMasterVol = 0.7f;
    [SerializeField] public float InitVol = 1f;
    [SerializeField] bool _isFading;

    [Header("AudioSourceData")]
    [SerializeField] List<AudioSourceData> _audioSourceDatas = new();
    Dictionary<string, AudioSourceData> _audioSourceDataDic = new();
    Dictionary<string, AudioClipData> _audioClipDic = new();




    #endregion
    #region Public Method

    #region Volume
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
            case VolumeType.Ambient:
                _ambient = vol;
                break;
        }
    }
    #endregion
    #region Load Audio
    public void LoadAudioClip(string clipName, SourceGroupType group)
    {
        AudioClipData clipData;
        AudioSource source = GetGroupSource(group);

        //*Try get music clip
        if (!_audioClipDic.ContainsKey(clipName))
            return;

        //*Get source and play the clip
        clipData = _audioClipDic[clipName];
        source.clip = clipData.AudiCilp;

        SetSourceChannelGroupVolume(group, 0f);

        //* Play in start volume
        PlayAudio(group, true);
    }

    public void PlayAudio(SourceGroupType group, bool isLoop)
    {
        GetGroupSource(group).loop = isLoop;
        GetGroupSource(group).Play();
    }

    #endregion
    #region Fade
    public void AudioFade(SourceGroupType group, float targetVol, float duration)
    {
        //StopAllCoroutines();
        // currentCor = 
        StartCoroutine(AudioFadeCoroutine(group.ToString(), targetVol, duration));
    }
    #endregion

    #region Music

    public void PlayAudio(SourceGroupType group, float targetVol, float fadeDuration, bool isLoop)
    {
        GetGroupSource(group).loop = isLoop;
        GetGroupSource(group).Play();

        //*Fade in the music clip through mixer
        //AudioFade(group, targetVol, fadeDuration);
    }


    public void StopAudio(SourceGroupType group)
        => GetGroupSource(group).Stop();

    public void PauseAudio(SourceGroupType group)
        => GetGroupSource(group).Pause();

    #endregion




    #region SFX
    void SFX_Init()
    {
        _sfxClipDatas.ForEach(item => item.InitRandom());
    }
    void LoadSfxClip(AudioClip clip, SourceGroupType group)
    {
        AudioSource source = GetGroupSource(group);
        source.clip = clip;
    }

    public void PlayRandomSfx(string sfxTypeName, SourceGroupType group)
    {
        var sfxData = GetSfxData(_sfxClipDatas, sfxTypeName);
        int randomClipIndex = sfxData.RandomValClass.GetNonDuplicatedRandomValue();
        LoadSfxClip(sfxData.AudiCilps[randomClipIndex], group);
        SetSourceChannelGroupVolume(group, sfxData.ClipVolume);
        GetGroupSource(group).Play();
    }
    public void PlaySfx(string sfxTypeName, SourceGroupType group, int sfxClipIndex)
    {
        var sfxData = GetSfxData(_sfxClipDatas, sfxTypeName);
        var clip = sfxData.AudiCilps[sfxClipIndex];
        LoadSfxClip(clip, group);
        SetSourceChannelGroupVolume(group, sfxData.ClipVolume);
        GetGroupSource(group).Play();
    }
    public void StopSfx(SourceGroupType group)
    {
        GetGroupSource(group).Stop();
    }
    public void SetSfxLoop(SourceGroupType group, bool isloop)
        => GetGroupSource(group).loop = isloop;

    BatchedAudioClipData GetSfxData(List<BatchedAudioClipData> batchData, string sfxName)
    {
        return batchData.Where(item => item.BatchDataName == sfxName).ToArray()[0];
    }

    #endregion


    #endregion
    //* (x=1, y=0), (x=0, y=-80)
    float ConvertNormVolume(float normVolume)
        => normVolume * 80f - 80f;

    float ConvertGroupVolume(float groupVolume)
        => (groupVolume + 80f) / 80f;

    AudioSource GetGroupSource(SourceGroupType group)
        => _audioSourceDataDic[group.ToString()].AudiSource;


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
        AudioSource[] sources = GenerateAudioSource();
        SourceGroupType[] channelTypes = (SourceGroupType[])Enum.GetValues(typeof(SourceGroupType));
        AudioSourceData[] sourceDataCount = new AudioSourceData[channelTypes.Length];

        dataList.AddRange(sourceDataCount);

        for (int i = 0; i < dataList.Count; i++)
        {
            var tempSourceData = dataList[i];
            tempSourceData.DataName = channelTypes[i].ToString();
            tempSourceData.AudiSource = sources[i];
            tempSourceData.SourceType = channelTypes[i];
            dataList[i] = tempSourceData;
        }
    }

    AudioSource[] GenerateAudioSource()
    {
        var groups = (SourceGroupType[])Enum.GetValues(typeof(SourceGroupType));
        GameObject child = new GameObject();
        child.name = "AudioSource";
        child.transform.parent = this.transform;
        for (int i = 0; i < groups.Length; i++)
        {
            child.AddComponent<AudioSource>();
        }
        return child.GetComponents<AudioSource>();
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
    public void SetSourceChannelGroupVolume(SourceGroupType channelType, float normVolume)
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
        _audioMixer.GetFloat("Ambient", out _ambient);

        //* Initialize Volume
        SetVolume(VolumeType.Master, InitMasterVol);
        SetVolume(VolumeType.Sfx, InitVol);
        SetVolume(VolumeType.Music, InitVol);
        SetVolume(VolumeType.Ambient, InitVol);

        

    }

    private void Update()
    {
        _audioMixer.SetFloat("MasterVol", _masterVol);
        _audioMixer.SetFloat("MusicVol", _musicVol);
        _audioMixer.SetFloat("SfxVol", _sfxVol);
        _audioMixer.SetFloat("Ambient", _ambient);


    }

    #endregion


}

#region DataType

public enum VolumeType
{
    Master, Music, Sfx, Ambient
}

//* AudioSource on each child object should match the number of SourceGroupType
public enum SourceGroupType
{
    Sfx1, Sfx2, Sfx3, Sfx4, Sfx5,
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
public class BatchedAudioClipData
{
    public string BatchDataName;
    public float ClipVolume;
    public AudioClip[] AudiCilps;
    public Ad_RandomValue RandomValClass;
    public void InitRandom()
        => RandomValClass = new(AudiCilps.Length);
}

[Serializable]
public struct AudioSourceData
{
    public string DataName;
    public SourceGroupType SourceType;
    public AudioSource AudiSource;
}



#endregion



#region Audio Fade

public class AudioFade
{
    AudioMixer _audioMixer;
    public AudioFade(AudioMixer mixer)
    {
        _audioMixer = mixer;
    }

    public IEnumerator AudioFadeCoroutine(SourceGroupType group, float targetVol, float duration)
    {
        string audioStr = group.ToString();
        float elipsedTime = 0f;
        _audioMixer.GetFloat(audioStr, out float currentVol);
        float groupVol = ConvertNormVolume(targetVol);

        while (elipsedTime < duration)
        {
            elipsedTime += Time.deltaTime;
            _audioMixer.SetFloat(audioStr, Mathf.Lerp(currentVol, groupVol, elipsedTime / duration));
            yield return null;
        }
        _audioMixer.SetFloat(audioStr, groupVol);
    }
    float ConvertNormVolume(float normVolume)
    => normVolume * 80f - 80f;
}
#endregion










#region Tool







#region Random Val
public class Ad_RandomValue
{
    int _min, _max;
    List<int> _usedValues = new();
    public Ad_RandomValue(int minInclude, int maxExclude)
    {
        _min = minInclude;
        _max = maxExclude;
    }
    public Ad_RandomValue(int listCount)
    {
        _min = 0;
        _max = listCount;
    }

    public int GetRandomValue()
      => UnityEngine.Random.Range(_min, _max);

    public int GetNonDuplicatedRandomValue()
    {
        if (_max == 0)
            return 0;

        int value = UnityEngine.Random.Range(_min, _max);
        //* Check if all values are used
        UsedValueCheck();
        while (_usedValues.Contains(value))
        {
            value = UnityEngine.Random.Range(_min, _max);
        }

        _usedValues.Add(value);
        return value;
    }
    void UsedValueCheck()
    {
        if (_usedValues.Count != _max - _min)
            return;

        var lastVal = _usedValues.Last();
        ResetUsedValues();
        _usedValues.Add(lastVal);
    }
    public void ResetUsedValues() => _usedValues.Clear();
}

#region Audio Config

[Serializable]
public struct Ad_AudioFXConfig
{
    public string AudioFX_Name;
    public bool IsLoop;
    [TextArea(2, 4)] public string Description;
    [Header("InGame")]
    public float TargetVolume;
    public float MaxVolume;
    public float MinVolume;
    public float FadeInDurtion;
    public float FadeOutDuration;

}




#endregion
#endregion
#endregion