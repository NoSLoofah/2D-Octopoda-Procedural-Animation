using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Internal;
/// <summary>
/// Mono管理器，非Mono
/// 作用是让非Mono脚本注册Update或协程事件
/// </summary>
public class MonoMgr : BaseMgr<MonoMgr>
{
    MonoController controller;
    public MonoMgr()
    {
        controller = MonoBehaviour.FindObjectOfType<MonoController>();    //将控制器绑定
    }
    #region Update
    /// <summary>
    /// 注册Update事件
    /// </summary>
    /// <param name="func">事件</param>
    public void AddUpdateListener(UnityAction func)
    {
        controller.AddUpdateListener(func);
    }
    /// <summary>
    /// 删除Update事件
    /// </summary>
    /// <param name="func">事件</param>
    public void RemoveUpdateListener(UnityAction func)
    {
        controller.RemoveUpdateListener(func);
    }
    #endregion
    #region 协程
    /// <summary>
    /// 开始一个协程
    /// </summary>
    /// <param name="routine">协程函数</param>
    /// <returns></returns>
    public Coroutine StartCoroutine(IEnumerator routine)
    {
        return controller.StartCoroutine(routine);
    }
    /// <summary>
    /// 开始一个单参数的协程
    /// </summary>
    /// <param name="methodName">协程的函数名</param>
    /// <param name="value"></param>
    /// <returns></returns>
    public Coroutine StartCoroutine(string methodName, object value = null)
    {
        return controller.StartCoroutine(methodName, value);
    }
    /// <summary>
    /// 开始一个无参的协程
    /// </summary>
    /// <param name="methodName">协程的函数名</param>
    /// <returns></returns>
    public Coroutine StartCoroutine(string methodName)
    {
        return controller.StartCoroutine(methodName);
    }
    /// <summary>
    /// 停止一个协程
    /// </summary>
    /// <param name="routine">协程函数名</param>
    public void StopCoroutine(Coroutine routine)
    {
        controller.StopCoroutine(routine);
    }
    /// <summary>
    /// 停止一个协程
    /// </summary>
    /// <param name="methodName">协程函数名</param>
    public void StopCoroutine(string methodName)
    {
        controller.StopCoroutine(methodName);
    }
    #endregion
}
