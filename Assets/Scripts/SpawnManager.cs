using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SpawnManager : MonoBehaviour
{
    [SerializeField]
    private GameObject _enemy;
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

        _powerUpWeights = new float[_powerUps.Length];
        
        foreach(var obj in _powerUps)
        {
            var weigth= obj.GetComponent<PowerUp>().GetSpawnWeight();
            _sumOfPowerUpWeights += weigth;
            _powerUpWeights[Array.IndexOf(_powerUps, obj)] = weigth;
        }

    }
    public void StartSpawning()
    {
        StartCoroutine("EnemySpawnRoutine");
        StartCoroutine("PowerUpSpawnRoutine");
    }
    IEnumerator EnemySpawnRoutine()
    {
        yield return new WaitForSeconds(2.0f);
        
        while (_stopSpawning==false)
        {
         
            var randomXPos = UnityEngine.Random.Range(_boundary.GetBottomCorner().x, _boundary.GetTopCorner().x);
            var YPos = _boundary.GetTopCorner().y;
            var enemyInstance=Instantiate(_enemy, new Vector3(randomXPos, YPos, 0), Quaternion.identity);
            //enemyInstance.transform.SetParent(this.transform);
            enemyInstance.transform.parent = _enemyContainer.transform;
            yield return new WaitForSeconds(_enemySpawnTime);
        }
    }

    IEnumerator PowerUpSpawnRoutine()
    {
        yield return new WaitForSeconds(5.0f);

        while (_stopSpawning == false)
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
