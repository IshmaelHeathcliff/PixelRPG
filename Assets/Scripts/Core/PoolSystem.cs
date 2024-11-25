using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace Core
{
    public interface IObjectPool<T> where T: class
    {
        T Pop();
        void Push(T obj);
    }
    
    public interface IAsyncObjectPool<T> where T: class
    {
        UniTask<T> Pop();
        void Push(T obj);
    }
    
    public abstract class PoolSystem<T> : AbstractSystem, IObjectPool<T> where T: class
    {
        public int MaxSize { private get; set; } = 1000;
        protected Stack<T> Pool;
        public virtual T Pop()
        {
            var obj = Pool.Count > 0 ? Pool.Pop() : CreateObject();

            PresetPopObject(obj);
            return obj;
        }

        public virtual void Push(T obj)
        {
            if (Pool.Count > MaxSize)
            {
                DestroyObject(obj);
                return;
            }

            PresetPushObject(obj);
            Pool.Push(obj);
        }

        public virtual void InitPool(int size)
        {
            Pool = new Stack<T>();

            for (var i = 0; i < size; i++)
            {
                Pool.Push(CreateObject());
            }
            
        }

        protected abstract T CreateObject();
        protected abstract void DestroyObject(T obj);
        protected abstract void PresetPopObject(T obj);
        protected abstract void PresetPushObject(T obj);

        protected override void OnInit()
        {
            Pool = new Stack<T>();
        }
    }

    public abstract class AsyncPoolSystem<T> : PoolSystem<T>, IAsyncObjectPool<T> where T: class
    {
        public new virtual async UniTask<T> Pop()
        {
            T obj;
            if(Pool.Count > 0)
            {
                obj = Pool.Pop();
            }
            else
            {
                obj = await CreateObjectAsync();
            }

            PresetPopObject(obj);
            return obj;
        }

        public new virtual async void InitPool(int size)
        {
            Pool = new Stack<T>();

            for (var i = 0; i < size; i++)
            {
                Pool.Push(await CreateObjectAsync());
            }
        }

        protected sealed override T CreateObject()
        {
            return null;
        }
        
        protected abstract UniTask<T> CreateObjectAsync();

    }
}