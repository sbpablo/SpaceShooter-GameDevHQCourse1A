﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Missile : MonoBehaviour
{
    private Transform[] _targets;
    private Transform _enemyContainer;
    private Transform _target;
    private bool _isTargetAquired;
    private Rigidbody2D _rb;
    [SerializeField]
    private float _angleChangingSpeed =15f;
    [SerializeField]
    private float _speed=5f;
    
    
    void Start()
    {
        _enemyContainer = GameObject.Find("SpawnManager").transform.Find("EnemyContainer");

        if (_enemyContainer == null)
        {
            Debug.LogError("EnemyContainer could not be found");
        }

        _rb = GetComponent<Rigidbody2D>();

        if (_rb == null)
        {
            Debug.LogError("Rigidbody Component could not be found");
        }

        

        Boundary.Instance.Offset = 5.0f;
    }

     void Update()
    {
        _targets = _enemyContainer.GetComponentsInChildren<Transform>();
        FindAndFollowTarget(); //Finds and follow the closest enemy.
        OutOfBoundsActions();
    }
    void FixedUpdate()
    {
        _rb.velocity = transform.up * _speed;
      
    }


    public void OutOfBoundsActions()
    {
         if (transform.position.y > Boundary.Instance.GetTopCorner().y + Boundary.Instance.Offset || transform.position.y<Boundary.Instance.GetBottomCorner().y - Boundary.Instance.Offset ||
             transform.position.x > Boundary.Instance.GetTopCorner().x + Boundary.Instance.Offset || transform.position.x<Boundary.Instance.GetBottomCorner().x - Boundary.Instance.Offset)
         {
           
            if (_target != null)
            {
                _target.GetComponent<Enemy>().IsbeingTargeted = false;
            }

            Destroy(this.gameObject);

         }
    }
    public void FindAndFollowTarget()
    {
        if (_isTargetAquired == false)
        {
            _target = GetClosestEnemy(_targets);

            if (_target != null)
            {
                _isTargetAquired = true;
            }
        }
        else
        {
            if (_target != null)
            {
                var direction = _target.position - transform.position;
                direction.Normalize();
                float angle = Vector3.Angle(transform.up, direction);
                float sign = Vector3.Cross(transform.up, direction).z;
                _rb.angularVelocity = angle * sign * _angleChangingSpeed;
            }
            else
            {
                _isTargetAquired = false;
            }
        }
    }

    public Transform GetClosestEnemy (Transform[] enemies)
    {
        float minDist = Mathf.Infinity;
        Transform closestEnemy = null;
        var currentPos = transform.position;

        foreach( var enemy in enemies)
        {
            var enemyComponent = enemy.gameObject.GetComponent<Enemy>();

            if (enemyComponent != null)
            {
              
                if ( enemyComponent.IsbeingTargeted == false)
                {
                    var distance = Vector3.Distance(enemy.transform.position, currentPos);
                    if (distance < minDist)
                    {
                        closestEnemy = enemy.transform;
                        minDist = distance;
                    }
                }
            }

        }
        
        if (closestEnemy != null && closestEnemy.gameObject.tag!="Boss")
        {
            
            closestEnemy.transform.GetComponent<Enemy>().IsbeingTargeted = true;
        }
       
        return closestEnemy;
    }

  
}
