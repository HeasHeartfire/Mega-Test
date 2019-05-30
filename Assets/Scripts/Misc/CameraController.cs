using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    private Transform player;
    private float moveSpeed = 2f;
    private float top;
    private float topLimit = 0.098f;
    
    void Start () {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }
	
	void LateUpdate () {
        top = player.position.y;
        if(top > topLimit)
        {
            top = topLimit;
        }
        Vector3 newPosition = new Vector3(player.position.x, top, transform.position.z);
        //transform.position = newPosition;
        transform.position = Vector3.Lerp(transform.position, newPosition, moveSpeed * Time.deltaTime);
    }
}
