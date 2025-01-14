
public abstract class Singleton<T> where T : new()
{
    protected static T _instance = default(T);
    private static readonly object _locker = new object();

    protected Singleton()
    {
    }

    public static T instance
    {
        get
        {
            if (_instance == null)
            {
                lock (_locker)
                {
                    if (_instance == null)
                    {
                        _instance = new T();
                    }
                }
            }
            return _instance;
        }
    }
}
