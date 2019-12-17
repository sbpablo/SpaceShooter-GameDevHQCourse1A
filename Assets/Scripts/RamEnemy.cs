using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class RamEnemy : Enemy
{
    // Start is called before the first frame update

    private Rigidbody2D _rb;
    private Transform _target;
    [SerializeField]
    private float _angleChangingSpeed =1f;
    
    void Start()
    {

      

        _rb = GetComponent<Rigidbody2D>();

        if (_rb == null)
        {
            Debug.LogError("Rigidbody Component could not be found");
        }

        _target = GameObject.FindWithTag("Player").transform;

        if (_target == null)
        {
            Debug.Log("Could not find the enemy Target, aka Player Object");
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

 
    private void FixedUpdate()
    {
       _rb.velocity = transform.up * _speed;

        CalculateMovement();


        if (_player.GetLives()!=0)
        {
            var direction = _target.position - transform.position;
            direction.Normalize();
            float angle = Vector3.Angle(transform.up, direction);
            float sign = Vector3.Cross(transform.up, direction).z;
        
            _rb.MoveRotation(_rb.rotation + angle*sign*_angleChangingSpeed* Time.fixedDeltaTime);
        }
    }

    private protected override void CalculateMovement()
    {
        {

            if (_rb.position.y <= Boundary.Instance.GetBottomCorner().y)
            {
                var randomX = UnityEngine.Random.Range(Boundary.Instance.GetBottomCorner().x, Boundary.Instance.GetTopCorner().x);

                _rb.position = new Vector3(randomX, Boundary.Instance.GetTopCorner().y, 0);
            }

           
        }
    }

    public void OnChildsDamage()
    {
      
        if (transform.childCount == 0)
        {
            SpawnManager.Instance.TotalEnemiesInCurrentWave--;
            Destroy(this.gameObject);
        }
    }

    private protected override void OnTriggerEnter2D(Collider2D other)
    {
        return;
    }
   

}
