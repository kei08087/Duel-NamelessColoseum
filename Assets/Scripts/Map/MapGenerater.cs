using UnityEngine;
using System;

public class MapGenerater : MonoBehaviour
{
    private int[,,] intMap = new int[17,17,2];
    private GameObject[,] objectMap = new GameObject[17, 17];
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void OnEnable()
    {
        EventManager.GameSetup += CreateMap;
    }

    private void OnDisable()
    {
        EventManager.GameSetup -= CreateMap;
    }

    void CreateMap()
    {
        int objectToLoad = 5;
        GameObject block = ReturnMapPrefab(objectToLoad);
        for (int i = 0; i < 17; i++)
        {
            InstantiateBlock(block, new Vector3(0, 1.5f, i), objectToLoad);
            InstantiateBlock(block, new Vector3(i, 1.5f, 0), objectToLoad);
            InstantiateBlock(block, new Vector3(16, 1.5f, i), objectToLoad);
            InstantiateBlock(block, new Vector3(i, 1.5f, 16), objectToLoad);
        }
        objectToLoad = 3;
        block = ReturnMapPrefab(objectToLoad);
        for (int i = 0; i < 15; i++)
        {
            for (int j = 0; j < 15; j++)
            {
                InstantiateBlock(block, new Vector3(i + 1, 0.5f, j + 1), objectToLoad);
            }
        }

        objectToLoad = 4;
        block = ReturnMapPrefab(objectToLoad);
        for (int i = 0; i < 11; i++)
        {
            InstantiateBlock(block, new Vector3(3, 1f, i + 3), objectToLoad);
            InstantiateBlock(block, new Vector3(i + 3, 1f, 3), objectToLoad);
            InstantiateBlock(block, new Vector3(13, 1f, i + 3), objectToLoad);
            InstantiateBlock(block, new Vector3(i + 3, 1f, 13), objectToLoad);
            if (i == 4)
                i += 2;
        }

        objectToLoad = 1;
        block = ReturnMapPrefab(objectToLoad);
        InstantiateBlock(block, new Vector3(8.5f, 1.75f, 1.5f), objectToLoad, true);
        objectToLoad = 2;
        block = ReturnMapPrefab(objectToLoad);
        InstantiateBlock(block, new Vector3(8.5f, 1.75f, 14.5f), objectToLoad, true);
    }

    GameObject ReturnMapPrefab(int objectToLoad)
    {
        string objectName = Enum.GetName(typeof(MapEnum), objectToLoad);
        if (objectName == null)
        {
            Debug.LogWarning($"Enum 값 {objectToLoad}에 해당하는 이름이 없습니다.");
            return null;
        }
        string path = $"Prefabs/MapBlocks/{objectName}";
        GameObject prefab = Resources.Load<GameObject>(path);
        return prefab;
    }

    void InstantiateBlock(GameObject objectToInstantiate, Vector3 location, int objectNum, bool isObject=false)
    {
        int x = (int)location.x;
        int z = (int)location.z;
        if (!isObject)
        {
            if (intMap[x, z, 0] == 0)
            {
                objectMap[x, z] = Instantiate(objectToInstantiate, location, Quaternion.identity, this.transform);
                intMap[x, z, 0] = 1;
                intMap[x, z, 1] = objectNum;

            }
            else
            {
                if (intMap[x, z, 1] == objectNum)
                {
                    return;
                }
                Destroy(objectMap[x, z]);
                objectMap[x, z] = Instantiate(objectToInstantiate, location, Quaternion.identity, this.transform);
                intMap[x, z, 1] = objectNum;
            }
        }
        else
        {
            Instantiate(objectToInstantiate, location, Quaternion.identity,this.transform);
        }
            return;
    }
}
