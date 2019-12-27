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
    Missile,
    NegativeMovement,
}

public class PowerUp : MonoBehaviour
{

    [SerializeField]
    private float _duration = 5.0f;
    [SerializeField]
    private float _speed = 3;
    [SerializeField]
    private PowerUpType _type;
    private AudioSource _powerUpAudioSource;
    private AudioSource _powerUpDestroyedAudioSource;
    [SerializeField]
    private float _spawnWeight;
    //public static event Action<Transform> OnPowerUpOnScreen;
    public static event Action <Transform> OnPowerUpOnScreen;
   
  
    void Start()
    {
       

        try
        {
            _powerUpAudioSource = GameObject.Find("AudioManager").transform.Find("PowerUpSound").gameObject.GetComponent<AudioSource>();
        }
        catch (Exception)
        {
            throw new ArgumentNullException("AudioManager or PowerUp Sound", "NULL, cannot find Audio Manager/ Clip");
        }

        try
        {
            _powerUpDestroyedAudioSource = GameObject.Find("AudioManager").transform.Find("PowerUpDestroyed").gameObject.GetComponent<AudioSource>();
        }
        catch (Exception)
        {
            throw new ArgumentNullException("AudioManager or PowerUpDestroyed Sound", "NULL, cannot find Audio Manager/ Clip");
        }

    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);
        
        if (this.gameObject.tag!="NegativeMovement")
        {
            OnPowerUp();
        }

        if (transform.position.y<= Boundary.Instance.GetBottomCorner().y)
        {
            Destroy(this.gameObject);
        }  
    }

    public float GetSpawnWeight()
    {
        return _spawnWeight;
    }
    
    public float PowerUpDuration()
    {
        return _duration;
    }

    private  void OnPowerUp()
    {
        if (OnPowerUpOnScreen != null)

        {
            OnPowerUpOnScreen(this.gameObject.transform);
        }
    }

    private IEnumerator ScaleCoroutine()
    {

        _powerUpDestroyedAudioSource.Play();
        
        for (float i= transform.localScale.x; i>=0.0f; i-= 0.1f)
        {
            Debug.Log(transform.localScale);
            transform.localScale -= new Vector3(0.1f,0.1f ,0.1f);
            yield return new WaitForSeconds(0.1f);
        }

        
        Destroy(this.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == ("Player"))
        {
            var player = collision.GetComponent<Player>();
            

            if (player != null)
            {

                if (gameObject.tag != "NegativeMovement")
                {
                    _powerUpAudioSource.Play();
                }
                
                

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
                    case 6:
                        player.OnNegativeMovementPowerUpCollection(_duration);
                        break;
                }
            }
            Destroy(this.gameObject);
        }

        if (collision.tag == "EnemyLaser")
        {
            this.GetComponent<Collider2D>().enabled = false;
            StartCoroutine(ScaleCoroutine());
            collision.gameObject.SetActive(false);

        }
    }
}
