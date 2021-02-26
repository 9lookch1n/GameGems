using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//enum GameState
public enum GameState
{
    //Ordered to wait or move
    wait,
    move
}
public class Ctrl_Board : MonoBehaviour
{
    //for use across Script
    public GameState currentState = GameState.move;
    
    //Set all Variable
    public int width;
    public int height;
    public int offSet;

    public GameObject BGprefabs;
    public GameObject delEffect;    

    public GameObject[] Gems;
    public GameObject[,] allGems;

    public int scoreAdd = 20;
    private int scoreStreak = 1;
    private Ctrl_Score ctrl_score;

    // Start is called before the first frame update
    void Start()
    {
        ctrl_score = FindObjectOfType<Ctrl_Score>();
        allGems = new GameObject[width, height];
        Setup();
    }

    //Method Swipe Setup
    private void Setup()
    {
        //Loop for width++ and height++
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                //-Part Instantiate BGprefabs-
                //set vector2 position in BlackGround
                Vector2 PositionBG = new Vector2(x, y + offSet);
                //create Gem
                GameObject Ctrl_BG = Instantiate(BGprefabs, PositionBG, Quaternion.identity) as GameObject;
                //set parent Gem
                Ctrl_BG.transform.parent = this.transform;
                //set name Gem is position
                Ctrl_BG.name = "(" + x + "," + y + ")";

                //-Part Instantiate gems-
                //Random create Gem in Array
                int dotToUse = Random.Range(0, Gems.Length);

                //set MaxIteration
                int maxIteration = 0;
                //Loop while check 
                while (MatchesAt(x, y, Gems[dotToUse]) && maxIteration < 100)
                {
                    //Random create Gem in Array
                    dotToUse = Random.Range(0, Gems.Length);
                    //add maxIteration One by one
                    maxIteration++;
                }
                maxIteration = 0;

                //set position Gem when setup
                GameObject gem = Instantiate(Gems[dotToUse], PositionBG, Quaternion.identity) as GameObject;
                gem.GetComponent<Ctrl_Gem>().row = y;
                gem.GetComponent<Ctrl_Gem>().column = x;
                gem.transform.parent = this.transform;
                //set name Gem is position
                gem.name = "(" + x + "," + y + ")";
                allGems[x, y] = gem;
            }
        }
    }
    //method bool check MatchesAt
    private bool MatchesAt(int column, int row, GameObject piece)
    {
        //check column and row when Metches
        if (column > 1 && row > 1)
        {
            if (allGems[column - 1, row].tag == piece.tag && allGems[column - 2, row].tag == piece.tag)
            {
                // == true
                return true;
            }
            if (allGems[column, row - 1].tag == piece.tag && allGems[column, row - 2].tag == piece.tag)
            {
                // == true
                return true;
            }
            else if (column <= 1 || row <= 1)
            {
                if (row > 1)
                {
                    if (allGems[column, row - 1].tag == piece.tag && allGems[column, row - 2].tag == piece.tag)
                    {
                        // == true
                        return true;
                    }
                }
                if (column > 1)
                {
                    if (allGems[column - 1, row].tag == piece.tag && allGems[column - 2, row].tag == piece.tag)
                    {
                        // == true
                        return true;
                    }
                }
            }
        }
        // == fales
        return false;
    }
    //method DestroyGems
    public void DestroyGems(int colmn, int row)
    {
        if (allGems[colmn, row].GetComponent<Ctrl_Gem>().isMatch)
        {
            GameObject particle =  Instantiate(delEffect, allGems[colmn, row].transform.position, Quaternion.identity);
            Destroy(particle, .5f);

            Destroy(allGems[colmn, row]);
            allGems[colmn, row] = null;

            ctrl_score.IncreaseScroe(scoreAdd * scoreStreak);
        }
    }
    //method DestroyGem
    public void DestroyGem()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (allGems[x, y] != null)
                {
                    DestroyGems(x, y);
                }
            }
        }
        StartCoroutine(DecreaseRow());
    }
    //IEnumerator for Method DecreaseRow
    private IEnumerator DecreaseRow()
    {
        int nullCount = 0;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (allGems[x, y] == null)
                {
                    nullCount++;
                }
                else if (nullCount > 0)
                {
                    allGems[x, y].GetComponent<Ctrl_Gem>().row -= nullCount;
                    allGems[x, y] = null;
                }
            }
            nullCount = 0;
        }
        yield return new WaitForSeconds(.4f);
        //StartCoroutine
        StartCoroutine(FillBoard());
    }
    //Method Swipe Refill "Gem"
    private void Refill()
    {
        for(int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (allGems[x, y] == null)
                {
                    Vector2 tempPos = new Vector2(x, y + offSet);
                    int dotToUse = Random.Range(0, Gems.Length);

                    GameObject piece = Instantiate(Gems[dotToUse], tempPos, Quaternion.identity);
                    allGems[x, y] = piece;

                    piece.GetComponent<Ctrl_Gem>().row = y;
                    piece.GetComponent<Ctrl_Gem>().column = x;
                }
            }
        }
    }
    //Method bool MatchOnBoard
    private bool MatchOnBoard()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (allGems[x, y] != null)
                {
                    if (allGems[x, y].GetComponent<Ctrl_Gem>().isMatch)
                    {
                        // == true
                        return true;
                    }
                }
            }
        }
        // == false
        return false;
    }
    //IEnumerator for Method ReFillBoard
    private IEnumerator FillBoard()
    {
        Refill();
        yield return new WaitForSeconds(.5f);

        while (MatchOnBoard())
        {
            scoreStreak ++;
            yield return new WaitForSeconds(.5f);
            DestroyGem();
        }
        yield return new WaitForSeconds(.5f);
        currentState = GameState.move;
        //reset scoreStreak
        scoreStreak = 1;
    }
}
