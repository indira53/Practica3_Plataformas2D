using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifeManager : MonoBehaviour
{
    // Start is called before the first frame update
    public int Health
    {
        get => _health;
        set
        {
            if (value <= maxHealth)
            {
                _health = value;
            }
            else
            {
                _health = maxHealth;
            }
        }
    }

    public int maxHealth;
    public int StartingHealth;
    private int _health;

    void Start()
    {
        Health = StartingHealth;
    }


    // Update is called once per frame
    void Update()
    {

    }

}
