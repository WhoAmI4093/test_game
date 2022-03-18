using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CanvasUpdate : MonoBehaviour
{
    TextMeshProUGUI fps;
    PlayerMovement playerMovement;
    CameraRotation cameraRotation;
    int DebugEnabled = 0;

    #region bars on screen
    public float barSpeed = 0.1f;
    TextMeshProUGUI hpText;
    Scrollbar hpBar;
    TextMeshProUGUI staminaText;
    Scrollbar staminaBar;

    public IEnumerator hpCoroutine;
    public IEnumerator staminaCoroutine;
    #endregion


    Image escapeMenu;
    #region escape fading
    public bool gamePaused = false;
    public float escapeOpenTimeSeconds = 0.25f;
    public float targetAlpha = 100;
    public float targetBlurSize = 1;
    public int steps = 100;
    Color change;
    bool escapeOpened = false;
    bool isFading = false;
    #endregion

    public static CanvasUpdate current;
    void Start()
    {
        

        escapeMenu = transform.GetChild(1).GetComponent<Image>();
        escapeMenu.enabled = false;
        
        DebugEnabled = PlayerPrefs.GetInt("f3");
        fps = transform.GetChild(2).GetComponent<TextMeshProUGUI>();
        StartCoroutine("updateFPS");

        playerMovement = FindObjectOfType<PlayerMovement>();
        cameraRotation = FindObjectOfType<CameraRotation>();

        hpText = transform.GetChild(0).GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>();
        hpBar = hpText.transform.parent.GetComponent<Scrollbar>();
        staminaText = transform.GetChild(0).GetChild(1).GetChild(1).GetComponent<TextMeshProUGUI>();
        staminaBar = staminaText.transform.parent.GetComponent<Scrollbar>();

        StartCoroutine("moveBarsToValues");

        targetAlpha /= 256;
        change = new Color(0, 0, 0, targetAlpha / steps);


    }
    void Update()
    {
        #region f3
        if (Input.GetKeyDown(KeyCode.F3))
        {
            int val = (DebugEnabled + 1) % 2;
            DebugEnabled = val;
            PlayerPrefs.SetInt("f3", val);
            if (val == 1) fps.enabled = true;
            else fps.enabled = false;
        }
        #endregion
        #region escape
        if (Input.GetKeyDown(KeyCode.Escape) && !isFading)
        {
            StartCoroutine("fadeEscape");
        }
        #endregion

    }

    IEnumerator updateFPS()
    {
        while (true)
        {
            if (DebugEnabled == 0) 
            {
                yield return new WaitForSeconds(0.5f);
            };
            fps.text = $"FPS: {Mathf.Round(1 / Time.deltaTime)}";
            yield return new WaitForSeconds(0.5f);
        }
    }
    IEnumerator fadeEscape()
    {
        isFading = true;
        if (!escapeOpened) escapeMenu.enabled = true;
        for (int i = 0; i < steps; i++)
        {
            if (!escapeOpened)
            {
                escapeMenu.color += change;
                escapeMenu.material.SetFloat("_Size", escapeMenu.material.GetFloat("_Size") + (targetBlurSize / steps));
            }
            else
            {
                escapeMenu.color -= change;
                escapeMenu.material.SetFloat("_Size", escapeMenu.material.GetFloat("_Size") - (targetBlurSize / steps));
            }
            yield return new WaitForSeconds(escapeOpenTimeSeconds / steps);
        }
        if (escapeOpened)
        {
            escapeMenu.enabled = false;
            cameraRotation.setCursorLock(1);
        }
        else cameraRotation.setCursorLock(0);
        isFading = false;
        escapeOpened = !escapeOpened;
    }
    IEnumerator moveBarsToValues()
    {
        while (true)
        {
            float staminaDivision = playerMovement.stamina / playerMovement.maxStamina;
            float hpDivision = playerMovement.hp / playerMovement.maxHp;
            string hpTextValue = $"{Mathf.Round(playerMovement.hp)} / {playerMovement.maxHp}";
            string staminaTextValue = $"{Mathf.Round(playerMovement.stamina)} / {playerMovement.maxStamina}";
            if (hpText.text != hpTextValue) hpText.text = hpTextValue;
            if (staminaText.text != staminaTextValue) staminaText.text = staminaTextValue;

            if (hpBar.size != hpDivision)
                hpBar.size = Mathf.Lerp(hpBar.size, hpDivision, barSpeed);
            if (staminaBar.size != staminaDivision)
                staminaBar.size = Mathf.Lerp(staminaBar.size, staminaDivision, barSpeed);
            yield return new WaitForEndOfFrame();
        }
    }
}
