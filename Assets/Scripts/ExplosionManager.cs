using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ExplosionManager : MonoBehaviour
{
    public static ExplosionManager Instance;

    private ObjectPool<Explosion> _explosionPool;
    [SerializeField]
    private Explosion _prefab;

    private void Awake() {
        Instance = this;
        _explosionPool = new ObjectPool<Explosion>(CreateExplosion,GetExplosion,ReleaseExplosion,DestroyExplosion);
    }

    private Explosion CreateExplosion()
    {
        var exp =  Instantiate(_prefab);
        exp.MotherPool = _explosionPool;
        return exp;
    }

    private void GetExplosion(Explosion obj)
    {
        obj.gameObject.SetActive(true);
    }

    private void ReleaseExplosion(Explosion obj)
    {
        obj.gameObject.SetActive(false);
    }

    private void DestroyExplosion(Explosion obj)
    {
        Destroy(obj.gameObject);
    }

    public void PlaceExplosion(Vector3 location){
        var exp = _explosionPool.Get();
        exp.transform.position = location;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
