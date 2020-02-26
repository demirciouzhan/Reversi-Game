using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class otherscene : MonoBehaviour
{
   


    public void scenegame()
    {
        SceneManager.LoadScene("ChessBoard");
    }
    public void menu()
    {
        SceneManager.LoadScene("menu");
    }
    public void Nasil()
    {
        SceneManager.LoadScene("nasiloynanir");
    }
    public void cikis()
    {
        Application.Quit();
        Debug.Log("çıkış");
    }
}
