using UnityEngine;
using UnityEngine.Pool;

public class Asteroid : MonoBehaviour
{
    public AsteroidManager.AsteroidSize Size;
    public ObjectPool<Asteroid> MotherPool;
    public AsteroidManager Manager;
    public Vector3 Velocity;
    public bool IsCompeletelyDestroyed = false;
    [SerializeField]
    private int _score;

    // Update is called once per frame
    void Update()
    {
        transform.position += Velocity * Time.deltaTime;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Bullet"))
        {
            IsCompeletelyDestroyed = false;
            var bullet = other.gameObject.GetComponent<Bullet>();
            if (bullet.Origin == Bullet.Origins.Player)
            {
                GameManager.Instance.Score += _score;
            }
        }
        else if (other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("UFO"))
        {
            IsCompeletelyDestroyed = true;
        }
        switch(Size){
            case AsteroidManager.AsteroidSize.Big:
            SFXPlayer.Instance.PlaySound(SFXPlayer.SoundTypes.ExplosionBig);
            break;
            case AsteroidManager.AsteroidSize.Mid:
            SFXPlayer.Instance.PlaySound(SFXPlayer.SoundTypes.ExplosionMid);
            break;
            case AsteroidManager.AsteroidSize.Small:
            SFXPlayer.Instance.PlaySound(SFXPlayer.SoundTypes.ExplosionSmall);
            break;
        }
        MotherPool.Release(this);
        ExplosionManager.Instance.PlaceExplosion(transform.position);
    }
}
