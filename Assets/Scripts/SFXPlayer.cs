using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXPlayer : MonoBehaviour
{
    public static SFXPlayer Instance;
    [SerializeField]
    private AudioClip _thrust, _explosionSmall, _explosionMid, _explosionBig, _fire;
    private AudioSource _source;
    [SerializeField]
    private AudioSource _thrustSource;

    public enum SoundTypes { ExplosionSmall, ExplosionMid, ExplosionBig, Fire }

    private void Awake()
    {
        Instance = this;
        _source = GetComponent<AudioSource>();
        PlayingThrust = _playingThrust;
    }

    private bool _playingThrust = true;

    public bool PlayingThrust
    {
        get => _playingThrust; set
        {
            if (_playingThrust != value)
            {
                _playingThrust = value;
                if(_playingThrust){
                    _thrustSource.UnPause();
                }else{
                    _thrustSource.Pause();
                }
            }
        }
    }

    public void PlaySound(SoundTypes soundType)
    {
        switch (soundType)
        {
            case SoundTypes.ExplosionBig:
                _source.PlayOneShot(_explosionBig);
                break;
            case SoundTypes.ExplosionMid:
                _source.PlayOneShot(_explosionMid);
                break;
            case SoundTypes.ExplosionSmall:
                _source.PlayOneShot(_explosionSmall);
                break;
            case SoundTypes.Fire:
                _source.PlayOneShot(_fire);
                break;
        }
    }
}
