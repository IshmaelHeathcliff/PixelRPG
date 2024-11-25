using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

namespace Character.Buff
{
    public class BuffUICell : MonoBehaviour
    {
        [SerializeField] Slider _slider;
        [SerializeField] TextMeshProUGUI _info;
        [SerializeField] TextMeshProUGUI _count;
        [SerializeField] Image _icon;

        AsyncOperationHandle<Sprite> _iconHandle;

        public void SetTime(float timeLeft, float duration)
        {
            _slider.value = 1 - timeLeft / duration;
        }

        public void SetCount(int count)
        {
           _count.text = $"{count}";
        }

        public void SetInfo(string buffName, string description)
        {
            _info.text = $"{buffName}\n<size=60%>{description}";
        }

        public void EnableInfo()
        {
            _info.gameObject.SetActive(true);
        }

        public void DisableInfo()
        {
            _info.gameObject.SetActive(false);
        }

        public async void SetIcon(string iconPath)
        {
            AddressablesManager.Release(_iconHandle);
            _iconHandle = Addressables.LoadAssetAsync<Sprite>(iconPath);
            _icon.sprite = await _iconHandle;
        }
        
        public void InitBuffUICell(IBuff buff)
        {
            if (buff is IBuffWithTime bt)
            {
                _slider.gameObject.SetActive(true);
                SetTime(bt.TimeLeft, bt.Duration);
            }

            if (buff is IBuffWithCount bc)
            {
               _count.gameObject.SetActive(true);
               SetCount(bc.Count);
            }
            
            SetInfo(buff.GetName(), buff.GetDescription());
            SetIcon(buff.GetIconPath());
        }


        void OnEnable()
        {
            _slider.gameObject.SetActive(false);
            _count.gameObject.SetActive(false);
            _info.gameObject.SetActive(false);
        }

        void OnDisable()
        {
            AddressablesManager.Release(_iconHandle);
        }
        
        void OnValidate()
        {
            _slider = GetComponentInChildren<Slider>(true);
            _info = transform.Find("Info").GetComponent<TextMeshProUGUI>();
            _count = transform.Find("Count").GetComponent<TextMeshProUGUI>();
            _icon = transform.Find("Icon").GetComponent<Image>();
        }
    }
}