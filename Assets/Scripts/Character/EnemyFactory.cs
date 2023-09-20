using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class EnemyFactory : MonoBehaviour
{
    [SerializeField] GameObject enemyPrefab;

    public int maxCount;
    public float generateGap;

    void CreateEnemy()
    {
        Instantiate(enemyPrefab, transform);
    }

    IEnumerator ProduceEnemies()
    {
        while (transform.childCount < maxCount)
        {
            CreateEnemy();
            yield return new WaitForSeconds(generateGap);
        }
    }

    void Start()
    {
        StartCoroutine(ProduceEnemies());
    }
}
