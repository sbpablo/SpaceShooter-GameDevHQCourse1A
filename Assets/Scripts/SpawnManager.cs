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


            _powerUpWeights = new float[_powerUps.Length];
        
        foreach(var obj in _powerUps)
        {
            var weigth= obj.GetComponent<PowerUp>().GetSpawnWeight();
            _sumOfPowerUpWeights += weigth;
            _powerUpWeights[Array.IndexOf(_powerUps, obj)] = weigth;
        }

        

        _currentWave = -1;
        _totalWaves = _waves.Length;
    }


    public void StartWave()
    {
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

        Debug.Log(_indexesOfEnemiesAlive.Count);

        
        
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


            // bool _hasfoundEnemy;

            //var randomEnemyinWave = UnityEngine.Random.Range(0, _waves[_currentWave].GetEnemiesInWave().Length);
            var randomEnemyinWave = UnityEngine.Random.Range(0, _indexesOfEnemiesAlive.Count);

            // NO ESTOY CONTROLANDO POR EL MOMENTO CANTIDADES INDIVIDUALES DE LOS GRUPOS DE ENEMIGOS para el Spawn, 
            // SOLO SI SE MATA UNA CANTIDAD GLOBAL

            if (_waves[_currentWave].GetEnemiesInWave()[randomEnemyinWave].GetEnemyCount() > 0)
            {
                var enemyInstance = Instantiate(_waves[_currentWave].GetEnemiesInWave()[randomEnemyinWave].GetEnemy(), new Vector3(randomXPos, YPos, 0), Quaternion.identity);
                //enemyInstance.transform.SetParent(this.transform);
                enemyInstance.transform.parent = _enemyContainer.transform;
                _waves[_currentWave].GetEnemiesInWave()[randomEnemyinWave].DecreaseEnemyCount(1);
            }
            else
            {
                _indexesOfEnemiesAlive.RemoveAt(randomEnemyinWave);
            }

           
            
  

            yield return new WaitForSeconds(_enemySpawnTime);

            if (TotalEnemiesInCurrentWave == 0)
            {
                StartWave();
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
