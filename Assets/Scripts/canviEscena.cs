using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class canviEscena : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void anarJoc()
    {
        SceneManager.LoadScene("Autopista");
    }
    public void anarGarage()
    {
        SceneManager.LoadScene("Garage");
    }
    
    

    // Update is called once per frame
    /*public void AnarAescenaTitol()
    {
        variablesGlobals.ReiniciarValors();
        SceneManager.LoadScene("escenaTitol");
    }*/
}