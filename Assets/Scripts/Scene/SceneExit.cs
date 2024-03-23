using UnityEngine;
using UnityEngine.SceneManagement;

namespace Scene
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class SceneExit : MonoBehaviour
    {
        [SerializeField] string nextSceneName;
        [SerializeField] string entranceTag;

        void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                GameManager.Instance.SceneController.LoadScene(nextSceneName, entranceTag);
            }
        }
    }
}