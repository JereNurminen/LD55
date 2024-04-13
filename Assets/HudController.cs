using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HudController : MonoBehaviour
{
    public Sprite crowIcon;
    public Sprite ratIcon;
    public float deathScreenPanelTargetAlpha = 0.25f;
    public float deathScreenFadeDuration = 2.0f;
    private float timeSinceFadeStart = 0.0f;

    private Image summoningIcon;
    private Image skullImage;
    private GameObject deathScreenPanel;
    private Animator frameAnimator;
    private Animator deathScreenAnimator;

    // Start is called before the first frame update
    void Start()
    {
        summoningIcon = GameObject.Find("SummoningIcon").GetComponent<Image>();
        frameAnimator = GameObject.Find("Frame").GetComponent<Animator>();
        deathScreenPanel = GameObject.Find("Death Screen").gameObject;
        deathScreenAnimator = deathScreenPanel.GetComponentInChildren<Animator>();
        skullImage = GameObject.Find("Death Message").GetComponent<Image>();

        deathScreenPanel.SetActive(false);
    }

    public void ChangeIcon(SelectedSummon selectedSummon)
    {
        switch (selectedSummon)
        {
            case SelectedSummon.Crow:
                summoningIcon.sprite = crowIcon;
                break;
            default:
                summoningIcon.sprite = ratIcon;
                break;
        }
    }

    public IEnumerator Die()
    {
        frameAnimator.SetTrigger("die");
        deathScreenPanel.SetActive(true);
        yield return StartCoroutine(FadeDeathScreen());
        deathScreenAnimator.SetTrigger("start");
    }

    public void SetFilling(bool isFilling)
    {
        frameAnimator.SetBool("isFilling", isFilling);
    }

    IEnumerator FadeDeathScreen()
    {
        Image panelImage = deathScreenPanel.GetComponent<Image>();
        Color panelColor = panelImage.color;
        Color skullColor = skullImage.color;
        while (timeSinceFadeStart < deathScreenFadeDuration)
        {
            timeSinceFadeStart += Time.deltaTime;
            panelColor.a = Mathf.Lerp(
                0.0f,
                deathScreenPanelTargetAlpha,
                timeSinceFadeStart / deathScreenFadeDuration
            );
            skullColor.a = Mathf.Lerp(0.0f, 1.0f, timeSinceFadeStart / deathScreenFadeDuration);
            panelImage.color = panelColor;
            skullImage.color = skullColor;
            yield return null;
        }
    }

    // Update is called once per frame
    void Update() { }
}
