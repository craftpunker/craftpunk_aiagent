using System.Collections;
using System.Collections.Generic;
using System;
using SimpleJSON;

public class ClassPool : Singleton<ClassPool>
{

    private Dictionary<Type, Queue> poolDictionary = new Dictionary<Type, Queue>();

    /// <summary>
    /// 
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    /// example: pool = new PoolManager();
    ///          Test aa = pool.Pop<Test>(typeof(Test));
    public T Pop<T>() where T : class
    {
        bool isHaveKey = poolDictionary.ContainsKey(typeof(T));

        if (isHaveKey) {
            if (poolDictionary[typeof(T)].Count > 0) {
                var value = poolDictionary[typeof(T)].Dequeue();
                return (T)value;
            }
        }
        else
        {
            Queue queue = new Queue();
            poolDictionary.Add(typeof(T), queue);
        }
        var newData = Activator.CreateInstance(typeof(T));    
        return (T)newData;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="type"></param>
    /// <param name="data"></param>
    public void Push<T>(T data) where T : class
    {
        poolDictionary[data.GetType()].Enqueue(data);
    }

    /// <summary>
    /// 
    /// </summary>
    public void Clear()
    {
        foreach(var queue in poolDictionary) {
            queue.Value.Clear();
        }
        poolDictionary.Clear();
    }

    public void ClearPool(int count) {
        foreach (var value in poolDictionary.Values) {
            int queueCount = value.Count;
            if(queueCount > count) {
                int max = queueCount - count;
                for(int i = 0;i< max; i++) {
                    value.Dequeue();
                }
            }
        }
    }
}

