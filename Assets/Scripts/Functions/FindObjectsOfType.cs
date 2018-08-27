using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindObjectsOfType : MonoBehaviour
{

    void Start()
    {
        List<GameObject> allGOs = new List<GameObject>();
        GameObject[] rootGOs = gameObject.scene.GetRootGameObjects();

        for (int i = 0; i < rootGOs.Length; i++)
            GetAllChildren(rootGOs[i].transform, allGOs);

        //Print them all out
        foreach (GameObject g in allGOs)
            Debug.Log(g.name);
    }

    void GetAllChildren(Transform current, List<GameObject> arrayToFill)
    {
        arrayToFill.Add(current.gameObject);

        for (int i = 0; i < current.childCount; i++)
            GetAllChildren(current.GetChild(i), arrayToFill);
    }

}
