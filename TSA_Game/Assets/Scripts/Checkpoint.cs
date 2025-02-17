using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        var Player = other.GetComponent<Player>();
        if (Player != null)
        {
            Player.SetRespawnPoint(transform.position);
        }
    }
}
