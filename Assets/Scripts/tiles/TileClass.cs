using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TileClass : MonoBehaviour
{
    /// <summary>
    /// 
    /// A method which takes a string as input and splits it, assigning the correct bits of data to the correct variables.
    /// 
    /// </summary>
    /// <param name="input"> a string of data sperated by commas </param>
    public abstract void SetUp(string input);

    /// <summary>
    /// 
    /// Gets and sets a string being the name of the tile
    /// 
    /// </summary>
    public string tileName { get; set; }

    /// <summary>
    /// 
    /// Gets and sets a string being the color of the tile
    /// 
    /// </summary>
    public string color { get; set; }

    // a list containing the classes of the other properties in the proeprties color group
    public List<TileClass> otherPropertiesInGroup;

    /// <summary>
    /// 
    /// Gets and sets an int being the position of the tile on the board
    /// 
    /// </summary>
    public int position { get; set; }

    /// <summary>
    /// 
    /// Gets and sets a bool determining if the property is pruchasable or not
    /// 
    /// </summary>
    public bool purchasable { get; set; }

    /// <summary>
    /// 
    /// Gets and sets a string being the type that the tyle is
    /// 
    /// </summary>
    public string tileType { get; set; }

    /// <summary>
    /// 
    /// Gets and sets an int being the price of the property to buy
    /// 
    /// </summary>
    public int price { get; set; }

    /// <summary>
    /// 
    /// Gets and sets a PlayerClass being the owner of the property
    /// 
    /// </summary>
    public PlayerClass owner { get; set; }

    /// <summary>
    /// 
    /// Gets and sets an int being the number of houses built on the property
    /// 
    /// </summary>
    public int houseNo { get; set; }

    /// <summary>
    /// 
    /// Gets and sets an array of ints being the rents depending on how many houses are built on the property
    /// 
    /// </summary>
    public int[] rents { get; set; }

    /// <summary>
    /// 
    /// Gets and sets an int value being the amount a player owes for landing on this tile
    /// 
    /// </summary>
    public int pay { get; set; }

    /// <summary>
    /// 
    /// Gets and sets an int being the price it costs to build a new house on the property
    /// 
    /// </summary>
    public int upgrade { get; set; }

    /// <summary>
    /// 
    /// Gets and sets a bool being whether the property is currently mortgaged or not
    /// 
    /// </summary>
    public bool mortgaged { get; set; }
}
