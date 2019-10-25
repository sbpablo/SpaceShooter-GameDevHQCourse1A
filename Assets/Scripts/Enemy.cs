using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Enemy : MonoBehaviour
{
    [SerializeField] 
    private float _speed=4.0f;
    private Boundary _boundary;
    private Player _player;
    [SerializeField]
    private int _scoreIfkilled = 10;
    private Animator _anim;
    private Collider2D _collider;
    private AudioSource _explosionAudioSource;
    [SerializeField]
    private GameObject _enemylaserPrefab;
    private int testfield;
    
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


        StartCoroutine(ShootingRoutine());

    }
    void Update()
    {
        CalculateMovement();
    }


    
    void CalculateMovement()
    {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);

        if (transform.position.y <= _boundary.GetBottomCorner().y)
        {
            var randomX = UnityEngine.Random.Range(_boundary.GetBottomCorner().x, _boundary.GetTopCorner().x);

            transform.position = new Vector3(randomX, _boundary.GetTopCorner().y, 0);
        }
    }
    
    
    public void ShootatPlayerDirection()
    {
        if (_player != null)
        {
            var direction =  _player.transform.position - transform.position;
            var angle = Vector3.Angle(transform.up, direction);
            var sign = Mathf.Sign(Vector3.Cross(transform.up, direction).z);
            var laser= Instantiate(_enemylaserPrefab, transform.position, Quaternion.Euler(0,0, angle*sign));
        }
    }
    

    IEnumerator ShootingRoutine ()
    {
        while (true) 
        {
            ShootatPlayerDirection();
            var randomSeconds = UnityEngine.Random.Range(3, 8);
            yield return new WaitForSeconds(randomSeconds);
        }

    }

    private void OnTriggerEnter2D (Collider2D other)
    {
        if (other.tag == "Laser")
        {
            Destroy(other.gameObject);

            if (_player != null)
            {
                _player.SetScore(_scoreIfkilled);
            }

            // anim.SetTrigger("OnEnemyDeath");
            _anim.Play("EnemyExplosion", 0, 0.16f);
            _collider.enabled = false;
            _speed = 0;
            _explosionAudioSource.Play();
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
            Destroy(this.gameObject,2.0f);        
        }
    }
}
