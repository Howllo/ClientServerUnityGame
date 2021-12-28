using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] float speed = 15.0f;
    [SerializeField] float increasingSpeed = 1.5f;
    private float inputHorizontal;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        inputHorizontal = Input.GetAxis("Horizontal");
        if (Input.GetButtonDown("Speedup"))
            speed *= increasingSpeed;
        if (Input.GetButtonUp("Speedup"))
            speed /= increasingSpeed;

        transform.Translate(Vector3.right * Time.deltaTime * speed * inputHorizontal);
    }
}
