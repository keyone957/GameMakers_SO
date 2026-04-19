using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// 제너릭 싱글톤 클래스
/// <br/>Inspector 창에서 DontDestroyOnLoad 설정 가능
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;
    [SerializeField] bool _dontDestroy = true;


    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<T>();
            }
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this as T;
            if (_dontDestroy)
            {
                DontDestroyOnLoad(gameObject);
            }
            Init();
        }
        else
        {
            if (_instance != this)
            {
                Destroy(gameObject);
            }
        }
    }

    protected virtual void Init() { }
}