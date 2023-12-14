using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    [SerializeField] private bool invert;

    private Transform cameraTransform;

    private void Awake(){
        cameraTransform = Camera.main.transform;
    }

    private void LateUpdate(){
        if(invert){
            transform.rotation = Quaternion.Inverse(cameraTransform.rotation);
        }
        else{
            transform.rotation = cameraTransform.rotation;
        }
    }
}
