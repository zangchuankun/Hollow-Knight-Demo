using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeoDeposit : Breakable
{
    [SerializeField] private AudioClip[] hitAudios;
    [SerializeField] private AudioClip deadAudio;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        rb2d=GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (CheckDead())
        {
            Dead();
        }
    }

    public override void Hurt(int damage)
    {
        base.Hurt(damage);
        StartCoroutine(DelayHurt());
    }
    IEnumerator DelayHurt()
    {
        if (!IsDead)
        {
            int num=Random.Range(0, hitAudios.Length);
            animator.SetTrigger("hurt");
            audioSource.PlayOneShot(hitAudios[num]);
        }
        yield return new WaitForSeconds(0.5f);
    }

    protected override void Dead()
    {
        animator.SetTrigger("dead");
        audioSource.PlayOneShot(deadAudio);
        ShotCollection();
    }
}
