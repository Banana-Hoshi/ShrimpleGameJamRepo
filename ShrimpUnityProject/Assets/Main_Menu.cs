using UnityEngine;
using UnityEngine.SceneManagement;

public class Main_Menu : MonoBehaviour
{

    private void Update()
    {
        transform.Rotate(Vector3.up, 10f * Time.deltaTime);
    }

    public void PlayGame()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void SecretLevel()
    {
        SceneManager.LoadScene("Secret");    
    }
}
