using UnityEngine;

public class Button : MonoBehaviour
{
    public HexController HexController;

    public int ButtonColor;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
    
    }

    private void OnMouseDown()
    {
        if (!HexController.IsWinBool)
        {
            HexController.HexSet(ButtonColor, true);
        }
    }
}
