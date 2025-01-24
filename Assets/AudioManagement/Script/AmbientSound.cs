using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(SphereCollider))]
public class AmbientSound : MonoBehaviour
{
    [Space(3f), Header("Ambient Sound Settings")]
    [SerializeField] AudioClip _ambientClip;
    [SerializeField] AudioRolloffMode _rollOffMode = AudioRolloffMode.Linear;
    AudioSource _audioSource;
    SphereCollider _sphereCollider;
    AudioMixer _audioMixer;

    private void OnTriggerEnter(Collider other)
    {
        _audioSource.Play();
    }

    private void OnTriggerExit(Collider other)
    {
        _audioSource.Pause();
    }

    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        _sphereCollider = GetComponent<SphereCollider>();
        _sphereCollider.isTrigger = true;

        _audioSource.clip = _ambientClip;
        _audioSource.playOnAwake = false;
        _audioSource.loop = false;
        _audioSource.spatialBlend = 1;
        _audioSource.rolloffMode = _rollOffMode;
        _audioSource.minDistance = 0.1f;
        _audioSource.maxDistance = _sphereCollider.radius;

        _audioMixer = AudioMgr.Instance.AudioMixer;
        _audioSource.outputAudioMixerGroup = _audioMixer.FindMatchingGroups("Ambient")[0];
    }
}
