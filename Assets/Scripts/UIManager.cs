using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private Text _score;
    [SerializeField]
    private Sprite[] _livesSprites;
    [SerializeField]
    private Image _livesUIImage;
    [SerializeField]
    private Text _gameOverText;
    [SerializeField]
    private Text _restartText;
    [SerializeField]
    private Text _ammoText;
    [SerializeField]
    private GameManager _gameManager;
    [SerializeField]
    private Slider _speedSlider;
    
    public bool IsAmmoCoroutineActive  { get; set; } 


    void Start()
    {
     
        _score.text = "Score: " + 0;
        _livesUIImage.sprite = _livesSprites[3];
        _gameOverText.gameObject.SetActive(false);
        _restartText.gameObject.SetActive(false);

        try
        {
            _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        }
        catch (System.Exception)
        {
            throw new ArgumentNullException("Game Manager", "NULL, cannot find GameManager");
        }

    }

    public void ShowThruster (float speed)
    {
        _speedSlider.value = speed;

    }
         
    public void ShowScore (int score)
    {
        _score.text = "Score: " + score;
        
    }
    public void ShowAmmoCount (int ammo, int maxAmmo)
    {
        _ammoText.text = $"Ammo: {ammo} / {maxAmmo}";
    }

    public void SetLivesImage(int lives)
    {
        _livesUIImage.sprite = _livesSprites[lives];

        if (lives == 0)
        {
            GameOverSecuence();      
        }
    }

    public void GameOverSecuence()
    {
        _gameManager.GameOver();
        StartCoroutine(GameOverFlickering());
        ShowRestartMessage();   
    }
    public void ShowGameOver()
    {
        _gameOverText.gameObject.SetActive(true);
        
    }

    public void DisableGameOver()
    {
        _gameOverText.gameObject.SetActive(false);
        
    }

    public void ShowRestartMessage()
    {
        _restartText.gameObject.SetActive(true);

    }

    IEnumerator GameOverFlickering()
    {
       
       while (true)
        {
            _gameOverText.gameObject.SetActive(!_gameOverText.gameObject.activeSelf);
            yield return new WaitForSeconds(0.5f);
        }
            
        
    }

      
    IEnumerator AmmoCountFlickering()
    {

        IsAmmoCoroutineActive = true;
        
        while (true)
        {
            _ammoText.gameObject.SetActive(!_ammoText.gameObject.activeSelf);
            yield return new  WaitForSeconds(0.5f);
            
        }

    }


  
    public void EnableAmmoText()
    {
        _ammoText.gameObject.SetActive(true);
    }       


    public void StopAmmoCoroutineSecuence()
    {
        StopCoroutine("AmmoCountFlickering");
        IsAmmoCoroutineActive = false;
       GetComponent<UIManager>().EnableAmmoText();  //In case Coroutine stops while Text is disabled
    } 

    public Slider GetSlider()
    {
        return _speedSlider;
    }
 }


   

