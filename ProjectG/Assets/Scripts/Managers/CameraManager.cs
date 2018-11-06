using UnityEngine;
using System.Collections.Generic;
using Direction = Utility.Direction;

public class CameraManager : Singleton<CameraManager>
{
    //-- Properties ---------------------------------------------------

    private GameObject m_Camera;
    public GameObject MainCamera
    {
        get
        {
            if (m_Camera == null)
            {
                if (Camera.main != null)
                {
                    m_Camera = Camera.main.gameObject;
                }
            }
            return m_Camera;
        }
        private set
        {
            m_Camera = value;
        }
    }

    //-- Vars ----------------------------------------------------------

    private List<GameObject> m_CameraTargets = new List<GameObject>();

    private Vector3 m_CameraZoom;

    private Vector3 m_PosVel;
     
    private float m_MinCameraZoom;
    private float m_MaxCameraZoom;

    private float m_EastMostFrustrum;
    private float m_WestMostFrustrum;
    private float m_NortMostFrustrum;
    private float m_SouthMostFrustrum;

    public bool dzEnabled;

    //-- Init -----------------------------------------------------------

    public void InitCamera(Vector3 initPosition, float minZoom = 4, float maxZoom = 15 )
    {
        MainCamera.transform.position = initPosition;
        SetCameraZoomBoundaries(minZoom, maxZoom);
        SetCameraZoom(maxZoom * 0.5f);
        SetCameraPositionBoundaries(6f, -6f, 10f, -10f);
    }

    public void InitCamera(Vector3 initPosition, float minZoom, float maxZoom, params GameObject[] cameraTargets)
    {
        InitCamera(initPosition, minZoom, maxZoom);
        AddTarget(cameraTargets);
    }

    //-- Update ----------------------------------------------------------

    public void UpdateCamera()
    {
        if (MainCamera == null) return;
        if (m_CameraTargets.Count <= 0) return;

        // Dynamically fit targets into view
        DynamicCameraZoom();

        Vector3 FocalPoint = GetFocalPoint(m_CameraTargets);
        //Debug.Log(FocalPoint.x);

        MainCamera.transform.LookAt(FocalPoint);

        // Smoothly move camera and zoom
        MainCamera.transform.position = Vector3.SmoothDamp(MainCamera.transform.position, FocalPoint + m_CameraZoom, ref m_PosVel, 0.1f);
        //if(dzEnabled)
        //{
        //    CameraSeMena(m_CameraTargets[1]);
        //}
    }

    //-- API Functions -----------------------------------------------------

    public void AddTarget(params GameObject[] target)
    {
        for(int i = 0; i < target.Length; i++)
        {
            if (m_CameraTargets.Contains(target[i])) continue;
            m_CameraTargets.Add(target[i]);
        }
    }

    public void RemoveTarget(params GameObject[] target)
    {
        for (int i = 0; i < target.Length; i++)
        {
            m_CameraTargets.Remove(target[i]);
        }
    }

    public void ClearAllTargets()
    {
        m_CameraTargets.Clear();
    }

    public void SetCameraZoom(float zoom)
    {
        if (zoom < m_MinCameraZoom || zoom > m_MaxCameraZoom) return;
        m_CameraZoom = new Vector3(0, zoom, -zoom);
    }

    public void SetCameraZoomBoundaries(float min, float max)
    {
        m_MinCameraZoom = min;
        m_MaxCameraZoom = max;
    }

    public void SetCameraPositionBoundaries(float east, float west, float north, float south)
    {
        m_EastMostFrustrum = east;
        m_WestMostFrustrum = west;
        m_NortMostFrustrum = north;
        m_SouthMostFrustrum = south;
    }

    public void CameraSeMena(GameObject target)
    {
        float currDistance = Vector3.Distance(MainCamera.transform.position, target.transform.position);
        float heightAtDistance = 2.0f * currDistance * Mathf.Tan(MainCamera.GetComponent<Camera>().fieldOfView * 0.5f * Mathf.Deg2Rad);
        MainCamera.GetComponent<Camera>().fieldOfView = 2.0f * Mathf.Atan(heightAtDistance * 0.5f / currDistance) * Mathf.Rad2Deg;
    }

    //-- Private / Utility Functions --------------------------------------

    private Vector3 GetFocalPoint(List<GameObject> target)
    { 
        if (target.Count <= 0) return Vector3.zero;
        //if (target.Count == 1) return target[0].transform.position;

        Vector3 focalPoint = target[0].transform.position;
        if (target.Count > 1)
        {
            for (int i = 1; i < target.Count; i++)
            {
                focalPoint = focalPoint + (target[i].transform.position - focalPoint) * 0.5f;
            }
        }
        //make sure it's within boundaries
        float distance = Vector3.Distance(focalPoint, MainCamera.transform.position);
        float frustrumHeight = 2.0f * distance * Mathf.Tan(MainCamera.GetComponent<Camera>().fieldOfView * 0.5f * Mathf.Deg2Rad);
        float frustrumWidth = frustrumHeight * MainCamera.GetComponent<Camera>().aspect;

        float maxX = m_EastMostFrustrum + frustrumWidth * 0.5f;
        float minX = m_WestMostFrustrum - frustrumWidth * 0.5f;

        float maxZ = m_NortMostFrustrum - frustrumHeight * 0.5f;
        float minZ = m_SouthMostFrustrum + frustrumHeight * 0.5f;

        focalPoint.x = Mathf.Clamp(focalPoint.x, minX, maxX);
        focalPoint.z = Mathf.Clamp(focalPoint.z, minZ, maxZ);

        return focalPoint;
    }

    private void DynamicCameraZoom()
    {
        GameObject northmost = Utility.GetFurthestTarget(m_CameraTargets, Direction.NORTH);
        GameObject southmost = Utility.GetFurthestTarget(m_CameraTargets, Direction.SOUTH);
        GameObject eastmost = Utility.GetFurthestTarget(m_CameraTargets, Direction.EAST);
        GameObject westmost = Utility.GetFurthestTarget(m_CameraTargets, Direction.WEST);

        float zBase = Vector3.Distance(northmost.transform.position, southmost.transform.position);
        float xBase = Vector3.Distance(eastmost.transform.position, westmost.transform.position);

        if (zBase > xBase)
        {
            SetCameraZoom(zBase * 1.25f * Mathf.Tan(60));
        }
        else
        {
            SetCameraZoom(xBase * 1.25f * Mathf.Tan(60));
        }
    }


    //-----------------------------------------------------------------
}
