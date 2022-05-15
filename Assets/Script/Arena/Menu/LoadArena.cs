using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadArena : MonoBehaviour
{
    public void loadArena()
    {
        SceneManager.LoadScene("Arena");
    }

    public void loadDOP()
    {
        SceneManager.LoadScene("JoinMenu");
    }
}
