using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
/// <summary>
/// Mono控制器，Mono，需要挂载在GameManager
/// MonoMgr利用的工具，所有通过MonoMgr注册的事件都在这里执行
/// </summary>
public class MonoController : MonoSingleton<MonoController>
{
    UnityAction updateAction = null;
    void Update()
    {
        if (updateAction != null)
        { updateAction(); }
    }
    /// <summary>
    /// 注册一个Update事件(只被MonoMgr调用)
    /// </summary>
    /// <param name="func">注册事件</param>
    public void AddUpdateListener(UnityAction func)
    {
        updateAction += func;
    }
    /// <summary>
    /// 删除一个Update事件(只被MonoMgr调用)
    /// </summary>
    /// <param name="func">删除事件</param>
    public void RemoveUpdateListener(UnityAction func)
    {
        updateAction -= func;
    }
}
