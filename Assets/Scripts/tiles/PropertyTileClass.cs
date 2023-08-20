using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class PropertyTileClass : TileClass
{
    public override void SetUp(string input) //gets starting tile info
    {
        mortgaged = false;
        otherPropertiesInGroup = new List<TileClass>();
        string[] line = input.Split(',');
        position = int.Parse(line[0]);
        name = line[1];
        tileName = line[1];
        propertyNameText.text = line[1];
        price = int.Parse(line[5]);
        owner = null;
        switch (line[4])
        {
            case "Yes":
                purchasable = true;
                break;
            case "No":
                purchasable = false;
                break;
            default:
                break;
        }
        houseNo = 0;
        rents = new int[6];
        for(int i = 6; i < 12; i++)
        {
            rents[i-6] = int.Parse(line[i]);
        }
        upgrade = int.Parse(line[12]);
        color = line[2];

        //sets the colour of the tile object to match the colour given
        switch (line[2])
        {
            case "Brown":
                colourBlock.GetComponent<Renderer>().material.SetColor("_Color", new Color(139f / 255f, 69f / 255f, 19f / 255f));
                break;
            case "Blue":
                colourBlock.GetComponent<Renderer>().material.SetColor("_Color", Color.blue);
                break;
            case "Purple":
                colourBlock.GetComponent<Renderer>().material.SetColor("_Color", new Color(128f/255f, 0, 128f/255f));
                break;
            case "Orange":
                colourBlock.GetComponent<Renderer>().material.SetColor("_Color", new Color(255f/255f, 165f/255f, 0));
                break;
            case "Red":
                colourBlock.GetComponent<Renderer>().material.SetColor("_Color", Color.red);
                break;
            case "Yellow":
                colourBlock.GetComponent<Renderer>().material.SetColor("_Color", Color.yellow);
                break;
            case "Green":
                colourBlock.GetComponent<Renderer>().material.SetColor("_Color", Color.green);
                break;
            case "Deep blue":
                colourBlock.GetComponent<Renderer>().material.SetColor("_Color", new Color(0, 0, 128f/255f));
                break;
            default:
                break;
        }
        tileType = line[13];
    }

    //a text object which displays the name of the property on the tile
    public Text propertyNameText;
    //a gameobject which displays the colour of the property on the board
    public GameObject colourBlock;
}
