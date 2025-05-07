using UnityEngine;

public class TestObject : MonoBehaviour
{
    public string _testString = "Test 0";
    void Awake()
    {
        Debug.Log($"Awake: {_testString}");
    }

    void OnEnable()
    {
        Debug.Log($"OnEnable: {_testString}");
    }

    void Start()
    {
        Debug.Log($"Start: {_testString}");
    }
}