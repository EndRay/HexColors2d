using UnityEngine;
using System.Collections;

public class Button : MonoBehaviour
{
    public HexController hexController;

    public int ButtonColor;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
    
    }

    private void OnMouseDown()
    {
        if (!hexController.IsWinBool)
        {
            hexController.HexSet(ButtonColor, true);
        }
    }
}
