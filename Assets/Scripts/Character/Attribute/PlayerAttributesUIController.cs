using System;
using System.Collections.Generic;
using System.Text;
using QFramework;
using TMPro;
using UnityEngine;

namespace Character
{
    public class PlayerAttributesUIController : MonoBehaviour, IController
    {
        [SerializeField] TextMeshProUGUI _text;
        
        PlayerModel _playerModel;

        void UpdateAttributesInfo()
        {
            var info = new StringBuilder();
            foreach (var attribute in _playerModel.PlayerAttributes.GetAllAttributes())
            {
                info.Append($"{attribute.Name}: {(int)attribute.Value}\n");
                info.Append($"  {attribute.Name}基础值: {(int)attribute.BaseValue}\n");
                info.Append($"  {attribute.Name}附加值: {(int)attribute.AddedValue}\n");
                info.Append($"  {attribute.Name}固定值: {(int)attribute.FixedValue}\n");
                info.Append($"  {attribute.Name}提高: {(int)attribute.Increase}%\n");
                info.Append($"  {attribute.Name}总增: {attribute.More}\n");
                info.Append($"  {attribute.Name}总增: {(int)((attribute.More-1)*100)}%\n");
            }
            
            _text.text = info.ToString();
        }

        void OnEnable()
        {
            _playerModel = this.GetModel<PlayerModel>();
            foreach (var attribute in _playerModel.PlayerAttributes.GetAllAttributes())
            {
                attribute.Register(UpdateAttributesInfo);
            }
            UpdateAttributesInfo();
        }

        void OnValidate()
        {
            _text = GetComponentInChildren<TextMeshProUGUI>();
        }

        public IArchitecture GetArchitecture()
        {
            return PixelRPG.Interface;
        }
    }
}