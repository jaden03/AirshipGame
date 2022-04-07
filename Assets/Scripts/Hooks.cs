using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hooks : MonoBehaviour
{
    Player player;
    bool canSet = true;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (canSet)
        {
            Destroy(GetComponent<Rigidbody>());
            Destroy(GetComponent<BoxCollider>());
            transform.parent.parent = collision.transform;
            canSet = false;
        }
    }

    private void FixedUpdate()
    {
        if (!canSet)
            player.updateGrapplePosition(transform.position);

        if (!Input.GetKey(KeyCode.Mouse0))
            Destroy(this);
    }
}
