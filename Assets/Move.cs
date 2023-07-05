using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{
    [SerializeField] private float speed;
    private Vector3 direction = new Vector2();
    private TweenRotater rotater;
    private void Awake()
    {
        rotater = GetComponentInChildren<TweenRotater>();
    }
    private void Update()
    {
        GatherInput();
        transform.position += direction * speed * Time.deltaTime;
    }
    private void GatherInput()
    {
        float x = 0, y = 0;

        if (Input.GetKey(KeyCode.D)) x = 1;
        else if (Input.GetKey(KeyCode.A)) x = -1;
        if (Input.GetKey(KeyCode.W)) y = 1;
        else if (Input.GetKey(KeyCode.S)) y = -1;
        direction.Set(x, y, 0);
        rotater.RotateTo(direction);
    }
}
