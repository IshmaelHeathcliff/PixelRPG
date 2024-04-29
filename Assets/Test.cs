using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class Test : MonoBehaviour
{
    public Vector2Int size;

    public Vector2Int GetSize()
    {
        return size;
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
