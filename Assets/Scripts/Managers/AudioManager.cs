using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public enum BGMType
{
    Title,
    InGame,
    FeverTime,
    GameOver
}

public enum SFXType
{
    Click,
    BallDrop,
    BallMerge,
    BlackHole,
    GameOver,
    Warning,
    LifeGone
}

public class AudioManager : SingletonBehaviour<AudioManager>
{
    [Header("Audio Mixer")]
    public AudioMixerGroup bgmMixerGroup;
    public AudioMixerGroup sfxMixerGroup;

    private Dictionary<string, AudioClip> _audioClips = new Dictionary<string, AudioClip>();

    private AudioSource _bgmPlayer;

    private AudioSource _loopSfxPlayer;

    // 다중 효과음 처리를 위한 SFX 리스트
    private List<AudioSource> _sfxPlayers = new List<AudioSource>();
    private int maxSfxPlayers = 10; 

    private BGMType _currentBGM;
    private bool _hasBGM = false;

    // bgm 캐싱 배열 (Enum 순서와 동일해야 함)
    private readonly string[] _bgmNames = { "Title", "InGame", "FeverTime", "GameOver" };

    // sfx 캐싱 배열 (Enum 순서와 동일해야 함)
    private readonly string[] _sfxNames = { 
        "Click", 
        "BallDrop", 
        "BallMerge", 
        "BlackHole", 
        "GameOver", 
        "Warning",
        "LifeGone" 
    };

    protected override void Init()
    {
        base.Init();

        GameObject bgmObject = new GameObject("BGM_Player");
        bgmObject.transform.SetParent(transform);
        _bgmPlayer = bgmObject.AddComponent<AudioSource>();
        _bgmPlayer.loop = true; 
        _bgmPlayer.playOnAwake = false;

        GameObject sfxRoot = new GameObject("SFX_Players");
        sfxRoot.transform.SetParent(transform);
        
        for (int i = 0; i < maxSfxPlayers; i++)
        {
            GameObject sfxObj = new GameObject($"SFX_Player_{i}");
            sfxObj.transform.SetParent(sfxRoot.transform);
            AudioSource source = sfxObj.AddComponent<AudioSource>();
            source.loop = false;
            source.playOnAwake = false;
            if (sfxMixerGroup != null) source.outputAudioMixerGroup = sfxMixerGroup;
            
            _sfxPlayers.Add(source);
        }

        // 루프 전용 스피커 세팅
        GameObject loopSfxObj = new GameObject("LoopSFX_Player");
        loopSfxObj.transform.SetParent(transform);
        _loopSfxPlayer = loopSfxObj.AddComponent<AudioSource>();
        _loopSfxPlayer.loop = true;
        _loopSfxPlayer.playOnAwake = false;
        if (sfxMixerGroup != null) _loopSfxPlayer.outputAudioMixerGroup = sfxMixerGroup;

        Logger.Log("AudioManager : Initialized");
    }

    // 배경음악 재생
    public void PlayBGM(BGMType bgmType, float volume = 1.0f)
    {
        if (_hasBGM && _currentBGM == bgmType && _bgmPlayer.isPlaying)
        {
            return;
        }

        string bgmName = _bgmNames[(int)bgmType]; 

        AudioClip clip = GetOrLoadClip(bgmName, "BGM");
        if (clip != null)
        {
            _bgmPlayer.clip = clip;
            _bgmPlayer.volume = volume;
            _bgmPlayer.Play();

            _currentBGM = bgmType;
            _hasBGM = true;
        }
    }

    // 효과음 재생
    public void PlaySFX(SFXType sfxType, float volume = 1.0f, float pitch = 1.0f)
    {
        string sfxName = _sfxNames[(int)sfxType]; 
        
        AudioClip clip = GetOrLoadClip(sfxName, "SFX");
        
        if (clip == null) return;

        AudioSource availableSource = GetAvailableSFXPlayer();
        
        if (availableSource != null)
        {
            // 이번 재생에만 피치 적용
            availableSource.pitch = pitch;
            availableSource.volume = volume;
            availableSource.PlayOneShot(clip);
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
        if (_bgmPlayer != null) 
        {
            _bgmPlayer.mute = isMute;
        }

        foreach (AudioSource source in _sfxPlayers)
        {
            if (source != null)
            {
                source.mute = isMute;
            }
        }

        if (_loopSfxPlayer != null) 
        {
            _loopSfxPlayer.mute = isMute;
        }
        
        Logger.Log($"Audio Muted: {isMute}");
    }

    // 모든 효과음 즉시 정지 
    public void StopAllSFX()
    {
        foreach (AudioSource source in _sfxPlayers)
        {
            if (source != null && source.isPlaying)
            {
                source.Stop();
            }
        }
        Logger.Log("All SFX Stopped.");
    }

    // 쉬고 있는 SFX 스피커 이용
    private AudioSource GetAvailableSFXPlayer()
    {
        foreach (var source in _sfxPlayers)
        {
            if (!source.isPlaying) return source;
        }
        
        // 모든 플레이어가 바쁘면 가장 오래된(또는 첫 번째) 플레이어 이용
        return _sfxPlayers[0]; 
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

    // BGM 배속
    public void SetBGMPitch(float pitch)
    {
        if (_bgmPlayer != null)
        {
            _bgmPlayer.pitch = pitch;
            Logger.Log($"BGM Pitch changed to: {pitch}");
        }
    }

    // 무한 루프 효과음 재생
    public void PlayLoopSFX(SFXType sfxType, float volume = 1.0f)
    {
        string sfxName = _sfxNames[(int)sfxType]; 
        AudioClip clip = GetOrLoadClip(sfxName, "SFX");
        
        if (clip == null) return;

        // 중복 재생 방지
        if (_loopSfxPlayer.isPlaying && _loopSfxPlayer.clip == clip) return;

        _loopSfxPlayer.clip = clip;
        _loopSfxPlayer.volume = volume;
        _loopSfxPlayer.Play();
    }

    // 무한 루프 효과음 정지
    public void StopLoopSFX()
    {
        if (_loopSfxPlayer != null && _loopSfxPlayer.isPlaying)
        {
            _loopSfxPlayer.Stop();
            _loopSfxPlayer.clip = null; 
        }
    }

    
    // 씬 이동 시 사용하지 않는 소리 메모리 해제 (선택)
    public void ClearCache()
    {
        _audioClips.Clear();
        Resources.UnloadUnusedAssets();
    }
}