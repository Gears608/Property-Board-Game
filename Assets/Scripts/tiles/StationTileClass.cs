using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class StationTileClass : TileClass
{
    public override void SetUp(string input)
    {
        mortgaged = false;
        otherPropertiesInGroup = new List<TileClass>();
        string[] line = input.Split(',');
        name = line[1];
        position = int.Parse(line[0]);
        tileName = line[1];
        stationNameText.text = line[1];
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
        tileType = line[13];
        houseNo = 0;
    }

    //a text object which displays the name of the station on the tile
    public Text stationNameText;
}
