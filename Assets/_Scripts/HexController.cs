using UnityEngine;
using UnityEngine.UI;

public class HexController : MonoBehaviour {

    //Red, Blue, Yellow, Green, Black, White

    private int width = 20;

    private int height = 16;

    private int[,] _hexes;

    private int[,] _currentHexes;

    private GameObject[,] _objHexes;

    private int _moves;

    private int[,] _border;

    private readonly int[][] _sides =
    {
        new[] {0,1},
        new[] {1,0},
        new[] {0,-1},
        new[] {-1,0}
    };

    private readonly int[][] _rSides =
    {
        new[] {1, 1},
        new[] {1, -1}
    };

    private string _seed;

    public Sprite[,] HexSprites = new Sprite[7, 2];
    
    public bool Pastel;

    public GameObject Hex;

    public Sprite[] StandartSprites;

    public Sprite[] PastelSprites;

    public TextMesh MovesText;

    public TextMesh BestText;

    public GameObject WinText;

    public bool IsWinBool;

    public InputField SeedInput;

    public GameObject[] Buttons;

    // Use this for initialization
	void Start () {
        _border = new int[width,height];
        _currentHexes = new int[width, height];
        _hexes = new int[width,height];
        _objHexes = new GameObject[width, height];
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
        if (PlayerPrefs.GetInt(seed) == 0)
        {
            PlayerPrefs.SetInt(seed, 1000);
            BestText.text = "?";
        }
        else
        {
            BestText.text = PlayerPrefs.GetInt(seed).ToString();
        }
        _seed = seed;
        Random.seed = seed.GetHashCode();
        foreach (var h in _objHexes)
        {
            Destroy(h);
        }
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                _border[x, y] = 0;
                if (x == 0 || x == width - 1 || y == 0 || y == height - 1)
                {
                    _currentHexes[x, y] = 7; 
                    _hexes[x, y] = 7;
                    continue;
                }
                int r = Random.Range(0, 6);
                if (x == 9 && y == 8)
                {
                    r = 6;
                } 
                _hexes[x, y] = r;
                _currentHexes[x, y] = r;
                GameObject createObj =
                Set(r, new Vector2(x + (y % 2 == 0 ? 0.5f : 0), y * 0.75f));
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
        for (int x = 1; x < width - 1; x++)
        {
            for (int y = 1; y < height - 1; y++)
            {
                if (_border[x, y] == 1)
                {
                    foreach (var s in _sides)
                    {
                        if (_hexes[x + s[0], y + s[1]] == color)
                        {
                            _hexes[x + s[0], y + s[1]] = 6;
                            _objHexes[x + s[0], y + s[1]].GetComponent<SpriteRenderer>().sprite = HexSprites[6,Pastel ? 1 : 0];
                            isSet = true;
                        }
                    }
                    foreach (var rS in _rSides)
                    {
                        int dx = y%2 == 0 ? x + rS[0]: x - rS[0];

                        if (_hexes[dx, y + rS[1]] == color)
                        {
                            isSet = true;
                            _objHexes[x + rS[0], y + rS[1]].GetComponent<SpriteRenderer>().sprite =
                                HexSprites[6, Pastel ? 1 : 0];
                            _hexes[x + rS[0], y + rS[1]] = 6;
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
        IsWin();
    }

    private void SetBorder()
    {
        for (int x = 1; x < width - 1; x++)
        {
            for (int y = 1; y < height - 1; y++)
            {
                if (_hexes[x, y] == 6)
                {
                    bool isAll = true; 
                    foreach (var s in _sides)
                    {
                        if (_hexes[x + s[0], y + s[1]] != 6)
                        {
                            isAll = false;
                        }
                    }
                    foreach (var rS in _rSides)
                    {
                        int dx = y % 2 == 0 ? x + rS[0] : x - rS[0];
                        if (_hexes[dx, y + rS[1]] != 6)
                        {
                            isAll = false;
                        }
                    }

                    if (!isAll)
                    {
                        _border[x, y] = 1;
                    }
                    else
                    {
                        _border[x, y] = 0;
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

    

    private void IsWin()
    {
        for (int x = 1; x < width - 1; x++)
        {
            for (int y = 1; y < height - 1; y++)
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
        if (_moves < PlayerPrefs.GetInt(_seed))
        {
            PlayerPrefs.SetInt(_seed,_moves);
            BestText.text = PlayerPrefs.GetInt(_seed).ToString();
        }
    }

    private GameObject Set(int color, Vector2 vector)
    {
        GameObject obj = Instantiate(Hex, vector, Quaternion.identity) as GameObject;
        obj.GetComponent<SpriteRenderer>().sprite = HexSprites[color, Pastel ? 1 : 0];
        return obj;
    }

    public void SetPastel()
    {
        Pastel = !Pastel;
        for (int x = 1; x < width - 1; x++)
        {
            for (int y = 1; y < height - 1; y++)
            {
                _objHexes[x, y].GetComponent<SpriteRenderer>().sprite = HexSprites[_hexes[x, y], Pastel ? 1 : 0];
            }
        }
        foreach (var b in Buttons)
        {
            b.GetComponent<SpriteRenderer>().sprite =
            HexSprites[b.GetComponent<Button>().ButtonColor, Pastel ? 1 : 0];   
        }
    }

    public void Exit()
    {
        Application.Quit();
    }
}
