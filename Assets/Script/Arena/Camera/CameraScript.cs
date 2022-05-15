using Unity.Netcode;
using UnityEngine;

namespace Arena
{
    public class CameraScript : MonoBehaviour
    {
        private Transform playerTransform;

        void LateUpdate()
        {
            if (NetworkManager.Singleton.SpawnManager != null && NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject() != null)
            {
                playerTransform = NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject().transform;
            }

            if (playerTransform != null)
            {
                transform.position = new Vector3(playerTransform.position.x, playerTransform.position.y + 20, playerTransform.position.z - 20);
            }
        }
    }
}
