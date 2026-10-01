using UnityEngine;
using UnityEngine.SceneManagement;

public class MonScript : MonoBehaviour
{

    public void ChangeLaScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}