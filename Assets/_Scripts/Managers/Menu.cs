using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
  

    private void Start()
    {
       
    }

    public void Play()
    {
        SceneManager.LoadScene(1); 
    }
    public void Exit()
    {
        Application.Quit();
    }
}
