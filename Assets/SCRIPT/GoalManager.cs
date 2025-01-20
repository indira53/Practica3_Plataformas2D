using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GoalManager : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public Sprite[] sprites;


    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = this.GetComponent<SpriteRenderer>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && spriteRenderer.sprite == sprites[0])
        {
            spriteRenderer.sprite = sprites[1];
            Invoke("WinScene", 1.5f);
        }
    }

    public void WinScene()
    {
        SceneManager.LoadScene("WinScene");
    }

}
