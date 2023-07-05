using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 非Mono的懒汉模式单例基类
/// </summary>
/// <typeparam name="T">新单例的类</typeparam>
public class BaseMgr<T> where T : new()
{
    private static T instance;
    public static T GetInstance()
    {
        if (instance == null)
        {
            instance = new T();
        }
        return instance;
    }
}
