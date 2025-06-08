using System;
using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    public EventReference MusicReference;
    public EventInstance _musicInstance;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        PlayMusic(MusicReference);
    }

    public void PlayMusic(EventReference fmodEvent)
    {
        try
        {
            _musicInstance = RuntimeManager.CreateInstance(fmodEvent.Guid);

            _musicInstance.set3DAttributes(RuntimeUtils.To3DAttributes(new Vector3()));
            _musicInstance.start();
            //_musicInstance.release();
        }
        catch (Exception except)
        {
            Debug.LogError("Error playing FMOD EventReference: " + except.Message);
        }
    }

    public void UpdateMusicParameter(string fmodParemeter, float fmodParameterValue)
    {
        try
        {
            _musicInstance.setParameterByName(fmodParemeter, fmodParameterValue);
            _musicInstance.release();
        }
        catch (Exception e)
        {
            Debug.LogError("Error playing FMOD EventReference: " + e.Message);
        }
    }

    public void PlaySFX(EventReference fmodEvent)
    {
        try
        {
            var instance = RuntimeManager.CreateInstance(fmodEvent.Guid);

            instance.set3DAttributes(RuntimeUtils.To3DAttributes(new Vector3()));
            instance.start();
            instance.release();
        }
        catch (Exception except)
        {
            Debug.LogError("Error playing FMOD EventReference: " + except.Message);
        }
    }

    public void PlaySFX(EventReference fmodEvent, string fmodParameter, float fmodParameterValue)
    {
        try
        {
            var instance = RuntimeManager.CreateInstance(fmodEvent.Guid);
            instance.setParameterByName(fmodParameter, fmodParameterValue);

            instance.set3DAttributes(RuntimeUtils.To3DAttributes(new Vector3()));
            instance.start();
            instance.release();
        }
        catch (Exception except)
        {
            Debug.LogError("Error playing FMOD EventReference with parameter: " + except.Message);
        }
    }
}
