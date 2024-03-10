using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(BoxCollider2D))]
public class Exit : MonoBehaviour
{
    public string nextSceneName;
    public Vector3 newPos;

    void OnTriggerEnter2D(Collider2D other)
    {
        SceneManager.LoadSceneAsync(nextSceneName);
        GameManager.Instance.Player.transform.position = newPos;
    }
}