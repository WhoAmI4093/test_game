using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class canvasUpdate : MonoBehaviour
{
    TextMeshProUGUI fps;
    Image escapeMenu;

    public bool gamePaused = false;
    int DebugEnabled = 0;

    public float escapeOpenTimeSeconds = 0.25f;
    public float targetAlpha = 100;
    public float targetBlurSize = 1;
    public int steps = 100;
    Color change;
    Material blur;
    bool escapeOpened = false;
    bool isFading = false;

    public static canvasUpdate current;



    void Start()
    {
        current = this;

        fps = transform.GetChild(2).GetComponent<TextMeshProUGUI>();
        escapeMenu = transform.GetChild(1).GetComponent<Image>();
        escapeMenu.enabled = false;

        targetAlpha /= 256;
        change = new Color(0, 0, 0, targetAlpha / steps);

        DebugEnabled = PlayerPrefs.GetInt("f3");
        StartCoroutine("updateFPS");

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
            CameraRotation.current.setCursorLock(1);
        }
        else CameraRotation.current.setCursorLock(0);
        isFading = false;
        escapeOpened = !escapeOpened;
    }
}
