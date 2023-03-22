using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Breakable : MonoBehaviour
{
    /// <summary>
    /// 所有可以破坏的环境物体、敌人都需要有Animator，其中需要有dead触发器，当物体破坏/敌人死亡时调用
    /// </summary>
    public int health;
    public GameObject collection;
    public int shotForce;
    public int coinsAmount;
    protected bool isDead=false;
    public bool IsDead
    {
        get { return isDead; }
    }
    protected Animator animator;
    protected Rigidbody2D rb2d;
    protected AudioSource audioSource;

    public virtual void Hurt(int damage)//对Breakable造成damage点伤害
    {
        if (!isDead)//当前物体没死，可以伤害到
        {
            if (health >= damage)
            {
                health -= damage;
            }
            else
            {
                health = 0;
            }  
        }   
    }
    protected bool CheckDead()//检测Breakable是否死亡
    {
        if (health <= 0 && !isDead)
        {
            isDead = true;
            return true;
        }
        return false;
    }
    protected virtual void Dead()//死亡后执行的代码
    {
        
    }

    protected void ShotCollection()//死亡后掉落物
    {
        for(int i = 0; i < coinsAmount; i++)
        {
            float randomX = Random.Range(-0.5f, 0.5f);
            float randomY = Random.Range(0.5f, 1.0f);
            GameObject coin = GameObject.Instantiate(collection, transform);
            Rigidbody2D rb = coin.GetComponent<Rigidbody2D>();
            Vector2 force = new Vector2(randomX, randomY);
            rb.AddForce(shotForce * force, ForceMode2D.Impulse);
        }
    }
}
