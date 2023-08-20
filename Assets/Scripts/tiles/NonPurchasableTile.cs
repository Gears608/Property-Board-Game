using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NonPurchasableTile : TileClass
{
    public override void SetUp(string input)
    {
        string[] line = input.Split(',');
        string tileText = line[1].Split('/')[0];
        this.GetComponentInChildren<Text>().text = tileText;
        position = int.Parse(line[0]);
        name = line[1];
        tileName = line[1];
        tileType = line[13];
        purchasable = false;
        if(tileType == "Jail")
        {
            pay = 50;
        }
    }
}
