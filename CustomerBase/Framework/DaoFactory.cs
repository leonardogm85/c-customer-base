namespace CustomerBase.Framework
{
    public static class DaoFactory
    {
        public static T CreateDao<T>() where T : Dao, new() => new T();
    }
}
