using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TweenRotater : MonoBehaviour
{
    [SerializeField] private float rotateSpeed;
    private Vector3 targetRight = Vector3.right;
    private Vector3 newRotate = new Vector3();
    private float dir;
    private void Update()
    {
        if (targetRight.Equals(transform.right)) return;
        float z = Vector3.Cross(transform.right, targetRight).z;
        dir = Mathf.Sign(z);
        newRotate.Set(0, 0, transform.eulerAngles.z + rotateSpeed * Time.deltaTime * dir);
        transform.eulerAngles = newRotate;
        if (Mathf.Approximately(Vector3.Angle(transform.right, targetRight), 0)) transform.right = targetRight;
    }
    public void RotateTo(Vector3 target)
    {
        if (target.Equals(Vector3.zero)) return;
        targetRight = target;
    }
}
