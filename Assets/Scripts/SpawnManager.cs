using System.Collections;
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


public class SpawnManager : MonoBehaviour
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
    private Boundary _boundary;
    private Player _player;
    private float _sumOfPowerUpWeights;
    private float[] _powerUpWeights;
    public int TotalEnemiesInCurrentWave{ get; set; }
    private int _totalWaves;
    private int _currentWave;
    private UIManager _uIManager;
    private List<int> _indexesOfEnemiesAlive = new List<int>();
    private AudioSource _nextWaveAudioSource;
    private AudioSource _playerHasWonAudioSorce;
    private AudioSource _backGroundMusic;


    void Start()
    {
        try
        {
            _boundary = GameObject.Find("BoundaryManager").GetComponent<Boundary>();
        }
        catch (Exception)
        {
            throw new ArgumentNullException("BoundaryManager", "NULL, cannot find BoundaryManager");
        }

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
            _uIManager = GameObject.Find("UIManager").GetComponent<UIManager>();
        }
        catch (Exception)
        {
            throw new ArgumentNullException("UIManager", "NULL, cannot find UIManager");
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
        _nextWaveAudioSource.Play();
        
        _indexesOfEnemiesAlive.Clear();

        _currentWave++;

        _uIManager.StartCoroutine("ShowWaveFlickeringSecuence",_currentWave+1);
        
        TotalEnemiesInCurrentWave = 0;
        
        var i = 0;
        foreach(var enemy in _waves[_currentWave].GetEnemiesInWave())
        {
            _indexesOfEnemiesAlive.Add(i);
            TotalEnemiesInCurrentWave += enemy.GetEnemyCount();
            i++;
        }
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
        
        while (_stopSpawning==false && TotalEnemiesInCurrentWave>0 )
        {
         
            var randomXPos = UnityEngine.Random.Range(_boundary.GetBottomCorner().x, _boundary.GetTopCorner().x);
            var YPos = _boundary.GetTopCorner().y;
            var randomEnemyinWave = UnityEngine.Random.Range(0, _indexesOfEnemiesAlive.Count);

            
            if (_indexesOfEnemiesAlive.Count > 0)
            {
                if (_waves[_currentWave].GetEnemiesInWave()[_indexesOfEnemiesAlive[randomEnemyinWave]].GetEnemyCount() > 0)
                {
                    var enemyInstance = Instantiate(_waves[_currentWave].GetEnemiesInWave()[_indexesOfEnemiesAlive[randomEnemyinWave]].GetEnemy(), new Vector3(randomXPos, YPos, 0), Quaternion.identity);
                    enemyInstance.transform.parent = _enemyContainer.transform;
                    _waves[_currentWave].GetEnemiesInWave()[_indexesOfEnemiesAlive[randomEnemyinWave]].DecreaseEnemyCount(1);
                }
                else
                        _indexesOfEnemiesAlive.RemoveAt(randomEnemyinWave);
                  
            }
            
            yield return new WaitForSeconds(_enemySpawnTime);

            if (TotalEnemiesInCurrentWave == 0 && _currentWave+1<_waves.Length) 
            {
                StartWave();

            }
            else if (TotalEnemiesInCurrentWave == 0 && _currentWave + 1 == _waves.Length)
            {
                OnPlayerVictory();
            }
        }
    }

    IEnumerator PowerUpSpawnRoutine()
    {
        yield return new WaitForSeconds(5.0f);

        while (_stopSpawning == false && TotalEnemiesInCurrentWave > 0)
        {
            var randomXpos = UnityEngine.Random.Range(_boundary.GetBottomCorner().x, _boundary.GetTopCorner().x);
            var Ypos = _boundary.GetTopCorner().y;

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
        _uIManager.GameOverSecuence(true);
        

    }

    public void OnWavesEnd()
    {
        _stopSpawning = true;
        Debug.Log("Waves ended, you won");
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

}
