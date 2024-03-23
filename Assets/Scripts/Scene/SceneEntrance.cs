using System;
using UnityEngine;

namespace Scene
{
    public class SceneEntrance : MonoBehaviour
    {
        public string entranceTag;

        void OnEnable()
        {
            GameManager.Instance.SceneController.RegisterEntrance(entranceTag, this);
        }
    }
}