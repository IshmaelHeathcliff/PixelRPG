using System.Collections.Generic;
using Character.State;
using Character.Stat;
using UnityEngine;

namespace Character
{
    public interface ICharacterModel
    {
        float Speed { get; set; }
        Vector2 Position { get; set; }
        Vector2 Direction { get; set; }
        Stats Stats { get; }
        IStateContainer StateContainer { get; }
        void BindTransform(Transform transform);
    }
    
    public abstract class CharacterModel: ICharacterModel
    {
        Transform _transform;
        public void BindTransform(Transform transform)
        {
            _transform = transform;
        }
        
        public float Speed { get; set; }

        public Vector2 Position
        {
            get => _transform.position;
            set => _transform.position = value;
        }

        public Vector2 Direction { get; set; }
        public Stats Stats { get; } = new Stats();
        public IStateContainer StateContainer { get; } = new StateContainer();
    }

    public abstract class CharactersModel<T> : AbstractModel where T : ICharacterModel
    {
        readonly Dictionary<string, T> _models = new();
        
        public T GetModel(string id)
        {
            return _models[id];
        }
        
        public T AddModel(string id, T model)
        {
            _models.Add(id, model);
            return model;
        }
        
        public void RemoveModel(string id)
        {
            _models.Remove(id);
        }

        public abstract T Default();
    }
}