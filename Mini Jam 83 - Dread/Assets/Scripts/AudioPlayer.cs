using UnityEngine;
using UnityEngine.Audio;

public class AudioPlayer : MonoBehaviour
{
    public AudioSource AudioSource { get; private set; }
    [SerializeField] AudioMixerGroup musicGroup, SFXGroup;

    AudioStation audioStation;

    bool started;

    void Awake()
    {
        AudioSource = GetComponent<AudioSource>();
        audioStation = AudioStation.Instance;
    }

    public void SetupRandomSFX(AudioClip[] clips, float audioPitchMin, float audioPitchMax)
    {
        AudioClip randomClip = clips[Random.Range(0, clips.Length)];
        AudioSource.clip = randomClip;
        AudioSource.outputAudioMixerGroup = SFXGroup;
        AudioSource.pitch = Random.Range(audioPitchMin, audioPitchMax);
    }

    public void SetupSFX(AudioClip clip, float audioPitchMin, float audioPitchMax)
    {
        AudioSource.clip = clip;
        AudioSource.outputAudioMixerGroup = SFXGroup;
        AudioSource.pitch = Random.Range(audioPitchMin, audioPitchMax);
    }

    public void SetupMusic(AudioClip clip, bool loop)
    {
        AudioSource.clip = clip;
        AudioSource.outputAudioMixerGroup = musicGroup;
        AudioSource.loop = loop;
        AudioSource.reverbZoneMix = 0; // NO REVERB ZONE EFFECT TO MUSIC
    }

    public void Play()
    {
        name = AudioSource.clip.name;
        AudioSource.Play();
        started = true;
    }

    void Update()
    {
        if (started)
            if (!AudioSource.loop)
            {
                audioStation.audioPlayers.Remove(this);
                Destroy(gameObject, AudioSource.clip.length);
            }
    }
}