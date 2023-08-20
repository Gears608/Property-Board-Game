using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerClass : MonoBehaviour
{
    public void setUp(string playerName, bool ai)
    {
        this.playerName = playerName;
        position = 0;
        jailTime = 0;
        money = 1500;
        properties = new List<TileClass>();
        canPurchase = false;
        isAI = ai;
    }
    public int position { get; set; }
    public bool canPurchase { get; set; }
    public int movement { get; set; }
    public int jailTime { get; set; }
    public bool isAI { get; set; }
    //public int jailFree { get; set; }

    public List<string> jailCards = new List<string>();
    public int money { get; set; }
    public string playerName { get; set; }

    public List<TileClass> properties;
}
