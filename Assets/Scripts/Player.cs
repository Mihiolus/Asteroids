using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private Vector3 _velocity = new Vector3();
    [SerializeField]
    private float _maxSpeed = 10f, _acceleration = 1f, _angularSpeed = 60f;
    [SerializeField]
    private float _cooldown = 1 / 3f;
    private float _cooldownTimer = 0;
    [SerializeField]
    private Color _bulletColor = Color.green;
    [SerializeField]
    private Transform _turret;
    [SerializeField]
    private float _invincibilityDuration = 3f;
    private bool _isInvincible = false;
    [SerializeField]
    private Animator _animator;

    // Start is called before the first frame update
    void Start()
    {
        Wraparound.Register(transform);
        if (!_animator)
        {
            _animator = GetComponentInChildren<Animator>();
        }
        StartCoroutine(AnimateInvincibility());
    }

    public void Respawn()
    {
        transform.position = new Vector3();
        _velocity.Set(0, 0, 0);
        StartCoroutine(AnimateInvincibility());
    }

    // Update is called once per frame
    void Update()
    {
        //Turning
        switch (PauseMenu.Instance.Scheme)
        {
            case PauseMenu.ControlSchemes.Keyboard:
                if (Input.GetKey(KeyCode.A))
                {
                    transform.Rotate(Vector3.forward * _angularSpeed * Time.deltaTime);
                }
                if (Input.GetKey(KeyCode.D))
                {
                    transform.Rotate(Vector3.back * _angularSpeed * Time.deltaTime);
                }
                break;
            case PauseMenu.ControlSchemes.KeyboardMouse:
                var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                var mouseDir = mousePos - transform.position;
                var angle = Mathf.Atan2(mouseDir.y, mouseDir.x) * Mathf.Rad2Deg;
                Quaternion targetRot = Quaternion.AngleAxis(angle, Vector3.forward);
                transform.rotation =
                    Quaternion.RotateTowards(transform.rotation, targetRot, _angularSpeed * Time.deltaTime);
                break;
        }
        //Thrust control
        if (Input.GetKey(KeyCode.W)
            || Input.GetKey(KeyCode.UpArrow)
            || (PauseMenu.Instance.Scheme == PauseMenu.ControlSchemes.KeyboardMouse
            && Input.GetMouseButton(1)))
        {
            _velocity = Vector2.ClampMagnitude(_velocity + transform.right * _acceleration * Time.deltaTime, _maxSpeed);
        }
        //Inertial movement
        transform.position += _velocity * Time.deltaTime;
        //Shooting
        if (Input.GetKeyDown(KeyCode.Space)
            || (PauseMenu.Instance.Scheme == PauseMenu.ControlSchemes.KeyboardMouse
            && Input.GetMouseButtonDown(0))
            && _cooldownTimer <= 0)
        {
            var bullet = BulletManager.Instance.BulletPool.Get();
            if (bullet != null)
            {
                bullet.transform.position = _turret.position;
                bullet.Direction = transform.right;
                _cooldownTimer = _cooldown;
                bullet.GetComponent<SpriteRenderer>().color = _bulletColor;
                bullet.Origin = Bullet.Origins.Player;
            }
        }
        if (_cooldownTimer > 0)
        {
            _cooldownTimer -= Time.deltaTime;
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (_isInvincible)
        {
            return;
        }
        GameManager.Instance.Lives--;
        GameManager.Instance.QueueRespawn(this);
        ExplosionManager.Instance.PlaceExplosion(transform.position);
    }

    private IEnumerator AnimateInvincibility()
    {
        _isInvincible = true;
        _animator.SetBool("Blink", true);
        yield return new WaitForSeconds(_invincibilityDuration);
        _animator.SetBool("Blink", false);
        _isInvincible = false;
    }
}
