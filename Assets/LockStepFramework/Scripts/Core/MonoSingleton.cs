#if _CLIENTLOGIC_
using UnityEngine;
 
public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static readonly object _locker = new object();
    private static T _instance = null;

    public static T instance
    {
        get
        {
            lock (_locker)
            {
                if (_instance == null)
                {
                    // search for existing instance
                    _instance = FindObjectOfType(typeof(T)) as T;

                    // create new instance if one doesn't already exist
                    if (_instance == null)
                    {
                        // Need to create a new GameObject to attach the singleton to
                        var singletonObject = new GameObject();
                        singletonObject.name = typeof(T).ToString() + " (MonoSingleton)";
                        DontDestroyOnLoad(singletonObject);

                        _instance = singletonObject.AddComponent<T>();
                    }
                }

                return _instance;
            }
        }
    }
}
#endif