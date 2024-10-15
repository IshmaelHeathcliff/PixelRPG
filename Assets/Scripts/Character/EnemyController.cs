using System.Collections;
using QFramework;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyController : MonoBehaviour, IController
{
    [SerializeField] float _speed;
    [SerializeField] float _detectRadius;

    Vector2 _direction;
    Animator _animator;
    Rigidbody2D _rigidbody;

    bool _isPlayerFound;

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
            if (_isPlayerFound)
            {
                break;
            }

            Face(RandomDirection());
            yield return new WaitForSeconds(2);
        }
    }

    void Move()
    {
        if (_isPlayerFound)
        {
            Face(((Vector2) (this.SendQuery(new PlayerPositionQuery()) - transform.position)).normalized);
        }

        _rigidbody.linearVelocity = _direction * _speed;
    }

    void FindPlayer()
    {
        if (Vector2.Distance(this.SendQuery(new PlayerPositionQuery()), transform.position) < _detectRadius)
        {
            _isPlayerFound = true;
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

    public IArchitecture GetArchitecture()
    {
        return PixelRPG.Interface;
    }
}