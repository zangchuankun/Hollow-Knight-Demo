using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collection : MonoBehaviour
{
    public int coinsValue;
    [SerializeField] private AudioClip[] audioClips;
    [SerializeField] private AudioClip[] hitGroundClips;
    //[SerializeField] private AudioClip[] hitGroundClips;
    private AudioSource audioSource;
    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    //收集品与Knight发生碰撞后会增加geo数量，并且摧毁该收集品
    private void OnCollisionEnter2D(Collision2D collision)
    {
        StartCoroutine(DetectCollisionEnter(collision));
    }
    protected virtual IEnumerator DetectCollisionEnter(Collision2D collision)
    {
        
        if (collision.gameObject.tag=="Knight")//geo捡起音效
        {
            if (!PlayerController.Instance.IsDead)//Knight存活
            {
                //播放捡起geo的音效
                int i=Random.Range(0,audioClips.Length);
                audioSource.PlayOneShot(audioClips[i]);
                //增加货币
                HealthController.Instance.AddCoins(coinsValue);
                spriteRenderer.color=new Color(spriteRenderer.color.r,spriteRenderer.color.g,spriteRenderer.color.b,0);
                yield return new WaitForSeconds(0.1f);
                Destroy(gameObject);
            }
        }
        if (collision.gameObject.layer == LayerMask.NameToLayer("Terrain"))//geo掉到地上音效
        {
            int i=Random.Range(0,hitGroundClips.Length);
            audioSource.PlayOneShot(hitGroundClips[i]);
        }
    }
}
