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
    
    public  GameObject Shield { get; set; }



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
        transform.Translate(Vector3.down * _speed * Time.deltaTime);

        if (transform.position.y <= Boundary.Instance.GetBottomCorner().y)
        {
            var randomX = UnityEngine.Random.Range(Boundary.Instance.GetBottomCorner().x, Boundary.Instance.GetTopCorner().x);

            transform.position = new Vector3(randomX, Boundary.Instance.GetTopCorner().y, 0);
        }
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

   
}
