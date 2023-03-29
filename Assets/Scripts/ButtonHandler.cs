using UnityEngine;

public class ButtonHandler : MonoBehaviour
{
    public void OnClick()
    {
        Debug.Log("Button clicked!"); 
        PressSpace();
    }

    private void PressSpace()
    {
        Debug.Log("Space pressed!"); 
        Input.GetKeyDown(KeyCode.Space); // Press the space key
    }
}