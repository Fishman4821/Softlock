using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialSceneChange : MonoBehaviour
{
    public void LoadTutorialScene()
    {
        SceneManager.LoadScene("Scenes/Tutorial");
    }
}
