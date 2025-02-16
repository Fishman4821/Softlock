using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelTwoSceneChange : MonoBehaviour
{
    public void LoadLevelTwoScene()
    {
        SceneManager.LoadScene("Scenes/LevelTwo");
    }
}
