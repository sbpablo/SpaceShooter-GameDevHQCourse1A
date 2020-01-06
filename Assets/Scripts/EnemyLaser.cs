using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EnemyLaser : MonoBehaviour
{
    [SerializeField]
    private float _speed=10;
    private Player _player;


    
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
    }
    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);

        if (transform.position.x < Boundary.Instance.GetBottomCorner().x || transform.position.x > Boundary.Instance.GetTopCorner().x ||
            transform.position.y < Boundary.Instance.GetBottomCorner().y || transform.position.y > Boundary.Instance.GetTopCorner().y )
        {
            // Destroy(this.gameObject);
            this.gameObject.SetActive(false);
        }   
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            _player.Damage(this.gameObject.tag);
            //Destroy(this.gameObject);
            this.gameObject.SetActive(false);
        }
    }
}
