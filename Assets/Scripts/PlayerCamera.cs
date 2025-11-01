using Unity.VisualScripting;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public Vector3 offset = new Vector3(0f, 20f, 0f); // 카메라 위치 오프셋
    public float smoothSpeed = 5f; // 부드럽게 따라가기 속도

    public Transform target;

    public float x;
    public float y;
    public float z;

    void OnEnable()
    {
        EventManager.PlayerSpawned += setTarget;
    }
    void OnDisable()
    {
        EventManager.PlayerSpawned -= setTarget;
    }

    void setTarget(GameObject player)
    {
        target = player.transform;
        x= target.position.x;
        y= target.position.y;
        z= target.position.z;
    }
    void LateUpdate()
    {
        if (!target) return;

        // 목표 위치 = 캐릭터 위치 + 오프셋
        Vector3 desiredPos = target.position + offset;

        // 부드러운 이동 (보간)
        Vector3 smoothedPos = Vector3.Lerp(transform.position, desiredPos, smoothSpeed * Time.deltaTime);

        transform.position = smoothedPos;


    }
}
