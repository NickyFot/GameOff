using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utility
{
    public enum Direction
    {
        NORTH,
        SOUTH,
        EAST,
        WEST
    };

    // -- Gameplay

    public static GameObject GetFurthestTarget(List<GameObject> target, Direction dir)
    {
        float position = 0;
        GameObject furthestTarget = target[0];

        switch (dir)
        {
            case Direction.NORTH:
                for (int i = 0; i < target.Count; i++)
                {
                    if (target[i].transform.position.z >= position)
                    {
                        position = target[i].transform.position.z;
                        furthestTarget = target[i];
                    }
                }
                break;
            case Direction.SOUTH:
                for (int i = 0; i < target.Count; i++)
                {
                    if (target[i].transform.position.z <= position)
                    {
                        position = target[i].transform.position.z;
                        furthestTarget = target[i];
                    }
                }
                break;
            case Direction.EAST:
                for (int i = 0; i < target.Count; i++)
                {
                    if (target[i].transform.position.x <= position)
                    {
                        position = target[i].transform.position.x;
                        furthestTarget = target[i];
                    }
                }
                break;
            case Direction.WEST:
                for (int i = 0; i < target.Count; i++)
                {
                    if (target[i].transform.position.x >= position)
                    {
                        position = target[i].transform.position.x;
                        furthestTarget = target[i];
                    }
                }
                break;
            default:
                furthestTarget = target[0];
                break;
        }
        return furthestTarget;
    }

    // -- Trigonometry

    public static float ClampToCircle(float degrees)
    {
        if(degrees > 360)
        {
            return degrees - ( 360 * ( Mathf.FloorToInt(degrees / 360) ) );
        }
        else if (degrees < 0)
        {
            int remainder = Mathf.CeilToInt(degrees / 360) == 0 ? 1 : Mathf.CeilToInt(degrees / 360);
            return degrees + ( 360 * ( remainder ) );
        }
        return degrees;
    }
}
