using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class Player : MonoBehaviour
{
    [SerializeField]
    private float _speed = 10;
    private float _minimumSpeed;
    [SerializeField]
    private float _speedMultiplier = 2;
    [SerializeField]
    private float _speedIncreasedRate = 2f;
    [SerializeField]
    private GameObject _laserPrefab;
    [SerializeField]
    private GameObject _tripleShotPrefab;
    private bool _isTripleShotEnabled;
    private bool _isShieldEnabled;
    private bool _isSpeedEnabled;
    [SerializeField]
    private float _fireRate = 0.5f;
    private float _nextFire = 0.0f;
    [SerializeField]
    private int _lives = 3;
    private Boundary _boundary;
    private SpawnManager _spawnManager;
    [SerializeField]
    private GameObject _shieldPrefab;
    [SerializeField]
    private int _shieldLaserHitEndurance = 3;
    [SerializeField]
    private int _ammoCount = 15;
    private int _score;
    private int _killCount = 0;
    private UIManager _ui;
    private GameObject _leftEngine;
    private GameObject _rightEngine;
    [SerializeField]
    private GameObject _explosionPrefab;
    private AudioSource _laserAudioSource;
    private AudioSource _explosionAudioSource;
    
    void Awake()
    {
        transform.position = new Vector3(0, 0, 0);

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
            _spawnManager = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();
        }
        catch (Exception)
        {
            throw new ArgumentNullException("SpawnManager", "NULL, cannot find SpawnManager");
        }

        try
        {
            _ui = GameObject.Find("UIManager").GetComponent<UIManager>();
        }
        catch (Exception)
        {
            throw new ArgumentNullException("UIManager", "NULL, cannot find UIManager");
        }

        
        _leftEngine = transform.Find("LeftEngine").gameObject;
        _rightEngine = transform.Find("RigthEngine").gameObject;

        try
        {
            _laserAudioSource = GameObject.Find("AudioManager").transform.Find("LaserSound").gameObject.GetComponent<AudioSource>();
        }
        catch (Exception)
        {
            throw new ArgumentNullException("AudioManager or_laserSound", "NULL, cannot find Audio Manager/ Clip");
        }

        try
        {
            _explosionAudioSource = GameObject.Find("AudioManager").transform.Find("ExplosionSound").gameObject.GetComponent<AudioSource>();
        }
        catch (Exception)
        {
            throw new ArgumentNullException("AudioManager or Explosion Sound", "NULL, cannot find Audio Manager/ Clip");
        }

        _minimumSpeed = _speed;

        _ui.ShowAmmoCount(_ammoCount);

    }

    void Update()
    {
        CalculateMovement();

        if (Input.GetKeyDown(KeyCode.Space) && Time.time > _nextFire && _ammoCount>0)
        {
            Fire();
            _nextFire = Time.time + _fireRate;
            AmmoManagement();
           
        }

        if (Input.GetKey(KeyCode.LeftShift))
        {
            _speed = _minimumSpeed * _speedIncreasedRate;
        }

        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
           
            if (_isSpeedEnabled == false)
            {
                _speed = _minimumSpeed;
            }
        }
    }

    private void CalculateMovement()
    {

        var horizontalImput = Input.GetAxis("Horizontal");
        var verticalImput = Input.GetAxis("Vertical");

        var movement = new Vector3(horizontalImput, verticalImput, 0);

        transform.Translate(movement * _speed * Time.deltaTime);

        // El objeto puede moverse en el eje X Libremente. Si pasa del límite de la pantalla, debe salir por el otro lado.

        float restrictedX = transform.position.x;


        if (transform.position.x <= _boundary.GetBottomCorner().x)
        {
            restrictedX = _boundary.GetTopCorner().x;

        }
        else if (transform.position.x >= _boundary.GetTopCorner().x)
        {
            restrictedX = _boundary.GetBottomCorner().x;
        }

        // El objeto se puede mover en el eje y hasta el limite inferior de la pantalla, sumando su volumen.  
        // Hacia arriba, solo llega hacia la mitad de la pantalla.

        var restrictedY = Mathf.Clamp(transform.position.y,
                                     _boundary.GetBottomCorner().y + this.GetComponent<SpriteRenderer>().bounds.extents.y,
                                      (_boundary.GetTopCorner().y + _boundary.GetBottomCorner().y) / 2);


        // Posicion restringida considerando tambien el tamaño del objeto
        /* var restrectedPos = new Vector3 (Mathf.Clamp(transform.position.x, _bottomCorner.x+ this.GetComponent<MeshFilter>().mesh.bounds.extents.x, _topCorner.x- this.GetComponent<MeshFilter>().mesh.bounds.extents.x), 
                                         Mathf.Clamp (transform.position.y, _bottomCorner.y + this.GetComponent<MeshFilter>().mesh.bounds.extents.y, (_topCorner.y+_bottomCorner.y)/2));
        */

        var restrectedPos = new Vector3(restrictedX, restrictedY);

        transform.position = restrectedPos;
    }

    
    private void Fire()
    {
        
        if (_isTripleShotEnabled == false)
        {
            FireLaser();
        }
        else
        {
            FireTripleShot();
        }

        _laserAudioSource.Play();
    }

    private void FireLaser()
    {
        var offset = new Vector3(0, 1, 0);
        Instantiate(_laserPrefab, transform.position + offset, Quaternion.identity);
    }

    private void FireTripleShot()
    {
        var offset = new Vector3(0, 1, 0);
        Instantiate(_tripleShotPrefab, transform.position, Quaternion.identity);
    }


    public void AmmoManagement()
    {
        _ammoCount--;
        _ui.ShowAmmoCount(_ammoCount);

        if (_ammoCount <= 5 && _ui.IsAmmoCoroutineActive == false)
        {
            _ui.StartCoroutine("AmmoCountFlickering");
        }
        else if (_ammoCount > 5 && _ui.IsAmmoCoroutineActive == true)
        {
            
            _ui.StopCoroutine("AmmoCountFlickering");
            _ui.IsAmmoCoroutineActive = false;
            _ui.GetComponent<UIManager>().EnableAmmoText();  //In case Coroutine stops while Text is disabled
        }
    }

    public void Damage(String sourceOfDamage)
    {
        if (_isShieldEnabled == false)
        {
            _lives -= 1;
            _ui.SetLivesImage(_lives);

            switch (_lives)
            {
                case 2:
                    var randomEngine = UnityEngine.Random.Range(0, 2);   //Left is 0, Right is 1.
                    if (randomEngine == 0)
                    {
                        _leftEngine.SetActive(true);
                    }
                    else
                    {
                        _rightEngine.SetActive(true);
                    }
                   
                     break;
                case 1:
                    if (_leftEngine.activeSelf == true)
                    {
                        _rightEngine.SetActive(true);
                    }
                    else
                    {
                        _leftEngine.SetActive(true);
                    }
                    break;

                case 0:
                    _spawnManager.OnPlayerDeath();
                    _explosionPrefab = Instantiate(_explosionPrefab, transform.position, Quaternion.identity);

                    if (_explosionPrefab == null)
                    {
                        Debug.LogError("Cannot instantiate explosion Prefab");
                    }
                    Destroy(_explosionPrefab,2.5f);
                    _explosionAudioSource.Play();
                    Destroy(this.gameObject);
                    break; 
            }

        }
        else
        {
            if (sourceOfDamage == "Enemy")   //The enemy ship destroys the shield completely.
            {
                StopCoroutine("ShieldCoolDownRoutine");
                _isShieldEnabled = false;
                _shieldPrefab.SetActive(false);
            }
            else // its the Enemy's Laser
            {
                _shieldLaserHitEndurance--;
                
                switch (_shieldLaserHitEndurance)  
                {
                    case 2:
                        _shieldPrefab.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.66f);
                        break;
                    case 1:
                        _shieldPrefab.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.33f);
                        break;
                    case 0:
                        StopCoroutine("ShieldCoolDownRoutine");
                        _isShieldEnabled = false;
                        _shieldPrefab.SetActive(false);
                        break;

                }

            }
        }
    }

    public void OnTripleShotPowerUpCollection(float duration)
    {
        _isTripleShotEnabled = true;
        StartCoroutine(TripleShotCoolDownRoutine(duration));

    }

    IEnumerator TripleShotCoolDownRoutine(float duration)
    {
        yield return new WaitForSeconds(duration);
        _isTripleShotEnabled = false;
    }

    public void OnSpeedPowerUpCollection(float duration)
    {
        _isSpeedEnabled = true;
        _speed *= _speedMultiplier;
        StartCoroutine(SpeedCoolDownRoutine(duration));

    }
    IEnumerator SpeedCoolDownRoutine(float duration)
    {
        yield return new WaitForSeconds(duration);
        _speed /= _speedMultiplier;
        _isSpeedEnabled = false;
    }
    public void OnShieldPowerUpCollection (float duration)
    {
        _isShieldEnabled = true;
        _shieldPrefab.SetActive(true);

        StartCoroutine(ShieldCoolDownRoutine(duration));
    }

    IEnumerator ShieldCoolDownRoutine(float duration)
    {
        yield return new WaitForSeconds(duration);
        _isShieldEnabled = false;
        _shieldPrefab.SetActive(false);
    }

    public void SetScore(int score)
    {
        _killCount += 1;
        _score += score;
        _ui.ShowScore(_score);
    }
    public int GetScore()
    {
        return _score;
    }

}