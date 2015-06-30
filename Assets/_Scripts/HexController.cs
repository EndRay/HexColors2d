using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine.UI;

public class HexController : MonoBehaviour {

    //Red, Blue, Yellow, Green, Black, White

    private int[,] _hexes = new int[20,14];

    private int[,] _currentHexes = new int[20, 14];
    
    private GameObject[,] _objHexes = new GameObject[20,14];

    private int _color;

    private int _moves;

    private int[,] _border = new int[20,14];

    private int[][] _sides =
    {
        new int[] {0,1},
        new int[] {1,0},
        new int[] {0,-1},
        new int[] {-1,0}
    };

    private int[][] _rSides =
    {
        new int[] {1, 1},
        new int[] {1, -1}
    };

    private int _best = 1000;

    public Sprite[,] HexSprites = new Sprite[7, 2];
    
    public int Pastel;

    public GameObject Hex;

    public Sprite[] StandartSprites;

    public Sprite[] PastelSprites;

    public TextMesh MovesText;

    public TextMesh BestText;

    public GameObject WinText;

    public bool IsWinBool;

    public InputField SeedInput;

    // Use this for initialization
	void Start () {
        for (int i = 0; i < 7; i++)
        {
            HexSprites[i, 0] = StandartSprites[i];
            HexSprites[i, 1] = PastelSprites[i];
        }
        LoadLevel();

	}
	
	// Update is called once per frame
	void Update () {
	}

    public void LoadLevel()
    {
        IsWinBool = false;
        _moves = 0;
        MovesText.text = "0";
        WinText.SetActive(false);
        string seed = SeedInput.text.Trim();
        
        if (seed.Length == 0)
        {
            seed = System.Guid.NewGuid().ToString().Substring(0,8);
            SeedInput.text = seed;
        }

        Random.seed = seed.GetHashCode();
        foreach (var h in _objHexes)
        {
            Destroy(h);
        }
        for (int x = 0; x < 20; x++)
        {
            for (int y = 0; y < 14; y++)
            {
                _border[x, y] = 0;
                if (x == 0 || x == 19 || y == 0 || y == 13)
                {
                    _currentHexes[x, y] = 7; 
                    _hexes[x, y] = 7;
                    continue;
                }
                int R = Random.Range(0, 6);
                _best = 1000;
                BestText.text = "?";
                if (x == 9 && y == 8)
                {
                    R = 6;
                } 
                _hexes[x, y] = R;
                _currentHexes[x, y] = R;
                GameObject createObj =
                Set(R, new Vector2(x + (y % 2 == 0 ? 0.5f : 0), y * 0.75f));
                _objHexes[x, y] = createObj;
                
                
            }
        }
        SetBorder();
    }

    public void HexSet(int color, bool isMove)
    {
        if (isMove)
        {
            MovesAdd();
        }
        bool isSet = false;
        for (int x = 1; x < 19; x++)
        {
            for (int y = 1; y < 13; y++)
            {
                if (_border[x, y] == 1)
                {
                    foreach (var s in _sides)
                    {
                        if (_hexes[x + s[0], y + s[1]] == color)
                        {
                            _hexes[x + s[0], y + s[1]] = 6;
                            _objHexes[x + s[0], y + s[1]].GetComponent<SpriteRenderer>().sprite = HexSprites[6,Pastel];
                            isSet = true;
                        }
                    }
                    foreach (var rS in _rSides)
                    {
                        if (y%2 == 0)
                        {
                            if (_hexes[x + rS[0], y + rS[1]] == color)
                            {
                                isSet = true;
                                _objHexes[x + rS[0], y +rS[1]].GetComponent<SpriteRenderer>().sprite = HexSprites[6, Pastel];
                                _hexes[x + rS[0], y + rS[1]] = 6;            
                            }
                        }
                        else
                        {
                            if (_hexes[x - rS[0], y + rS[1]] == color)
                            {
                                isSet = true;
                                _objHexes[x - rS[0], y + rS[1]].GetComponent<SpriteRenderer>().sprite = HexSprites[6, Pastel];
                                _hexes[x - rS[0], y + rS[1]] = 6;
                            }
                        }
                    }
                }
            }
        }

        if (isSet)
        {
            SetBorder();
            HexSet(color,false);
        }
        isWin();
    }

    private void SetBorder()
    {
        for (int x = 1; x < 19; x++)
        {
            for (int y = 1; y < 13; y++)
            {
                if (_hexes[x, y] == 6)
                {
                    bool isAll = true; 
                    foreach (var s in _sides)
                    {
                        if (_hexes[x+s[0], y+s[1]] == 6)
                        {
                            foreach (var rS in _rSides)
                            {
                                if (y%2 == 0)
                                {
                                    {
                                        if (_hexes[x + rS[0], y + rS[1]] == 6)
                                        {
                                            continue;
                                        }
                                    }
                                }
                                else
                                {
                                    {
                                        if (_hexes[x - rS[0], y + rS[1]] == 6)
                                        {
                                            continue;
                                        }
                                    }
                                }
                            }
                        }
                        isAll = false;
                        break;
                    }
                    if (!isAll)
                    {
                        _border[x, y] = 1;
                    }
                }
            }
        }
    }

    private void MovesAdd()
    {
        _moves++;
        MovesText.text = _moves.ToString();
    }

    

    private void isWin()
    {
        for (int x = 1; x < 19; x++)
        {
            for (int y = 1; y < 13; y++)
            {
                if (_hexes[x, y] == 6)
                {
                    continue;
                }
                return;
            }   
        }
        WinText.SetActive(true);
        IsWinBool = true;
        if (_moves < _best)
        {
            _best = _moves;
            BestText.text = _best.ToString();
        }
    }

    private GameObject Set(int color, Vector2 vector)
    {
        GameObject obj = Instantiate(Hex, vector, Quaternion.identity) as GameObject;
        obj.GetComponent<SpriteRenderer>().sprite = HexSprites[color, Pastel];
        return obj;
    }

    public void SetPastel()
    {
        for (int x = 1; x < 19; x++)
        {
            for (int y = 1; y < 13; y++)
            {
                _objHexes[x, y].GetComponent<SpriteRenderer>().sprite = HexSprites[_hexes[x, y], Pastel];
            }
        }
    }
}
