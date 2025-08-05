using UnityEngine;
using UnityEngine.SceneManagement;

public class LocationSelector : MonoBehaviour
{
    public void loads1()
    {
        SceneManager.LoadScene("s1"); 
    }

    public void loads2()
    {
        SceneManager.LoadScene("s2"); 
    }
    public void loads3()
    {
        SceneManager.LoadScene("s3");
    }
    public void ToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
