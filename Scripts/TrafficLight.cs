using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class TrafficLight : MonoBehaviour
{

    SpriteRenderer spriteRenderer;

    public Sprite[] sprites;
    public float timeInterval;
    private float timeLeft;
    private int index; // green is 0, yellow is 1, red is 2

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        timeLeft = timeInterval;
    }

    // Update is called once per frame
    void Update()
    {
        timeLeft -= Time.deltaTime;
        if (timeLeft < 0){
            timeLeft = timeInterval;
            if (index == 2){
                index = 0;
                spriteRenderer.sprite = sprites[index];
            }
            else{
                index++;
                spriteRenderer.sprite = sprites[index];
            }
        }
    }
}
