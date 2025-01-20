using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class PlayerHealthDisplay : MonoBehaviour
{
    private SpriteRenderer HPRenderer;
    public Sprite[] sprites;
    // Start is called before the first frame update
    void Start()
    {
        HPRenderer = this.GetComponent<SpriteRenderer>();
    }

    public void UpdateDisplay(int newHealth)
    {
        HPRenderer.sprite = sprites[newHealth];
    }

    // Update is called once per frame
    void Update()
    {

    }
}
