namespace Core
{
    public interface IObjectPool<T> where T: class
    {
        T CreatObject(); 
        
        public T Pop();
        
        public void Push(T obj);
    }
}