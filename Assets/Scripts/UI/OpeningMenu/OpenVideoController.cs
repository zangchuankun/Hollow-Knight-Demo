using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class OpenVideoController : MonoBehaviour
{
    [SerializeField] private VideoPlayer prologue;
    [SerializeField] private VideoPlayer intro;
    [SerializeField] private AudioSource bgm;

    private void Start()
    {
        prologue.loopPointReached += PrologueLoop;
        intro.loopPointReached += IntroLoop;
    }

    public void ProloguePlay()
    {
        prologue.Play();
        bgm.Stop();
    }

    private void PrologueLoop(VideoPlayer source)
    {
        intro.Play();
    }
    private void IntroLoop(VideoPlayer source)
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
