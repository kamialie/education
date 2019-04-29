using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //void Update() // called before rendering a frame
    //{
    //
    //}
    public float speed;
    public Text countText;
    public Text winText;

    private Rigidbody rb; // create a variable to hold reference
    private int count;

    void Start()
    {
        rb = GetComponent<Rigidbody>(); // get a reference
        count = 0;
        SetCountText();
        winText.text = "";
    }

    void FixedUpdate() // called before performing physics calculations
    {
        float moveHorizonal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(moveHorizonal, 0.0f, moveVertical);

        rb.AddForce(movement * speed);
    }

    void OnTriggerEnter(Collider other) // gets the collider trigger even and gives a reference to that object
    {
        if (other.gameObject.CompareTag("Pick Up"))
        {
            other.gameObject.SetActive(false);
            ++count;
            SetCountText();
        }
    }

    void SetCountText()
    {
        countText.text = "Count: " + count.ToString();
        if (count >= 12)
        {
            winText.text = "You win!";
        }
    }
}
