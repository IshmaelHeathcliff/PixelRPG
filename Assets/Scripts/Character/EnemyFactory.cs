using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.Serialization;

public class EnemyFactory : MonoBehaviour
{
    GameObject _enemyPrefab;

    [SerializeField] int _maxCount;
    [SerializeField] float _generateGap;

    void CreateEnemy()
    {
        Instantiate(_enemyPrefab, transform);
    }

    IEnumerator ProduceEnemies()
    {
        while (transform.childCount < _maxCount)
        {
            CreateEnemy();
            yield return new WaitForSeconds(_generateGap);
        }
    }

    void Start()
    {
        StartCoroutine(ProduceEnemies());
    }
}
