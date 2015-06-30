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
        if (Input.GetButtonDown("Fire1") && ButtonColor != 7)
        {
            gameObject.GetComponent<SpriteRenderer>().sprite =
                hexController.HexSprites[ButtonColor, hexController.Pastel];
        }
	}

    private void OnMouseDown()
    {
        if (ButtonColor == 7)
        {
            Application.Quit();
            return;
        }
        if (ButtonColor == 6)
        {
            if (hexController.Pastel == 0)
            {
                hexController.Pastel = 1;
            }
            else
            {
                hexController.Pastel = 0;
            }
            hexController.SetPastel();
            return;
        }
        if (!hexController.IsWinBool)
        {
            hexController.HexSet(ButtonColor, true);
        }
    }
}
