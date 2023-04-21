using UnityEngine;
using System.Collections.Generic;

public class AudioStation : MonoBehaviour
{
    [SerializeField] AudioPlayer audioPlayerPrefab;
    public static AudioStation Instance { get; private set; }

    [HideInInspector] public List<AudioPlayer> audioPlayers = new List<AudioPlayer>();
    AudioPlayer currentMusicPlayer;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    public void StartNewRandomSFXPlayer(AudioClip[] clips, Vector3 pos = default,
                                        float pitchMin = 1, float pitchMax = 1)
    {
        AudioPlayer newAudioPlayer = Instantiate(audioPlayerPrefab, pos, Quaternion.identity);
        newAudioPlayer.transform.SetParent(transform);
        audioPlayers.Add(newAudioPlayer);
        newAudioPlayer.SetupRandomSFX(clips, pitchMin, pitchMax);
        newAudioPlayer.Play();
    }

    public void StartNewSFXPlayer(AudioClip clip, Vector3 pos = default,
                                  float pitchMin = 1, float pitchMax = 1)
    {
        AudioPlayer newAudioPlayer = Instantiate(audioPlayerPrefab, pos, Quaternion.identity);
        newAudioPlayer.transform.SetParent(transform);
        audioPlayers.Add(newAudioPlayer);
        newAudioPlayer.SetupSFX(clip, pitchMin, pitchMax);
        newAudioPlayer.Play();
    }

    public void StartNewMusicPlayer(AudioClip clip, bool loop = true)
    {
        if (currentMusicPlayer && clip == currentMusicPlayer.AudioSource.clip)
            return;
        else if (currentMusicPlayer)
        {
            audioPlayers.Remove(currentMusicPlayer);
            Destroy(currentMusicPlayer.gameObject);
        }

        currentMusicPlayer = Instantiate(audioPlayerPrefab, transform);
        audioPlayers.Add(currentMusicPlayer);
        currentMusicPlayer.SetupMusic(clip, loop);
        currentMusicPlayer.Play();
    }

    public void ToggleAllPlayerPause(int type)
    {
        for (int i = 0; i < audioPlayers.Count; i++)
            if (type == 0)
                audioPlayers[i].AudioSource.Pause();
            else
                audioPlayers[i].AudioSource.UnPause();
    }
}