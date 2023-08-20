using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class TaxTileClass : TileClass
{
    public override void SetUp(string input)
    {
        string[] line = input.Split(',');
        position = int.Parse(line[0]);
        name = line[1];
        tileName = line[1];
        taxNameText.text = line[1];
        pay = int.Parse(line[3])*-1;
        valueText.text = "Pay " + pay;
        purchasable = false;
        tileType = line[13];
    }

    //a text object which displays the name of the tax on the tile
    public Text taxNameText;
    //a text object which displays the value of the tax on the tile
    public Text valueText;
}
