using Character.Enemy;
using Cysharp.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Character
{
    public class EnemyFactory : MonoBehaviour, IController
    {
        [SerializeField] GameObject _enemyPrefab;

        [SerializeField] int _maxCount;
        [SerializeField] float _generateGap;

        EnemiesModel _model;

        /// <summary>
        /// 创建EnemyModel，将EnemyModel的相关依赖注入到EnemiesModel中
        /// </summary>
        void CreateEnemy()
        {
            string id = GUID.Generate().ToString();
            _model.AddModel(id, new EnemyModel());
            _enemyPrefab.GetComponent<EnemyController>().ID = id;
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

        void Awake()
        {
            _model = this.GetModel<EnemiesModel>();
        }

        void Start()
        {
            // _enemyPrefab = await AddressablesManager.LoadAsset<GameObject>("Enemy101");
            ProduceEnemies().Forget();
        }

        public IArchitecture GetArchitecture()
        {
            return PixelRPG.Interface;
        }
    }
}
