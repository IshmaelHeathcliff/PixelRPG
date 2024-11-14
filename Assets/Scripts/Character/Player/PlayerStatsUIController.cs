using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

namespace Character.Stat
{
    public class PlayerStatsUIController : MonoBehaviour, IController
    {
        [SerializeField] TextMeshProUGUI _text;
        
        PlayerModel _playerModel;

        // TODO: 优化属性更新方式，不再一次性更新所有
        void UpdateStatsInfo()
        {
            var info = new StringBuilder();
            foreach (var stat in _playerModel.PlayerStats.GetAllStats())
            {
                info.Append($"{stat.Name}: {(int)stat.Value}\n");
                info.Append($"  {stat.Name}基础值: {(int)stat.BaseValue}\n");
                info.Append($"  {stat.Name}附加值: {(int)stat.AddedValue}\n");
                info.Append($"  {stat.Name}固定值: {(int)stat.FixedValue}\n");
                info.Append($"  {stat.Name}提高: {(int)stat.Increase}%\n");
                info.Append($"  {stat.Name}总增: {(int)((stat.More-1)*100)}%\n");
            }
            
            _text.text = info.ToString();
        }

        void OnEnable()
        {
            _playerModel = this.GetModel<PlayerModel>();
            foreach (var stat in _playerModel.PlayerStats.GetAllStats())
            {
                stat.Register(UpdateStatsInfo);
            }
            UpdateStatsInfo();
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