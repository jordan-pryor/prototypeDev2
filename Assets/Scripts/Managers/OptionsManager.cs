using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections.Generic;

public class OptionsManager : MonoBehaviour
{
    //Tabs
    public GameObject audioPanel;
    public GameObject graphicsPanel;
    public GameObject controlPanel;

    //Audio
    public Slider volumeSlider;

    //Graphcs
    public TMP_Dropdown resolutionDropdown;
    public Toggle fullscreenToggle;
    private int pendingResolutionIndex;
    private bool pendingFullscreen;

    //Controls

    [Serializable] public struct RebindRow
    {
        public string actionName;
        public TextMeshProUGUI keyText;
        public Button rebindButton;
    }

    public RebindRow[] rebindRows;

    Resolution[] resolutions;
    string pendingRebindAction = null;

    private void Awake()
    {
        //Audio
        volumeSlider.value = PlayerPrefs.GetFloat("masterVolume", 1f);
        volumeSlider.onValueChanged.AddListener(SetVolume);

        //Graphics
        resolutions = Screen.resolutions;
        var options = new List<String>();
        int currentIndex = 0;
        for (int i = 0; i < resolutions.Length; i++)
        {
            var r = resolutions[i];
            options.Add($"{r.width}×{r.height}");
            if (r.width == Screen.width && r.height == Screen.height)
                currentIndex = i;
        }
        resolutionDropdown.ClearOptions();
        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = PlayerPrefs.GetInt("resolutionIndex", currentIndex);
        resolutionDropdown.onValueChanged.AddListener(SetResolution);

        fullscreenToggle.isOn = PlayerPrefs.GetInt("fullscreen", Screen.fullScreen ? 1 : 0) == 1;
        fullscreenToggle.onValueChanged.AddListener(SetFullScreen);
        pendingResolutionIndex = resolutionDropdown.value;
        pendingFullscreen = fullscreenToggle.isOn;

        resolutionDropdown.onValueChanged.AddListener((index) =>
        {
            pendingResolutionIndex = index;
        });

        fullscreenToggle.onValueChanged.AddListener((isFull) =>
        {
            pendingFullscreen = isFull;
        });

        //Controls
        foreach (var row in rebindRows)
        {
            var key = (KeyCode)System.Enum.Parse(typeof(KeyCode),
                    PlayerPrefs.GetString(row.actionName, GetDefaultKey(row.actionName).ToString()));
            row.keyText.text = key.ToString();
            row.rebindButton.onClick.AddListener(() => StartRebind(row.actionName));
        }

        ShowTab(audioPanel);
    }

    //Switching Tabs
    public void ShowAudio() => ShowTab(audioPanel);
    public void ShowGraphics() => ShowTab(graphicsPanel);
    public void ShowControls() => ShowTab(controlPanel);

    void ShowTab(GameObject panel)
    {
        audioPanel.SetActive(panel == audioPanel);
        graphicsPanel.SetActive(panel == graphicsPanel);
        controlPanel.SetActive(panel == controlPanel);
    }

    //Audio volume
    public void SetVolume(float vol)
    {
        AudioListener.volume = vol;
        PlayerPrefs.SetFloat("masterVolume", vol);
    }

    //Resolution Set
    public void SetResolution(int index)
    {
        var res = resolutions[index];
        Screen.SetResolution(res.width, res.height, fullscreenToggle.isOn);
        PlayerPrefs.SetInt("resolutionIndex", index);
    }

    public void SetFullScreen(bool full)
    {
        Screen.fullScreen = full;
        PlayerPrefs.SetInt("fullscreen", full ? 1 : 0);
    }
    public void ApplyGraphicsSettings()
    {
        SetResolution(pendingResolutionIndex);
        SetFullScreen(pendingFullscreen);
    }

    //Controls Rebinding
    void StartRebind(string actionName)
    {
        pendingRebindAction = actionName;
        FindRow(actionName).keyText.text = "...";
    }

    private void Update()
    {
        if(pendingRebindAction != null)
        {
            if (Input.anyKeyDown)
            {
                foreach (KeyCode kc in Enum.GetValues(typeof(KeyCode)))
                {
                    if (Input.GetKeyDown(kc))
                    {
                        CompleteRebind(pendingRebindAction, kc);
                        break;
                    }
                }
            }
        }
    }

    void CompleteRebind(string actionName, KeyCode newKey)
    {
        PlayerPrefs.SetString(actionName, newKey.ToString());
        var row = FindRow(actionName);
        row.keyText.text = newKey.ToString();
        pendingRebindAction = null;
    }

    RebindRow FindRow(string actionName)
    {
        foreach (var row in rebindRows)
            if (row.actionName == actionName)
                return row;
        throw new Exception($"No rebind row for {actionName}");
    }

    KeyCode GetDefaultKey(string action)
    {
        switch(action)
        {
            case "MoveForward": return KeyCode.W;
            case "MoveBack": return KeyCode.S;
            case "MoveLeft": return KeyCode.A;
            case "MoveRight": return KeyCode.D;
            case "Jump": return KeyCode.Space;
            case "Sprint": return KeyCode.LeftShift;
            case "Interact": return KeyCode.E;
            case "Inventory": return KeyCode.I;
            case "Reload": return KeyCode.E;
            case "Cancel": return KeyCode.Escape;
            case "Fire1": return KeyCode.Mouse0;
            default: return KeyCode.None;
        }
    }

    public void ResetToDefaultKeys()
    {
        foreach (var row in rebindRows)
        {
            KeyCode defKey = GetDefaultKey(row.actionName);
            PlayerPrefs.SetString(row.actionName, defKey.ToString());
            row.keyText.text = defKey.ToString();
        }
        PlayerPrefs.Save();
    }
}
