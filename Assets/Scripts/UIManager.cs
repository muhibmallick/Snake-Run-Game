using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [SerializeField] private float desiredDuration;
    [SerializeField] private RectTransform fingerTransform;
    [SerializeField] private Vector3 startPosition;
    [SerializeField] private Vector3 endPosition;
    [SerializeField] private SnakeController snakeController;
    [SerializeField] Transform UIPanelToRemoveOnTouch;
    [SerializeField] Text currentLevel;
    private float elapsedTime = 0f;

    private void Start()
    {
        // snakeController.canMove = false;
        string? context = (SceneManager.GetActiveScene().buildIndex).ToString();
        currentLevel.text = $"level {context}";
    }

    public void OnClickPlay()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        // it will open the very next scene in the file>settings>sceneHeirarchy
    }

    public void OnClickQuit()
    {
        Application.Quit();
    }

    public void OnClickRestart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void KeepMovingTheFinger()
    {
        elapsedTime += Time.deltaTime;
        float percentage = elapsedTime / desiredDuration;

        var rt = fingerTransform;

        rt.localPosition = Vector3.Lerp(startPosition, endPosition, percentage);

        if (rt.localPosition == endPosition)
        {
            Vector3 tempPosition = startPosition;
            startPosition = endPosition;
            endPosition = tempPosition;
            elapsedTime = 0f;
        }


    }

    private void Update()
    {
        if (IfPanelTouched())
        {
            UIPanelToRemoveOnTouch.gameObject.SetActive(false);
            snakeController.canMove = true;
        }

        KeepMovingTheFinger();
    }

    private bool IfPanelTouched()
    {
        // if the user touches the screen at start, then the game should start
        return Input.touchCount == 1;
    }
}
