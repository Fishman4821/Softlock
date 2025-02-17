using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelFourSceneChange : MonoBehaviour
{
    public void LoadLevelFourScene()
    {
        SceneManager.LoadScene("Scenes/LevelFour");
    }
}
