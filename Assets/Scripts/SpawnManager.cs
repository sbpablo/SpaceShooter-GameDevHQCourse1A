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


       // When Asteroid is destroyed, the spawn beggins.
       
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


            var randomIndex = UnityEngine.Random.Range(0, _powerUps.Length);
            var powerUpInstance = Instantiate(_powerUps[randomIndex], new Vector3(randomXpos, Ypos, 0), Quaternion.identity);


            /* NO TIENE SENTIDO HACER SWITCH, mucho mejor lo de arriba con lista (array) !!!
            
            var powerUpType = UnityEngine.Random.Range(0 , 3 ); // Al azar si es 0,1,2 para identificar enum de Tipo de PowerUp

            GameObject powerUpInstance=null;

            switch (powerUpType)
            {
               
                case 0: 
                    powerUpInstance = Instantiate(_tripleShotPowerUp, new Vector3(randomXpos, Ypos, 0), Quaternion.identity);
                    break;
                case 1:
                    powerUpInstance = Instantiate(_speedPowerUp, new Vector3(randomXpos, Ypos, 0), Quaternion.identity);
                    break;
                case 2:
                    powerUpInstance = Instantiate(_shieldPowerUp, new Vector3(randomXpos, Ypos, 0), Quaternion.identity);
                    break;
            }

            */

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
}
