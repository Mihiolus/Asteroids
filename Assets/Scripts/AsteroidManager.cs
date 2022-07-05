using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

public class AsteroidManager : MonoBehaviour
{
    public static AsteroidManager Instance;
    private ObjectPool<Asteroid> _asteroidBigPool, _asteroidMidPool, _asteroidSmallPool;
    [SerializeField]
    private Asteroid _asteroidBig, _asteroidMid, _asteroidSmall;
    public enum AsteroidSize { Big, Mid, Small }
    [SerializeField]
    private int _startNumber = 2;
    [SerializeField]
    private float _minSpeed = 1f, _maxSpeed = 5f,
    _repopulateCooldown = 2f, _fragmentVelocityAngle = 45;

    private void Awake()
    {
        Instance = this;
        _asteroidBigPool = new ObjectPool<Asteroid>(CreateBig, GetAsteroid, ReleaseAsteroid, DestroyAsteroid, false);
        _asteroidMidPool = new ObjectPool<Asteroid>(CreateMid, GetAsteroid, ReleaseAsteroid, DestroyAsteroid, false);
        _asteroidSmallPool = new ObjectPool<Asteroid>(CreateSmall, GetAsteroid, ReleaseAsteroid, DestroyAsteroid, false);
    }

    public void Populate()
    {
        for (int i = 0; i < _startNumber + GameManager.Instance.Difficulty; i++)
        {
            Asteroid asteroid = _asteroidBigPool.Get();
            Vector3 randomPos = new Vector3();
            randomPos.x = Random.Range(Wraparound.WorldBounds.xMin, Wraparound.WorldBounds.xMax);
            randomPos.x = Random.Range(Wraparound.WorldBounds.yMin, Wraparound.WorldBounds.yMax);
            asteroid.transform.position = randomPos;
            asteroid.Velocity = Random.insideUnitCircle.normalized * Random.Range(_minSpeed, _maxSpeed);
        }
    }

    private Asteroid CreateBig()
    {
        return CreateAsteroid(AsteroidSize.Big);
    }

    private Asteroid CreateMid()
    {
        return CreateAsteroid(AsteroidSize.Mid);
    }

    private Asteroid CreateSmall()
    {
        return CreateAsteroid(AsteroidSize.Small);
    }

    private Asteroid CreateAsteroid(AsteroidSize size)
    {
        Asteroid result = null;
        switch (size)
        {
            case AsteroidSize.Big:
                result = Instantiate(_asteroidBig);
                result.MotherPool = _asteroidBigPool;
                result.Size = size; break;
            case AsteroidSize.Mid:
                result = Instantiate(_asteroidMid);
                result.MotherPool = _asteroidMidPool;
                result.Size = size; break;
            case AsteroidSize.Small:
                result = Instantiate(_asteroidSmall);
                result.MotherPool = _asteroidSmallPool;
                result.Size = size; break;
        }
        Wraparound.Register(result.transform);
        result.Manager = this;
        return result;
    }

    private void GetAsteroid(Asteroid a)
    {
        a.gameObject.SetActive(true);
        a.transform.rotation = Quaternion.AngleAxis(Random.Range(0, 360), Vector3.forward);
    }

    private void ReleaseAsteroid(Asteroid a)
    {
        a.gameObject.SetActive(false);
        if (a.Size != AsteroidSize.Small && !a.IsCompeletelyDestroyed)
        {
            var speed = Random.Range(_minSpeed, _maxSpeed);
            a.Velocity = a.Velocity.normalized;
            for (int i = -1; i < 2; i += 2)
            {
                Asteroid newAsteroid = a.Size == AsteroidSize.Big ? _asteroidMidPool.Get() : _asteroidSmallPool.Get();
                newAsteroid.transform.position = a.transform.position;
                newAsteroid.Velocity = Quaternion.AngleAxis(i * _fragmentVelocityAngle, Vector3.forward) * a.Velocity * speed;
            }
        }
        var remaining = _asteroidBigPool.CountActive + _asteroidMidPool.CountActive + _asteroidSmallPool.CountActive;

        if (remaining == 1)
        {
            StartCoroutine(Repopulate());
        }
    }

    private IEnumerator Repopulate()
    {
        yield return new WaitForSeconds(_repopulateCooldown);
        GameManager.Instance.Difficulty++;
        Populate();
    }

    private void DestroyAsteroid(Asteroid a)
    {
        Wraparound.Unregister(a.transform);
        Destroy(a.gameObject);
    }
}
