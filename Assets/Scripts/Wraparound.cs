using System.Collections.Generic;
using UnityEngine;

public class Wraparound : MonoBehaviour
{
    public static Rect WorldBounds
    {
        get
        {
            if (!_boundsInitialized)
            {
                GetWorldBounds();
            }
            return _worldBounds;
        }
    }
    private static Rect _worldBounds;
    private static bool _boundsInitialized = false;

    private void Awake()
    {
        wrappables = new List<Transform>();
    }

    private static List<Transform> wrappables;

    public static void Register(Transform wrappable)
    {
        wrappables.Add(wrappable);
    }

    public static void Unregister(Transform wrappable)
    {
        wrappables.Remove(wrappable);
    }

    private static void GetWorldBounds()
    {
        _worldBounds = new Rect();
        var cameraOrigin = Camera.main.ViewportToWorldPoint(Vector3.zero);
        _worldBounds.xMin = cameraOrigin.x;
        _worldBounds.yMin = cameraOrigin.y;
        var cameraSize = Camera.main.ViewportToWorldPoint(Vector3.one);
        _worldBounds.xMax = cameraSize.x;
        _worldBounds.yMax = cameraSize.y;
        _boundsInitialized = true;
    }

    // Update is called once per frame
    void Update()
    {
        foreach (var w in wrappables)
        {
            if(!w.gameObject.activeSelf){
                continue;
            }
            if (w.position.x > _worldBounds.xMax)
            {
                var newPos = w.position;
                newPos.x = _worldBounds.xMin;
                w.position = newPos;
            }
            if (w.position.x < _worldBounds.xMin)
            {
                var newPos = w.position;
                newPos.x = _worldBounds.xMax;
                w.position = newPos;
            }
            if (w.position.y > _worldBounds.yMax)
            {
                var newPos = w.position;
                newPos.y = _worldBounds.yMin;
                w.position = newPos;
            }
            if (w.position.y < _worldBounds.yMin)
            {
                var newPos = w.position;
                newPos.y = _worldBounds.yMax;
                w.position = newPos;
            }
        }
    }
}
