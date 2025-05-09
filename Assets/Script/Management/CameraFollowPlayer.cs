using UnityEngine;
using Cinemachine;

public class CameraFollowPlayer : MonoBehaviour
{
    private CinemachineStateDrivenCamera virtualCam;

    void Start()
    {
        virtualCam = GetComponent<CinemachineStateDrivenCamera>();

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            virtualCam.Follow = player.transform;
        }
    }

    void LateUpdate()
    {
        if (virtualCam.Follow != null)
        {
            Vector3 cameraPosition = virtualCam.transform.position;
            cameraPosition.z = -20f;
            virtualCam.transform.position = cameraPosition;
        }
    }
}
