
#if _CLIENTLOGIC_
using System.Collections.Generic;
using UnityEngine;

public class GOContainer
{
    private string m_assetName;

    private Vector3 m_orgPosition = new Vector3(1000, 0, 1000);
    private Vector3 m_orgScale = Vector3.one;
    private Quaternion m_orgRotation = Quaternion.identity;

    private Queue<GameObject> m_goQueue = new Queue<GameObject>(); //

    private int m_capacity = 128; //

    private float m_lastAccessTime = 0f; //
    private float m_expirationTime = 120; // 

    private float m_lastCullTime = 0; //
    private float m_cullInterval = 30; // 

    public bool IsEmpty
    {
        get { return m_goQueue.Count == 0; }
    }

    public GOContainer(string assetName, float cullInterval = 30, float expirationTime = 120)
    {
        m_assetName = assetName;
        m_expirationTime = expirationTime;
        m_cullInterval = cullInterval;

        m_lastAccessTime = Time.realtimeSinceStartup;
    }

    public GameObject PopInstance()
    {
        if (IsEmpty)
            return null;

        var go = m_goQueue.Dequeue();
        go.SetActive(true);

        m_lastAccessTime = Time.realtimeSinceStartup;

        return go;
    }

    public void PushInstance(GameObject go)
    {
        if (go == null)
            return;

        if(m_goQueue.Count <= m_capacity)
        {
            m_goQueue.Enqueue(go);
            go.transform.localPosition = m_orgPosition;
            go.transform.localScale = m_orgScale;
            go.transform.localRotation = m_orgRotation;
            go.SetActive(false);
        }
        else
        {
            //
            GameObject.Destroy(go);
        }
    }

    public void CullInstances()
    {
        float curTime = Time.realtimeSinceStartup;
        if (curTime - m_lastCullTime < m_cullInterval)
            return;

        m_lastCullTime = curTime;

        int total = m_goQueue.Count;
        if (total == 0)
            return;

        int cull = (total == 1) ? 1 : (total / 2);
        for (int index = 0; index < cull; ++index)
        {
            GameObject go = m_goQueue.Dequeue();
            GameObject.Destroy(go);
        }
    }

    public bool IsExpired()
    {
        return Time.realtimeSinceStartup - m_lastAccessTime > m_expirationTime;
    }

    public void Clear()
    {
        var etor = m_goQueue.GetEnumerator();
        while (etor.MoveNext())
        {
            //go etor.Current;
        }

        m_goQueue.Clear();
    }
}
#endif