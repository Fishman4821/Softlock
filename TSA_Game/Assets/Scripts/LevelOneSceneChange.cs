using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelOneSceneChange : MonoBehaviour
{
    public void LoadLevelOneScene()
    {
        SceneManager.LoadScene("Scenes/LevelOne");
    }
}
