using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;
using System;
using static ParkourUtils;
using UnityEditor;

public class UIManager : MonoBehaviour
{
    public GameObject PauseUI;
    public GameObject IngameUI;
    public bool JetpackActive;
    
    private TMP_Text infoText;
    private TMP_Text jetpackFuel;
    
    private Button easyMode;
    private Button quitGame;
    private Button resetGame;

    private Coroutine displayingText;
    private ParkourGameManager parkourGameManager;
    private CameraController cameraController;
    private Jetpack jetpack;

    void Start()
    {
        infoText = IngameUI.transform.Find("InfoText").GetComponent<TMP_Text>();
        jetpackFuel = IngameUI.transform.Find("Jetpack").GetComponent<TMP_Text>();
        
        easyMode = PauseUI.transform.Find("EasyMode").GetComponent<Button>();
        quitGame = PauseUI.transform.Find("QuitGame").GetComponent<Button>();
        resetGame = PauseUI.transform.Find("ResetGame").GetComponent<Button>();

        parkourGameManager = GetComponent<ParkourGameManager>();
        cameraController = transform.GetChild(0).GetComponent<CameraController>();

        infoText.text = "";
        jetpackFuel.text = "";
        easyMode.onClick.AddListener(() => SetEasyMode());
        quitGame.onClick.AddListener(() => QuitGame());
        resetGame.onClick.AddListener(() => ResetGame());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (PauseUI.activeSelf)
            {
                PauseUI.SetActive(false);
                Cursor.lockState = CursorLockMode.Locked;
                cameraController.FreezeCamera = false;
            }
            else
            {
                PauseUI.SetActive(true);
                Cursor.lockState = CursorLockMode.Confined;
                cameraController.FreezeCamera = true;
            }
        }
        
        if(JetpackActive)
        {
            DisplayJetpackFuel();
        }
    }

    public void DisplayInfoText(string text, float displayTime)
    {
        if (text == "" || infoText.text == text || displayTime == 0)
        {
            return;
        }

        if (displayingText != null)
        {
            StopCoroutine(displayingText);
        }

        displayingText = StartCoroutine(DisplayInfoTextForSeconds(text, displayTime));
    }

    IEnumerator DisplayInfoTextForSeconds(string text, float displayTime)
    {
        infoText.text = text;
        yield return new WaitForSeconds(displayTime);
        infoText.text = "";
        
    }

    public void DisplayJetpackFuel()
    {
        if (!jetpack)
            jetpack = GetComponent<Jetpack>();
        
        jetpackFuel.text = String.Concat(Enumerable.Repeat("|", (int)(jetpack.JetpackFuel * 20)));
    }

    public void SetEasyMode()
    {
        parkourGameManager.EnableAllFeatures();
        parkourGameManager.EasyModeEnabled = true;
        DisplayInfoText(EasyModeMessage, DisplayTime);
    }

    public void QuitGame()
    {
        #if UNITY_STANDALONE
                Application.Quit();
        #endif
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }

    public void ResetGame()
    {
        parkourGameManager.ResetGame();
        parkourGameManager.resetGame.Invoke();
    }
}
