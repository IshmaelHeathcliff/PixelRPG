using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

public class Test : MonoBehaviour
{
    [FormerlySerializedAs("size")] public Vector2Int _size;

    public Vector2Int GetSize()
    {
        return _size;
    }

    [Button]
    void ChangeSize(Vector2Int newSize)
    {
        var s = GetSize();
        s = newSize;
    }
    // Start is called before the first frame update
    void Start()
    {
        


    }
}
