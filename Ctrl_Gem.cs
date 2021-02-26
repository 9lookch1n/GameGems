using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Ctrl_Gem : MonoBehaviour
{
    //Set all Variable
    [Header("Board")]
    public int column;
    public int row;
    public int beforeColumn;
    public int beforeRow;
    public int targetX;
    public int targetY;
    private Ctrl_Board board;
    private GameObject otherGem;
    //bool check Matches
    public bool isMatch = false;
    //Set position Touch
    private Vector2 firstTouchPos;
    private Vector2 finalTouchPos;
    private Vector2 tempPos;
    //set Angle
    public float swipeAngle = 0f;
    public float swipeResist = 1f;


    // Start is called before the first frame update
    void Start()
    {
        //use method form Script Ctrl_Board
        board = FindObjectOfType<Ctrl_Board>();

        //targetX = (int)transform.position.x;
        //targetY = (int)transform.position.y;
        //row = targetY;
        //column = targetX;
        //beforeColumn = column;
        //beforeRow = row;
    }

    // Update is called once per frame
    void Update()
    {
        //use Method FindMatch()
        FindMatch();
        //Check Matches is true
        if (isMatch == true)
        {
            //Set SpriteRenderer
            SpriteRenderer mySprite = GetComponent<SpriteRenderer>();
            //Black
            mySprite.color = new Color(1f, 1f, 1f, .2f);
        }
        //Set values equal
        targetX = column;
        targetY = row;

        //Check Movement axis X
        if (Mathf.Abs(targetX - transform.position.x) > .1f)
        {
            //move Toward the target
            tempPos = new Vector2(targetX, transform.position.y);
            transform.position = Vector2.Lerp(transform.position, tempPos, .1f);
            if (board.allGems[column,row] !=  this.gameObject)
            {
                board.allGems[column, row] = this.gameObject;  
            }
        }
        else
        {
            //Diractly set the Pos
            tempPos = new Vector2(targetX, transform.position.y);
            transform.position = tempPos;
        }
        //Check Movement axis Y
        if (Mathf.Abs(targetY - transform.position.y) > .1f)
        {
            //move Toward the target
            tempPos = new Vector2(transform.position.x, targetY);
            transform.position = Vector2.Lerp(transform.position, tempPos, .1f);
            if (board.allGems[column, row] != this.gameObject)
            {
                board.allGems[column, row] = this.gameObject;
            }
        }
        else
        {
            //Diractly set the Pos
            tempPos = new Vector2(transform.position.x, targetY);
            transform.position = tempPos;
        }
    }
    //IEnumerator for Method CheckMove
    public IEnumerator CheckMove()
    {
        yield return new WaitForSeconds(.5f);
        //Check Gem Not equal to null
        if (otherGem != null)
        {
            //Check Matches
            if (!isMatch && !otherGem.GetComponent<Ctrl_Gem>().isMatch)
            {
                //Still not recover
                otherGem.GetComponent<Ctrl_Gem>().column = column;
                otherGem.GetComponent<Ctrl_Gem>().row = row;

                column = beforeColumn;
                row = beforeRow;

                yield return new WaitForSeconds(5f);
                board.currentState = GameState.move;
            }
            else
            {
                //if Matches are destroyed
                board.DestroyGem();
            }
            //if otherGem Not equal to null. Let it be equal to null
            otherGem = null;
        }
    }
    
    //Method Swipe Down
    private void OnMouseDown()
    {
        //can move
        if (board.currentState == GameState.move)
        {
            //position touch
            firstTouchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
    }
    //Method Swipe Up
    private void OnMouseUp()
    {
        //can move
        if (board.currentState == GameState.move)
        {
            //position touch
            finalTouchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            //use method CalculateAngle()
            CalculateAngle();
        }
    }
    //Method set CalculateAngle
    void CalculateAngle()
    {
        if (Mathf.Abs(finalTouchPos.y - firstTouchPos.y) > swipeResist || Mathf.Abs(finalTouchPos.x - firstTouchPos.x) > swipeResist)
        {
            //Limited exposure settings
            swipeAngle = Mathf.Atan2(finalTouchPos.y - firstTouchPos.y, finalTouchPos.x - firstTouchPos.x) * 180 / Mathf.PI;
            ////use method MoveGem()
            MoveGem();
            //can't move keep waiting when metches
            board.currentState = GameState.wait;
        }
        else
        {
            //can move
            board.currentState = GameState.move;
        }
    }
    //Method check MoveGem
    void MoveGem()
    {
        //RightSwipe
        if (swipeAngle > -45 && swipeAngle <= 45 && column < board.width-1)
        {
            otherGem = board.allGems[column + 1 , row];
            beforeColumn = column;
            beforeRow = row;
            otherGem.GetComponent<Ctrl_Gem>().column -= 1;
            column += 1;
        }

        //UpSwipe
        else if (swipeAngle > 45 && swipeAngle <= 135 && row < board.height -1)
        {
            otherGem = board.allGems[column, row + 1];
            beforeColumn = column;
            beforeRow = row;
            otherGem.GetComponent<Ctrl_Gem>().row -= 1;
            row += 1;
        }

        //LeftSwipe
        else if ((swipeAngle > 135 || swipeAngle <= -135) && column > 0)
        {

            otherGem = board.allGems[column - 1, row];
            beforeColumn = column;
            beforeRow = row;
            otherGem.GetComponent<Ctrl_Gem>().column += 1;
            column -= 1;
        }

        //DownSwipe
        else if (swipeAngle < -45 && swipeAngle >= -135 && row > 0)
        {
            otherGem = board.allGems[column, row - 1];
            beforeColumn = column;
            beforeRow = row;
            otherGem.GetComponent<Ctrl_Gem>().row += 1;
            row -= 1;
        }
        StartCoroutine(CheckMove());
    }
    //method Find the Matches
    void FindMatch()
    {
        //Check Matches for column , axis y
        if (column > 0 && column < board.width - 1)
        {
            GameObject leftDot1 = board.allGems[column - 1, row];
            GameObject rightDot1 = board.allGems[column + 1, row];
            if (leftDot1 != null && rightDot1 != null)
            {
                if (leftDot1.tag == this.gameObject.tag && rightDot1.tag == this.gameObject.tag)
                {
                    //bool isMatch true
                    leftDot1.GetComponent<Ctrl_Gem>().isMatch = true;
                    rightDot1.GetComponent<Ctrl_Gem>().isMatch = true;
                    isMatch = true;
                }
            }

        }
        //Check Matches for row , axis x
        if (row > 0 && row < board.height - 1)
        {
            GameObject upDot1 = board.allGems[column, row - 1];
            GameObject downDot1 = board.allGems[column, row + 1];
            if (upDot1 != null && downDot1 != null)
            {
                if (upDot1.tag == this.gameObject.tag && downDot1.tag == this.gameObject.tag)
                {
                    //bool isMatch true
                    upDot1.GetComponent<Ctrl_Gem>().isMatch = true;
                    downDot1.GetComponent<Ctrl_Gem>().isMatch = true;
                    isMatch = true;
                }
            }
        }
    }
}
