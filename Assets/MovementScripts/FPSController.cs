using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using StarterAssets;
using UnityEngine.InputSystem;

public class FPSController : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera aimVirtualCamera;
    [SerializeField] private float normalSensitivity;
    [SerializeField] private float aimSensitivity;
    [SerializeField] private LayerMask aimColliderLayerMask = new LayerMask();
    //[SerializeField] private Transform debugTransform;
    [SerializeField] private Transform pfBulletProjectile;
    [SerializeField] private Transform spawnBulletPosition;


    private StarterAssetsInputs starterAssetsInputs;
    private FirstPersonController firstPersonController;
    private Animator animator;

    private void Awake()
    {
        firstPersonController = GetComponent<FirstPersonController>();
        starterAssetsInputs= GetComponent<StarterAssetsInputs>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        Vector3 mouseWorldPosition = Vector3.zero;
        Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);
        if (Physics.Raycast(ray, out RaycastHit raycastHit, 999f, aimColliderLayerMask))
        {
            mouseWorldPosition = raycastHit.point;
        }
        if (starterAssetsInputs.aim)
        {
            aimVirtualCamera.gameObject.SetActive(true);
            firstPersonController.SetRotateOnMove(false);
            firstPersonController.SetSensitivty(aimSensitivity);

            Vector3 worldAimTarget = mouseWorldPosition;
            worldAimTarget.y = transform.position.y;
            Vector3 aimDirection = (worldAimTarget - transform.position).normalized;

            // Only update rotation if significant mouse movement is detected
            if (starterAssetsInputs.look.sqrMagnitude > 0.01f)
            {
                transform.forward = Vector3.Lerp(transform.forward, aimDirection, Time.deltaTime * 10f);
            }
        }
        else
        {
            firstPersonController.SetSensitivty(normalSensitivity);
            aimVirtualCamera.gameObject.SetActive(false);
            firstPersonController.SetRotateOnMove(true);
        }

        if (starterAssetsInputs.shoot)
        {
            Vector3 aimDir = (mouseWorldPosition - spawnBulletPosition.position).normalized;
            Instantiate(pfBulletProjectile, spawnBulletPosition.position, Quaternion.LookRotation(aimDir, Vector3.up));
            starterAssetsInputs.shoot = false;
        }
    }


}

