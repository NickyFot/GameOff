using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine;

public class ArenaManager : Singleton<ArenaManager>
{
    //-----------------------------------------------------------------

    private List<Arena> m_AllArenas = new List<Arena>();
    private List<Arena> m_PooledArenas = new List<Arena>();

    public Arena ActiveArena;

    //-----------------------------------------------------------------

    public ArenaManager()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.activeSceneChanged += OnSceneChanged;

        //Debug
        Arena newArena = new Arena("/Scenes/Dev/GDsTestBed", "GDsTestBed");
        m_AllArenas.Add(newArena);
    }

    //-----------------------------------------------------------------

    public void TransitionToArena(Arena newArena)
    {
        ActiveArena = newArena;
        GameManager.Instance.TransitionToNewState(GameManager.Instance.State<TransitionState>());
    }

    public void TransitionToArena()
    {
        ActiveArena = GetRandomArena();
        GameManager.Instance.TransitionToNewState(GameManager.Instance.State<TransitionState>());
    }

    //-----------------------------------------------------------------

    public Arena GetRandomArena()
    {
        if(m_PooledArenas.Count <= 0)
        {
            m_PooledArenas = new List<Arena>(m_AllArenas);
        }
        int index = Random.Range(0, m_PooledArenas.Count);
        Arena randomArena = m_PooledArenas[index];
        m_PooledArenas.RemoveAt(index);
        return randomArena;
    }

    //-----------------------------------------------------------------

    private void OnSceneChanged(Scene oldScene, Scene newScene)
    {
        GameManager.Instance.TransitionToNewState(GameManager.Instance.State<InGameState>());
    }

    private void OnSceneLoaded(Scene loadedScene, LoadSceneMode mode)
    {
        SceneManager.SetActiveScene(loadedScene);
    }

    //-----------------------------------------------------------------
}
