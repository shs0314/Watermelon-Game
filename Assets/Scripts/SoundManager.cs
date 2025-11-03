using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{

    public static SoundManager instance;

    public AudioSource bgmPlayer;
    public AudioSource[] sfxPlayers;
    public AudioClip[] sfxClips;
    private int sfxCursor;

    public void Awake()
    {
        instance = this;
    }

    public void PlayBgm()
    {
        bgmPlayer.Play();
    }

    public void StopBgm()
    {
        bgmPlayer.Stop();
    }

    public void PlaySfx(Sfx sfx)
    {
        sfxPlayers[sfxCursor].clip = sfxClips[(int) sfx];
        sfxPlayers[sfxCursor].Play();
        sfxCursor = (sfxCursor + 1) % sfxPlayers.Length;
    }
    
}

public enum Sfx
{
    LevelUp = 0,
    Drop = 1,
    Button = 2,
    Finish = 3
}