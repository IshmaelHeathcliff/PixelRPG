using System;
using System.Collections.Generic;
using QFramework;
using UnityEngine;

namespace Character.Buff
{
    public interface IBuffUI
    {
        public void AddBuff(IBuff buff);
        public void RemoveBuff(string id);
        public void ChangeBuffTime(IBuffWithTime buff);
        public void ChangeBuffCount(IBuffWithCount buff);
    }
    
    public abstract class BuffUI: MonoBehaviour, IBuffUI
    {
        public abstract void AddBuff(IBuff buff);
        public abstract void RemoveBuff(string id);
        public abstract void ChangeBuffTime(IBuffWithTime buff);
        public abstract void ChangeBuffCount(IBuffWithCount buff);
    }

    public abstract class BuffController : MonoBehaviour, IController
    {
        protected IBuffContainer BuffContainer;
        [SerializeField]protected BuffUI BuffUI;


        void OnValidate()
        {
            BuffUI = GetComponentInChildren<BuffUI>();
        }

        protected virtual void Awake()
        {
            BuffContainer.OnBuffAdded += BuffUI.AddBuff;
            BuffContainer.OnBuffRemoved += BuffUI.RemoveBuff;
            BuffContainer.OnBuffTimeChanged += BuffUI.ChangeBuffTime;
            BuffContainer.OnBuffCountChanged += BuffUI.ChangeBuffCount;
        }

        protected void FixedUpdate()
        {
            BuffContainer.DecreaseBuffTime(Time.fixedDeltaTime);
        }

        public IArchitecture GetArchitecture()
        {
            return PixelRPG.Interface;
        }
    }
}