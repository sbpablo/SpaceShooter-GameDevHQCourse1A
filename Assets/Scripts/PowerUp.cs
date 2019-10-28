using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum PowerUpType
{
    TripleShot,
    Speed,
    Shield,
    Ammo,
    Life,
    Missile
}

public class PowerUp : MonoBehaviour
{

    [SerializeField]
    private float _duration = 5.0f;
    private Boundary _boundary;
    [SerializeField]
    private float _speed = 3;
    [SerializeField]
    private PowerUpType _type;
    private AudioSource _powerUpAudioSource;
  
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
            _powerUpAudioSource = GameObject.Find("AudioManager").transform.Find("PowerUpSound").gameObject.GetComponent<AudioSource>();
        }
        catch (Exception)
        {
            throw new ArgumentNullException("AudioManager or PowerUp Sound", "NULL, cannot find Audio Manager/ Clip");
        }

    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);

        if (transform.position.y<= _boundary.GetBottomCorner().y)
        {
            Destroy(this.gameObject);
        }  
    }

    public float PowerUpDuration()
    {
        return _duration;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == ("Player"))
        {
            var player = collision.GetComponent<Player>();
            

            if (player != null)
            {

                _powerUpAudioSource.Play();

                switch ((int) this._type)
                {
                    case 0: 
                        player.OnTripleShotPowerUpCollection(_duration);
                        break;

                    case 1:
                        player.OnSpeedPowerUpCollection(_duration);
                        break;

                    case 2:
                        player.OnShieldPowerUpCollection(_duration);
                        break;
                    case 3:
                        player.OnAmmoPowerUpCollection();
                        break;
                    case 4:
                        player.OnLifePowerUpCollection();
                        break;
                    case 5:
                        player.OnMissilePowerUpCollection(_duration);
                        break;


                }

               
               
            }
            Destroy(this.gameObject);

        }
    }


}
