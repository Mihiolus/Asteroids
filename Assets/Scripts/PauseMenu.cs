using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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

    private void Awake()
    {
        Instance = this;
        IsGameStarted = false;
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

    }

    public void OnExit()
    {
        Application.Quit();
    }
}
