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

    private float m_CameraZoom;

    private Vector3 m_PosVel;
     
    private float m_MinCameraZoom;
    private float m_MaxCameraZoom;

    private float m_EastMostFrustrum;
    private float m_WestMostFrustrum;
    private float m_NortMostFrustrum;
    private float m_SouthMostFrustrum;

    private float aspectRatio;
    private float tanFov;

    public bool dzEnabled;

    //-- Init -----------------------------------------------------------

    public void InitCamera(Vector3 initPosition, float minZoom = 4, float maxZoom = 20 )
    {
        MainCamera.transform.position = initPosition;
        SetCameraZoomBoundaries(minZoom, maxZoom);
        SetCameraZoom(maxZoom);
        aspectRatio = Screen.width / Screen.height;
        tanFov = Mathf.Tan(Mathf.Deg2Rad * MainCamera.GetComponent<Camera>().fieldOfView * 0.5f);
        if (m_EastMostFrustrum == 0f && m_WestMostFrustrum == 0f && m_NortMostFrustrum == 0f && m_SouthMostFrustrum == 0f)
        {
            SetCameraPositionBoundaries(6f, -6f, 10f, -10f);
        }        
    }

    public void InitCamera(Vector3 initPosition, float minZoom, float maxZoom, params GameObject[] cameraTargets)
    {
        InitCamera(initPosition, minZoom, maxZoom);
        AddTarget(cameraTargets);
    }

    public void InitCamera(Vector3 initPosition, float EastMostFrostrum, float WestMostFrostrum, float NorthMostFrostrum, float SouthMostFrostrum, params GameObject[] cameraTargets)
    {
        AddTarget(cameraTargets);
        InitCamera(initPosition);
        SetCameraPositionBoundaries(EastMostFrostrum, WestMostFrostrum, NorthMostFrostrum, SouthMostFrostrum);
    }
    //-- Update ----------------------------------------------------------

    public void UpdateCamera()
    {
        if (MainCamera == null) return;
        if (m_CameraTargets.Count <= 0) return;

        // Dynamically fit targets into view
        DynamicCameraZoom();    
        Vector3 FocalPoint = GetFocalPoint(m_CameraTargets);

        MainCamera.transform.LookAt(FocalPoint);
        Vector3 dir = (MainCamera.transform.position - FocalPoint).normalized;
        float cameraDistance = (m_CameraZoom * 0.5f / aspectRatio) / tanFov;
        Vector3 newPosition = FocalPoint + dir * (cameraDistance + 1f);
        //newPosition.y = Mathf.Max(newPosition.y, 3);
        if (Mathf.Abs(newPosition.z - FocalPoint.z) < 4.5f)
        {
            newPosition.z -= 2;
        }
        for(int i = 0; i < m_CameraTargets.Count; i++)
        {
            if(newPosition.z - m_CameraTargets[i].transform.position.z < 4.5f)
            {
                newPosition.z -= 2;
            }
        }

        newPosition.x = Mathf.Clamp(newPosition.x, -7, 5);

        newPosition.y += 1.5f;

        MainCamera.transform.position = Vector3.SmoothDamp(MainCamera.transform.position, newPosition, ref m_PosVel, 0.5f);
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
        m_CameraZoom = zoom;
    }

    public void SetCameraZoomBoundaries(float min, float max)
    {
        m_MinCameraZoom = min;
        m_MaxCameraZoom = max;
    }

    public void SetCameraPositionBoundaries(float east, float west, float north, float south)
    {
        m_MaxCameraZoom = Mathf.Max(east - west, north - south) * 0.5f * 1.732f; //a*sqrt(3)/2 to get max height
        m_MinCameraZoom = 4;
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

        Vector3 focalPoint = target[0].transform.position * 1.5f;
        if (target.Count > 1)
        {
            for (int i = 1; i < target.Count; i++)
            {
                focalPoint = focalPoint + (target[i].transform.position * 1.7f - focalPoint) * 0.5f;
            }
        }
        //make sure it's within boundaries
        //float distance = Vector3.Distance(focalPoint, MainCamera.transform.position);
        //float frustrumHeight = 2.0f * distance * Mathf.Tan(MainCamera.GetComponent<Camera>().fieldOfView * 0.5f * Mathf.Deg2Rad);
        //float frustrumWidth = frustrumHeight * MainCamera.GetComponent<Camera>().aspect;

        //float maxX = m_EastMostFrustrum + frustrumWidth * 0.5f;
        //float minX = m_WestMostFrustrum - frustrumWidth * 0.5f;

        //float maxZ = m_NortMostFrustrum - frustrumHeight * 0.5f;
        //float minZ = m_SouthMostFrustrum + frustrumHeight * 0.5f;

        //focalPoint.x = Mathf.Clamp(focalPoint.x, minX, maxX);
        //focalPoint.z = Mathf.Clamp(focalPoint.z, minZ, maxZ);

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
            SetCameraZoom(zBase);
        }
        else
        {
            SetCameraZoom(xBase);
        }
    }


    //-----------------------------------------------------------------
}
