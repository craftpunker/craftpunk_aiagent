#if _CLIENTLOGIC_
using System.Collections.Generic;
using UnityEngine;

public class GameObjectPool : MonoSingleton<GameObjectPool>
{
    private Transform m_poolTrans;
    private float m_lastCullTime;
    private float m_poolCullInterval = 30; //

    //
    private Dictionary<string, GOContainer> m_containerDict = new Dictionary<string, GOContainer>();

    private void Start()
    {
        m_poolTrans = transform;
        m_lastCullTime = Time.realtimeSinceStartup;
    }

    private void Update()
    {
        CullGOContainer();
    }

    public GameObject PopInstance(string assetName)
    {
        if (m_containerDict.TryGetValue(assetName, out GOContainer container))
        {
            var go = container.PopInstance();
            return go;
        }
        else
            return null;
    }

    public void PushInstance(GameObject go)
    {
        string assetName = go.name;
        if (m_containerDict.TryGetValue(assetName, out GOContainer container))
        {
            container.PushInstance(go);
        }
        else
        {
            GOContainer newContainer = new GOContainer(assetName);
            newContainer.PushInstance(go);
            m_containerDict.Add(assetName, newContainer);
        }

        go.transform.SetParent(m_poolTrans);
    }

    public bool IsExist(string assetName)
    {
        if (m_containerDict.TryGetValue(assetName, out GOContainer container))
        {
            if(container.IsEmpty)
                return false;
            else return true;
        }

        return false;
    }

    public void CullGOContainer()
    {
        if (Time.realtimeSinceStartup - m_lastCullTime < m_poolCullInterval)
            return;

        m_lastCullTime += m_poolCullInterval;

        List<string> expiration = new List<string>();
        var etor = m_containerDict.GetEnumerator();
        while (etor.MoveNext())
        {
            GOContainer container = etor.Current.Value;
            if (container.IsEmpty && container.IsExpired())
                expiration.Add(etor.Current.Key);
            else
                container.CullInstances();
        }

        foreach (var containerName in expiration)
        {
            GOContainer container = m_containerDict[containerName];
            container.Clear();
        }
    }
}
#endif