using UnityEngine;
using UnityEngine.Pool;

public class BulletManager : MonoBehaviour
{
    public static BulletManager Instance;
    [SerializeField]
    private Bullet _bulletPrefab;
    public ObjectPool<Bullet> BulletPool;

    private void Awake()
    {
        Instance = this;
        BulletPool = new ObjectPool<Bullet>(CreateBullet, GetBullet, ReleaseBullet, DestroyBullet);
    }

    private Bullet CreateBullet()
    {
        var newBullet = Instantiate(_bulletPrefab);
        newBullet.MotherPool = BulletPool;
        Wraparound.Register(newBullet.transform);
        return newBullet;
    }

    private void GetBullet(Bullet bullet)
    {
        bullet.gameObject.SetActive(true);
        bullet.Distance = 0;
    }

    private void ReleaseBullet(Bullet bullet)
    {
        bullet.gameObject.SetActive(false);
    }

    private void DestroyBullet(Bullet bullet)
    {
        Wraparound.Unregister(bullet.transform);
        Destroy(bullet.gameObject);
    }
}
