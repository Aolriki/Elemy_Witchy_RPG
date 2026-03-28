using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public class AudioConfigs
{
    public float musicVolume = 1;
    public float sfxVolume = 1;
}
public enum MusicID
{
    None,
    MainMenuMusic,
    FlorestaTheme,    // Tema Floresta.ogg
    BossFightTheme    // Area BossFight.mp3
}

public enum SFXID
{
    None,
    ButtonClick,
    ArenaBossCompleted,   // Arena Boss Fight Completed.mp3
    SystemConfirm,        // SystemOptionConfirm.mp3
    LevelUp,              // LevelUp.mp3
    SelectArchetype,      // SelectArchetype-UpgradeAttribute.mp3
    SelectSubPage,        // SelectSubPage.mp3
    OpenPause,            // Open Pause.mp3
    DamageTake,           // Damage Take.wav
    Attack,               // Attack.mp3
    GoodnessStatue,       // Godness Statue.mp3
    ElemyCollect          // Elemy Collect.mp3
}

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        InitDicts();
    }

    [System.Serializable]
    public class MusicData
    {
        public MusicID music;
        public AudioClip[] clips;
    }

    [System.Serializable]
    public class SFXData
    {
        public SFXID sfx;
        public AudioClip[] clips;
    }

    public MusicData[] musics;
    public SFXData[] effects;

    private Dictionary<MusicID, AudioClip[]> musicsDict;
    private Dictionary<SFXID, AudioClip[]> effectsDict;

    public AudioClip[] randomAmbienceFXs;
    public AudioMixer audioMixer;
    public AudioSource musicAudioSource;
    public AudioSource sfxAudioSource;
    public MusicID startMusic = MusicID.None;
    public float timeBetweenRandomAmbienceFXs;
    private float nextRandomAmbienceFXsTime;
    public bool playRandomAmbienceFXs;
    [SerializeField] private AudioSource soundFXObject;

    private void InitDicts()
    {
        musicsDict = new Dictionary<MusicID, AudioClip[]>();
        foreach (var entry in musics)
            musicsDict.Add(entry.music, entry.clips);

        effectsDict = new Dictionary<SFXID, AudioClip[]>();
        foreach (var entry in effects)
            effectsDict.Add(entry.sfx, entry.clips);
    }

    private void Start()
    {
        if (startMusic != MusicID.None) PlayMusic(startMusic, true);

        if (playRandomAmbienceFXs)
            StartCoroutine(RandomAmbienceLoop());

        //ScreenManager.instance.SetSoundConfigs();
    }

    // Método para tocar uma música específica pelo seu índice no array
    public void PlayMusic(MusicID musicID, bool loop, float volume = 1f)
    {
        if (!musicsDict.TryGetValue(musicID, out var clips))
        {
            Debug.LogWarning("MusicID not found: " + musicID);
            return;
        }

        int rand = Random.Range(0, clips.Length);
        musicAudioSource.loop = loop;
        musicAudioSource.volume = volume;
        musicAudioSource.clip = clips[rand];
        musicAudioSource.Play();

        musicAudioSource.DOFade(0, 2f).From();
    }
    public void TransitionToMusic(MusicID musicID, float fadeDuration = 3.0f)
    {
        // Fade out da música atual, depois toca a nova
        musicAudioSource.DOFade(0f, fadeDuration).OnComplete(() =>
        {
            PlayMusic(musicID, loop: true);
        });
    }
    public void PauseMusic(float fadeDuration = 0.5f)
    {
        musicAudioSource.DOFade(0f, fadeDuration)
            .SetUpdate(true) // ignora timeScale
            .OnComplete(() => musicAudioSource.Pause());
    }

    public void ResumeMusic(float fadeDuration = 0.5f)
    {
        musicAudioSource.UnPause();
        musicAudioSource.DOFade(1f, fadeDuration)
            .SetUpdate(true); // ignora timeScale
    }

    public void StopMusic()
    {
        if (musicAudioSource != null)
        {
            musicAudioSource.DOFade(0f, 2f).OnComplete(() => musicAudioSource.Stop());
        }
    }

    public void PlayEffect(SFXID sfxID, bool loop = false)
    {
        if (!effectsDict.TryGetValue(sfxID, out var clips))
        {
            Debug.LogWarning("SFXID not found: " + sfxID);
            return;
        }

        int rand = Random.Range(0, clips.Length);
        sfxAudioSource.loop = loop;
        sfxAudioSource.clip = clips[rand];
        sfxAudioSource.Play();
    }

    public void PlayEffect(AudioClip[] audioClips, bool loop)
    {
        sfxAudioSource.loop = loop;

        int rand = Random.Range(0, audioClips.Length);

        sfxAudioSource.clip = audioClips[rand];
        sfxAudioSource.Play();
    }

    private IEnumerator RandomAmbienceLoop()
    {
        yield return new WaitForSeconds(timeBetweenRandomAmbienceFXs); // evita o primeiro som imediato

        while (playRandomAmbienceFXs)
        {
            PlayRandomAmbienceEffect();
            yield return new WaitForSeconds(timeBetweenRandomAmbienceFXs);
        }
    }

    public void PlayRandomAmbienceEffect()
    {
        int rand = Random.Range(0, randomAmbienceFXs.Length);
        sfxAudioSource.clip = randomAmbienceFXs[rand];
        sfxAudioSource.Play();
    }

    public IEnumerator PlaySoundFXClipAfterTime(Vector3 spawnPosition, float time = 0, SFXID sfxID = SFXID.None, AudioClip[] audioClips = null, float volume = 0.6f)
    {
        yield return new WaitForSeconds(time);
        PlaySoundFXClip(spawnPosition, sfxID, audioClips, volume);
    }

    public void PlaySoundFXClip(Vector3 spawnPosition, SFXID sfxID = SFXID.None, AudioClip[] audioClips = null, float volume = 1f)
    {
        AudioClip audioClip = null;
        int randIndex = 0;

        if (sfxID == SFXID.None)
        {
            randIndex = Random.Range(0, audioClips.Length);
            audioClip = audioClips[randIndex];
        }
        else
        {
            if (!effectsDict.TryGetValue(sfxID, out var clips))
            {
                Debug.LogWarning("SFXID not found: " + sfxID);
                return;
            }

            randIndex = Random.Range(0, clips.Length);

            audioClip = clips[randIndex];
        }

        if (audioClip == null) return;

        AudioSource audioSource = Instantiate(soundFXObject, spawnPosition, Quaternion.identity);

        audioSource.clip = audioClip;

        audioSource.volume = volume;

        audioSource.Play();

        float clipLength = audioSource.clip.length;

        Destroy(audioSource.gameObject, clipLength);
    }
    public void PlayMusicByString(string musicID)
    {
        if (System.Enum.TryParse(musicID, out MusicID id))
            TransitionToMusic(id);
        else
            Debug.LogWarning("MusicID inválido: " + musicID);
    }

    public void PlayEffectByString(string sfxID)
    {
        if (System.Enum.TryParse(sfxID, out SFXID id))
            PlayEffect(id);
        else
            Debug.LogWarning("SFXID inválido: " + sfxID);
    }

    public void StopMusicEvent() => StopMusic();
}
