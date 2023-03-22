using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuButtonController : MonoBehaviour
{
    [SerializeField] private Animator[] anims;
    [SerializeField] private Slider[] sliders;
    private float masterValue;
    private float musicValue;
    private float acousticValue;
    //Animator：0-MainMenu，1-AudioMenu，2-BtnStart，3-BtnOptiion，4-BtnAudioBack
    //Slider：0-Master，1-Music，2-Acoustic
    private void Start()
    {
        masterValue=sliders[0].value;
        musicValue=sliders[1].value;
        acousticValue=sliders[2].value;
    }

    public void BtnStart()//开始按钮
    {
        StartCoroutine(DelayMenuStart());
    }
    IEnumerator DelayMenuStart()
    {
        anims[0].Play("FadeOut");
        yield return new WaitForSeconds(1f);
        anims[2].SetBool("IsSelect", false);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    public void BtnOption()//选项按钮
    {
        StartCoroutine (DelayMenuOption());
    }
    IEnumerator DelayMenuOption()
    {
        anims[0].Play("FadeOut");
        yield return new WaitForSeconds(1f);
        anims[3].SetBool("IsSelect",false);
        anims[1].Play("Enter");
    }
    public void BtnExit()//退出按钮
    {
        StartCoroutine(DelayMenuExit());
    }
    IEnumerator DelayMenuExit()
    {
        anims[0].Play("FadeOut");
        yield return new WaitForSeconds(1f);
        Debug.Log("已退出");
    }

    public void BtnBack()//选项界面中的返回按钮
    {
        StartCoroutine(DelayAudioBack());
    }
    IEnumerator DelayAudioBack()
    {
        anims[1].Play("Fade");
        anims[4].SetBool("IsSelect", false);
        yield return new WaitForSeconds (1f);
        anims[0].Play("Enter");
    }

    public void BtnReset()//选项界面中的重设按钮
    {
        sliders[0].value = masterValue;
        sliders[1].value = musicValue;
        sliders[2].value = acousticValue;
    }
}
