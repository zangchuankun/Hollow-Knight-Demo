using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonController : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler,IPointerDownHandler
{
    private Animator animator;
    private AudioSource audioSource;
    [SerializeField] private AudioClip selectAudio;
    [SerializeField] private AudioClip clickAudio;
    private void Start()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        animator.SetBool("IsSelect",true);
        audioSource.PlayOneShot(selectAudio);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        animator.SetBool("IsSelect",false);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        animator.Play("Click");
        audioSource.PlayOneShot(clickAudio);
    }

}
