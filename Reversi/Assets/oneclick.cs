using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class oneclick : MonoBehaviour
{
    
    void OnMouseDown()
    {
        SceneManager.LoadScene("ChessBoard");
    }
}
