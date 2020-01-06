using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoSingleton<GameManager>
{
    private bool _isGameOver=false ;
    
  

    // Update is called once per frame
    void Update()
    {
        if ( Input.GetKeyDown(KeyCode.R) && _isGameOver == true)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(1); //Puedo poner numero como id o el nombre "Game" - Ver build Settings
        }

        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    public void GameOver()
    {
        _isGameOver = true;
    }
}
