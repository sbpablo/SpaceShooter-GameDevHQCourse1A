using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SmallFighterForGroup : Enemy
{
    private RamEnemy _parentRamEnemy;
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

        _parentRamEnemy = GetComponentInParent<RamEnemy>();

        if (_parentRamEnemy == null)
        {
            Debug.LogError("This enemy requires a parentGameObject");
        }

        try
        {
            _player = GameObject.Find("Player").GetComponent<Player>();
        }
        catch (Exception)
        {

            throw new ArgumentNullException("Player", "NULL, cannot find Player");
        }

    }


    // Update is called once per frame
    void Update()
    { 
    
    }

    private protected override void  OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Laser" || collision.tag == "Missile")
        {

            Destroy(collision.gameObject);

            if (_player != null)
            {
                _player.SetScore(_scoreIfkilled);
            }

            // anim.SetTrigger("OnEnemyDeath");
            _anim.Play("EnemyExplosion", 0, 0.16f);
            _collider.enabled = false;
            _speed = 0;
            _explosionAudioSource.Play();
            this.transform.parent = null;
            _parentRamEnemy.OnChildsDamage();
            Destroy(this.gameObject,2.0f);

           
        }


        if (collision.tag == "Player")
        {
            _player.Damage(this.gameObject.tag);
            _anim.Play("EnemyExplosion", 0, 0.16f); // 0.16 starts the animation not at the beginning to avoid the enemy sprite in the animation.
            //_anim.SetTrigger("OnEnemyDeath");
            _collider.enabled = false;
            _speed = 0;
            _explosionAudioSource.Play();
            this.transform.parent = null;
            _parentRamEnemy.OnChildsDamage();
            Destroy(this.gameObject,2.0f);
            
        }

        Debug.Log("On Trigger Enter - SmallFighterforGroups");
        Debug.Log("I was hitted: " + this.gameObject.name + " by: " + collision.gameObject.tag);


    }
}


