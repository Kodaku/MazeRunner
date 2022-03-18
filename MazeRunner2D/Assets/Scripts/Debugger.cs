using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Debugger : MonoBehaviour
{
    private float scaledQ;
    public GameObject valueUp;
    public GameObject valueDown;
    public GameObject valueLeft;
    public GameObject valueRight;
    private Dictionary<Vector2, GameObject> valueBreadcrumbs = new Dictionary<Vector2, GameObject>();

    public void SetColors(float currentQ, float minQValue, float maxQValue)
    {
        scaledQ = (currentQ - minQValue) / (maxQValue - minQValue + Mathf.Epsilon);
    }

    public void SpawnMove(Player player, MyGrid grid)
    {
        // print(currentQ);
        Action playerAction = player.GetCurrentAction();
        Cell playerCell = grid.WorldPointToCell(player.transform.position);
        Vector2 playerPosition = new Vector2(playerCell.column, playerCell.row);
        switch(playerAction)
        {
            case Action.UP:
            {
                if(valueBreadcrumbs.ContainsKey(playerPosition))
                {
                    Destroy(valueBreadcrumbs[playerPosition]);
                    valueBreadcrumbs.Remove(playerPosition);
                }
                GameObject newArrow = Instantiate(valueUp, playerPosition, Quaternion.identity);
                newArrow.transform.parent = transform;
                SpriteRenderer spriteRenderer = newArrow.GetComponent<SpriteRenderer>();
                spriteRenderer.color = new Color(scaledQ, scaledQ, 0.0f);
                valueBreadcrumbs.Add(playerPosition, newArrow);
                break;
            }
            case Action.DOWN:
            {
                if(valueBreadcrumbs.ContainsKey(playerPosition))
                {
                    Destroy(valueBreadcrumbs[playerPosition]);
                    valueBreadcrumbs.Remove(playerPosition);
                }
                GameObject newArrow = Instantiate(valueDown, playerPosition, Quaternion.identity);
                newArrow.transform.parent = transform;
                SpriteRenderer spriteRenderer = newArrow.GetComponent<SpriteRenderer>();
                spriteRenderer.color = new Color(scaledQ, scaledQ, 0.0f);
                valueBreadcrumbs.Add(playerPosition, newArrow);
                break;
            }
            case Action.LEFT:
            {
                if(valueBreadcrumbs.ContainsKey(playerPosition))
                {
                    Destroy(valueBreadcrumbs[playerPosition]);
                    valueBreadcrumbs.Remove(playerPosition);
                }
                GameObject newArrow = Instantiate(valueLeft, playerPosition, Quaternion.identity);
                newArrow.transform.parent = transform;
                SpriteRenderer spriteRenderer = newArrow.GetComponent<SpriteRenderer>();
                spriteRenderer.color = new Color(scaledQ, scaledQ, 0.0f);
                valueBreadcrumbs.Add(playerPosition, newArrow);
                break;
            }
            case Action.RIGHT:
            {
                if(valueBreadcrumbs.ContainsKey(playerPosition))
                {
                    Destroy(valueBreadcrumbs[playerPosition]);
                    valueBreadcrumbs.Remove(playerPosition);
                }
                GameObject newArrow = Instantiate(valueRight, playerPosition, Quaternion.identity);
                newArrow.transform.parent = transform;
                SpriteRenderer spriteRenderer = newArrow.GetComponent<SpriteRenderer>();
                spriteRenderer.color = new Color(scaledQ, scaledQ, 0.0f);
                valueBreadcrumbs.Add(playerPosition, newArrow);
                break;
            }
        }
    }

    public void Reset()
    {
        foreach(Vector2 value in valueBreadcrumbs.Keys)
        {
            Destroy(valueBreadcrumbs[value]);
        }
        valueBreadcrumbs.Clear();
    }
}
