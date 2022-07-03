using UnityEngine;
using UnityEngine.Pool;

public class Bullet : MonoBehaviour
{
    public ObjectPool<Bullet> MotherPool;
    private static float _maxDistance = -1;
    public float Distance;
    [SerializeField]
    private float _speed = 30f;
    private Vector3 direction;

    public Vector3 Direction
    {
        get => direction; set
        {
            direction = value.normalized;
        }
    }

    public enum Origins{ Player, UFO }
    public Origins Origin; 

    // Start is called before the first frame update
    void Start()
    {
        if (_maxDistance < 0)
        {
            var cameraOrigin = Camera.main.ViewportToWorldPoint(Vector3.zero);
            var cameraSize = Camera.main.ViewportToWorldPoint(Vector3.one);
            _maxDistance = cameraSize.x - cameraOrigin.x;
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += Direction * _speed * Time.deltaTime;
        Distance += _speed * Time.deltaTime;
        if (Distance > _maxDistance)
        {
            MotherPool.Release(this);
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (gameObject.activeSelf)
            MotherPool.Release(this);
    }
}
