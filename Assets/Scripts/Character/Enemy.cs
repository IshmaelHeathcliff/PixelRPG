using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class Enemy : MonoBehaviour
{
    public float speed;
    public float detectRadius;

    Vector2 _direction;
    Animator _animator;
    Rigidbody2D _rigidbody;

    bool _isPlayerFount;

    static readonly int X = Animator.StringToHash("X");
    static readonly int Y = Animator.StringToHash("Y");

    void Face(Vector2 direction)
    {
        _animator.SetFloat(X, direction.x);
        _animator.SetFloat(Y, direction.y);
        _direction = direction.normalized;
    }


    Vector2 RandomDirection()
    {
        float x = Random.Range(-1f, 1f);
        float y = Random.Range(-1f, 1f);
        return new Vector2(x, y);
    }

    IEnumerator ChangeDirection()
    {
        while (true)
        {
            if (_isPlayerFount)
            {
                break;
            }

            Face(RandomDirection());
            yield return new WaitForSeconds(2);
        }
    }

    void Move()
    {
        if (_isPlayerFount)
        {
            Face(((Vector2) (GameManager.Instance.Player.transform.position - transform.position)).normalized);
        }

        _rigidbody.velocity = _direction * speed;
    }

    void FindPlayer()
    {
        if (Vector2.Distance(GameManager.Instance.Player.transform.position, transform.position) < detectRadius)
        {
            _isPlayerFount = true;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
        _rigidbody = GetComponent<Rigidbody2D>();
        StartCoroutine(ChangeDirection());
    }

    void FixedUpdate()
    {
        FindPlayer();
        Move();
    }
}