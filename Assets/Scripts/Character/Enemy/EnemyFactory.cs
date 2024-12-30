using System.Collections;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Character
{
    public class EnemyFactory : MonoBehaviour
    {
        [SerializeField] GameObject _enemyPrefab;

        [SerializeField] int _maxCount;
        [SerializeField] float _generateGap;

        void CreateEnemy()
        {
            Instantiate(_enemyPrefab, transform);
        }

        async UniTask ProduceEnemies()
        {
            while (transform.childCount < _maxCount)
            {
                CreateEnemy();
                await UniTask.Delay((int)(_generateGap * 1000)); // ms
            }
        }

        void Start()
        {
            // _enemyPrefab = await AddressablesManager.LoadAsset<GameObject>("Enemy101");
            ProduceEnemies().Forget();
        }
    }
}
