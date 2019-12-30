﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;



public class Enemy : MonoBehaviour
{
    public enum ShotDirection
    {
        vertical,
        AtEnemy
    };

    public enum AggresionLevel
    {
        None,  // Do not follow player
        Medium, // Follow Player only if the distance < Certain Amount
        Max // Once the enemy starts the chace, it will continue regardless of the distance
    };

    [SerializeField] 
    private protected float _speed=4.0f;
    private protected Player _player;
    [SerializeField]
    private protected int _scoreIfkilled = 10;
    private protected Animator _anim;
    private protected Collider2D _collider;
    private protected AudioSource _explosionAudioSource;
    [SerializeField]
    private GameObject _enemylaserPrefab;
    public bool IsbeingTargeted { get; set; }
    [SerializeField]
    private protected ShotDirection _shotDirection;
    [SerializeField]
    private protected AggresionLevel _aggresion;
    [SerializeField]
    private protected float _distanceToStartChase=8;
    [SerializeField]
    private protected float _chaseSpeedMultiplier = 3f;
    private bool _hasStartedChasing;
    [SerializeField]
    private protected bool _attacksPowerUps;
    [SerializeField]
    [Range(0,1)]
    private float  _attackPowerUpsProbability;
    [SerializeField]
    private protected float _attackPUDistance;
    [SerializeField]
    private protected float _attackPUAngle;

    private bool _hasCheckPowerUpAsTarget;





    public  GameObject Shield { get; set; }



    private void OnEnable()
    {
        PowerUp.OnPowerUpOnScreen += OnPowerUp;
    }

    private void OnDisable()
    {
        PowerUp.OnPowerUpOnScreen -= OnPowerUp;
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

        
        _anim = GetComponent<Animator>(); 

        if (_anim == null)
        {
            Debug.LogError("Animator Component is NULL");
        }

        _collider = GetComponent<Collider2D>();

        if (_collider == null)
        {
            Debug.LogError("Enemy Collider Component is NULL");
        }

        try
        {
            _explosionAudioSource = GameObject.Find("AudioManager").transform.Find("ExplosionSound").GetComponent<AudioSource>();
        }
        catch (Exception)
        {
            throw new ArgumentNullException(" Audio MAnager / Explosion Sound", "NULL, cannot find the AudioManager or  audio source of enemy explosion");
        }

        if (transform.Find("EnemyShield"))
        {
            Shield = transform.Find("EnemyShield").gameObject;
            Debug.Log($"Encontre algun shield!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!! en {this.gameObject.name}" +
                $" y es {Shield.name}");
        }



        StartCoroutine(ShootingRoutine());

    }
    void Update()
    {
        CalculateMovement();
    }

   
    
    
    private protected virtual void  CalculateMovement()
   {

        

        switch (_aggresion)
        {

            case AggresionLevel.None:
                transform.Translate(Vector3.down * _speed * Time.deltaTime);
                break;

            case AggresionLevel.Medium:

                if (_player != null)
                {

                    var direction = _player.transform.position - transform.position;
                    float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + 90;

                    if (direction.magnitude > _distanceToStartChase)
                    {

                        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.identity, 100 * Time.deltaTime);
                        transform.Translate(Vector3.down * _speed * Time.deltaTime);
                    }
                    else
                    {
                        RotateAndTranslate(angle);

                    }
                }
                
               
                break;

            case AggresionLevel.Max:

                if (_player != null)
                {
                    var direction2 = _player.transform.position - transform.position;
                    float angle2 = Mathf.Atan2(direction2.y, direction2.x) * Mathf.Rad2Deg + 90;


                    if (direction2.magnitude <= _distanceToStartChase && _hasStartedChasing == false)
                    {
                        _hasStartedChasing = true;

                    }
                    else
                    {
                        transform.Translate(Vector3.down * _speed * Time.deltaTime);
                    }


                    if (_hasStartedChasing)
                    {
                        RotateAndTranslate(angle2);
                    }

                }


                break; 
        }
        

        if (transform.position.y <= Boundary.Instance.GetBottomCorner().y)
        {
            var randomX = UnityEngine.Random.Range(Boundary.Instance.GetBottomCorner().x, Boundary.Instance.GetTopCorner().x);

            transform.position = new Vector3(randomX, Boundary.Instance.GetTopCorner().y, 0);
        }

    }

    private void RotateAndTranslate(float angle)
    {
        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, 100 * Time.deltaTime);
        transform.Translate(Vector3.down * _speed * _chaseSpeedMultiplier * Time.deltaTime);
        Debug.Log($"Speed:{_speed}, ChaseSpeed:{_chaseSpeedMultiplier}, Sum: {_speed*_chaseSpeedMultiplier}");
    }
    
    public void Shoot (ShotDirection dir)
    {
        if (_player != null)
        { 
            switch (dir)
            {
                case ShotDirection.AtEnemy:

                    var direction = _player.transform.position - transform.position;
                    var angle = Vector3.Angle(-transform.up, direction);
                    var sign = Mathf.Sign(Vector3.Cross(-transform.up, direction).z);

                    //var laser = Instantiate(_enemylaserPrefab, transform.position, Quaternion.Euler(0, 0, angle * sign));
                     var laser = PoolManager.Instance.RetrieveObjectFromPool(_enemylaserPrefab.gameObject.tag);
                     
                    if (laser != null)
                    {
                        laser.transform.position = transform.position;
                        laser.transform.rotation = Quaternion.Euler(0, 0, angle * sign);
                        laser.SetActive(true);
                    } 
                    
                  
                     break;

                case ShotDirection.vertical:

                    //var VerticalLaser = Instantiate(_enemylaserPrefab, transform.position, Quaternion.identity);
                    var verticalLaser= PoolManager.Instance.RetrieveObjectFromPool(_enemylaserPrefab.gameObject.tag);
                    
                    if (verticalLaser != null)
                    {
                        verticalLaser.transform.position = transform.position;
                        verticalLaser.transform.rotation = this.transform.rotation;
                        verticalLaser.SetActive(true);
                    }
                    break;
            }
        }
    }

    
    IEnumerator ShootingRoutine ()
    {
        while (true) 
        {
            
            Shoot(_shotDirection);
            var randomSeconds = UnityEngine.Random.Range(3, 8);
            yield return new WaitForSeconds(randomSeconds);
        }

    }
    private protected virtual void OnTriggerEnter2D (Collider2D other)
    {
        //La funcionalidad del laser o misil es la misma. Voy separarlos provisoriamente para usar el pool de laser.

        if (other.tag == "Laser")
        {
            
            if (Shield==null || Shield.activeSelf==false)
            {
               
                Debug.Log("que carajo pasa " + this.gameObject.tag + "  " + other.gameObject.tag);
                _collider.enabled = false;


                if (_player != null)
                {
                    _player.SetScore(_scoreIfkilled);
                }
                // anim.SetTrigger("OnEnemyDeath");
                _anim.Play("EnemyExplosion", 0, 0.16f);
                _speed = 0;
                _explosionAudioSource.Play();
                Debug.Log("I was hitted: " + this.gameObject.name + " by: " + other.gameObject.tag);
                SpawnManager.Instance.TotalEnemiesInCurrentWave--;
                Destroy(this.gameObject, 2.0f);

            }
            else
            {
                Shield.SetActive(false);
            }

            other.gameObject.SetActive(false);
        }
        
        
        if (other.tag=="Missile")
        {
            Destroy(other.gameObject);
            Debug.Log("que carajo pasa " + this.gameObject.tag + "  " + other.gameObject.tag);
            _collider.enabled = false;
        
            

            if (_player != null)
            {
                _player.SetScore(_scoreIfkilled);
            }
            // anim.SetTrigger("OnEnemyDeath");
            _anim.Play("EnemyExplosion", 0, 0.16f);
            _speed = 0;
            _explosionAudioSource.Play();
            Debug.Log("I was hitted: " + this.gameObject.name + " by: " + other.gameObject.tag);
            SpawnManager.Instance.TotalEnemiesInCurrentWave--;

            if (Shield != null && Shield.activeSelf == true)
            {
                Shield.SetActive(false);
            }

            Destroy(this.gameObject,2.0f);
            
        }


        if (other.tag == "Player")
        {
            _player.Damage(this.gameObject.tag);
            _anim.Play("EnemyExplosion", 0, 0.16f); // 0.16 starts the animation not at the beginning to avoid the enemy sprite in the animation.
            //_anim.SetTrigger("OnEnemyDeath");
            _collider.enabled = false;
            _speed = 0;
            _explosionAudioSource.Play();
            Debug.Log("I was hitted: " + this.gameObject.name + " by: " + other.gameObject.tag);
            SpawnManager.Instance.TotalEnemiesInCurrentWave--;
            

            if (Shield != null && Shield.activeSelf == true)
            {
                Shield.SetActive(false);
            }

            Destroy(this.gameObject, 2.0f);
        }
    }

    private void OnPowerUp(Transform transf)
    {

        var direction = transf.position - this.transform.position;
        var angle = Vector3.Angle(-transform.up, direction);
        var sign = Mathf.Sign(Vector3.Cross(-transform.up, direction).z);

        if (_attacksPowerUps && _hasCheckPowerUpAsTarget==false && direction.magnitude<=_attackPUDistance && angle<=_attackPUAngle)
        {
            _hasCheckPowerUpAsTarget =true;
            
            var probability = UnityEngine.Random.Range(0, 100);

            if (probability < _attackPowerUpsProbability * 100)
            {

                //var laser = Instantiate(_enemylaserPrefab, transform.position, Quaternion.Euler(0, 0, angle * sign));
                var laser = PoolManager.Instance.RetrieveObjectFromPool(_enemylaserPrefab.gameObject.tag);

                if (laser != null)
                {
                    laser.transform.position = transform.position;
                    laser.transform.rotation = Quaternion.Euler(0, 0, (angle-10) * sign);
                    laser.SetActive(true);
                    Debug.Log($"Hay un powerup ubicado en:{transform.localPosition} y lo voy a atacar ya que mi probabilidad es {probability}, la distancia es {direction.magnitude}" +
                        $" y el angulo es {angle}");
                }
            }
        }
    }

   
}
