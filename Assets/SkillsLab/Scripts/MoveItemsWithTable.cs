﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveItemsWithTable : MonoBehaviour {

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<Item>())
        {
            other.gameObject.GetComponent<Item>().touchesTable = true;
            if (other.gameObject.GetComponent<Item>().isGrabbed == false)
            {
                StartCoroutine(ConstrainMovement(other));
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponent<Item>())
        {
            other.gameObject.GetComponent<Item>().touchesTable = false;
            other.gameObject.GetComponent<Item>().isGrabbed = false;
        }
    }

    private IEnumerator ConstrainMovement(Collider other)
    {
        yield return new WaitForSeconds(.3f);
        if (other.gameObject.GetComponent<Item>().touchesTable)
        {
            other.gameObject.transform.parent = gameObject.transform;
            other.gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        }
    }
}
