using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelEnd : MonoBehaviour
{

    private void OnTriggerEnter2D (Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            SceneManager.LoadScene("Scenes/WorldMenu");
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
