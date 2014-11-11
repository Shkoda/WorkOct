using UnityEngine;
using System.Collections;

public class FieldBuilder : MonoBehaviour
{
    public GameObject ParentContainer;
    public GameObject BorderBlock;
//    public int TopLeftX, TopLeftY, FieldWidth, FieldHeight, WallWidth;


    // Use this for initialization
    private void Start()
    {
        int size = 10;
        int halfSize = size/2;
        //vertical
        for (int i = -halfSize; i <= halfSize; i++)
        {
            AddBlock(-halfSize, i);
            AddBlock(halfSize, i);
        }

        //horizontal
        for (int i = - halfSize+1; i <=  halfSize-1; i++)
        {
            AddBlock(i, -halfSize);
            AddBlock(i, halfSize);
        }
    }

    // Update is called once per frame
    private void Update()
    {
    }

    private void AddBlock(float x, float y)
    {
        var tile = (GameObject) Instantiate(BorderBlock);
        tile.transform.position = new Vector3(x, y, 0);
        tile.transform.parent = ParentContainer.transform;
    }
}