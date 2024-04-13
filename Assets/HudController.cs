using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HudController : MonoBehaviour
{
    public Sprite crowIcon;
    public Sprite ratIcon;

    private Image image;

    // Start is called before the first frame update
    void Start()
    {
        image = GetComponent<Image>();
    }

    public void ChangeIcon(SelectedSummon selectedSummon)
    {
        switch (selectedSummon)
        {
            case SelectedSummon.Crow:
                image.sprite = crowIcon;
                break;
            default:
                image.sprite = ratIcon;
                break;
        }
    }

    // Update is called once per frame
    void Update() { }
}
