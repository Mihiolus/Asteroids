using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [SerializeField]
    private float _respawnCooldown = 5f, _gameoverDelay = 2f;
    [SerializeField]
    private UFO _ufo;
    [SerializeField]
    private float _minUFOSpawnInterval = 20f, _maxUFOSpawnInterval = 40f,
     _UFOPassageDuration = 10f;

    private int _score, _lives;
    [SerializeField]
    private TMPro.TMP_Text _scoreLabel, _livesLabel;
    [SerializeField]
    private char _livesSymbol = '|';
    public enum GameModes { Play, Pause }
    private GameModes _mode;
    public GameModes Mode
    {
        get => _mode; set
        {
            _mode = value;
            if (_mode == GameModes.Pause)
            {
                Time.timeScale = 0;
                PauseMenu.SetActive(true);
            }
            else
            {
                Time.timeScale = 1;
                PauseMenu.SetActive(false);
            }
        }
    }
    public GameObject PauseMenu;

    public int Score
    {
        get => _score; set
        {
            _score = value;
            _scoreLabel.text = _score.ToString();
        }
    }

    private StringBuilder _livesText = new StringBuilder();

    public int Lives
    {
        get => _lives; set
        {
            _lives = value;
            _livesText.Clear();
            _livesText.Append(_livesSymbol, _lives);
            _livesLabel.text = _livesText.ToString();
        }
    }

    private int _difficulty = 0;
    public int Difficulty { get => _difficulty; set => _difficulty = value; }

    [SerializeField]
    private int _startingLives = 3;

    [SerializeField]
    private GameObject _HUD;

    private void Awake()
    {
        Instance = this;
        if (!_HUD)
        {
            _HUD = GameObject.Find("HUD");
        }
    }

    public void Init()
    {
        Score = 0;
        Lives = _startingLives;
        Difficulty = 0;
        StartCoroutine(LaunchUFO());
        AsteroidManager.Instance.Populate();
    }

    public void QueueRespawn(Player player)
    {
        StartCoroutine(Respawn(player));
    }

    public void OnUFODestroyed()
    {
        StartCoroutine(LaunchUFO());
    }

    private IEnumerator LaunchUFO()
    {
        yield return new WaitForSeconds(Random.Range(_minUFOSpawnInterval, _maxUFOSpawnInterval));
        _ufo.gameObject.SetActive(true);
        float minY = Wraparound.WorldBounds.yMin + Wraparound.WorldBounds.height * 0.2f;
        float maxY = minY + Wraparound.WorldBounds.height * 0.6f;
        float y = Random.Range(minY, maxY);
        _ufo.FlyingRight = Random.value < 0.5f;
        float x = _ufo.FlyingRight ? Wraparound.WorldBounds.xMin : Wraparound.WorldBounds.xMax;
        _ufo.transform.position = new Vector3(x, y, 0);
        _ufo.Velocity = (_ufo.FlyingRight ? 1 : -1) * Vector3.right * Wraparound.WorldBounds.width / _UFOPassageDuration;
        _ufo.StartFiring();
    }

    private IEnumerator Respawn(Player player)
    {
        player.gameObject.SetActive(false);
        yield return new WaitForSeconds(_respawnCooldown);
        player.gameObject.SetActive(true);
        player.Respawn();
    }

    public void Gameover(Player player)
    {
        StartCoroutine(QueueGameover(player));
    }

    private IEnumerator QueueGameover(Player player)
    {
        player.gameObject.SetActive(false);
        yield return new WaitForSeconds(_gameoverDelay);
        Mode = GameModes.Pause;
        PauseMenu.GetComponent<PauseMenu>().IsGameStarted = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (global::PauseMenu.Instance.IsGameStarted)
            {
                Mode = Mode == GameModes.Play ? GameModes.Pause : GameModes.Play;
            }
        }
    }
}
