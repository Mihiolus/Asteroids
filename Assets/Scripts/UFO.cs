using System.Collections;
using UnityEngine;

public class UFO : MonoBehaviour
{
    public Vector3 Velocity;
    public bool FlyingRight;
    [SerializeField]
    private float _minFireInterval = 2f, _maxFireInterval = 5f;
    [SerializeField]
    private Color _bulletColor = Color.red;
    [SerializeField]
    private Transform _player;
    [SerializeField]
    private float _bulletSpawnRadius = 0.6f;
    [SerializeField]
    private int _score = 200;

    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(false);
        if (!_player)
        {
            _player = GameObject.FindGameObjectWithTag("Player").transform;
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += Velocity * Time.deltaTime;
        if (FlyingRight)
        {
            if (transform.position.x > Wraparound.WorldBounds.xMax)
            {
                gameObject.SetActive(false);
                GameManager.Instance.OnUFODestroyed();
            }
        }
        else
        {
            if (transform.position.x < Wraparound.WorldBounds.xMin)
            {
                gameObject.SetActive(false);
                GameManager.Instance.OnUFODestroyed();
            }
        }
    }

    public void StartFiring()
    {
        StartCoroutine(Fire());
    }

    private IEnumerator Fire()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(_minFireInterval, _maxFireInterval));
            if (!_player.gameObject.activeSelf)
            {
                yield return null;
            }
            var bullet = BulletManager.Instance.BulletPool.Get();
            bullet.Direction = GetNearestPlayerPosition() - transform.position;
            bullet.transform.position = transform.position + bullet.Direction * _bulletSpawnRadius;
            bullet.GetComponent<SpriteRenderer>().color = _bulletColor;
            bullet.Origin = Bullet.Origins.UFO;
        }
    }

    private Vector3 GetNearestPlayerPosition()
    {
        var positions = new Vector3[4];
        positions[0] = _player.position;
        positions[0].y += Wraparound.WorldBounds.height;
        positions[1] = _player.position;
        positions[1].y -= Wraparound.WorldBounds.height;
        positions[2] = _player.position;
        positions[2].x -= Wraparound.WorldBounds.width;
        positions[3] = _player.position;
        positions[3].x += Wraparound.WorldBounds.width;
        Vector3 nearest = _player.position;
        float nearestDistance = (nearest - transform.position).sqrMagnitude;
        foreach (var pos in positions)
        {
            var distSq = (pos - transform.position).sqrMagnitude;
            if (distSq < nearestDistance)
            {
                nearest = pos;
                nearestDistance = distSq;
            }
        }
        return nearest;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        ExplosionManager.Instance.PlaceExplosion(transform.position);
        gameObject.SetActive(false);
        GameManager.Instance.OnUFODestroyed();
        if (other.gameObject.CompareTag("Bullet"))
        {
            var bullet = other.gameObject.GetComponent<Bullet>();
            if (bullet.Origin == Bullet.Origins.Player)
            {
                GameManager.Instance.Score += _score;
            }
        }
    }
}
