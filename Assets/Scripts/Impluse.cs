using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Impluse : MonoBehaviour
{
    #region 单例模式
    private static Impluse _instance;
    public static Impluse Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance=GameObject.Find("CM vcam1").GetComponent<Impluse>();
            }
            return _instance;
        }
    }
    #endregion
    private Cinemachine.CinemachineImpulseSource impluse;

    private void Awake()
    {
        impluse=FindObjectOfType<Cinemachine.CinemachineImpulseSource>();
    }
    public void SendImpluse()
    {
        impluse.GenerateImpulse();
    }
}
