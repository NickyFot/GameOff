using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetCinematicShot : MonoBehaviour
{
    public int index;

    private Animator m_Anim;

    private void Start()
    {
        m_Anim = GetComponent<Animator>();
        m_Anim.SetFloat("Pose", index);
    }
}
