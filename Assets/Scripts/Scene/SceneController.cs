using UnityEngine.SceneManagement;

public class SceneController : Singleton<SceneController>
{
    public void LoadScene(string sceneName)
    {
        // InputController.Instance.Disable();
        SceneManager.LoadSceneAsync(sceneName);
    }

}