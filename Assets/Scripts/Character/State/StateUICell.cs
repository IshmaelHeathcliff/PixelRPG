using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

namespace Character.State
{
    public class StateUICell : MonoBehaviour
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

        public void SetInfo(string stateName, string description)
        {
            _info.text = $"{stateName}\n<size=60%>{description}";
        }

        public void EnableInfo()
        {
            _info.gameObject.SetActive(true);
        }

        public void DisableInfo()
        {
            _info.gameObject.SetActive(false);
        }

        public async UniTaskVoid SetIcon(string iconPath)
        {
            AddressablesManager.Release(_iconHandle);
            _iconHandle = Addressables.LoadAssetAsync<Sprite>(iconPath);
            _icon.sprite = await _iconHandle;
        }
        
        public void InitStateUICell(IState state)
        {
            if (state is IStateWithTime bt)
            {
                _slider.gameObject.SetActive(true);
                SetTime(bt.TimeLeft, bt.Duration);
            }

            if (state is IStateWithCount bc)
            {
               _count.gameObject.SetActive(true);
               SetCount(bc.Count);
            }
            
            SetInfo(state.GetName(), state.GetDescription());
            SetIcon(state.GetIconPath()).Forget();
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