using QFramework;
using UnityEngine;

namespace Character.Buff
{
    public class BuffController : MonoBehaviour, IController
    {
        public IArchitecture GetArchitecture()
        {
            return PixelRPG.Interface;
        }
    }
}