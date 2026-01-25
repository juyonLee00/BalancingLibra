using System.Collections.Generic;
using UnityEngine;

public class AudioManager : SingletonBehaviour<AudioManager>
{
    private Dictionary<string, AudioClip> _audioClips = new Dictionary<string, AudioClip>();

    private AudioSource _bgmPlayer;
    private AudioSource _sfxPlayer;

    protected override void Init()
    {
        base.Init();

        GameObject bgmObject = new GameObject("BGM_Player");
        bgmObject.transform.SetParent(transform);
        _bgmPlayer = bgmObject.AddComponent<AudioSource>();
        _bgmPlayer.loop = true; 
        _bgmPlayer.playOnAwake = false;

        GameObject sfxObject = new GameObject("SFX_Player");
        sfxObject.transform.SetParent(transform);
        _sfxPlayer = sfxObject.AddComponent<AudioSource>();
        _sfxPlayer.loop = false;
        _sfxPlayer.playOnAwake = false;

        Logger.Log("AudioManager : Initialized");
    }

    // 배경음악 재생
    public void PlayBGM(string bgmName, float volume = 1.0f)
    {
        if (string.IsNullOrEmpty(bgmName))
        {
            Logger.Log("AudioManager.PlayBGM: bgmName is null or empty");
            return;
        }

        if (_bgmPlayer == null)
        {
            Logger.Log("AudioManager.PlayBGM: _bgmPlayer is not initialized");
            return;
        }

        if (_bgmPlayer.clip != null && _bgmPlayer.clip.name == bgmName)
            return;

        AudioClip clip = GetOrLoadClip(bgmName, "BGM");
        if (clip != null)
        {
            _bgmPlayer.clip = clip;
            _bgmPlayer.volume = volume;
            _bgmPlayer.Play();
            Logger.Log($"Play BGM : {bgmName}");
        }
        else
        {
            Logger.Log($"AudioManager.PlayBGM: Failed to load clip '{bgmName}'");
        }
    }

    // 효과음 재생
    public void PlaySFX(string sfxName, float volume = 1.0f)
    {
        if (string.IsNullOrEmpty(sfxName))
        {
            Logger.Log("AudioManager.PlaySFX: sfxName is null or empty");
            return;
        }

        if (_sfxPlayer == null)
        {
            Logger.Log("AudioManager.PlaySFX: _sfxPlayer is not initialized");
            return;
        }

        AudioClip clip = GetOrLoadClip(sfxName, "SFX");
        if (clip != null)
        {
            _sfxPlayer.PlayOneShot(clip, volume);
        }
        else
        {
            Logger.Log($"AudioManager.PlaySFX: Failed to load clip '{sfxName}'");
        }
    }

    // 소리 끄기
    public void StopBGM()
    {
        if (_bgmPlayer == null)
        {
            Logger.Log("AudioManager.StopBGM: _bgmPlayer is not initialized");
            return;
        }
        _bgmPlayer.Stop();
    }

    // 음소거 기능
    public void Mute(bool isMute)
    {
        if (_bgmPlayer == null || _sfxPlayer == null)
        {
            Logger.Log("AudioManager.Mute: AudioSource is not initialized");
            return;
        }
        _bgmPlayer.mute = isMute;
        _sfxPlayer.mute = isMute;
    }

    // 리소스 로드
    private AudioClip GetOrLoadClip(string name, string type)
    {
        if (_audioClips.TryGetValue(name, out AudioClip clip))
        {
            return clip;
        }

        string path = $"Sounds/{type}/{name}"; // 경로: Resources/Sounds/BGM/이름
        clip = Resources.Load<AudioClip>(path);

        if (clip == null)
        {
            Logger.Log($"Audio Clip Missing! Path: {path}");
            return null;
        }

        _audioClips.Add(name, clip);
        return clip;
    }
    
    // 씬 이동 시 사용하지 않는 소리 메모리 해제 (선택)
    public void ClearCache()
    {
        _audioClips.Clear();
        Resources.UnloadUnusedAssets();
    }
}