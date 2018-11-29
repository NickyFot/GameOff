using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamSelector : MonoBehaviour
{
    public GameObject[] Cameras;

    public void SelectCamera(int index)
    {
        for(int i = 0; i < Cameras.Length; i++)
        {
            Cameras[i].SetActive(index == i);
        }
    }
}
