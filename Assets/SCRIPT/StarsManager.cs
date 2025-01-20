using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StarsManager : MonoBehaviour
{
    public TextMeshProUGUI counter;
    private int starCounter ;
    // Start is called before the first frame update
    void Start()
    {
        starCounter = 0 ;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            starCounter++;
            counter.text = starCounter.ToString();
            this.gameObject.SetActive(false);
        }
    }
}
