using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletProjectile : MonoBehaviour
{
    [SerializeField] private TrailRenderer trailRenderer;
    [SerializeField] private Transform bulletHitVfxPrefab;
    [SerializeField] private float moveSpeed = 200f;

    private Vector3 targetPosition;

    public void Setup(Vector3 targetPosition){
        this.targetPosition = targetPosition;
    }

    public void Update(){
        Vector3 moveDirection = (targetPosition - transform.position).normalized;

        float distanceBeforeMoving = Vector3.Distance(transform.position, targetPosition);
        transform.position += moveDirection * moveSpeed * Time.deltaTime;

        float distanceAfterMoving = Vector3.Distance(transform.position, targetPosition);

        if(distanceBeforeMoving < distanceAfterMoving){
            //Per evitare che il trail sia oltre il nemico prima della scomparsa
            transform.position = targetPosition;

            trailRenderer.transform.parent = null;

            Destroy(gameObject);

            Instantiate(bulletHitVfxPrefab, targetPosition, Quaternion.identity);
        }
    }
}
