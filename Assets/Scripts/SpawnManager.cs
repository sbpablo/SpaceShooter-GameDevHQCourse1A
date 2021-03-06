﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;



[Serializable]
public class EnemyBlueprint 
{
    [SerializeField]
    private GameObject _enemy;
    [SerializeField]
    private int _enemyCount;

    public GameObject GetEnemy()
    {
        return _enemy;
    }

    public int GetEnemyCount()
    {
        return _enemyCount;
    }

    public void DecreaseEnemyCount(int value)
    {
        _enemyCount -= value;
    }
}

[Serializable]
public class Wave
{
    [SerializeField]
    private EnemyBlueprint[] _enemiesInWave;
    [SerializeField]
    public int WaveCount { get; set; }

    public EnemyBlueprint[] GetEnemiesInWave()
    {
        return _enemiesInWave;
    }
   
}


public class SpawnManager : MonoSingleton<SpawnManager>
{

    [SerializeField]
    private Wave[] _waves;
    [SerializeField]
    private GameObject _enemyContainer;
    [SerializeField]
    private float _enemySpawnTime=3.0f;
    [SerializeField]
    private GameObject[] _powerUps;
    [SerializeField]
    private GameObject _powerUpContainer;
    [SerializeField]
    private float _powerUpSpawnTime=7.0f;
    private bool _stopSpawning = false;
    private Player _player;
    private float _sumOfPowerUpWeights;
    private float[] _powerUpWeights;
    [SerializeField]
    public int TotalEnemiesInCurrentWave{ get; set; }
    private int _totalWaves;
    [SerializeField]
    private int _currentWave;
    private List<int> _indexesOfEnemiesAlive = new List<int>();
    private AudioSource _nextWaveAudioSource;
    private AudioSource _playerHasWonAudioSorce;
    private AudioSource _backGroundMusic;
    [SerializeField]
    private bool _waveInit;
    private bool _lastBossDefetead;


    private void OnEnable()
    {
        Boss.Destroyed += LastBossDefeated;
    }

    private void OnDisable()
    {
        Boss.Destroyed -= LastBossDefeated;
    }
    void Start()
    {

        try
        {
            _player = GameObject.Find("Player").GetComponent<Player>();
        }
        catch (Exception)
        {
            throw new ArgumentNullException("Player", "NULL, cannot find Player");
        }


        try
        {
            _nextWaveAudioSource = GameObject.Find("AudioManager").transform.Find("WaveFinish").gameObject.GetComponent<AudioSource>();
        }
        catch (Exception)
        {
            throw new ArgumentNullException("AudioManager or Wave Finish Sound", "NULL, cannot find Audio Manager/ Clip");
        }


        try
        {
            _playerHasWonAudioSorce = GameObject.Find("AudioManager").transform.Find("PlayerHasWon").gameObject.GetComponent<AudioSource>();
        }
        catch (Exception)
        {
            throw new ArgumentNullException("AudioManager or PlayerHasWon", "NULL, cannot find Audio Manager/ Clip");
        }

        try
        {
            _backGroundMusic= GameObject.Find("AudioManager").transform.Find("BackgroundMusic").gameObject.GetComponent<AudioSource>();
        }
        catch (Exception)
        {
            throw new ArgumentNullException("AudioManager or BackgroundMusic", "NULL, cannot find Audio Manager/ Clip");
        }


        _powerUpWeights = new float[_powerUps.Length];
        
        foreach(var obj in _powerUps)
        {
            var weigth= obj.GetComponent<PowerUp>().GetSpawnWeight();
            _sumOfPowerUpWeights += weigth;
            _powerUpWeights[Array.IndexOf(_powerUps, obj)] = weigth;
        }

        _currentWave = -1;
        _totalWaves = _waves.Length;


        for (int i=0; i< _waves.Length; i++)
        {
            for (int j=0; j< _waves[i].GetEnemiesInWave().Length;j++)
            {
                if (_waves[i].GetEnemiesInWave()[j].GetEnemy().gameObject == null)
                {
                    Debug.LogError($"SpawnManager Error: In Waves Element {i}, the Enemy Prefab Element {j} has not been asiggned in the inspector ");
                }

                if (_waves[i].GetEnemiesInWave()[j].GetEnemyCount() == 0)
                {
                    Debug.LogWarning($"SpawnManager Warning: In Waves Element {i}, in the Enemy Prefab Element {j}, the enemy count is set to 0, so it will not spawn");
                }
            }
        }

    }

    public void StartWave()
    {

        _waveInit = true;
        _currentWave++;
        _nextWaveAudioSource.Play();
        _indexesOfEnemiesAlive.Clear();
        UIManager.Instance.StartCoroutine("ShowWaveFlickeringSecuence",_currentWave+1);
        
        TotalEnemiesInCurrentWave = 0;
        
        var i = 0;
        foreach(var enemy in _waves[_currentWave].GetEnemiesInWave())
        {
            _indexesOfEnemiesAlive.Add(i);
            TotalEnemiesInCurrentWave += enemy.GetEnemyCount();
            i++;
        }

        _waveInit = false;
    }
   

    public void StartSpawning()
    {
        StartWave();
        StartCoroutine("EnemySpawnRoutine");
        StartCoroutine("PowerUpSpawnRoutine");
    }
    IEnumerator EnemySpawnRoutine()
    {
        yield return new WaitForSeconds(2.0f);
        
        while (_stopSpawning==false && _waveInit==false  )  //  TotalEnemiesInCurrentWave > 0
        {
         
            var randomXPos = UnityEngine.Random.Range(Boundary.Instance.GetBottomCorner().x, Boundary.Instance.GetTopCorner().x);
            var YPos = Boundary.Instance.GetTopCorner().y;
            var randomEnemyinWave = UnityEngine.Random.Range(0, _indexesOfEnemiesAlive.Count);

            
            if (_indexesOfEnemiesAlive.Count > 0)
            {
                if (_waves[_currentWave].GetEnemiesInWave()[_indexesOfEnemiesAlive[randomEnemyinWave]].GetEnemyCount() > 0)
                {
                    var enemyInstance = Instantiate(_waves[_currentWave].GetEnemiesInWave()[_indexesOfEnemiesAlive[randomEnemyinWave]].GetEnemy(), new Vector3(randomXPos, YPos, 0), Quaternion.identity);
                    enemyInstance.transform.parent = _enemyContainer.transform;
                    enemyInstance.name = enemyInstance.name +" Wave: "+  (_currentWave + 1) +" Element: " + randomEnemyinWave 
                                         + " " + "Count: " + _waves[_currentWave].GetEnemiesInWave()[_indexesOfEnemiesAlive[randomEnemyinWave]].GetEnemyCount();

                    
                    _waves[_currentWave].GetEnemiesInWave()[_indexesOfEnemiesAlive[randomEnemyinWave]].DecreaseEnemyCount(1);
                    
                    if (_waves[_currentWave].GetEnemiesInWave()[_indexesOfEnemiesAlive[randomEnemyinWave]].GetEnemyCount() == 0)
                    {
                        _indexesOfEnemiesAlive.RemoveAt(randomEnemyinWave);
                    }  
                }
            }
            
            if (TotalEnemiesInCurrentWave <= 0)
            {
                if (_currentWave+1 < _waves.Length)
                {
                    StartWave();
                }
                else  if (_lastBossDefetead)
                {
                    OnPlayerVictory();
                }
            }
            
            yield return new WaitForSeconds(_enemySpawnTime);

        }
    }

    IEnumerator PowerUpSpawnRoutine()
    {
        yield return new WaitForSeconds(5.0f);

        while (_stopSpawning == false) // && TotalEnemiesInCurrentWave > 0
        {
            var randomXpos = UnityEngine.Random.Range(Boundary.Instance.GetBottomCorner().x, Boundary.Instance.GetTopCorner().x);
            var Ypos = Boundary.Instance.GetTopCorner().y;

            int randomIndex; 
            
            if (_player.GetAmmoCount() == 0) 
            { 
                randomIndex = 3; //Ammo PowerUp
            }
            else 
            {
                randomIndex = PowerUpSelection(_powerUpWeights);
            }

                var powerUpInstance = Instantiate(_powerUps[randomIndex], new Vector3(randomXpos, Ypos, 0), Quaternion.identity);


            if (powerUpInstance!= null)
            {
                powerUpInstance.transform.parent = _powerUpContainer.transform;
            }
            
            yield return new WaitForSeconds(_powerUpSpawnTime);
        }        
    }

    public void OnPlayerDeath()
    {
        _stopSpawning = true;

    }

    public void OnPlayerVictory()
    {
        _stopSpawning = true;
        _backGroundMusic.Stop();
        _playerHasWonAudioSorce.Play();
        UIManager.Instance.GameOverSecuence(true);
        
    }

    public int PowerUpSelection (float [] probs)
    {
        float randomPoint = UnityEngine.Random.value * _sumOfPowerUpWeights;

        for (int i = 0; i < _powerUpWeights.Length; i++)
        {
            if (randomPoint < _powerUpWeights[i])
            {
                return i;
            }
            else
            {
                randomPoint -= _powerUpWeights[i];
            }   
        }
        return _powerUpWeights.Length - 1;   
    }

    public bool LastBossDefeated()
    {
        _lastBossDefetead = true;
        return _lastBossDefetead;
    }


}
