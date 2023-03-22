using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour//J攻击，K跳跃，上下左右WASD移动
{
    #region 单例模式
    private static PlayerController instance;
    public static PlayerController Instance
    {
        get
        {
            if (instance == null)
            {
                instance=GameObject.Find("Knight").GetComponent<PlayerController>();
            }
            return instance;
        }
    }
    #endregion
    Vector3 flippedScale =new Vector3(-1,1,1);
    private Rigidbody2D rb2D;
    private Animator animator;
    private AudioSource audioSource;
    private AnimatorStateInfo animatorStateInfo;
    //移动跳跃
    float moveX;
    float moveY;
    public float speed;
    public float jumpForce;
    public int jumpNum;
    private int jumpAmount;
    private bool isOnGround=false;
    private bool isFaceRight=false;
    [SerializeField] private AudioClip audioFall;
    [SerializeField] private AudioClip audioJump;
    [SerializeField] private AudioClip[] audioLand;
    int isMove;
    private Vector2 direction = new Vector2(0, 0);
    public Vector2 Direction
    {
        get { return direction; }
        set
        {
            direction=new Vector2 (value.x, value.y);
        }
    }
    //角色受到攻击
    public float damageForce;
    private bool isDead = false;
    public bool IsDead
    {
        get { return isDead; }
    }
    [SerializeField] private AudioClip[] audiosHurt;
    //角色攻击变量
    public int knightDamage;
    public AnimationClip[] attackAnims;
    public AnimationClip attackTopAnim;
    public AnimationClip attackBottomAnim;
    private int attackLevel=1;//用于切换连击
    public float attackBottomForce;//在空中攻击的时候获得向上的推力
    private bool isAttack = false;
    private bool isAttackEnd = true;
    private float attackTimer=0;//每一次攻击的动画计时器
    private float attackEndTimer=0;//攻击结束后重置连击的计时器
    [SerializeField] private AudioClip[] attackClips;//播放攻击音效
    private AttackType attackType;
    private enum AttackType
    {
        normal,
        top,
        bottom
    }
    #region 无敌时间计时器
    public float invincibleTime;
    private float invincibleTimer;
    private bool isInvincible=false;
    #endregion
    
    // Start is called before the first frame update
    void Start()
    {
        Physics2D.queriesStartInColliders = false;
        rb2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        //初始化无敌时间计时器
        invincibleTimer = invincibleTime;
        //初始化最大跳跃次数
        jumpAmount = jumpNum;
    }

    // Update is called once per frame
    void Update()
    {
        KnightDirection();
        CheckDead();
        if (isInvincible)
        {
            invincibleTimer=invincibleTimer-Time.deltaTime;
            if (invincibleTimer < 0)
            {
                isInvincible=false;
                invincibleTimer = invincibleTime;
                Debug.Log("计时结束");
            }
        }
        else
        {
            KnightMove();
            KnightJump();
            //CheckFall();
        }
        //按下J则进行攻击
        Attack();
        AttackEndCheck();
        //检查点是否在区域内
        //Debug.Log(CheckDamageRange(new Vector2(transform.position.x+1f,transform.position.y)));
    }

    //Knight移动以及移动动画的切换
    private void KnightMove()
    {
        if (!isDead)
        {
            moveX = Input.GetAxis("Horizontal");
            moveY = Input.GetAxisRaw("Vertical");
            rb2D.velocity = new Vector2(moveX * speed, rb2D.velocity.y);
            if (moveX > 0)
            {
                isMove = 1;
            }
            else if (moveX < 0)
            {
                isMove = -1;
            }
            else
            {
                isMove = 0;
            }
            animator.SetInteger("movement", isMove);
        }  
    }
    //Knight朝向的动画切换
    private void KnightDirection()
    {
        if (!isDead)
        {
            if (moveX > 0)
            {
                transform.localScale = flippedScale;
                isFaceRight = true;
            }
            else if (moveX < 0)
            {
                transform.localScale = Vector3.one;
                isFaceRight = false;
            }
            
        }     
    }
    //Knight的跳跃实现以及动画触发
    private void KnightJump()
    {
        if (!isDead&&Input.GetKeyDown(KeyCode.K))
        {
            if (jumpAmount>0)
            {
                rb2D.velocity=new Vector2(rb2D.velocity.x,0);
                rb2D.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
                animator.Play("Jump");
                audioSource.PlayOneShot(audioJump);
                jumpAmount--;
            }    
        }
        //if (animator.GetCurrentAnimatorClipInfo(0).ToString()=="Jump" && animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.9f)
        //{
        //    animator.Play("Fall");
        //}
    }
    
    /// <summary>
    /// 判断当前是否为地面
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionEnter2D(Collision2D collision)
    {
        Grounding(collision,false);
        Debug.Log("enter"+isOnGround);
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        Grounding(collision,false);
        //Debug.Log("stay"+isOnGround);
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        Grounding(collision,true);
        Debug.Log("exit" + isOnGround);
    }
    private void Grounding(Collision2D collision,bool exitState)
    {
        if (exitState)
        {
            if (collision.gameObject.layer == LayerMask.NameToLayer("Terrain"))
            {
                isOnGround = false;
            }
        }
        else
        {
            if (collision.gameObject.layer == LayerMask.NameToLayer("Terrain") && !isOnGround && collision.contacts[0].normal == Vector2.up)
            {
                isOnGround = true;
                JumpCancel();
                //animator.Play("Land");
                int i=Random.Range(0,audioLand.Length);
                audioSource.PlayOneShot(audioLand[i]);
                
            }else if(collision.gameObject.layer == LayerMask.NameToLayer("Terrain") && !isOnGround && collision.contacts[0].normal == Vector2.down)
            {
                //TODO
            }
        }
        animator.SetBool("isOnGround", isOnGround);
    }
    private void JumpCancel()
    {
        animator.ResetTrigger("isJump");
        jumpAmount = jumpNum;
    }

    private void CheckFall()
    {
        if (!isOnGround&&rb2D.velocity.y<0)
        {
            //animator.Play("Fall");
            audioSource.PlayOneShot(audioFall);
        }
    }

    public void TakeDamage(int damage,Vector2 dirV2)//Knight受到伤害
    {
        if (!isDead)
        {
            if (isInvincible)
            {
                return;
            }
            else
            {
                //先清空速度，然后对角色施加力
                rb2D.velocity = new Vector2(0, 0);
                //direction=transform.position-Crawlid.Instance.transform.position;
                
                int x = 0;
                if(dirV2.x > 0)
                {
                    x = 1;
                }else if(dirV2.x < 0)
                {
                    x = -1;
                }
                //击退的力
                Vector2 dir = new Vector2(x, 1);
                rb2D.AddForce(dir*damageForce, ForceMode2D.Impulse);
                //设置动画
                animator.SetTrigger("damage");
                //播放音效
                int num = Random.Range(0, audiosHurt.Length);
                audioSource.PlayOneShot(audiosHurt[num]);
                //减少生命值
                StartCoroutine(HealthController.Instance.HurtHealth(damage));
                isInvincible = true;
                //屏幕抖动
                Impluse.Instance.SendImpluse();
                Debug.Log("计时开始");
            }
        }
        else
        {
            Debug.Log("人已经死了 再怎么伤害也没有用了");
        }   
    }
    
    private void OnTriggerEnter2D(Collider2D collision)//Knight碰到Enemy后击退
    {
        if (!isDead&&collision.gameObject.tag == "Enemy"&&!collision.gameObject.GetComponent<Enemy>().IsDead)
        {
            direction = transform.position - collision.transform.position;
            TakeDamage(collision.gameObject.GetComponent<Enemy>().damage,direction);
        }   
    }

    private void CheckDead()//检查角色是否死亡
    {
        if (HealthController.Instance.CurrentHealth <= 0&&!isDead)//角色生命值小于等于零并且还没有调用Dead方法
        {
            isDead = true;
            Dead();
        }
    }
    private void Dead()//角色死亡后执行的代码
    {
        gameObject.layer = LayerMask.NameToLayer("Terrain");
        animator.SetTrigger("dead");
        rb2D.bodyType = RigidbodyType2D.Static;
        Debug.Log("角色已死亡");
    }

    private void Attack()//攻击动画
    {
        if (!isDead)
        {
            
            //AttackDamage();
            if (isAttack)
            {
                attackTimer += Time.deltaTime;
                if (attackTimer >= GetAttackAnimLength(attackType))
                {
                    isAttack = false;
                    isAttackEnd = true;
                    attackTimer = 0;
                }
            }
            else
            {
                Vector2 attackPosition = transform.position;
                attackTimer = 0;  
                if (Input.GetKeyDown(KeyCode.J))
                {
                    int num = Random.Range(0, attackClips.Length);
                    audioSource.PlayOneShot(attackClips[num]);
                    rb2D.velocity = Vector2.zero;
                    if (moveY > 0)
                    {
                        if (!isOnGround)
                        {
                            transform.position= attackPosition;
                            rb2D.velocity = Vector2.zero;
                            rb2D.AddForce(new Vector2(0, attackBottomForce), ForceMode2D.Impulse);
                        }
                        attackType = AttackType.top;
                        animator.Play("AttackTop");
                        isAttack = true;
                        isAttackEnd = false;
                    }
                    else if (moveY < 0)
                    {
                        if (!isOnGround)
                        {
                            attackType = AttackType.bottom;
                            animator.Play("AttackBottom");
                            rb2D.velocity=Vector2.zero;
                            rb2D.AddForce(new Vector2(0,attackBottomForce),ForceMode2D.Impulse);
                        }
                        else
                        {
                            attackType = AttackType.normal;
                            animator.Play("Attack_" + attackLevel);
                            attackLevel++;
                            if (attackLevel > attackAnims.Length)
                            {
                                attackLevel = 1;
                            }
                        }
                        isAttack = true;
                        isAttackEnd = false;
                    }
                    else
                    {
                        if (!isOnGround)
                        {
                            rb2D.velocity = Vector2.zero;
                            rb2D.AddForce(new Vector2(0, attackBottomForce), ForceMode2D.Impulse);
                        }
                        attackType = AttackType.normal;
                        animator.Play("Attack_"+attackLevel);
                        isAttack = true;
                        isAttackEnd = false;
                        attackLevel++;
                        if(attackLevel > attackAnims.Length)
                        {
                            attackLevel = 1;
                        }
                    }
                    AttackDamage();
                }
                
            }
        }
    }
    private float GetAttackAnimLength(AttackType attackType)//返回当前攻击动画长度
    {
        switch (attackType)
        {
            case AttackType.normal:
                return attackAnims[attackLevel - 1].length;
            case AttackType.top:
                return attackTopAnim.length;
            case AttackType.bottom:
                return attackBottomAnim.length;
        }
        return 0;
    }
    private void AttackEndCheck()//当停止攻击时计时，大于1s便重置地面普攻level
    {
        if (isAttackEnd)
        {
            attackEndTimer+=Time.deltaTime;
            if(attackEndTimer > 1)
            {
                attackLevel = 1;
                attackEndTimer = 0;
            }
        }
        else
        {
            attackEndTimer = 0;
        }
    }
    private void AttackDamage()
    {
        Vector2 left;
        Vector2 right;
        Vector2 left_up;
        Vector2 right_up;
        GameObject[] enemy = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject[] breakable = GameObject.FindGameObjectsWithTag("Breakable");
        
        if (moveY >0)
        {
            left = new Vector2(transform.position.x - 1.5f, transform.position.y);
            right = new Vector2(transform.position.x + 1.5f, left.y);
            left_up = new Vector2(left.x, left.y + 3f);
            right_up = new Vector2(right.x, left_up.y);
            Debug.DrawLine(left, right);
            Debug.DrawLine(left, left_up);
            Debug.DrawLine(right, right_up);
            Debug.DrawLine(left_up, right_up);
        }
        else if(!isOnGround&&moveY < 0)
        {
            left = new Vector2(transform.position.x-1.5f, transform.position.y);
            right = new Vector2(transform.position.x +1.5f, left.y);
            left_up = new Vector2(left.x, left.y - 3.5f);
            right_up = new Vector2(right.x, left_up.y);
            Debug.DrawLine(left, right);
            Debug.DrawLine(left, left_up);
            Debug.DrawLine(right, right_up);
            Debug.DrawLine(left_up, right_up);
        }
        else
        {
            int x = isFaceRight?1:-1;
            //矩形区域检测
            left = new Vector2(transform.position.x, transform.position.y - 1.5f);
            left_up = new Vector2(left.x, transform.position.y + 1.5f);
            right = new Vector2(left.x + x * 3f, left.y);
            right_up = new Vector2(right.x, left_up.y);
            Debug.DrawLine(left, right);
            Debug.DrawLine(left, left_up);
            Debug.DrawLine(right, right_up);
            Debug.DrawLine(left_up, right_up);
        }
        foreach (GameObject e in enemy)
        {
            if (IsPointInRectangle(left, right, left_up, right_up, e.transform.position))
            {
                e.transform.GetComponent<Breakable>().Hurt(knightDamage);
            }
        }
        foreach(GameObject b in breakable)
        {
            if (IsPointInRectangle(left, right, left_up, right_up, b.transform.position))
            {
                b.transform.GetComponent<Breakable>().Hurt(knightDamage);
            }
        }
    }
    
    /// <summary>
    /// 检验点是否在指定区域内
    /// </summary>
    /// <returns></returns>
    private bool IsPointInRectangle(Vector2 left,Vector2 right,Vector2 left_up,Vector2 right_up,Vector2 point)
    {
        return GetCross(right_up, left_up, point) * GetCross(left, right, point) >= 0 && GetCross(left_up, left, point) * GetCross(right, right_up, point) >= 0;
    }
    private float GetCross(Vector2 point1,Vector2 point2,Vector2 point)//获取向量point2-point1，point-point1的叉积
    {
        return (point.x-point1.x)*(point2.y-point1.y)-(point2.x-point1.x)*(point.y-point1.y);
    }
}
