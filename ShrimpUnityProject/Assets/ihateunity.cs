using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ihateunity : MonoBehaviour
{
}

[CustomEditor(typeof(ihateunity))]
public class bruh : Editor
{
    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("death"))
        {
            Debug.Log("bang");
            List<GameObject> list = new List<GameObject>();
            for (int i = 0; i < (target as ihateunity).transform.childCount - 1; ++i)
            {
                for (int j = i + 1; j < (target as ihateunity).transform.childCount; ++j)
                {
                    if ((target as ihateunity).transform.GetChild(i).position == (target as ihateunity).transform.GetChild(j).position)
                    {
                        if (list.Contains((target as ihateunity).transform.GetChild(j).gameObject))
                            Debug.Log("dupe");
                        else
                            list.Add((target as ihateunity).transform.GetChild(j).gameObject);
                    }
                }
            }
            Debug.Log(list.Count);
            foreach (GameObject go in list)
            {
                DestroyImmediate(go);
            }
        }
    }
}