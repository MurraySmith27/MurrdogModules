using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimAtCamera : MonoBehaviour
{
    [SerializeField] private Transform transform;

    [SerializeField] private Camera camera;

    private void Awake()
    {
        if (camera == null)
        {
            camera = Camera.main;
        }
    }

    private void Update()
    {
        transform.rotation = Quaternion.Euler(Quaternion.LookRotation(transform.position - camera.transform.position, Vector3.up).eulerAngles.x, 90, 0);
    }
}
