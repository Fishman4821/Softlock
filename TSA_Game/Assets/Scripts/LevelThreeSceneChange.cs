using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelThreeSceneChange : MonoBehaviour
{
    public void LoadLevelThreeScene()
    {
        SceneManager.LoadScene("Scenes/LevelThree");
    }
}
