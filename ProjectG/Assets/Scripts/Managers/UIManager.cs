using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;

public class UIManager : Singleton<UIManager>
{
    public static GameObject MainCanvas;

    public MainMenuUI MainMenu;
    public PlaySettingsUI PlaySettings;
    public CreditsUI Credits;
    public InGameUI GameUI;

    private bool m_WaitingToEnd = false;
    private Action OnTransitionsEnd;

    #region Function Vars

    // Fade 
    private const float FADE_SPEED = 0.6f;

    private float m_FadeTimer;

    private class FadeDetails
    {
        public GameObject go;
        public bool IsFadeIn;
        public float FadeTimer;
        public float FadeSpeed;
        public bool ToDisable;
        public float InitAlpha;
    }

    private List<FadeDetails> m_FadeDetailsList = new List<FadeDetails>();

    // Animation
    private enum AnimationType
    {
        FlyAway,
        AnimateTo,
        FlyInAndAway
    }

    private class RectToAnimate
    {
        public RectTransform Rect;
        public AnimationType Type;
        public float Speed;
        public float LerpTimer;
        public float Time;
        public float Distance;
        public Vector3 Direction;

        public Vector2 OriginalPos;
        public Vector2 TargetPos;

        public float AnimTimer;
        public Vector2 SmoothVel;

        public int AnimStateCounter;
    }

    private List<RectToAnimate> m_UiAnimationList = new List<RectToAnimate>();

    #endregion

    //-----------------------------------------------------------------

    public void SetupUI()
    {
        MainCanvas = GameObject.Find("Canvas");

        MainMenu = new MainMenuUI();
        PlaySettings = new PlaySettingsUI();
        Credits = new CreditsUI();
        GameUI = new InGameUI();

        ////debug
        //CreatePlayerUIPanel("");
        //CreatePlayerUIPanel("");
        //CreatePlayerUIPanel("");
        //CreatePlayerUIPanel("");

    }

    //-----------------------------------------------------------------
    
    public void CreatePlayerUIPanel(string playerName)
    {
        GameUI.CreatePanel(playerName);
    }

    public void StartCountDown()
    {
        GameUI.StartCountDown();
    }
    //-----------------------------------------------------------------

    public void WaitForTransitionToEnd(Action callback)
    {
        m_WaitingToEnd = true;
        OnTransitionsEnd = callback;
    }

    //-----------------------------------------------------------------

    public void Update()
    {
        if( m_FadeDetailsList.Count <= 0 && m_UiAnimationList.Count <= 0 && m_WaitingToEnd)
        {
            m_WaitingToEnd = false;
            ClearFadeLists();
            OnTransitionsEnd();
        }
    }

    //-----------------------------------------------------------------

    #region Fade Functions

    public void FadeInUI(GameObject go, float fadeSpeed = 0.6f, bool toEnable = false)
    {
        if (go.GetComponent<CanvasGroup>() == null) return;

        for(int i = 0; i < m_FadeDetailsList.Count; i++)
        {
            if(m_FadeDetailsList[i].go.Equals(go))
            {
                m_FadeDetailsList[i] = null;
                m_FadeDetailsList.RemoveAt(i);
                break;
            }
        }

        FadeDetails details = new FadeDetails();
        details.go = go;
        details.IsFadeIn = true;
        details.FadeTimer = 0;
        details.FadeSpeed = fadeSpeed;
        details.ToDisable = false;
        details.InitAlpha = go.GetComponent<CanvasGroup>().alpha;

        if(toEnable)
        {
           go.SetActive(true);
        }

        m_FadeDetailsList.Add(details);
    }

    public void FadeOutUI(GameObject go, float fadeSpeed = 0.6f, bool toDisable = false)
    {
        if(go.GetComponent<CanvasGroup>() == null) return;

        for(int i = 0; i < m_FadeDetailsList.Count; i++)
        {
            if(m_FadeDetailsList[i].go.Equals(go))
            {
                m_FadeDetailsList[i] = null;
                m_FadeDetailsList.RemoveAt(i);
                break;
            }
        }

        FadeDetails details = new FadeDetails();
        details.go = go;
        details.IsFadeIn = false;
        details.FadeTimer = 0;
        details.FadeSpeed = fadeSpeed;
        details.ToDisable = toDisable;
        details.InitAlpha = go.GetComponent<CanvasGroup>().alpha;

        m_FadeDetailsList.Add(details);
    }

    public void ClearFadeLists()
    {
        m_FadeDetailsList.Clear();
    }

    public void UpdateFade()
    {
        if(m_FadeDetailsList.Count > 0)
        {
            for(int i = 0; i < m_FadeDetailsList.Count; i++)
            {
                float alpha = m_FadeDetailsList[i].go.GetComponent<CanvasGroup>() != null ? m_FadeDetailsList[i].go.GetComponent<CanvasGroup>().alpha : m_FadeDetailsList[i].go.GetComponent<SpriteRenderer>().color.a;
                m_FadeDetailsList[i].FadeTimer = Mathf.Min(1.0f, m_FadeDetailsList[i].FadeTimer + Time.deltaTime * FADE_SPEED);

                if(m_FadeDetailsList[i].IsFadeIn)
                {
                    if(alpha >= 1)
                    {
                        m_FadeDetailsList[i] = null;
                        m_FadeDetailsList.RemoveAt(i);
                    }
                    else
                    {
                        if(m_FadeDetailsList[i].go.GetComponent<CanvasGroup>() != null)
                        {
                            m_FadeDetailsList[i].go.GetComponent<CanvasGroup>().alpha = Mathf.SmoothStep(m_FadeDetailsList[i].InitAlpha, 1, m_FadeDetailsList[i].FadeTimer);
                        }
                        else
                        {
                            Color col = m_FadeDetailsList[i].go.GetComponent<SpriteRenderer>().color;
                            m_FadeDetailsList[i].go.GetComponent<SpriteRenderer>().color = new Color(col.r, col.g, col.b, Mathf.SmoothStep(m_FadeDetailsList[i].InitAlpha, 1, m_FadeDetailsList[i].FadeTimer));
                        }
                    }                   
                }
                else
                {
                    if(alpha <= 0)
                    {
                        if(m_FadeDetailsList[i].ToDisable)
                        {
                            m_FadeDetailsList[i].go.SetActive(false);
                        }
                        m_FadeDetailsList[i] = null;
                        m_FadeDetailsList.RemoveAt(i);
                    }
                    else
                    {
                        if(( m_FadeDetailsList[i].go.GetComponent<CanvasGroup>() != null))
                        {
                            m_FadeDetailsList[i].go.GetComponent<CanvasGroup>().alpha = Mathf.SmoothStep(m_FadeDetailsList[i].InitAlpha, 0, m_FadeDetailsList[i].FadeTimer);
                        }
                        else
                        {
                            Color col = m_FadeDetailsList[i].go.GetComponent<SpriteRenderer>().color;
                            m_FadeDetailsList[i].go.GetComponent<SpriteRenderer>().color = new Color(col.r, col.g, col.b, Mathf.SmoothStep(m_FadeDetailsList[i].InitAlpha, 0, m_FadeDetailsList[i].FadeTimer));
                        }
                    }
                }
            }
        }
    }

    #endregion

    //-----------------------------------------------------------------

    #region Animation Functions

    public void UpdateUIAnimations()
    {
        if (m_UiAnimationList.Count <= 0) return;
        for (int i = 0; i < m_UiAnimationList.Count; i++)
        {
            RectToAnimate r = m_UiAnimationList[i];
            switch (r.Type)
            {
                case AnimationType.FlyAway:
                    {
                        r.AnimTimer += Time.deltaTime;
                        if (r.AnimTimer < r.Time)
                        {
                            r.Rect.offsetMax = Vector2.SmoothDamp(r.Rect.offsetMax, new Vector2(r.Direction.x * r.Distance, r.Direction.y * r.Distance), ref r.SmoothVel, r.Time, 100, Time.deltaTime);
                            r.Rect.offsetMin = Vector2.SmoothDamp(r.Rect.offsetMin, new Vector2(r.Direction.x * r.Distance, r.Direction.y * r.Distance), ref r.SmoothVel, r.Time, 100, Time.deltaTime);
                        }
                        else
                        {
                            m_UiAnimationList.RemoveAt(i);
                        }
                        break;
                    }
                case AnimationType.AnimateTo:
                    {
                        r.AnimTimer += Time.deltaTime;
                        if (r.AnimTimer <= r.Time * 2)
                        {
                            r.Rect.offsetMax = Vector2.SmoothDamp(r.Rect.offsetMax, r.TargetPos, ref r.SmoothVel, r.Time * 0.5f, 100, Time.deltaTime);
                            r.Rect.offsetMin = Vector2.SmoothDamp(r.Rect.offsetMax, r.TargetPos, ref r.SmoothVel, r.Time * 0.5f, 100, Time.deltaTime);
                        }
                        else
                        {
                            m_UiAnimationList.RemoveAt(i);
                        }
                        break;
                    }
                case AnimationType.FlyInAndAway:
                    {
                        switch (r.AnimStateCounter)
                        {
                            case 0: // Fly in
                                r.AnimTimer += Time.deltaTime;
                                if (r.AnimTimer <= r.Time)
                                {
                                    r.Rect.offsetMax = Vector2.SmoothDamp(r.Rect.offsetMax, r.TargetPos, ref r.SmoothVel, r.Time * 0.25f, 100, Time.deltaTime);
                                    r.Rect.offsetMin = Vector2.SmoothDamp(r.Rect.offsetMax, r.TargetPos, ref r.SmoothVel, r.Time * 0.25f, 100, Time.deltaTime);
                                }
                                else
                                {
                                    r.AnimStateCounter++;
                                    r.AnimTimer = 0;
                                }
                                break;
                            case 1: // Small Sway
                                r.AnimTimer += Time.deltaTime;
                                if (r.AnimTimer < r.Time * 0.2f)
                                {
                                    r.Rect.offsetMax = Vector2.SmoothDamp(r.Rect.offsetMax, new Vector2(r.Direction.x * r.Distance, r.Direction.y * r.Distance), ref r.SmoothVel, r.Time, 100, Time.deltaTime);
                                    r.Rect.offsetMin = Vector2.SmoothDamp(r.Rect.offsetMin, new Vector2(r.Direction.x * r.Distance, r.Direction.y * r.Distance), ref r.SmoothVel, r.Time, 100, Time.deltaTime);
                                }
                                else
                                {
                                    r.AnimStateCounter++;
                                    r.AnimTimer = 0;
                                }
                                break;
                            case 2: // Fly Away
                                r.AnimTimer += Time.deltaTime;
                                if (r.AnimTimer < r.Time)
                                {
                                    r.Rect.offsetMax = Vector2.SmoothDamp(r.Rect.offsetMax, new Vector2(r.Direction.x * r.Distance, r.Direction.y * r.Distance), ref r.SmoothVel, r.Time * 0.5f, 100, Time.deltaTime);
                                    r.Rect.offsetMin = Vector2.SmoothDamp(r.Rect.offsetMin, new Vector2(r.Direction.x * r.Distance, r.Direction.y * r.Distance), ref r.SmoothVel, r.Time * 0.5f, 100, Time.deltaTime);
                                }
                                else
                                {
                                    m_UiAnimationList.RemoveAt(i);
                                }
                                break;
                        }
                        break;
                    }

            }
        }
    }

    public void AnimateFlyAway(GameObject uiObj, Vector3 direction, float speed, float time)
    {
        RectToAnimate newRect = new RectToAnimate();
        newRect.Rect = uiObj.GetComponent<RectTransform>();
        newRect.Direction = direction;
        newRect.Speed = speed * 100;
        newRect.Time = time;
        newRect.Type = AnimationType.FlyAway;
        newRect.AnimTimer = 0;
        newRect.Distance = time * newRect.Speed;
        m_UiAnimationList.Add(newRect);
    }

    public void AnimateToPosition(GameObject uiObj, Vector2 targetPos, float time)
    {
        RectToAnimate newRect = new RectToAnimate();
        newRect.Rect = uiObj.GetComponent<RectTransform>();
        newRect.Time = time;
        newRect.Type = AnimationType.AnimateTo;
        newRect.TargetPos = targetPos;
        newRect.OriginalPos = newRect.Rect.offsetMax;
        newRect.AnimTimer = 0;
        m_UiAnimationList.Add(newRect);
    }

    public void AnimateFlyInAndAway(GameObject uiObj, Vector2 targetPos, Vector3 direction, float speed, float time)
    {
        RectToAnimate newRect = new RectToAnimate();
        newRect.Rect = uiObj.GetComponent<RectTransform>();
        newRect.Direction = direction;
        newRect.Speed = speed * 100;
        newRect.TargetPos = targetPos;
        newRect.Time = time;
        newRect.Type = AnimationType.FlyInAndAway;
        newRect.AnimTimer = 0;
        newRect.Distance = time * newRect.Speed;
        newRect.AnimStateCounter = 0;
        m_UiAnimationList.Add(newRect);
    }

    #endregion

    //-----------------------------------------------------------------
}
