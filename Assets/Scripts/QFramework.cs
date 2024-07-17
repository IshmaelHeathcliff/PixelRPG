/****************************************************************************
 * Copyright (c) 2015 ~ 2024 liangxiegame MIT License
 *
 * QFramework v1.0
 *
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 *
 * Author:
 *  liangxie        https://github.com/liangxie
 *  soso            https://github.com/so-sos-so
 *
 * Contributor
 *  TastSong        https://github.com/TastSong
 *  京产肠饭         https://gitee.com/JingChanChangFan/hk_-unity-tools
 *  猫叔(一只皮皮虾) https://space.bilibili.com/656352/
 *  misakiMeiii     https://github.com/misakiMeiii
 *  New一天
 *  幽飞冷凝雪～冷
 *
 * Community
 *  QQ Group: 623597263
 * 
 * Latest Update: 2024.5.12 20:17 add UnRegisterWhenCurrentSceneUnloaded(Suggested by misakiMeiii) 
 ****************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace QFramework
{
    #region Architecture

    public interface IArchitecture
    {
        void RegisterSystem<T>(T system) where T : ISystem;

        void RegisterModel<T>(T model) where T : IModel;

        void RegisterUtility<T>(T utility) where T : IUtility;

        T GetSystem<T>() where T : class, ISystem;

        T GetModel<T>() where T : class, IModel;

        T GetUtility<T>() where T : class, IUtility;

        void SendCommand<T>(T command) where T : ICommand;

        TResult SendCommand<TResult>(ICommand<TResult> command);

        TResult SendQuery<TResult>(IQuery<TResult> query);

        void SendEvent<T>() where T : new();
        void SendEvent<T>(T e);

        IUnRegister RegisterEvent<T>(Action<T> onEvent);
        void UnRegisterEvent<T>(Action<T> onEvent);

        void Deinit();
    }

    public abstract class Architecture<T> : IArchitecture where T : Architecture<T>, new()
    {
        private bool _mInited = false;


        public static Action<T> OnRegisterPatch = architecture => { };

        protected static T MArchitecture;

        public static IArchitecture Interface
        {
            get
            {
                if (MArchitecture == null) MakeSureArchitecture();
                return MArchitecture;
            }
        }


        static void MakeSureArchitecture()
        {
            if (MArchitecture == null)
            {
                MArchitecture = new T();
                MArchitecture.Init();

                OnRegisterPatch?.Invoke(MArchitecture);

                foreach (var model in MArchitecture._mContainer.GetInstancesByType<IModel>().Where(m => !m.Initialized))
                {
                    model.Init();
                    model.Initialized = true;
                }

                foreach (var system in MArchitecture._mContainer.GetInstancesByType<ISystem>()
                             .Where(m => !m.Initialized))
                {
                    system.Init();
                    system.Initialized = true;
                }

                MArchitecture._mInited = true;
            }
        }

        protected abstract void Init();

        public void Deinit()
        {
            OnDeinit();
            foreach (var system in _mContainer.GetInstancesByType<ISystem>().Where(s => s.Initialized)) system.Deinit();
            foreach (var model in _mContainer.GetInstancesByType<IModel>().Where(m => m.Initialized)) model.Deinit();
            _mContainer.Clear();
            MArchitecture = null;
        }

        protected virtual void OnDeinit()
        {
        }

        private IocContainer _mContainer = new IocContainer();

        public void RegisterSystem<TSystem>(TSystem system) where TSystem : ISystem
        {
            system.SetArchitecture(this);
            _mContainer.Register<TSystem>(system);

            if (_mInited)
            {
                system.Init();
                system.Initialized = true;
            }
        }

        public void RegisterModel<TModel>(TModel model) where TModel : IModel
        {
            model.SetArchitecture(this);
            _mContainer.Register<TModel>(model);

            if (_mInited)
            {
                model.Init();
                model.Initialized = true;
            }
        }

        public void RegisterUtility<TUtility>(TUtility utility) where TUtility : IUtility =>
            _mContainer.Register<TUtility>(utility);

        public TSystem GetSystem<TSystem>() where TSystem : class, ISystem => _mContainer.Get<TSystem>();

        public TModel GetModel<TModel>() where TModel : class, IModel => _mContainer.Get<TModel>();

        public TUtility GetUtility<TUtility>() where TUtility : class, IUtility => _mContainer.Get<TUtility>();

        public TResult SendCommand<TResult>(ICommand<TResult> command) => ExecuteCommand(command);

        public void SendCommand<TCommand>(TCommand command) where TCommand : ICommand => ExecuteCommand(command);

        protected virtual TResult ExecuteCommand<TResult>(ICommand<TResult> command)
        {
            command.SetArchitecture(this);
            return command.Execute();
        }

        protected virtual void ExecuteCommand(ICommand command)
        {
            command.SetArchitecture(this);
            command.Execute();
        }

        public TResult SendQuery<TResult>(IQuery<TResult> query) => DoQuery<TResult>(query);

        protected virtual TResult DoQuery<TResult>(IQuery<TResult> query)
        {
            query.SetArchitecture(this);
            return query.Do();
        }

        private TypeEventSystem _mTypeEventSystem = new TypeEventSystem();

        public void SendEvent<TEvent>() where TEvent : new() => _mTypeEventSystem.Send<TEvent>();

        public void SendEvent<TEvent>(TEvent e) => _mTypeEventSystem.Send<TEvent>(e);

        public IUnRegister RegisterEvent<TEvent>(Action<TEvent> onEvent) => _mTypeEventSystem.Register<TEvent>(onEvent);

        public void UnRegisterEvent<TEvent>(Action<TEvent> onEvent) => _mTypeEventSystem.UnRegister<TEvent>(onEvent);
    }

    public interface IOnEvent<T>
    {
        void OnEvent(T e);
    }

    public static class OnGlobalEventExtension
    {
        public static IUnRegister RegisterEvent<T>(this IOnEvent<T> self) where T : struct =>
            TypeEventSystem.Global.Register<T>(self.OnEvent);

        public static void UnRegisterEvent<T>(this IOnEvent<T> self) where T : struct =>
            TypeEventSystem.Global.UnRegister<T>(self.OnEvent);
    }

    #endregion

    #region Controller

    public interface IController : IBelongToArchitecture, ICanSendCommand, ICanGetSystem, ICanGetModel,
        ICanRegisterEvent, ICanSendQuery, ICanGetUtility
    {
    }

    #endregion

    #region System

    public interface ISystem : IBelongToArchitecture, ICanSetArchitecture, ICanGetModel, ICanGetUtility,
        ICanRegisterEvent, ICanSendEvent, ICanGetSystem, ICanInit
    {
    }

    public abstract class AbstractSystem : ISystem
    {
        private IArchitecture _mArchitecture;

        IArchitecture IBelongToArchitecture.GetArchitecture() => _mArchitecture;

        void ICanSetArchitecture.SetArchitecture(IArchitecture architecture) => _mArchitecture = architecture;

        public bool Initialized { get; set; }
        void ICanInit.Init() => OnInit();

        public void Deinit() => OnDeinit();

        protected virtual void OnDeinit()
        {
        }

        protected abstract void OnInit();
    }

    #endregion

    #region Model

    public interface IModel : IBelongToArchitecture, ICanSetArchitecture, ICanGetUtility, ICanSendEvent, ICanInit
    {
    }

    public abstract class AbstractModel : IModel
    {
        private IArchitecture _mArchitecturel;

        IArchitecture IBelongToArchitecture.GetArchitecture() => _mArchitecturel;

        void ICanSetArchitecture.SetArchitecture(IArchitecture architecture) => _mArchitecturel = architecture;

        public bool Initialized { get; set; }
        void ICanInit.Init() => OnInit();
        public void Deinit() => OnDeinit();

        protected virtual void OnDeinit()
        {
        }

        protected abstract void OnInit();
    }

    #endregion

    #region Utility

    public interface IUtility
    {
    }

    #endregion

    #region Command

    public interface ICommand : IBelongToArchitecture, ICanSetArchitecture, ICanGetSystem, ICanGetModel, ICanGetUtility,
        ICanSendEvent, ICanSendCommand, ICanSendQuery
    {
        void Execute();
    }

    public interface ICommand<TResult> : IBelongToArchitecture, ICanSetArchitecture, ICanGetSystem, ICanGetModel,
        ICanGetUtility,
        ICanSendEvent, ICanSendCommand, ICanSendQuery
    {
        TResult Execute();
    }

    public abstract class AbstractCommand : ICommand
    {
        private IArchitecture _mArchitecture;

        IArchitecture IBelongToArchitecture.GetArchitecture() => _mArchitecture;

        void ICanSetArchitecture.SetArchitecture(IArchitecture architecture) => _mArchitecture = architecture;

        void ICommand.Execute() => OnExecute();

        protected abstract void OnExecute();
    }

    public abstract class AbstractCommand<TResult> : ICommand<TResult>
    {
        private IArchitecture _mArchitecture;

        IArchitecture IBelongToArchitecture.GetArchitecture() => _mArchitecture;

        void ICanSetArchitecture.SetArchitecture(IArchitecture architecture) => _mArchitecture = architecture;

        TResult ICommand<TResult>.Execute() => OnExecute();

        protected abstract TResult OnExecute();
    }

    #endregion

    #region Query

    public interface IQuery<TResult> : IBelongToArchitecture, ICanSetArchitecture, ICanGetModel, ICanGetSystem,
        ICanSendQuery
    {
        TResult Do();
    }

    public abstract class AbstractQuery<T> : IQuery<T>
    {
        public T Do() => OnDo();

        protected abstract T OnDo();


        private IArchitecture _mArchitecture;

        public IArchitecture GetArchitecture() => _mArchitecture;

        public void SetArchitecture(IArchitecture architecture) => _mArchitecture = architecture;
    }

    #endregion

    #region Rule

    public interface IBelongToArchitecture
    {
        IArchitecture GetArchitecture();
    }

    public interface ICanSetArchitecture
    {
        void SetArchitecture(IArchitecture architecture);
    }

    public interface ICanGetModel : IBelongToArchitecture
    {
    }

    public static class CanGetModelExtension
    {
        public static T GetModel<T>(this ICanGetModel self) where T : class, IModel =>
            self.GetArchitecture().GetModel<T>();
    }

    public interface ICanGetSystem : IBelongToArchitecture
    {
    }

    public static class CanGetSystemExtension
    {
        public static T GetSystem<T>(this ICanGetSystem self) where T : class, ISystem =>
            self.GetArchitecture().GetSystem<T>();
    }

    public interface ICanGetUtility : IBelongToArchitecture
    {
    }

    public static class CanGetUtilityExtension
    {
        public static T GetUtility<T>(this ICanGetUtility self) where T : class, IUtility =>
            self.GetArchitecture().GetUtility<T>();
    }

    public interface ICanRegisterEvent : IBelongToArchitecture
    {
    }

    public static class CanRegisterEventExtension
    {
        public static IUnRegister RegisterEvent<T>(this ICanRegisterEvent self, Action<T> onEvent) =>
            self.GetArchitecture().RegisterEvent<T>(onEvent);

        public static void UnRegisterEvent<T>(this ICanRegisterEvent self, Action<T> onEvent) =>
            self.GetArchitecture().UnRegisterEvent<T>(onEvent);
    }

    public interface ICanSendCommand : IBelongToArchitecture
    {
    }

    public static class CanSendCommandExtension
    {
        public static void SendCommand<T>(this ICanSendCommand self) where T : ICommand, new() =>
            self.GetArchitecture().SendCommand<T>(new T());

        public static void SendCommand<T>(this ICanSendCommand self, T command) where T : ICommand =>
            self.GetArchitecture().SendCommand<T>(command);

        public static TResult SendCommand<TResult>(this ICanSendCommand self, ICommand<TResult> command) =>
            self.GetArchitecture().SendCommand(command);
    }

    public interface ICanSendEvent : IBelongToArchitecture
    {
    }

    public static class CanSendEventExtension
    {
        public static void SendEvent<T>(this ICanSendEvent self) where T : new() =>
            self.GetArchitecture().SendEvent<T>();

        public static void SendEvent<T>(this ICanSendEvent self, T e) => self.GetArchitecture().SendEvent<T>(e);
    }

    public interface ICanSendQuery : IBelongToArchitecture
    {
    }

    public static class CanSendQueryExtension
    {
        public static TResult SendQuery<TResult>(this ICanSendQuery self, IQuery<TResult> query) =>
            self.GetArchitecture().SendQuery(query);
    }

    public interface ICanInit
    {
        bool Initialized { get; set; }
        void Init();
        void Deinit();
    }

    #endregion

    #region TypeEventSystem

    public interface IUnRegister
    {
        void UnRegister();
    }

    public interface IUnRegisterList
    {
        List<IUnRegister> UnregisterList { get; }
    }

    public static class UnRegisterListExtension
    {
        public static void AddToUnregisterList(this IUnRegister self, IUnRegisterList unRegisterList) =>
            unRegisterList.UnregisterList.Add(self);

        public static void UnRegisterAll(this IUnRegisterList self)
        {
            foreach (var unRegister in self.UnregisterList)
            {
                unRegister.UnRegister();
            }

            self.UnregisterList.Clear();
        }
    }

    public struct CustomUnRegister : IUnRegister
    {
        private Action MOnUnRegister { get; set; }
        public CustomUnRegister(Action onUnRegister) => MOnUnRegister = onUnRegister;

        public void UnRegister()
        {
            MOnUnRegister.Invoke();
            MOnUnRegister = null;
        }
    }

#if UNITY_5_6_OR_NEWER
    public abstract class UnRegisterTrigger : UnityEngine.MonoBehaviour
    {
        private readonly HashSet<IUnRegister> _mUnRegisters = new HashSet<IUnRegister>();

        public IUnRegister AddUnRegister(IUnRegister unRegister)
        {
            _mUnRegisters.Add(unRegister);
            return unRegister;
        }

        public void RemoveUnRegister(IUnRegister unRegister) => _mUnRegisters.Remove(unRegister);

        public void UnRegister()
        {
            foreach (var unRegister in _mUnRegisters)
            {
                unRegister.UnRegister();
            }

            _mUnRegisters.Clear();
        }
    }

    public class UnRegisterOnDestroyTrigger : UnRegisterTrigger
    {
        private void OnDestroy()
        {
            UnRegister();
        }
    }

    public class UnRegisterOnDisableTrigger : UnRegisterTrigger
    {
        private void OnDisable()
        {
            UnRegister();
        }
    }

    public class UnRegisterCurrentSceneUnloadedTrigger : UnRegisterTrigger
    {
        private static UnRegisterCurrentSceneUnloadedTrigger _mDefault;

        public static UnRegisterCurrentSceneUnloadedTrigger Get
        {
            get
            {
                if (!_mDefault)
                {
                    _mDefault = new GameObject("UnRegisterCurrentSceneUnloadedTrigger")
                        .AddComponent<UnRegisterCurrentSceneUnloadedTrigger>();
                }

                return _mDefault;
            }
        }

        private void Awake()
        {
            DontDestroyOnLoad(this);
            hideFlags = HideFlags.HideInHierarchy;
            SceneManager.sceneUnloaded += OnSceneUnloaded;
        }

        private void OnDestroy() => SceneManager.sceneUnloaded -= OnSceneUnloaded;
        void OnSceneUnloaded(UnityEngine.SceneManagement.Scene scene) => UnRegister();
    }
#endif

    public static class UnRegisterExtension
    {
#if UNITY_5_6_OR_NEWER

        static T GetOrAddComponent<T>(GameObject gameObject) where T : Component
        {
            var trigger = gameObject.GetComponent<T>();

            if (!trigger)
            {
                trigger = gameObject.AddComponent<T>();
            }

            return trigger;
        }

        public static IUnRegister UnRegisterWhenGameObjectDestroyed(this IUnRegister unRegister,
            UnityEngine.GameObject gameObject) =>
            GetOrAddComponent<UnRegisterOnDestroyTrigger>(gameObject)
                .AddUnRegister(unRegister);

        public static IUnRegister UnRegisterWhenGameObjectDestroyed<T>(this IUnRegister self, T component)
            where T : UnityEngine.Component =>
            self.UnRegisterWhenGameObjectDestroyed(component.gameObject);

        public static IUnRegister UnRegisterWhenDisabled<T>(this IUnRegister self, T component)
            where T : UnityEngine.Component =>
            self.UnRegisterWhenDisabled(component.gameObject);

        public static IUnRegister UnRegisterWhenDisabled(this IUnRegister unRegister,
            UnityEngine.GameObject gameObject) =>
            GetOrAddComponent<UnRegisterOnDisableTrigger>(gameObject)
                .AddUnRegister(unRegister);
        
        public static IUnRegister UnRegisterWhenCurrentSceneUnloaded(this IUnRegister self) =>
            UnRegisterCurrentSceneUnloadedTrigger.Get.AddUnRegister(self);
#endif


#if GODOT
		public static IUnRegister UnRegisterWhenNodeExitTree(this IUnRegister unRegister, Godot.Node node)
		{
			node.TreeExiting += unRegister.UnRegister;
			return unRegister;
		}
#endif
    }

    public class TypeEventSystem
    {
        private readonly EasyEvents _mEvents = new EasyEvents();

        public static readonly TypeEventSystem Global = new TypeEventSystem();

        public void Send<T>() where T : new() => _mEvents.GetEvent<EasyEvent<T>>()?.Trigger(new T());

        public void Send<T>(T e) => _mEvents.GetEvent<EasyEvent<T>>()?.Trigger(e);

        public IUnRegister Register<T>(Action<T> onEvent) => _mEvents.GetOrAddEvent<EasyEvent<T>>().Register(onEvent);

        public void UnRegister<T>(Action<T> onEvent)
        {
            var e = _mEvents.GetEvent<EasyEvent<T>>();
            e?.UnRegister(onEvent);
        }
    }

    #endregion

    #region IOC

    public class IocContainer
    {
        private Dictionary<Type, object> _mInstances = new Dictionary<Type, object>();

        public void Register<T>(T instance)
        {
            var key = typeof(T);

            if (_mInstances.ContainsKey(key))
            {
                _mInstances[key] = instance;
            }
            else
            {
                _mInstances.Add(key, instance);
            }
        }

        public T Get<T>() where T : class
        {
            var key = typeof(T);

            if (_mInstances.TryGetValue(key, out var retInstance))
            {
                return retInstance as T;
            }

            return null;
        }

        public IEnumerable<T> GetInstancesByType<T>()
        {
            var type = typeof(T);
            return _mInstances.Values.Where(instance => type.IsInstanceOfType(instance)).Cast<T>();
        }

        public void Clear() => _mInstances.Clear();
    }

    #endregion

    #region BindableProperty

    public interface IBindableProperty<T> : IReadonlyBindableProperty<T>
    {
        new T Value { get; set; }
        void SetValueWithoutEvent(T newValue);
    }

    public interface IReadonlyBindableProperty<T> : IEasyEvent
    {
        T Value { get; }

        IUnRegister RegisterWithInitValue(Action<T> action);
        void UnRegister(Action<T> onValueChanged);
        IUnRegister Register(Action<T> onValueChanged);
    }

    public class BindableProperty<T> : IBindableProperty<T>
    {
        public BindableProperty(T defaultValue = default) => MValue = defaultValue;

        protected T MValue;

        public static Func<T, T, bool> Comparer { get; set; } = (a, b) => a.Equals(b);

        public BindableProperty<T> WithComparer(Func<T, T, bool> comparer)
        {
            Comparer = comparer;
            return this;
        }

        public T Value
        {
            get => GetValue();
            set
            {
                if (value == null && MValue == null) return;
                if (value != null && Comparer(value, MValue)) return;

                SetValue(value);
                _mOnValueChanged.Trigger(value);
            }
        }

        protected virtual void SetValue(T newValue) => MValue = newValue;

        protected virtual T GetValue() => MValue;

        public void SetValueWithoutEvent(T newValue) => MValue = newValue;

        private EasyEvent<T> _mOnValueChanged = new EasyEvent<T>();

        public IUnRegister Register(Action<T> onValueChanged)
        {
            return _mOnValueChanged.Register(onValueChanged);
        }

        public IUnRegister RegisterWithInitValue(Action<T> onValueChanged)
        {
            onValueChanged(MValue);
            return Register(onValueChanged);
        }

        public void UnRegister(Action<T> onValueChanged) => _mOnValueChanged.UnRegister(onValueChanged);

        IUnRegister IEasyEvent.Register(Action onEvent)
        {
            return Register(Action);
            void Action(T _) => onEvent();
        }

        public override string ToString() => Value.ToString();
    }

    internal class ComparerAutoRegister
    {
#if UNITY_5_6_OR_NEWER
        [UnityEngine.RuntimeInitializeOnLoadMethod(UnityEngine.RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void AutoRegister()
        {
            BindableProperty<int>.Comparer = (a, b) => a == b;
            BindableProperty<float>.Comparer = (a, b) => a == b;
            BindableProperty<double>.Comparer = (a, b) => a == b;
            BindableProperty<string>.Comparer = (a, b) => a == b;
            BindableProperty<long>.Comparer = (a, b) => a == b;
            BindableProperty<UnityEngine.Vector2>.Comparer = (a, b) => a == b;
            BindableProperty<UnityEngine.Vector3>.Comparer = (a, b) => a == b;
            BindableProperty<UnityEngine.Vector4>.Comparer = (a, b) => a == b;
            BindableProperty<UnityEngine.Color>.Comparer = (a, b) => a == b;
            BindableProperty<UnityEngine.Color32>.Comparer =
                (a, b) => a.r == b.r && a.g == b.g && a.b == b.b && a.a == b.a;
            BindableProperty<UnityEngine.Bounds>.Comparer = (a, b) => a == b;
            BindableProperty<UnityEngine.Rect>.Comparer = (a, b) => a == b;
            BindableProperty<UnityEngine.Quaternion>.Comparer = (a, b) => a == b;
            BindableProperty<UnityEngine.Vector2Int>.Comparer = (a, b) => a == b;
            BindableProperty<UnityEngine.Vector3Int>.Comparer = (a, b) => a == b;
            BindableProperty<UnityEngine.BoundsInt>.Comparer = (a, b) => a == b;
            BindableProperty<UnityEngine.RangeInt>.Comparer = (a, b) => a.start == b.start && a.length == b.length;
            BindableProperty<UnityEngine.RectInt>.Comparer = (a, b) => a.Equals(b);
        }
#endif
    }

    #endregion

    #region EasyEvent

    public interface IEasyEvent
    {
        IUnRegister Register(Action onEvent);
    }

    public class EasyEvent : IEasyEvent
    {
        private Action _mOnEvent = () => { };

        public IUnRegister Register(Action onEvent)
        {
            _mOnEvent += onEvent;
            return new CustomUnRegister(() => { UnRegister(onEvent); });
        }

        public void UnRegister(Action onEvent) => _mOnEvent -= onEvent;

        public void Trigger() => _mOnEvent?.Invoke();
    }

    public class EasyEvent<T> : IEasyEvent
    {
        private Action<T> _mOnEvent = e => { };

        public IUnRegister Register(Action<T> onEvent)
        {
            _mOnEvent += onEvent;
            return new CustomUnRegister(() => { UnRegister(onEvent); });
        }

        public void UnRegister(Action<T> onEvent) => _mOnEvent -= onEvent;

        public void Trigger(T t) => _mOnEvent?.Invoke(t);

        IUnRegister IEasyEvent.Register(Action onEvent)
        {
            return Register(Action);
            void Action(T _) => onEvent();
        }
    }

    public class EasyEvent<T, TK> : IEasyEvent
    {
        private Action<T, TK> _mOnEvent = (t, k) => { };

        public IUnRegister Register(Action<T, TK> onEvent)
        {
            _mOnEvent += onEvent;
            return new CustomUnRegister(() => { UnRegister(onEvent); });
        }

        public void UnRegister(Action<T, TK> onEvent) => _mOnEvent -= onEvent;

        public void Trigger(T t, TK k) => _mOnEvent?.Invoke(t, k);

        IUnRegister IEasyEvent.Register(Action onEvent)
        {
            return Register(Action);
            void Action(T _, TK __) => onEvent();
        }
    }

    public class EasyEvent<T, TK, TS> : IEasyEvent
    {
        private Action<T, TK, TS> _mOnEvent = (t, k, s) => { };

        public IUnRegister Register(Action<T, TK, TS> onEvent)
        {
            _mOnEvent += onEvent;
            return new CustomUnRegister(() => { UnRegister(onEvent); });
        }

        public void UnRegister(Action<T, TK, TS> onEvent) => _mOnEvent -= onEvent;

        public void Trigger(T t, TK k, TS s) => _mOnEvent?.Invoke(t, k, s);

        IUnRegister IEasyEvent.Register(Action onEvent)
        {
            return Register(Action);
            void Action(T _, TK __, TS ___) => onEvent();
        }
    }

    public class EasyEvents
    {
        private static readonly EasyEvents MGlobalEvents = new EasyEvents();

        public static T Get<T>() where T : IEasyEvent => MGlobalEvents.GetEvent<T>();

        public static void Register<T>() where T : IEasyEvent, new() => MGlobalEvents.AddEvent<T>();

        private readonly Dictionary<Type, IEasyEvent> _mTypeEvents = new Dictionary<Type, IEasyEvent>();

        public void AddEvent<T>() where T : IEasyEvent, new() => _mTypeEvents.Add(typeof(T), new T());

        public T GetEvent<T>() where T : IEasyEvent
        {
            return _mTypeEvents.TryGetValue(typeof(T), out var e) ? (T)e : default;
        }

        public T GetOrAddEvent<T>() where T : IEasyEvent, new()
        {
            var eType = typeof(T);
            if (_mTypeEvents.TryGetValue(eType, out var e))
            {
                return (T)e;
            }

            var t = new T();
            _mTypeEvents.Add(eType, t);
            return t;
        }
    }

    #endregion


    #region Event Extension

    public class OrEvent : IUnRegisterList
    {
        public OrEvent Or(IEasyEvent easyEvent)
        {
            easyEvent.Register(Trigger).AddToUnregisterList(this);
            return this;
        }

        private Action _mOnEvent = () => { };

        public IUnRegister Register(Action onEvent)
        {
            _mOnEvent += onEvent;
            return new CustomUnRegister(() => { UnRegister(onEvent); });
        }

        public void UnRegister(Action onEvent)
        {
            _mOnEvent -= onEvent;
            this.UnRegisterAll();
        }

        private void Trigger() => _mOnEvent?.Invoke();

        public List<IUnRegister> UnregisterList { get; } = new List<IUnRegister>();
    }

    public static class OrEventExtensions
    {
        public static OrEvent Or(this IEasyEvent self, IEasyEvent e) => new OrEvent().Or(self).Or(e);
    }

    #endregion

#if UNITY_EDITOR
    internal class EditorMenus
    {
        [UnityEditor.MenuItem("QFramework/Install QFrameworkWithToolKits")]
        public static void InstallPackageKit() => UnityEngine.Application.OpenURL("https://qframework.cn/qf");
    }
#endif
}