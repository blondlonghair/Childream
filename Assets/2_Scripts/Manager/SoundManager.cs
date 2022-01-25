using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SoundManager : SingletonMonoDestroy<SoundManager>
{
    private AudioSource bgmPlayer;
    private AudioSource sfxPlayer;

    public float masterVolumeSFX = 1f;
    public float masterVolumeBGM = 1f;

    [SerializeField] private AudioClip[] bgmAudioClips;
    [SerializeField] private AudioClip[] sfxAudioClips;

    Dictionary<string, AudioClip> sfxAudioClipsDic = new Dictionary<string, AudioClip>();
    Dictionary<string, AudioClip> bgmAudioClipsDic = new Dictionary<string, AudioClip>();

    private string curSceneName;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        
        var bgm = new GameObject("BGMSoundPlayer");
        bgm.transform.SetParent(this.transform);
        var sfx = new GameObject("SFXSoundPlayer");
        sfx.transform.SetParent(this.transform);

        bgmPlayer = bgm.AddComponent<AudioSource>();
        sfxPlayer = sfx.AddComponent<AudioSource>();

        foreach (AudioClip audioClip in sfxAudioClips)
        {
            sfxAudioClipsDic.Add(audioClip.name, audioClip);
        }

        foreach (var audioClip in bgmAudioClips)
        {
            bgmAudioClipsDic.Add(audioClip.name, audioClip);
        }

        curSceneName = SceneManager.GetActiveScene().name;
    }

    public void PlaySFXSound(string name, float volume = 1f)
    {
        if (sfxAudioClipsDic.ContainsKey(name) == false)
        {
            Debug.Log(name + " : 존재하지 않는 오디오 클립");
            return;
        }
        sfxPlayer.PlayOneShot(sfxAudioClipsDic[name], volume * masterVolumeSFX);
    }

    public void PlayBGMSound(float volume = 1f)
    {
        bgmPlayer.loop = true;
        bgmPlayer.volume = volume * masterVolumeBGM;

        if (SceneManager.GetActiveScene().name == "CutScene")
        {
            bgmPlayer.clip = bgmAudioClipsDic["CutScene"];
            bgmPlayer.Play();
        }
        else if (SceneManager.GetActiveScene().name == "IngameScene")
        {
            bgmPlayer.clip = bgmAudioClipsDic["IngameScene"];
            bgmPlayer.Play();
        }
        
        else if (SceneManager.GetActiveScene().name == "IntroScene")
        {
            bgmPlayer.clip = bgmAudioClipsDic["IntroScene"];
            bgmPlayer.Play();
        }
        
        else if (SceneManager.GetActiveScene().name == "LobbyScene")
        {
            bgmPlayer.clip = bgmAudioClipsDic["LobbyScene"];
            bgmPlayer.Play();
        }
        
        else if (SceneManager.GetActiveScene().name == "TutorialScene")
        {
            bgmPlayer.clip = bgmAudioClipsDic["TutorialScene"];
            bgmPlayer.Play();
        }
    }

    private void Update()
    {
        if (curSceneName != SceneManager.GetActiveScene().name)
        {
            PlayBGMSound(0.2f);
            curSceneName = SceneManager.GetActiveScene().name;
        }
    }

    // private void Start()
    // {
    //     PlayBGMSound(0.2f);
    // }
}