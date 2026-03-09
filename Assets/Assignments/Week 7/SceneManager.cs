using UnityEngine;

public class SceneManager : MonoBehaviour
{
    public void OpenScene(int sceneIndex)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneIndex);
    }
}
