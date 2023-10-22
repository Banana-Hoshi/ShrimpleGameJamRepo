using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public GameObject[] gridSpaces;
    public GameObject[] optionalGridSpaces;
    public GameObject[] buildingList;
    public float distance = 25.2f;
    public LayerMask mask;
	public GameManager manager;

    private void Start()
    {
        GenerateBuildings();
    }


    public void ChooseOptionalGridSpaces()
    {
        foreach (var space in optionalGridSpaces) 
        {
            space.SetActive(true);
        }
        for (int i = 0; i < optionalGridSpaces.Length/2;)  
        {
            int num = Random.Range(0, optionalGridSpaces.Length);
            if (optionalGridSpaces[num].activeInHierarchy)
            {
                optionalGridSpaces[num].SetActive(false);
                ++i;
            }
        }
    }

    public void GenerateBuildings()
    {
        ChooseOptionalGridSpaces();
		List<OnTriggerEnterEvent> triggers = new List<OnTriggerEnterEvent>();
        foreach (var tile in gridSpaces)
        {
            GameObject obj = Instantiate(buildingList[Random.Range(0, buildingList.Length)], tile.transform);
            obj.transform.localRotation = GetAngle(obj.transform.position);

			triggers.Add(obj.GetComponentInChildren<OnTriggerEnterEvent>());
        }
        foreach (var tile in optionalGridSpaces)
        {
            GameObject obj = Instantiate(buildingList[Random.Range(0, buildingList.Length)], tile.transform);
            obj.transform.localRotation = GetAngle(obj.transform.position);

			triggers.Add(obj.GetComponentInChildren<OnTriggerEnterEvent>());
        }

		manager.Send(triggers);
    }

    Quaternion GetAngle(Vector3 position)
    {
        do
        {
            Quaternion angle = Quaternion.Euler(0f, Random.Range(0, 4) * 90f, 0f);
            if (!Physics.CheckSphere(position + Vector3.up * 2f + angle * Vector3.left * distance, 1f, mask))
            {
                return angle;
            }
        } while (true);
    }
}
