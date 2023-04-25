using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SpaceShooter
{
    public class MazeFinish : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision == null) return;
            if (collision.transform.root.GetComponent<SpaceShip>() == null) return;

            //перезапуск уровня
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
