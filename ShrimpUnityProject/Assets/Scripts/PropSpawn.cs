using UnityEngine;

public class PropSpawn : MonoBehaviour
{

    [SerializeField] GameObject[] props;

    // Start is called before the first frame update
    void Start()
    {
		for (int i = 0; i < props.Length - 1; ++i)
        	props[Random.Range(0, props.Length)].SetActive(false);
		
        foreach (GameObject obj in props)
        {
            Rigidbody rb = obj.GetComponent<Rigidbody>();
            if (rb)
            {
                rb.mass = 0.1f;
            }
        }

    }

    // Update is called once per frame
    void Update()
    {

    }
}
