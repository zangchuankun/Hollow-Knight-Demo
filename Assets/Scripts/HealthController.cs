using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthController : MonoBehaviour
{
    #region 单例
    private static HealthController _instance;
    public static HealthController Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.Find("Health").GetComponent<HealthController>();
            }
            return _instance;
        }
    }
    #endregion
    //控制生命、货币等动画
    private Animator[] health;
    private Animator soulOrb;
    private Animator geo;

    private Text coinText;
    private int coinAmount=0;

    private int currentHealth;
    public int CurrentHealth
    {
        get { return currentHealth; }
    }

    private void Awake()
    {
        health = GetComponentsInChildren<Animator>();
        soulOrb = GameObject.Find("SoulOrb").GetComponent<Animator>();
        geo = GameObject.Find("Geo").GetComponent<Animator>();
        coinText=GameObject.Find("GeoNum").GetComponent<Text>();
        //初始化当前生命值为health数组长度
        currentHealth = health.Length;
    }
    // Start is called before the first frame update
    void Start()
    {  
        StartCoroutine(ShowHealth());
        
    }
    private void Update()
    {
        coinText.text=coinAmount.ToString();
    }

    //协程显示health,soulOrb,geo的进入动画
    private IEnumerator ShowHealth()
    {
        Debug.Log(soulOrb.name+geo.name);
        soulOrb.SetTrigger("Enter");
        yield return new WaitForSeconds(0.2f);
        foreach (Animator anim in health)
        {
            anim.SetTrigger("Enter");
            yield return new WaitForSeconds(0.2f); 
        }
        yield return new WaitForSeconds(0.2f);
        geo.SetTrigger("Enter");
    }
    //隐藏血量ui
    public void HideHealth()
    {
        soulOrb.SetTrigger("Exit");
        foreach (Animator anim in health)
        {
            anim.SetTrigger("Exit");
        }
        geo.SetTrigger("Exit");
    }
    //受到伤害时调用
    public IEnumerator HurtHealth(int damage)
    {
        
        if (currentHealth >= 1)//还有血可以扣
        {
            int num = currentHealth > damage ? damage : currentHealth;
            for (int i = 0; i < num; i++)
            {
                health[currentHealth - 1].SetTrigger("Hurt");
                currentHealth--;
                yield return new WaitForSeconds(0.1f);
            }
        }
        else
        {
            //TODO
            Debug.Log("没血了扣不了");
        }    
    }

    public void AddCoins(int num)
    {
        coinAmount += num;
    }

}
