using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplashFade : MonoBehaviour
{
    public GameObject UI;
    public float FadeSpeed;

    public bool FadeIn;

    private void Start()
    {
        if(FadeIn)
        {
            UIManager.Instance.FadeInUI(UI, FadeSpeed);
        }
        else
        {
            UIManager.Instance.FadeOutUI(UI, FadeSpeed);
        }
    }
}
