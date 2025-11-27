using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class AddressablesMgr
{
    private static AddressablesMgr instance = new AddressablesMgr();
    public static AddressablesMgr Instance => instance;

    //有一个容器 帮助我们存储 异步加载的返回值
    public Dictionary<string, IEnumerator> resDic = new Dictionary<string, IEnumerator>();

    private AddressablesMgr() { }

    //异步加载资源的方法
    public void LoadAssetAsync<T>(string name, Action<AsyncOperationHandle<T>> callBack)
    {
        //由于存在同名 不同类型资源的区分加载
        //所以我们通过名字和类型拼接作为 key
        string keyName = name + "_" + typeof(T).Name;
        AsyncOperationHandle<T> handle;
        //如果已经加载过该资源
        if (resDic.ContainsKey(keyName))
        {
            //获取异步加载返回的操作内容
            handle = (AsyncOperationHandle<T>)resDic[keyName];

            //判断 这个异步加载是否结束
            if(handle.IsDone)
            {
                //如果成功 就不需要异步了 直接相当于同步调用了 这个委托函数 传入对应的返回值
                callBack(handle);
            }
            //还没有加载完成
            else
            {
                //如果这个时候 还没有异步加载完成 那么我们只需要 告诉它 完成时做什么就行了
                handle.Completed += (obj) => {
                    if (obj.Status == AsyncOperationStatus.Succeeded)
                        callBack(obj);
                };
            }
            return;
        }
        
        //如果没有加载过该资源
        //直接进行异步加载 并且记录
        handle = Addressables.LoadAssetAsync<T>(name);
        handle.Completed += (obj)=> {
            if (obj.Status == AsyncOperationStatus.Succeeded)
                callBack(obj);
            else
            {
                Debug.LogWarning(keyName + "资源加载失败");
                if(resDic.ContainsKey(keyName))
                    resDic.Remove(keyName);
            }
        };
        resDic.Add(keyName, handle);
    }

    //释放资源的方法 
    public void Release<T>(string name)
    {
        //由于存在同名 不同类型资源的区分加载
        //所以我们通过名字和类型拼接作为 key
        string keyName = name + "_" + typeof(T).Name;
        if(resDic.ContainsKey(keyName))
        {
            //取出对象 移除资源 并且从字典里面移除
            AsyncOperationHandle<T> handle = (AsyncOperationHandle<T>)resDic[keyName];
            Addressables.Release(handle);
            resDic.Remove(keyName);
        }
    }

    //清空资源
    public void Clear()
    {
        resDic.Clear();
        AssetBundle.UnloadAllAssetBundles(true);
        Resources.UnloadUnusedAssets();
        GC.Collect();
    }
}
