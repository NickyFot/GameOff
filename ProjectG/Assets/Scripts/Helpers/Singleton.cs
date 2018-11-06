public class Singleton<T> where T : Singleton<T>, new()
{
    private static readonly T s_Instance = new T();
    public static T Instance
    {
        get
        {
            return s_Instance;
        }
    }
}