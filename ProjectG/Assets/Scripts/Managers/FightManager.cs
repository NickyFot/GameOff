using UnityEngine;
using System.Collections.Generic;

public class FightManager : Singleton<FightManager>
{
    //-----------------------------------------------------------------

    public GameObject ArenaObject;

    public List<Vector3> SpawnPointList = new List<Vector3>();
    public List<Unit> AliveFightersList = new List<Unit>();

    enum PLAYER_ID { ONE, TWO, THREE, FOUR };

    //-----------------------------------------------------------------

    // TO - DO: ADD LEVEL PARAM
    public void SetupNewRound()
    {
        SpawnPointList = new List<Vector3>(GetSpawnPoints(ArenaObject));

        Unit fighter1 = new PlayerUnit("PlayerUnit", SpawnPointList[0]);
        InputManager.Instance.AssignUnitToNextController(fighter1);

        CameraManager.Instance.InitCamera(Vector3.zero);
        CameraManager.Instance.AddTarget(fighter1.UnitObj);

        Unit fighter2 = new NPCUnit("PlayerUnit", SpawnPointList[1]);
        fighter2.UnitParentObj.name = "NPC";
        CameraManager.Instance.AddTarget(fighter2.UnitObj);

        AliveFightersList.Add(fighter1);
        AliveFightersList.Add(fighter2);

        //Unit fighter3 = new NPCUnit("PlayerUnit", SpawnPointList[1]);
        //fighter3.UnitObj.name = "NPC";

        //Debug.Log(fighter2.Equals(fighter3));

        //CameraManager.Instance.AddTarget(fighter2.UnitObj);
        //CameraManager.Instance.AddTarget(fighter3.UnitObj);
    }

    //-----------------------------------------------------------------

    public void UpdateFight()
    {
        for(int i = 0; i < AliveFightersList.Count; i++)
        {
            AliveFightersList[i].Update();
        }
    }

    //-----------------------------------------------------------------

    private List<Vector3> GetSpawnPoints(GameObject Level)
    {
        Transform parent = Level.transform.Find("SpawnPoints");
        List<Vector3> spawnList = new List<Vector3>();
        for(int i = 0; i < parent.childCount; i++)
        {
            spawnList.Add(parent.GetChild(i).position);
        }
        return spawnList;
    }

    //-----------------------------------------------------------------
}
