using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EnemyLaser : MonoBehaviour
{
    [SerializeField]
    private float _speed=10;
    private Boundary _boundary;
    private Player _player;
    void Start()
    {

        _boundary = GameObject.Find("BoundaryManager").GetComponent<Boundary>();

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
    }
    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);

        if (transform.position.x < _boundary.GetBottomCorner().x || transform.position.x > _boundary.GetTopCorner().x ||
            transform.position.y < _boundary.GetBottomCorner().y || transform.position.y > _boundary.GetTopCorner().y )
        {
            Destroy(this.gameObject);
        }   
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            _player.Damage(this.gameObject.tag);
            Destroy(this.gameObject);
        }
    }
}
