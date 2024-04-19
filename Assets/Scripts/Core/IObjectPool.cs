namespace Core
{
    public interface IObjectPool<T> where T: class
    {
        public T Pop();
        
        public void Push(T obj);
    }
}