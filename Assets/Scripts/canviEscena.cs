using UnityEngine;
using UnityEngine.SceneManagement;

public class canviEscena : MonoBehaviour
{
    public void anarJoc()
    {
        SceneManager.LoadScene("Autopista");
    }
    public void anarHistoria()
    {
        SceneManager.LoadScene("historia");
    }
    public void anarGarage()
    {
        SceneManager.LoadScene("Garaje");
    }

    public void anarMenu()
    {
        SceneManager.LoadScene("Inicio");
    }

    public void sortirApp()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
