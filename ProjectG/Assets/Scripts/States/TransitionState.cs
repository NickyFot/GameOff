using UnityEngine;
using UnityEngine.SceneManagement;

public class TransitionState : GameState
{
    private float m_TransitionTrigger;
    private float m_TransitionTimer;

    private bool m_InTransition;

    protected override void OnStart()
    {
        m_TransitionTrigger = 1;
        m_TransitionTimer = 0;
        m_InTransition = false;
    }

    protected override void OnEnd()
    {
    }

    protected override void OnUpdate()
    {
        if(m_InTransition || ArenaManager.Instance.ActiveArena == null)
        {
            Debug.LogError("Already in transition or Stage Pool is Empty"); // Handle the null stage on the stage manager, this is just an information error in case everything goes wrong!
            return;
        }
        m_TransitionTimer += Time.deltaTime;
        if(m_TransitionTimer > m_TransitionTrigger)
        {
            m_InTransition = true;
            //Debug.Log("Loading : [" + StageManager.Instance.StageToTransition.StageName + "];");
            SceneManager.LoadScene(ArenaManager.Instance.ActiveArena.Name);
        }
    }

    protected override void OnFixedUpdate()
    {
    }
}
