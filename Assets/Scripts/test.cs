using UnityEngine;
using UnityEngine.InputSystem;

public class test : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    bool locked = true;
    void Update()
    {
        if (locked)
        {
            Mouse.current.WarpCursorPosition(Vector2.zero);
            if(Input.GetKeyDown(KeyCode.Escape))
            {
                locked = false;
            }
        }
    }
}