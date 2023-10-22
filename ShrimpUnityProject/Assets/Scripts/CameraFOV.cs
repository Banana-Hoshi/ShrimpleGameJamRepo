using Cinemachine;
using UnityEngine;

public class CameraFOV : MonoBehaviour
{

    CinemachineVirtualCamera vcam;
    [SerializeField] Rigidbody bike;


    // Start is called before the first frame update
    void Start()
    {
        vcam = GetComponent<CinemachineVirtualCamera>();
    }

    // Update is called once per frame
    void Update()
    {
        vcam.m_Lens.FieldOfView = 60 + bike.velocity.magnitude * 0.1f;
    }
}
