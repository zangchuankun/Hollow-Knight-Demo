using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Breakable
{ 
    private float moveX = 0;
    public float moveSpeed;
    //private bool isRight=true;
    public int damage;
    public int hurtForce;
    protected bool isHurt=false;
    public Vector2 direction;
    [SerializeField] private AudioClip audioHurt;
    [SerializeField] private AudioClip audioDead;


    // Start is called before the first frame update
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        audioSource=GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        Direction();
        if (!isHurt)
        {
            Move();
        }
        if (CheckDead())
        {
            Dead();
        }
    }
    private void Direction()//判断敌人移动的方向
    {
        moveX = transform.localScale.x;
        //if (transform.localScale.x == 1)
        //{
        //    isRight = true;
        //}else if(transform.localScale.x == -1)
        //{
        //    isRight=false;
        //}
        //else
        //{
        //    return;
        //}
    }
    private void Move()//敌人存活时自动移动，检测到平台边缘则自动调头
    {
        if (!isDead)
        {
            rb2d.velocity = new Vector2(moveX*moveSpeed, rb2d.velocity.y);
            if (IfChangeDirection())
            {
                DirectionSwitch();
            }
        }
        else
        {
            rb2d.velocity = Vector2.zero;
        }
        
    }
    private void DirectionSwitch()//当敌人存活时，调用该方法切换敌人移动的方向
    {
        if (!isDead)
        {
            float x = moveX;
            x *= -1;
            transform.localScale = new Vector3(x, transform.localScale.y, transform.localScale.z);
        }   
    }
    private bool IfChangeDirection()//返回敌人是否来到平台边缘，以及是否将要碰到障碍物（图层为Terrain）
    {
        //平台边缘检测
        Ray2D ray1 = new Ray2D(transform.position, new Vector2(moveX, -1));
        Debug.DrawRay(ray1.origin, ray1.direction);
        RaycastHit2D hit1 = Physics2D.Raycast(ray1.origin, ray1.direction,2f,LayerMask.GetMask("Terrain"));
        //墙体碰撞检测
        Ray2D ray2 = new Ray2D(transform.position, new Vector2(moveX, 0));
        RaycastHit2D hit2 = Physics2D.Raycast(ray2.origin, ray2.direction, 1.5f, LayerMask.GetMask("Terrain"));
        Debug.DrawRay(ray2.origin, ray2.direction);
        if (hit1.collider==null||hit2.collider!=null)
        {           
            return true;
        }
        else
        {
            return false;
        }  
    }

    protected override void Dead()//Enemy死后调用该方法
    {
        animator.SetTrigger("dead");
        audioSource.PlayOneShot(audioDead);
        ShotCollection();
        Debug.Log("敌人已死亡");
    }

    public override void Hurt(int damage)
    {
        base.Hurt(damage);
        StartCoroutine(DelayHurt());
    }
    IEnumerator DelayHurt()
    {
        if (!isDead)
        {
            isHurt = true;
            Vector2 direction=transform.position-PlayerController.Instance.transform.position;
            int hurtX = 0;
            if (direction.x > 0)
            {
                hurtX = 1;
            }else if(direction.x < 0)
            {
                hurtX = -1;
            }
            else
            {
                hurtX = 0;
            }
            //速度清零后给一个后方的力
            rb2d.velocity = new Vector2(0, 0);
            Vector2 force = new Vector2(hurtX, 0);
            rb2d.AddForce(hurtForce * force, ForceMode2D.Impulse);
            //受伤动画
            animator.SetTrigger("hurt");
            audioSource.PlayOneShot(audioHurt);
            //屏幕抖动
            Impluse.Instance.SendImpluse();
            yield return new WaitForSeconds(0.6f);
            isHurt=false;
        }
        
    }

}
