using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class PauseMenu : MonoBehaviour
{
    public static PauseMenu Instance;

    [SerializeField]
    private Button _continueButton, _newGameButton, _controlsButton, _exitButton;

    private bool _isGameStarted;
    public bool IsGameStarted
    {
        get => _isGameStarted; set
        {
            _isGameStarted = value;
            _continueButton.interactable = _isGameStarted;
        }
    }

    [SerializeField]
    [TextArea]
    private string _keyboardLabel, _keyboardMouseLabel;

    public enum ControlSchemes { Keyboard, KeyboardMouse }
    private ControlSchemes _scheme;
    public ControlSchemes Scheme
    {
        get => _scheme; set
        {
            _scheme = value;
            var label = _controlsButton.GetComponent<TMP_Text>();
            switch (_scheme)
            {
                case ControlSchemes.Keyboard:
                    label.text = _keyboardLabel; break;
                case ControlSchemes.KeyboardMouse:
                    label.text = _keyboardMouseLabel; break;
            }
        }
    }
    [SerializeField]
    private ControlSchemes _defaultScheme;

    private void Awake()
    {
        Instance = this;
        IsGameStarted = false;
        Scheme = _defaultScheme;
    }

    public void OnContinue()
    {
        GameManager.Instance.Mode = GameManager.GameModes.Play;
    }

    public void OnNewGame()
    {
        StartCoroutine(ReloadScene());
    }

    private IEnumerator ReloadScene()
    {
        _continueButton.interactable = false;
        _newGameButton.interactable = false;
        _controlsButton.interactable = false;
        _exitButton.interactable = false;
        if (SceneManager.GetSceneByBuildIndex(1).isLoaded)
        {
            var unloading = SceneManager.UnloadSceneAsync(SceneManager.GetSceneByBuildIndex(1));
            while (!unloading.isDone)
            {
                yield return null;
            }
        }
        var loading = SceneManager.LoadSceneAsync(1, LoadSceneMode.Additive);
        while (!loading.isDone)
        {
            yield return null;
        }
        SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(1));
        IsGameStarted = true;
        GameManager.Instance.PauseMenu = gameObject;
        GameManager.Instance.Init();
        GameManager.Instance.Mode = GameManager.GameModes.Play;
        _continueButton.interactable = true;
        _newGameButton.interactable = true;
        _controlsButton.interactable = true;
        _exitButton.interactable = true;
    }

    public void OnControls()
    {
        Scheme = Scheme == ControlSchemes.Keyboard ? ControlSchemes.KeyboardMouse : ControlSchemes.Keyboard;
    }

    public void OnExit()
    {
        Application.Quit();
    }
}
