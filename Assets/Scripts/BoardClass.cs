using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class BoardClass : MonoBehaviour
{
    private Transform[] children;
    public List<Transform> tileTransformList = new List<Transform>();  //list which contains the transforms of all the board tiles

    private List<string> opKnocksCards = new List<string>();
    private List<string> potluckCards = new List<string>();

    public List<PlayerClass> players;

    private int turn;
    private int turns;
    private int extraTurn;
    private int finesValue;

    //Tile prefabs
    public GameObject propertyTile;
    public GameObject goTile;
    public GameObject stationTile;
    public GameObject taxTile;
    public GameObject cardTile;
    public GameObject utilityTile;
    public GameObject toJailTile;
    public GameObject jailTile;
    public GameObject finesTile;

    //player prefab
    public GameObject[] playerPrefabs;

    //UI elements
    public GameObject diceButton;
    public GameObject propertyTileUI;
    public GameObject textboxUI;
    public GameObject taxUI;
    public Text[] playerDisplay;
    public GameObject playerDisplayButton;
    public GameObject propertyDisplay;
    public GameObject propertyButtonPrefab;
    public GameObject personalPropertyUI;
    public GameObject endTurnButton;
    public GameObject auctionUI;
    public Text diceText;
    public GameObject loseUI;
    public GameObject setupUI;
    public GameObject timeUI;
    public GameObject playerPanel;

    private List<PlayerClass> auctionPlayers = new List<PlayerClass>();
    private int auctionAmount = 0;
    private int auctionPointer;

    private bool moving;  //bool if the player is currently in transit

    private bool start;
    private bool timed;
    private float timeLimit;

    private List<GameObject> playerPanels = new List<GameObject>();

    /// <summary>
    /// 
    /// A method that runs as soon as the script is run
    /// 
    /// </summary>
    void Start()
    {
        //initialises values
        turn = 0;
        turns = 1;
        finesValue = 0;
        extraTurn = 0;
        timeLimit = 1;
        moving = false;
        start = false;
        timed = false;

        //reads data from files
        createBoard(File.ReadAllLines("Assets/PropertyTycoonBoardData.csv"));
        sortCards(File.ReadAllLines("Assets/PropertyTycoonCardData.csv"));

        //displays the setup ui to the players
        setupUI.SetActive(true);
        setupUI.transform.GetChild(0).gameObject.SetActive(true);
        setupUI.transform.GetChild(1).gameObject.SetActive(false);
    }

    /// <summary>
    /// 
    /// A method that runs for every frame that the script is enabled
    /// 
    /// </summary>
    void Update()
    {
        if(start == true)
        {
            //updates the players totals
            for (int i = 0; i < players.Count; i++)
            {
                playerDisplay[i].text = players[i].playerName + ": " + players[i].money;
            }

            //updates the time limit
            if (timed == true)
            {
                //checks if the time is up
                if (timeLimit - Time.deltaTime > 0)
                {
                    timeLimit -= Time.deltaTime;
                    timeUI.transform.GetChild(0).GetComponent<Text>().text = "Time: " + Mathf.FloorToInt(timeLimit / 60) + ":" + Mathf.FloorToInt(timeLimit % 60);
                }
                else
                {
                    //sets the timelimit to 0 and changes the time ui to show as 0
                    timeUI.transform.GetChild(0).GetComponent<Text>().text = "Time: 0";
                    timeLimit = 0;
                }
        }
        }
    }

    /// <summary>
    /// 
    /// A method which controls the timed button which enables the timed gamemode.
    /// 
    /// </summary>
    public void timedButton()
    {
        //sets the mode to timed
        timed = true;
        //sets the ui to the player select screen
        setupUI.transform.GetChild(0).gameObject.SetActive(false);
        setupUI.transform.GetChild(1).gameObject.SetActive(true);
        //adds the time selection to the menu
        setupUI.transform.GetChild(1).Find("TimeLimit").gameObject.SetActive(true);
    }

    /// <summary>
    /// 
    /// A method which controls the classic gamemode button which enables the classic gamemode.
    /// 
    /// </summary>
    public void classicButton()
    {
        //swaps the ui to the player select screen
        setupUI.transform.GetChild(0).gameObject.SetActive(false);
        setupUI.transform.GetChild(1).gameObject.SetActive(true);
    }

    /// <summary>
    /// 
    /// A method which controls a button which allows the user to add a new player to the list
    /// 
    /// </summary>
    public void addPlayerButton()
    {
        //checks if the maximum number of players is reached
        if (playerPanels.Count < 6)
        {
            //instantiates a new player panel and adds it to the list
            GameObject newPlayer = Instantiate(playerPanel);
            newPlayer.GetComponentInChildren<Text>().text = "Player " + (playerPanels.Count + 1);
            newPlayer.transform.SetParent(setupUI.transform.GetChild(1).GetChild(1).GetChild(0).GetChild(0));
            playerPanels.Add(newPlayer);
        }
        else
        {
            //displays an error to the user informing them that the maximum number of players is reached
            textboxUI.gameObject.transform.Find("Text").GetComponent<Text>().text = "Maximum number of players reached.";
            textboxUI.SetActive(true);
        }
    }

    /// <summary>
    /// 
    /// A method which controls a button which allows the user to remove a player from the list
    /// 
    /// </summary>
    public void removePlayerButton()
    {
        //checks if the list is empty
        if(playerPanels.Count == 0)
        {
            //displays an error to the user informing them that there are no players in the list
            textboxUI.gameObject.transform.Find("Text").GetComponent<Text>().text = "No players to remove.";
            textboxUI.SetActive(true);
        }
        else
        {
            //removes a player from the players list
            GameObject removePlayer = playerPanels[playerPanels.Count -1];
            playerPanels.Remove(removePlayer);
            Destroy(removePlayer);
        }
    }

    /// <summary>
    /// 
    /// A method which controls a button whick allows the player to navigate back to the mode select menu
    /// 
    /// </summary>
    public void setupBackButton()
    {
        //swaps the ui back to the mode select screen
        setupUI.transform.GetChild(0).gameObject.SetActive(true);
        setupUI.transform.GetChild(1).gameObject.SetActive(false);

        timed = false;

        //removes the time selection to the menu
        setupUI.transform.GetChild(1).Find("TimeLimit").gameObject.SetActive(false);
    }

    /// <summary>
    /// 
    /// A method which controls a button which starts the game from the player select screen with some basic checking to ensure the variables are correct
    /// 
    /// </summary>
    public void doneSetupButton()
    {
        //checks that there are at least 2 players
        if (playerPanels.Count >= 2)
        {
            //checks if the mode is set to timed
            if (timed == true)
            {
                //checks that there is a value given for the time limit
                if (setupUI.transform.GetChild(1).Find("TimeLimit").Find("InputField").Find("Text").gameObject.GetComponent<Text>().text == "")
                {
                    //displays an error to the user asking them to input a time limit
                    textboxUI.gameObject.transform.Find("Text").GetComponent<Text>().text = "Please enter a value for time limit.";
                    textboxUI.SetActive(true);
                    return;
                }
                else
                {
                    //the time limit is set
                    timeLimit = float.Parse(setupUI.transform.GetChild(1).Find("TimeLimit").Find("InputField").Find("Text").gameObject.GetComponent<Text>().text.ToString()) * 60;
                    //the ui to display the time remaining is initialised and acivated
                    timeUI.transform.GetChild(0).GetComponent<Text>().text = "Time: " + (timeLimit / 60);
                    timeUI.SetActive(true);
                }
            }

            //calls a method to create the player pieces
            setPlayers(playerPanels.Count);
            //activates the first players camera
            players[turn].gameObject.GetComponentInChildren<Camera>().enabled = true;

            //starts the game
            start = true;
            //hides the setup ui
            setupUI.SetActive(false);
            //activates the roll button for the first player
            if (players[turn].isAI)
            {
                aiRollTurn();
            }
            else
            {
                diceButton.SetActive(true);
            }
        }
        else
        {
            //displays an error to the user informing them that there are not enough players in the list
            textboxUI.gameObject.transform.Find("Text").GetComponent<Text>().text = "Not enough players to start game.";
            textboxUI.SetActive(true);
        }
    }

    /// <summary>
    /// 
    /// A method which instantiates all the cards based on the data read from the card data file
    /// 
    /// </summary>
    /// <param name="lines"> the card data as strings </param>
    private void sortCards(string[] lines)
    {
        foreach(string line in lines)
        {
            //checks if the card is a potluck card
            if(line.Split(',')[2] == "Potluck")
            {
                //adds it to the potluck array
                potluckCards.Add(line);
            }
            //else assumes it is an opportunity knocks card
            else
            {
                //adds it to the opKnocks array
                opKnocksCards.Add(line);
            }
        }
    }

    /// <summary>
    /// 
    /// A method which instantiates all the tiles based on the data read from the board data file
    /// 
    /// </summary>
    /// <param name="lines"> the board data as strings </param>
    private void createBoard(string[] lines)
    {

        //creates an array of strings to read the board data into
        string[,] boardData = new string[40, 14];

        //loops over the file lines and splits it into lines
        for (int i = 0; i < 40; i++)
        {
            string[] line = lines[i].Split(',');

            //loops over the lines and adds them to the board data array
            for (int j = 0; j < 14; j++)
            {
                boardData[i, j] = line[j];
            }
        }

        children = this.gameObject.GetComponentsInChildren<Transform>();  //adds all the children's transforms from heiarchy to array

        int x = 0;

        //loops over all the children
        foreach(Transform child in children)
        {
            if (x > 0)
            {
                //insantiates the correct board tile at that position based on the type indicated in the array
                switch (boardData[x-1, 13])
                {
                    case "Go":
                        children[x] = Instantiate(goTile, child.position, child.rotation, this.gameObject.transform).transform;
                        break;
                    case "Property":
                        children[x] = (Instantiate(propertyTile, child.position, child.rotation, this.gameObject.transform)).transform;
                        break;
                    case "Station":
                        children[x] = Instantiate(stationTile, child.position, child.rotation, this.gameObject.transform).transform;
                        break;
                    case "Utility":
                        children[x] = Instantiate(utilityTile, child.position, child.rotation, this.gameObject.transform).transform;
                        break;
                    case "Tax":
                        children[x] = Instantiate(taxTile, child.position, child.rotation, this.gameObject.transform).transform;
                        break;
                    case "Card":
                        children[x] = Instantiate(cardTile, child.position, child.rotation, this.gameObject.transform).transform;
                        break;
                    case "Jail":
                        children[x] = Instantiate(jailTile, child.position, child.rotation, this.gameObject.transform).transform;
                        break;
                    case "ToJail":
                        children[x] = Instantiate(toJailTile, child.position, child.rotation, this.gameObject.transform).transform;
                        break;
                    case "Fines":
                        children[x] = Instantiate(finesTile, child.position, child.rotation, this.gameObject.transform).transform;
                        break;
                    default:
                        break;
                }
                //destroys the placeholder tiles
                Destroy(child.gameObject);
                //calls a method in the tileclass script to to input data from the board class array into the tiles
                children[x].GetComponent<TileClass>().SetUp(lines[x - 1]);
            }
            x++;
        }

        tileTransformList.Clear();  //ensures the list is clear

        //loops over all the children in the new array
        foreach (Transform child in children)
        {
            if (child != transform)
            {
                tileTransformList.Add(child);  //adds all the children from the array to the list
            }
            if (child != children[0])
            {
                //checks if the tile is purchasable
                if (child.GetComponent<TileClass>().purchasable == true)
                {
                    //adds the properties to the properties scrolling display
                    GameObject button = Instantiate(propertyButtonPrefab);
                    button.transform.SetParent(propertyDisplay.transform.GetChild(0).GetChild(0).transform);
                    button.GetComponentInChildren<Text>().text = child.GetComponent<TileClass>().tileName;
                    button.GetComponent<Image>().color = child.gameObject.GetComponentInChildren<Renderer>().material.GetColor("_Color");
                    button.GetComponent<Image>().color = new Color(button.GetComponent<Image>().color.r, button.GetComponent<Image>().color.g, button.GetComponent<Image>().color.b, 0.6f);
                    button.name = child.name;
                    button.GetComponent<Button>().onClick.RemoveAllListeners();
                    button.GetComponent<Button>().onClick.AddListener(delegate { displayPropertyButton(this.transform.Find(button.name).GetComponent<TileClass>()); });
                    //sets them to false so they are not visable to the player
                    button.gameObject.SetActive(false);

                    //loops through to group properties together
                    foreach (Transform child1 in children)
                    {
                        if (child1 != children[0])
                        {
                            if (child != child1)
                            {
                                if (child.GetComponent<TileClass>().tileType == child1.GetComponent<TileClass>().tileType)
                                {
                                    if (child.GetComponent<TileClass>().tileType == "Property")
                                    {
                                        if (child.GetComponent<TileClass>().color == child1.GetComponent<TileClass>().color)
                                        {
                                            //groups properties of the same colour together
                                            child.GetComponent<TileClass>().otherPropertiesInGroup.Add(child1.GetComponent<TileClass>());
                                        }
                                    }
                                    else
                                    {
                                        //groups utility and station tiles together
                                        child.GetComponent<TileClass>().otherPropertiesInGroup.Add(child1.GetComponent<TileClass>());
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

    }

    /// <summary>
    /// 
    /// A method which creates and sets the players
    /// 
    /// </summary>
    /// <param name="count"> number of players </param>
    private void setPlayers(int count)
    {
        for(int i = 0; i < count; i++)
        {
            playerDisplay[i].gameObject.transform.parent.gameObject.SetActive(true);
            //instantiates a new player
            GameObject player = Instantiate(playerPrefabs[i]);
            //moves the player to the starting position
            player.transform.position = tileTransformList[0].position;
            //sets up the playerclass component
            player.GetComponent<PlayerClass>().setUp("Player " + (i + 1), playerPanels[i].transform.Find("Toggle").GetComponent<Toggle>().isOn);
            
            //adds the playerclass component to the players array
            players.Add(player.GetComponent<PlayerClass>());
        }
    }

    /// <summary>
    /// 
    /// A method which moves the player a set number of spaces taken as an input
    /// 
    /// </summary>
    /// <param name="spaces"> the number of spaces the player should move </param>
    /// <returns> null or wait while the player is moving </returns>
    private IEnumerator move(int spaces)
    {
        int boardPosition = players[turn].position; //gets the players starting position

        if (moving)
        {
            yield break;
        }

        moving = true;

        while (spaces > 0)
        {
            Vector3 nextTilePos;  //creates an empty vector which will store the position of the next tile to visit
            boardPosition++;
            if (boardPosition > 39)
            {
                nextTilePos = tileTransformList[0].position;  //sets the first tile as the next position restarts the loop round the board when the player reaches the end of the board
                players[turn].canPurchase = true;
                boardPosition = 0;
            }
            else
            {
                nextTilePos = tileTransformList[boardPosition].position;   //sets the position of the next tile
            }
            if (tileTransformList[boardPosition].gameObject.GetComponent<TileClass>().tileType == "Go")
            {
                players[turn].money += 200;
            }

            while (goToNode(nextTilePos))   //calls the method which moves the player
            {
                yield return null;  //waits while the player is moving
            }
            spaces--;
            if (boardPosition % 10 == 0)
            {
                while (rotatePlayer(tileTransformList[boardPosition + 1].transform.rotation))
                {
                    yield return null;
                }
            }
            yield return new WaitForSeconds(0.075f);  //pauses on each tile for visual effect

        }
        moving = false;
        players[turn].position = boardPosition;  //stores the players new position in its script
        TileClass currentTile = tileTransformList[players[turn].position].gameObject.GetComponent<TileClass>();

        //checks if the owner is the current player
        if (currentTile.owner == players[turn])
        {
            endTurnButton.SetActive(true);
            displayPropertyButton(currentTile);
        } 
        else if (currentTile.mortgaged != true)
        {
            //displays the ui for the tile the player is currently on
            displayTileUI(currentTile);
        }
        if (players[turn].isAI)
        {
            aiMidTurn();
        }
    }

    /// <summary>
    /// 
    /// A function which rotates the player and returns true if the player is successfully rotated
    /// 
    /// </summary>
    /// <param name="targetRot"> the target rotation of the player </param>
    /// <returns> whether the player makes the target rotation or not </returns>
    private bool rotatePlayer(Quaternion targetRot)
    {
        return targetRot != (players[turn].gameObject.transform.rotation = Quaternion.RotateTowards(players[turn].gameObject.transform.rotation, targetRot, 200f * Time.deltaTime)); //rotates the player 90 degrees over time
    }

    /// <summary>
    /// 
    /// A function which moves the player and returns true if the player is successfully moved
    /// 
    /// </summary>
    /// <param name="nodePos"> the target position of the player </param>
    /// <returns> whether the player makes the target position or not </returns>
    private bool goToNode(Vector3 nodePos)
    {
        return nodePos != (players[turn].gameObject.transform.position = Vector3.MoveTowards(players[turn].gameObject.transform.position, nodePos, 7f * Time.deltaTime));  //moves the player towards its new position
    }

    /// <summary>
    /// 
    /// A method to generate the dice roll when the player presses the roll button and controls players extra turns and jail time
    /// 
    /// </summary>
    public void rollButton()
    {
        int x = UnityEngine.Random.Range(1, 7);
        int y = UnityEngine.Random.Range(1, 7);
        //sets the dice button inactive
        diceButton.SetActive(false);
        //checks if the player is in jail
        if (players[turn].jailTime == 0)
        {
            //player gets an extra turn if a double is rolled
            if (x == y)
            {
                extraTurn++;
                turns++;
            }
            if (turns == 4)
            {
                toJail();
            }
            players[turn].movement = x + y;
            diceText.text = "Roll: " + x + ", " + y;
            //starts the coroutine that moves the player
            StartCoroutine(move( x + y ));
        }
        else
        {
            //reduces the jail time
            players[turn].jailTime--;
        }
        Debug.Log("Button Pressed " + x + "," + y);  //debug msg to show button is working
    }

    /// <summary>
    /// 
    /// A method to control the end of a players turn
    /// 
    /// </summary>
    public void endTurn()
    {
        //disables the camera from the player
        players[turn].gameObject.GetComponentInChildren<Camera>().enabled = false;
        //disables some ui which are related the previous player
        propertyTileUI.SetActive(false);
        taxUI.SetActive(false);
        endTurnButton.SetActive(false);
        closeButton();
        textboxUI.SetActive(false);
        propertyDisplay.SetActive(false);
        //disables all the players properties in the property scroll view
        for (int i = 0; i < players[turn].properties.Count; i++)
        {
            propertyDisplay.transform.GetChild(0).GetChild(0).transform.Find(players[turn].properties[i].tileName).gameObject.SetActive(false);
        }

        //checks if the player has lost the game by ending their turn with less than 0 money
        if (players[turn].money < 0)
        {
            //prepares and enables the loseui
            loseUI.GetComponentInChildren<Text>().text = players[turn].playerName + " you are out.";
            loseUI.transform.Find("Button").GetComponent<Button>().onClick.AddListener(delegate { lose(); });
            loseUI.SetActive(true);
        }
        else
        {
            //increases turn to move to the next player
            if (extraTurn == 0)
            {
                if (turn == players.Count - 1)
                {
                    if (timeLimit == 0)
                    {
                        //end game
                        endTimedGame();
                        return;
                    }
                    else
                    {
                        turn = 0;
                    }
                }
                else
                {
                    turn++;
                }
            }
            else
            {
                extraTurn--;
            }
            turns = 1;
            //reactivates the dice button for the next player
            if (players[turn].isAI)
            {
                aiRollTurn();
            }
            else
            {
                diceButton.SetActive(true);
            }
            //changes the camera to the next player
            players[turn].gameObject.GetComponentInChildren<Camera>().enabled = true;
        }
    }

    /// <summary>
    /// 
    /// A method to handle the end of timed games by calculating the winner from their total value.
    /// 
    /// </summary>
    private void endTimedGame()
    {
        //initialises an array to store the scores of the players
        int[] scores = new int[players.Count];
        //the index of the winner
        int winnerIndex = 0;

        //loops over the players
        for (int i = 0; i < players.Count; i++)
        {
            //integer to store their total value
            int total = 0;
            //adds their money to the total
            total += players[i].money;
            //loops over all the properties in their properties list
            foreach(TileClass property in players[i].properties)
            {
                //checks if the property is morgaged
                if(property.mortgaged == true)
                {
                    //adds half the properties value
                    total += property.price / 2;
                }
                else
                {
                    //adds the value of the property
                    total = property.price;
                }
                //adds the value of all the houses on the property
                total += property.houseNo * property.upgrade;
            }
            //adds the total to the array of totals
            scores[i] = total;
            //checks if it is the highest score
            if(scores[i] > scores[winnerIndex])
            {
                //changes the index of the winner to point to the new highest valued player
                winnerIndex = i;
            }
        }

        //sends the ui to show a win message
        loseUI.GetComponentInChildren<Text>().text = players[winnerIndex].playerName + " you win!";
        loseUI.transform.Find("Button").GetComponent<Button>().onClick.RemoveAllListeners();
        //routes the button to send the game back to the main menu
        loseUI.transform.Find("Button").GetComponent<Button>().onClick.AddListener(delegate { SceneManager.LoadScene("MainMenu"); });
        loseUI.SetActive(true);
    }

    /// <summary>
    /// 
    /// A method to control a player losing the game and dropping out.
    /// 
    /// </summary>
    private void lose()
    {
        loseUI.SetActive(false);
        //gives all their properties back to the bank
        foreach (TileClass property in players[turn].properties)
        {
            property.owner = null;
            property.houseNo = 0;
            property.mortgaged = false;
        }

        //removes them from the turn order
        PlayerClass player = players[turn];
        players.Remove(player);

        //destroys their player piece
        Destroy(player.gameObject);

        //increases turn to move to the next player
        if (extraTurn == 0)
        {
            if (turn == players.Count - 1)
            {
                if (timeLimit == 0)
                {
                    //end game
                    endTimedGame();
                    return;
                }
                else
                {
                    turn = 0;
                }
            }
            else
            {
                turn++;
            }
        }
        else
        {
            extraTurn--;
        }
        turns = 1;

        //checks if there is only one player remaining
        if (players.Count == 1)
        {
            //displays a win message to the player
            loseUI.GetComponentInChildren<Text>().text = players[turn].playerName + " you win!";
            loseUI.transform.Find("Button").GetComponent<Button>().onClick.RemoveAllListeners();
            //routes the button to send the game back to the main menu
            loseUI.transform.Find("Button").GetComponent<Button>().onClick.AddListener(delegate { SceneManager.LoadScene("MainMenu"); });
            loseUI.SetActive(true);
        }
        else
        {
            //reactivates the dice button for the next player
            diceButton.SetActive(true);
            //changes the camera to the next player
            players[turn].gameObject.GetComponentInChildren<Camera>().enabled = true;
        }
    }

    /// <summary>
    /// 
    /// A method to display a UI window to the user for the tile they are on.
    /// Achieved by reading data from the TileClass of the tile.
    /// 
    /// </summary>
    /// <param name="currentTile"> the tile the user is currently on </param>
    private void displayTileUI(TileClass currentTile)
    {
        //gets the text elements of the UI window
        Text[] displayTexts = propertyTileUI.GetComponentsInChildren<Text>();
        if (currentTile.purchasable == true)
        {
            if (players[turn].canPurchase == true)
            {
                if (currentTile.owner != null)
                {
                    if (currentTile.owner.jailTime != 0)
                    {
                        return;
                    }
                }
                //displays the name of the property
                displayTexts[0].text = currentTile.tileName;
                //calls ownedUI to display the correct buttons
                ownedUI(currentTile, displayTexts[3]);
                //changes the property ui depending on which property the user is on
                switch (currentTile.tileType)
                {
                    case "Property":
                        //displays the current rent of the property
                        displayTexts[1].text = "Current rent: " + currentTile.rents[currentTile.houseNo];
                        //displays the current number of houses and cost of houses of the property
                        displayTexts[2].text = "Current houses: " + currentTile.houseNo + "\nHouse cost: " + currentTile.upgrade;
                        //changes the colour of the a panel on the ui to display the colour of the property
                        propertyTileUI.gameObject.transform.Find("ColourPanel").GetComponent<Image>().color = currentTile.gameObject.GetComponentInChildren<Renderer>().material.GetColor("_Color");
                        //changes the opacity of the image to ensure text is readable
                        Color panelColour = propertyTileUI.gameObject.transform.Find("ColourPanel").GetComponent<Image>().color;
                        propertyTileUI.gameObject.transform.Find("ColourPanel").GetComponent<Image>().color = new Color(panelColour.r, panelColour.g, panelColour.b, 0.6f);
                        break;
                    case "Utility":
                        displayTexts[2].text = "If 1 owned, rent equals\n4 times dice roll\n If 2 owned, rent equals\n10 times dice roll";
                        propertyTileUI.gameObject.transform.Find("ColourPanel").GetComponent<Image>().color = new Color(207f, 207f, 207f, 1f);
                        break;
                    case "Station":
                        displayTexts[2].text = "If player owns 1 station, rent is £25\nIf player owns 2 stations, rent is £50\nIf player owns 3 stations, rent is £100\nIf player owns 4 stations, rent is £200";
                        propertyTileUI.gameObject.transform.Find("ColourPanel").GetComponent<Image>().color = new Color(207f, 207f, 207f, 1f);
                        break;
                    default:
                        break;
                }
                //enables the UI window after it is modified to fit the current tile
                propertyTileUI.SetActive(!propertyTileUI.activeSelf);
            }
            else
            {
                endTurnButton.SetActive(true);
            }
        }
        else
        {
            //gives an outcome depending on which type of tile the player is on
            switch (currentTile.tileType) 
            {
                case "Card":
                    //draws a card by calling the drawcard method
                    drawCard(tileTransformList[players[turn].position].GetComponent<TileClass>().tileName);
                    break;
                case "Tax":
                    //displays a tax ui to the player informing them that they must pay
                    taxUI.gameObject.transform.Find("Text").GetComponent<Text>().text = currentTile.tileName + " please pay: " + currentTile.pay;
                    taxUI.SetActive(true);
                    break;
                case "Go":
                    //nothing happens
                    endTurnButton.SetActive(true);
                    break;
                case "Jail":
                    //nothing happens
                    endTurnButton.SetActive(true);
                    break;
                case "Fines":
                    //the value of money currently on the files tile is added to the players total
                    players[turn].money += finesValue;
                    //the value of money currently on the files tile is reset to 0
                    finesValue = 0;
                    endTurnButton.SetActive(true);
                    break;
                case "ToJail":
                    //sends the current player to jail
                    toJail();
                    break;
                default:
                    endTurnButton.SetActive(true);
                    break;
            }
        }
    }

    /// <summary>
    /// 
    /// A method which sends the current player to jail by setting their position there and increaseing the jailtime value for their playerclass
    /// 
    /// </summary>
    public void toJail()
    {
        //increases the players jailtime
        players[turn].jailTime = 3;

        int i = 0;
        //locates the jailtile on the board
        while(i < tileTransformList.Count)
        {
            if(tileTransformList[i].GetComponent<TileClass>().tileType == "Jail")
            {
                //moves the player to the position of the jail by setting its position and rotation and updating its position in the playerclass
                players[turn].gameObject.transform.position = tileTransformList[i].position;
                players[turn].gameObject.transform.rotation = tileTransformList[i].rotation;
                players[turn].position = tileTransformList[i].gameObject.GetComponent<TileClass>().position;
                break;
            }
            i++;
        }

        //checks if the user has a jailfree card
        if (players[turn].jailCards.Count != 0)
        {
            //gives the user the option to use their jailfree card by displaying a ui popup to them
            textboxUI.gameObject.transform.Find("Text").GetComponent<Text>().text = "Use get out of jail free card?";
            textboxUI.transform.Find("OkayButton").gameObject.SetActive(false);
            textboxUI.transform.Find("Option1").gameObject.SetActive(true);
            textboxUI.transform.Find("Option1").GetComponent<Text>().text = "Use";
            textboxUI.transform.Find("Option2").gameObject.SetActive(true);
            textboxUI.transform.Find("Option1").GetComponent<Text>().text = "Don't";
            textboxUI.transform.Find("Option1").GetComponent<Button>().onClick.RemoveAllListeners();
            //delegates the use button to free them from jail
            textboxUI.transform.Find("Option1").GetComponent<Button>().onClick.AddListener(delegate
            {
                if(players[turn].jailCards[0].Split(',')[2] == "Potluck")
                {
                    potluckCards.Add(players[turn].jailCards[0]);
                }
                else
                {
                    opKnocksCards.Add(players[turn].jailCards[0]);
                }
                players[turn].jailCards.RemoveAt(0);
                players[turn].jailTime = 0;
            });
            textboxUI.transform.Find("Option2").GetComponent<Button>().onClick.RemoveAllListeners();
            //delegates the dont use button to hide the textbox and leave the player in jail
            textboxUI.transform.Find("Option2").GetComponent<Button>().onClick.AddListener(delegate
            {
                textboxUI.transform.Find("OkayButton").gameObject.SetActive(true);
                textboxUI.transform.Find("Option1").gameObject.SetActive(false);
                textboxUI.transform.Find("Option2").gameObject.SetActive(false);
                textboxUI.SetActive(false);
            });
            textboxUI.SetActive(true);
        }
        else
        {
            if (players[turn].isAI)
            {
                aiEndTurn();
            }
            else
            {
                //gives the user the option to free themselves from jail by spending £50
                taxUI.gameObject.transform.Find("Text").GetComponent<Text>().text = "Go to jail, pay £50 or end your turn";
                taxUI.SetActive(true);
            }
        }
        //activates the end turn button
        endTurnButton.SetActive(true);
    }

    /// <summary>
    /// 
    /// A method to display the correct UI buttons within the property window
    /// 
    /// </summary>
    /// <param name="currentTile"> the tile the user is currently on </param>
    /// <param name="priceText"> the text which displays the price of the property to the user </param>
    private void ownedUI(TileClass currentTile, Text priceText)
    {
        if (currentTile.owner == null)
        {
            //when a property is not owned, the player has the buttons to buy or put up for auction
            priceText.text = "Price: " + currentTile.price;
            propertyTileUI.gameObject.transform.Find("BuyButton").gameObject.SetActive(true);
            propertyTileUI.gameObject.transform.Find("AuctionButton").gameObject.SetActive(true);
            propertyTileUI.gameObject.transform.Find("RentButton").gameObject.SetActive(false);

        }
        else
        {
            //when a property is owned, the player has the option to pay the rent only
            priceText.text = "Owned by: " + currentTile.owner.playerName;
            propertyTileUI.gameObject.transform.Find("BuyButton").gameObject.SetActive(false);
            propertyTileUI.gameObject.transform.Find("AuctionButton").gameObject.SetActive(false);
            propertyTileUI.gameObject.transform.Find("RentButton").gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// 
    /// A method to buy the property the user is on when the press the buy button if they have the money
    /// 
    /// </summary>
    public void buyButton()
    {
        //checks if the user has enough money
        if (players[turn].money >= tileTransformList[players[turn].position].gameObject.GetComponent<TileClass>().price)
        {
            //minuses the money from the players total
            players[turn].money -= tileTransformList[players[turn].position].gameObject.GetComponent<TileClass>().price;
            //sets the owner of the property
            tileTransformList[players[turn].position].gameObject.GetComponent<TileClass>().owner = players[turn];
            //adds the property to the list of players properties
            players[turn].properties.Add(tileTransformList[players[turn].position].gameObject.GetComponent<TileClass>());
            //closes the ui window
            propertyTileUI.SetActive(false);
            endTurnButton.SetActive(true);
        } 
        else
        {
            //sets the text of the error
            textboxUI.gameObject.transform.Find("Text").gameObject.GetComponentInChildren<Text>().text = "Insufficient funds";
            //displays an error to the user
            textboxUI.SetActive(true);
        }
    }

    /// <summary>
    /// 
    /// A method which controls a button to dismiss a textbox
    /// 
    /// </summary>
    public void okButton()
    {
        textboxUI.SetActive(false);
    }

    /// <summary>
    /// 
    /// A method which controls the button which the player uses to pay tax
    /// 
    /// </summary>
    public void payTaxButton()
    {
        //minuses the value of the tax from the users total
        players[turn].money -= tileTransformList[players[turn].position].gameObject.GetComponent<TileClass>().pay;
        //removes the display
        taxUI.SetActive(false);
        endTurnButton.SetActive(true);
        if(players[turn].jailTime != 0)
        {
            players[turn].jailTime = 0;
        }
    }

    /// <summary>
    /// 
    /// A method which controls the button which displays a list of the current players properties to them
    /// 
    /// </summary>
    public void displayProperties()
    {
        for(int i = 0; i < players[turn].properties.Count; i++)
        {
            GameObject propertyButton = propertyDisplay.transform.GetChild(0).GetChild(0).transform.Find(players[turn].properties[i].tileName).gameObject;
            propertyButton.SetActive(!propertyButton.activeSelf);
        }
        propertyDisplay.SetActive(!propertyDisplay.activeSelf);
    }

    /// <summary>
    /// 
    /// A method which displays the UI of an owned property to the player
    /// 
    /// </summary>
    /// <param name="ownedTile"> the tile to be displayed </param>
    public void displayPropertyButton(TileClass ownedTile) {
        personalPropertyUI.gameObject.transform.Find("Rent").gameObject.SetActive(true);
        //gets the text elements of the UI window
        Text[] displayTexts = personalPropertyUI.GetComponentsInChildren<Text>();
        //displays the name of the property
        displayTexts[0].text = ownedTile.tileName;
        personalPropertyUI.gameObject.transform.Find("MortageButton").GetComponent<Button>().onClick.RemoveAllListeners();
        personalPropertyUI.gameObject.transform.Find("MortageButton").GetComponent<Button>().onClick.AddListener(delegate { mortgageButton(ownedTile); });
        personalPropertyUI.gameObject.transform.Find("SellProperty").GetComponent<Button>().onClick.RemoveAllListeners();
        personalPropertyUI.gameObject.transform.Find("SellProperty").GetComponent<Button>().onClick.AddListener(delegate { sellPropertyButton(ownedTile); });
        switch (ownedTile.tileType)
        {
            case "Property":
                personalPropertyUI.gameObject.transform.Find("Rent").GetComponent<Text>().text = "Rent: " + ownedTile.rents[ownedTile.houseNo];
                personalPropertyUI.gameObject.transform.Find("BuyHouseButton").GetComponent<Button>().onClick.RemoveAllListeners();
                personalPropertyUI.gameObject.transform.Find("BuyHouseButton").GetComponent<Button>().onClick.AddListener(delegate { buyHouseButton(ownedTile); });
                personalPropertyUI.gameObject.transform.Find("SellHouseButton").GetComponent<Button>().onClick.RemoveAllListeners();
                personalPropertyUI.gameObject.transform.Find("SellHouseButton").GetComponent<Button>().onClick.AddListener(delegate { sellHouseButton(ownedTile); });
                personalPropertyUI.gameObject.transform.Find("BuyHouseButton").gameObject.SetActive(true);
                if(ownedTile.houseNo != 0)
                {
                    personalPropertyUI.gameObject.transform.Find("SellHouseButton").gameObject.SetActive(true);
                } else
                {
                    personalPropertyUI.gameObject.transform.Find("SellHouseButton").gameObject.SetActive(false);
                }
                //displays the current rent of the property
                displayTexts[1].text = "Current rent: " + ownedTile.rents[ownedTile.houseNo];
                //displays the current number of houses and cost of houses of the property
                displayTexts[2].text = "Current houses: " + ownedTile.houseNo + "\nHouse cost: " + ownedTile.upgrade;
                //changes the colour of the a panel on the ui to display the colour of the property
                personalPropertyUI.gameObject.transform.Find("ColourPanel").GetComponent<Image>().color = ownedTile.gameObject.GetComponentInChildren<Renderer>().material.GetColor("_Color");
                //changes the opacity of the image to ensure text is readable
                Color panelColour = personalPropertyUI.gameObject.transform.Find("ColourPanel").GetComponent<Image>().color;
                personalPropertyUI.gameObject.transform.Find("ColourPanel").GetComponent<Image>().color = new Color(panelColour.r, panelColour.g, panelColour.b, 0.6f);
                break;
            case "Utility":
                personalPropertyUI.gameObject.transform.Find("Rent").gameObject.SetActive(false);
                personalPropertyUI.gameObject.transform.Find("BuyHouseButton").gameObject.SetActive(false);
                personalPropertyUI.gameObject.transform.Find("SellHouseButton").gameObject.SetActive(false);
                displayTexts[2].text = "If 1 owned, rent equals\n4 times dice roll\n If 2 owned, rent equals\n10 times dice roll";
                personalPropertyUI.gameObject.transform.Find("ColourPanel").GetComponent<Image>().color = new Color(207f, 207f, 207f, 1f);
                break;
            case "Station":
                personalPropertyUI.gameObject.transform.Find("Rent").gameObject.SetActive(false);
                personalPropertyUI.gameObject.transform.Find("BuyHouseButton").gameObject.SetActive(false);
                personalPropertyUI.gameObject.transform.Find("SellHouseButton").gameObject.SetActive(false);
                displayTexts[2].text = "If player owns 1 station, rent is £25\nIf player owns 2 stations, rent is £50\nIf player owns 3 stations, rent is £100\nIf player owns 4 stations, rent is £200";
                personalPropertyUI.gameObject.transform.Find("ColourPanel").GetComponent<Image>().color = new Color(207f, 207f, 207f, 1f);
                break;
            default:
                break;
        }
        //enables the UI window after it is modified to fit the current tile
        personalPropertyUI.SetActive(!personalPropertyUI.activeSelf);
    }

    /// <summary>
    /// 
    /// A method to control the button which closes the property window
    /// 
    /// </summary>
    public void closeButton()
    {
        personalPropertyUI.SetActive(false);
    }

    /// <summary>
    /// 
    /// A method which controls the button which allows the player to buy houses/hotel for their property
    /// 
    /// </summary>
    /// <param name="currentTile"> property for which a house should be bought </param>
    public void buyHouseButton(TileClass currentTile)
    {
        bool allOwned = true;
        bool allUpgraded = true;

        //loops through the other proeprties in the tiles group to check if the player owns them all and if they are upgraded equally
        foreach(TileClass tile in currentTile.otherPropertiesInGroup)
        {
            if(tile.owner != players[turn])
            {
                allOwned = false;
            }
            if(currentTile.houseNo - tile.houseNo > 1)
            {
                allUpgraded = false;
            }
        }

        //if all the houses are upgraded equally
        if (allUpgraded == true)
        {
            //if all the tiles in the group are owned
            if (allOwned == true)
            {
                //if 4 houses are currently owned
                if (currentTile.houseNo <= 4)
                {
                    //if the user has sufficient funds
                    if (players[turn].money >= currentTile.upgrade)
                    {
                        //removes the money from the players total
                        players[turn].money -= currentTile.upgrade;
                        //increases the amount of houses on the tile
                        currentTile.houseNo++;
                        if (currentTile.houseNo == 5)
                        {
                            //loops through and deacivates all the house objects
                            for (int i = 4; i > 0; i--)
                            {
                                currentTile.transform.GetChild(i).gameObject.SetActive(false);
                            }
                            //activates a hotel piece
                            currentTile.transform.GetChild(5).gameObject.SetActive(true);
                        }
                        //activates a house object on the tile
                        currentTile.transform.GetChild(currentTile.houseNo).gameObject.SetActive(true);
                    }
                    else
                    {
                        //displays a message to the user informing them that they do not have enough money to build another house
                        textboxUI.gameObject.transform.Find("Text").gameObject.GetComponentInChildren<Text>().text = "Insufficient funds";
                        textboxUI.SetActive(true);
                    }
                }
                else
                {
                    //displays a message to the user informing them that they have built the maximum amount of houses
                    textboxUI.gameObject.transform.Find("Text").gameObject.GetComponentInChildren<Text>().text = "Maximum houses built";
                    textboxUI.SetActive(true);
                }
            }
            else
            {
                //displays a message to the user informing them that they do not own all the properties in the group
                textboxUI.gameObject.transform.Find("Text").gameObject.GetComponentInChildren<Text>().text = "Not all properties in group owned";
                textboxUI.SetActive(true);
            }
        }
        else
        {
            //displays a message to the user informing them that not all the properties in the group are upgraded equally
            textboxUI.gameObject.transform.Find("Text").gameObject.GetComponentInChildren<Text>().text = "Not all properties in group are upgraded";
            textboxUI.SetActive(true);
        }
    }

    /// <summary>
    /// 
    /// A method which controls a button which allows the player to sell a house/hotel from their property
    /// 
    /// </summary>
    /// <param name="currentTile"> tile from which a house should be sold </param>
    public void sellHouseButton(TileClass currentTile)
    {
        //adds the value of the house to the players total
        players[turn].money += currentTile.upgrade;
        if (currentTile.houseNo == 5)
        {
            //loops through and activates all the house objects
            for (int i = 4; i > 0; i--)
            {
                currentTile.transform.GetChild(i).gameObject.SetActive(true);
            }
            //deactivates a hotel piece
            currentTile.transform.GetChild(5).gameObject.SetActive(false);
        }
        else
        {
            //removes the house from the tile
            currentTile.gameObject.transform.GetChild(currentTile.houseNo).gameObject.SetActive(false);
        }
        //decreases the number of houses
        currentTile.houseNo--;
    }

    /// <summary>
    /// 
    /// A method which controls a button which allows the player to sell their property
    /// 
    /// </summary>
    /// <param name="currentTile"> tile to be sold </param>
    public void sellPropertyButton(TileClass currentTile)
    {
        Debug.Log("Button Pressed");
        //hides the property ui
        personalPropertyUI.SetActive(false);
        //hides the properties list
        propertyDisplay.SetActive(false);
        //if the current tile is mortgaged
        if (currentTile.mortgaged == true)
        {
            //adds half the value to the players total
            players[turn].money += (currentTile.price / 2);
            //unmortgages the property
            currentTile.mortgaged = false;
        }
        else
        {
            //adds the value of the property to the players total
            players[turn].money += currentTile.price;
        }
        //removes the owner from the tile
        currentTile.owner = null;
        //removes the property from the players owned list
        players[turn].properties.Remove(currentTile);
        //hides the button in the properties list
        propertyDisplay.transform.GetChild(0).GetChild(0).transform.Find(currentTile.tileName).gameObject.SetActive(false);
    }

    /// <summary>
    /// 
    /// A method which controls the button which allows the player to mortgage one of their properties
    /// 
    /// </summary>
    /// <param name="currentTile"> the tile to be mortgaged </param>
    public void mortgageButton(TileClass currentTile)
    {
        if(currentTile.mortgaged == false)
        {
            //adds half the value of the property to the players total
            players[turn].money += currentTile.price / 2;
            //mortgages the property
            currentTile.mortgaged = true;
        }
        else
        {
            //adds half the value of the property to the players total
            players[turn].money -= currentTile.price / 2;
            //mortgages the property
            currentTile.mortgaged = false;
        }
    }

    /// <summary>
    /// 
    /// A method which controls the button which allows the player to pay rent to another player of which the property belongs
    /// 
    /// </summary>
    public void payRentButton()
    {
        bool allOwned = true;
        int stationsOwned = 0;

        //checks if all the properties in the group is owned and increases the stationsowned number
        foreach (TileClass tile in tileTransformList[players[turn].position].GetComponent<TileClass>().otherPropertiesInGroup)
        {
            if (tileTransformList[players[turn].position].GetComponent<TileClass>().owner != tile.owner)
            {
                allOwned = false;
            } 
            else
            {
                stationsOwned++;
            }
        }

        //doubles the rent if all properties in the group are owned
        if (allOwned == true && tileTransformList[players[turn].position].GetComponent<TileClass>().houseNo == 0)
        {
            //gives an appropriate outcome based on the type of tile the player lands on
            switch (tileTransformList[players[turn].position].GetComponent<TileClass>().tileType)
            {
                case "Property":
                    //minuses the rent from the non-owner player
                    players[turn].money -= tileTransformList[players[turn].position].GetComponent<TileClass>().rents[tileTransformList[players[turn].position].GetComponent<TileClass>().houseNo] * 2;
                    //adds the rent to the owner player
                    tileTransformList[players[turn].position].GetComponent<TileClass>().owner.money += tileTransformList[players[turn].position].GetComponent<TileClass>().rents[tileTransformList[players[turn].position].GetComponent<TileClass>().houseNo] * 2;
                    break;
                case "Utility":
                    //minuses the rent from the non-owner player
                    players[turn].money -= players[turn].movement * 10;
                    //adds the rent to the owner player
                    tileTransformList[players[turn].position].GetComponent<TileClass>().owner.money += players[turn].movement * 10;
                    break;
                case "Station":
                    //minuses the rent from the non-owner player
                    players[turn].money -= 200;
                    //adds the rent to the owner player
                    tileTransformList[players[turn].position].GetComponent<TileClass>().owner.money += 200;
                    break;
                default:
                    break;
            }
        }
        else
        {
            if (tileTransformList[players[turn].position].GetComponent<TileClass>().tileType == "Station")
            {
                //minuses the rent from the non-owner player
                players[turn].money -= 25 * stationsOwned;
                //adds the rent to the owner player
                tileTransformList[players[turn].position].GetComponent<TileClass>().owner.money += 25 * stationsOwned;
            }
            else if (tileTransformList[players[turn].position].GetComponent<TileClass>().tileType == "Utility")
            {
                //minuses the rent from the non-owner player
                players[turn].money -= players[turn].movement * 4;
                //adds the rent to the owner player
                tileTransformList[players[turn].position].GetComponent<TileClass>().owner.money += players[turn].movement * 4;
            }
            else
            {
                //minuses the rent from the non-owner player
                players[turn].money -= tileTransformList[players[turn].position].GetComponent<TileClass>().rents[tileTransformList[players[turn].position].GetComponent<TileClass>().houseNo];
                //adds the rent to the owner player
                tileTransformList[players[turn].position].GetComponent<TileClass>().owner.money += tileTransformList[players[turn].position].GetComponent<TileClass>().rents[tileTransformList[players[turn].position].GetComponent<TileClass>().houseNo];
            }
        }

        //deactivates the ui
        propertyTileUI.SetActive(false);
        //activates the end turn button
        endTurnButton.SetActive(true);
    }

    /// <summary>
    /// 
    /// A method to setup an auction, is called when the auction button on a property is pressed.
    /// 
    /// </summary>
    public void auctionSetup()
    {

        auctionPointer = 0;
        auctionPlayers.Clear();

        //loops over the players and adds all but the player whos turn it is to the auction turn order
        foreach(PlayerClass player in players)
        {
            if (player != players[turn])
            {
                auctionPlayers.Add(player);
            }
        }

        //activates the auctionui 
        auctionUI.transform.Find("PlayerText").gameObject.GetComponent<Text>().text = auctionPlayers[auctionPointer].playerName;
        auctionUI.transform.Find("PropertyText").gameObject.GetComponent<Text>().text = tileTransformList[players[turn].position].GetComponent<TileClass>().tileName;
        auctionUI.SetActive(true);
        if (auctionPlayers[auctionPointer].isAI)
        {
            aiBid(0, tileTransformList[players[turn].position].GetComponent<TileClass>());
        }
    }

    /// <summary>
    /// 
    /// A method for adding a value to the auction from the auctionui
    /// 
    /// </summary>
    public void addToAuction()
    {       
        int amount;
        //checks if any value was input
        if(auctionUI.transform.Find("InputField").transform.Find("Text").gameObject.GetComponent<Text>().text == "")
        {
            //displays a message to the user asking them to input an amount
            textboxUI.gameObject.transform.Find("Text").gameObject.GetComponentInChildren<Text>().text = "Amount cannot be blank.";
            textboxUI.SetActive(true);
            return;
        }
        else
        {
            //reads the amount added from the input field
            amount = int.Parse(auctionUI.transform.Find("InputField").transform.Find("Text").gameObject.GetComponent<Text>().text.ToString());
        }
        //checks that the input amount is greater than the current bid
        if(amount <= auctionAmount)
        {
            //displays a message to the user informing them that they need to increase their bid
            textboxUI.gameObject.transform.Find("Text").gameObject.GetComponentInChildren<Text>().text = "Amount cannot be less than or equal to current bid.";
            textboxUI.SetActive(true);
        }
        //checks that the player actually has enough money to pay their bid
        else if(amount > auctionPlayers[auctionPointer].money)
        {
            //displays a message to the user informing them that they do not have the funds to bid
            textboxUI.gameObject.transform.Find("Text").gameObject.GetComponentInChildren<Text>().text = "Amount cannot be more than balance.";
            textboxUI.SetActive(true);
        }
        else
        {
            //sets the new auction amount
            auctionAmount = amount;
            //changes the text to display the new price
            auctionUI.transform.Find("CurrentPrice").gameObject.GetComponent<Text>().text = "Current bid: " + amount.ToString();
            //moves the pointer to the next player in the auction turn order
            if(auctionPointer == auctionPlayers.Count -1)
            {
                auctionPointer = 0;
            }
            else
            {
                auctionPointer++;
            }
            if (auctionPlayers[auctionPointer].isAI)
            {
                aiBid(auctionAmount, tileTransformList[players[turn].position].GetComponent<TileClass>());
            }
        }
        //if only one player is left in the auction and they have a non-zero bid
        if (auctionPlayers.Count == 1 && auctionAmount > 0)
        {
            //buys the property
            auctionPlayers[0].money -= auctionAmount;
            auctionPlayers[0].properties.Add(tileTransformList[players[turn].position].GetComponent<TileClass>());
            tileTransformList[players[turn].position].GetComponent<TileClass>().owner = auctionPlayers[0];
            auctionAmount = 0;
            //sets the input field to be blank
            auctionUI.transform.Find("InputField").GetComponent<InputField>().text = "";
            //hides the ui
            propertyTileUI.SetActive(false);
            auctionUI.SetActive(false);
            endTurnButton.SetActive(true);
        }
        else
        {
            auctionUI.transform.Find("PlayerText").gameObject.GetComponent<Text>().text = auctionPlayers[auctionPointer].playerName;
            auctionUI.transform.Find("InputField").GetComponent<InputField>().text = "";
        }
    }

    /// <summary>
    /// 
    /// A method to remove a player from the auction when they press the cancel button on the auctionui.
    /// 
    /// </summary>
    public void removeFromAuction()
    {
        //removes the player from the auction players list
        auctionPlayers.Remove(auctionPlayers[auctionPointer]);
        //moves the pointer to the next player
        if (auctionPointer == auctionPlayers.Count -1)
        {
            auctionPointer = 0;
        }
        else
        {
            auctionPointer++;
        }
        //if only one player is left in the auction and they have a non-zero bid
        if (auctionPlayers.Count == 1 && auctionAmount > 0)
        {
            //buys the property
            auctionPlayers[0].money -= auctionAmount;
            auctionPlayers[0].properties.Add(tileTransformList[players[turn].position].GetComponent<TileClass>());
            tileTransformList[players[turn].position].GetComponent<TileClass>().owner = auctionPlayers[0];
            auctionAmount = 0;
            //sets the input field to be blank
            auctionUI.transform.Find("InputField").GetComponent<InputField>().text = "";
            //hides the ui
            propertyTileUI.SetActive(false);
            auctionUI.SetActive(false);
            endTurnButton.SetActive(true);
        }
        //ends the auction if no bets are made
        else if(auctionPlayers.Count == 0)
        {
            //hides the ui
            propertyTileUI.SetActive(false);
            auctionUI.SetActive(false);
            endTurnButton.SetActive(true);
        }
        else
        {
            auctionUI.transform.Find("PlayerText").gameObject.GetComponent<Text>().text = auctionPlayers[auctionPointer].playerName;
            auctionUI.transform.Find("InputField").GetComponent<InputField>().text = "";
        }
    }

    /// <summary>
    /// 
    /// A method to draw a card from one of the card decks based on the input string.
    /// 
    /// </summary>
    /// <param name="type"> a string detailing the type of card to be drawn; potluck or opknock </param>
    public void drawCard(string type)
    {
        textboxUI.transform.Find("OkayButton").gameObject.SetActive(true);
        textboxUI.transform.Find("Option1").gameObject.SetActive(false);
        textboxUI.transform.Find("Option2").gameObject.SetActive(false);

        string card;
        bool returnCard = true;

        //draws a potluck card
        if(type == "Pot Luck")
        {
            //removes the card from the deck
            card = potluckCards[0];
            potluckCards.Remove(card);
        }
        //draws an opknocks card
        else
        {
            //removes the card from the deck
            card = opKnocksCards[0];
            opKnocksCards.Remove(card);
        }

        //creates an outcome based on the type of card read from the card
        switch (card.Split(',')[3])
        {
            case "collect":
                //adds the money to the players total
                players[turn].money += int.Parse(card.Split(',')[1]);
                endTurnButton.SetActive(true);
                break;
            case "backward":
                //moves the player backwards to the specified tile
                players[turn].transform.position = this.gameObject.transform.Find(card.Split(',')[1]).position;
                players[turn].transform.rotation = this.gameObject.transform.Find(card.Split(',')[1]).rotation;
                players[turn].position = this.gameObject.transform.Find(card.Split(',')[1]).GetComponent<TileClass>().position;
                break;
            case "forward":
                //moves the player forwards to the specified tile
                if(this.gameObject.transform.Find(card.Split(',')[1]).GetComponent<TileClass>().position > players[turn].position)
                {
                    StartCoroutine(move(this.gameObject.transform.Find(card.Split(',')[1]).GetComponent<TileClass>().position - players[turn].position -1));
                }
                else
                {
                    StartCoroutine(move((40 - players[turn].position) + this.gameObject.transform.Find(card.Split(',')[1]).GetComponent<TileClass>().position-1));
                }
                break;
            case "intbackward":
                //moves the player backwards by a set int
                if (players[turn].position - int.Parse(card.Split(',')[1]) < 0)
                {
                    players[turn].transform.position = this.gameObject.transform.GetChild(40 - (int.Parse(card.Split(',')[1]) - players[turn].position)).position;
                    players[turn].transform.rotation = this.gameObject.transform.GetChild(40 - (int.Parse(card.Split(',')[1]) - players[turn].position)).rotation;
                    players[turn].position = 40 - (int.Parse(card.Split(',')[1]) - players[turn].position);
                }
                else
                {
                    players[turn].transform.position = this.gameObject.transform.GetChild(players[turn].position - int.Parse(card.Split(',')[1])).position;
                    players[turn].transform.rotation = this.gameObject.transform.GetChild(players[turn].position - int.Parse(card.Split(',')[1])).rotation;
                    players[turn].position = players[turn].position - int.Parse(card.Split(',')[1]);
                }
                break;
            case "intforward":
                //moves he player forward by a set int
                StartCoroutine(move(int.Parse(card.Split(',')[1])));
                break;
            case "bank":
                //charges the player the correct amount
                players[turn].money -= int.Parse(card.Split(',')[1]);
                endTurnButton.SetActive(true);
                break;
            case "parking":
                //adds the value of the fine to the parking space total and charges the player
                finesValue += int.Parse(card.Split(',')[1]);
                players[turn].money -= int.Parse(card.Split(',')[1]);
                endTurnButton.SetActive(true);
                break;
            case "jail":
                //sends the player whose turn it is to jail
                toJail();
                endTurnButton.SetActive(true);
                break;
            case "playerpay":
                //charges each player in the player list and adds the money to the player whose turn it is
                foreach(PlayerClass player in players)
                {
                    if(player != players[turn])
                    {
                        player.money -= int.Parse(card.Split(',')[1]);
                        players[turn].money += int.Parse(card.Split(',')[1]);
                    }
                }
                endTurnButton.SetActive(true);
                break;
            case "jailfree":
                //gives the player 1 get out of jail free cards and prevents the card from being returned
                players[turn].jailCards.Add(card);
                returnCard = false;
                endTurnButton.SetActive(true);
                break;
            case "repairs":
                //chargees the player the appropriate amount for each hotel and house on all of their properties
                foreach(TileClass property in players[turn].properties)
                {
                    if(property.houseNo == 5)
                    {
                        players[turn].money -= int.Parse(card.Split(',')[1].Split(';')[1]);
                    } 
                    else
                    {
                        players[turn].money -= int.Parse(card.Split(',')[1].Split(';')[0]) * property.houseNo;
                    }
                }
                endTurnButton.SetActive(true);
                break;
            case "choice":
                textboxUI.transform.Find("OkayButton").gameObject.SetActive(false);
                textboxUI.transform.Find("Option1").gameObject.SetActive(true);
                textboxUI.transform.Find("Option1").GetComponent<Text>().text = card.Split(',')[1].Split(';')[0];
                textboxUI.transform.Find("Option1").GetComponent<Button>().onClick.RemoveAllListeners();
                textboxUI.transform.Find("Option1").GetComponent<Button>().onClick.AddListener(delegate { drawCard(card.Split(',')[1].Split(';')[0]); endTurnButton.SetActive(true); });
                textboxUI.transform.Find("Option2").gameObject.SetActive(true);
                textboxUI.transform.Find("Option2").GetComponent<Text>().text = card.Split(',')[1].Split(';')[1];
                textboxUI.transform.Find("Option2").GetComponent<Button>().onClick.RemoveAllListeners();
                textboxUI.transform.Find("Option2").GetComponent<Button>().onClick.AddListener(delegate { pay(int.Parse(card.Split(',')[1].Split(';')[1])); endTurnButton.SetActive(true); });
                break;
        }

        //displays to the user the outcome of the card
        //sets the text of the textbox
        textboxUI.gameObject.transform.Find("Text").gameObject.GetComponentInChildren<Text>().text = card.Split(',')[0];
        //displays an text to the user
        textboxUI.SetActive(true);

        //returns the card to the correct deck
        if (returnCard == true)
        {
            if (type == "Pot Luck")
            {
                potluckCards.Add(card);
            }
            else
            {
                opKnocksCards.Add(card);
            }
        }
    }

    /// <summary>
    /// 
    /// A method which removes a given amount from the users total
    /// 
    /// </summary>
    /// <param name="amount"> the amount to be removed </param>
    public void pay(int amount)
    {
        players[turn].money -= amount;
    }
   

    /// <summary>
    /// 
    /// A method which starts an AI turn with a roll
    /// 
    /// </summary>
    private void aiRollTurn()
    {
        rollButton();
    }

    /// <summary>
    /// 
    /// A method which handles the middle of an AIs turn, checks the tile it has landed on and performs the relevant action
    /// 
    /// </summary>
    private void aiMidTurn()
    {
        TileClass currentTile = tileTransformList[players[turn].position].gameObject.GetComponent<TileClass>();
        if (currentTile.purchasable)
        {
            if (currentTile.owner == null)
            {
                aiBuy();
            }
            else if (currentTile.owner != players[turn])
            {
                payRentButton();
            }
            else
            {
                propertyDisplay.SetActive(false);
            }
        }
        else if (currentTile.tileType == "Tax")
        {
            payTaxButton();
        }
        aiEndTurn();
    }


    /// <summary>
    /// 
    /// A method to handle the end of the AIs turn, pays any rent/tax, decides if it wants to buy houses
    /// If the AI has less than 0 money, in order to not lose the game they will start selling property and houses
    /// 
    /// </summary>
    private void aiEndTurn()
    {
        payRentButton();
        payTaxButton();
        okButton();
        aiBuyHouses();
        if (0 > players[turn].money)
        {
            sellToMake(1 - players[turn].money, true);
        }
    }


    /// <summary>
    /// 
    /// A method to calculate the closest int to a double in order to avoid any possibility of sending a double when an int is required
    /// 
    /// </summary>
    /// <param name="input"> The double that we want to be closest to </param>
    /// <returns></returns>
    private int calNearestInt(double input)
    {
        int startV = 0;
        while (startV < input)
        {
            startV++;
        }
        return startV--;
    }

    /// <summary>
    /// 
    /// A method to determine if the ai will bid and if so how much it will bid
    /// 
    /// </summary>
    /// <param name="curPrice"></param>
    /// <param name="inputTile"></param>
    private void aiBid(int curPrice, TileClass inputTile)
    {
        double[] minCost = new double[] { 0.35, 0.55, 0.7, 0.4, 0.5 };
        int curMoney = players[turn].money;
        TileClass curTile = inputTile;
        int regPrice = inputTile.price;
        int curBid = curPrice;
        int missing = 0;
        //loops through the other proeprties in the tiles group to check if the player owns them all 
        foreach (TileClass tile in curTile.otherPropertiesInGroup)
        {
            if (tile.owner != players[turn])
            {
                missing++;
            }
        }

        //If it doesnt own all the proeprties in the set
        if (missing > 0)
        {
            //Bids x amount based on how many it is missing from set
            if (missing == 2)
            {
                if (curMoney * minCost[1] > curBid)
                {
                    if (curBid < regPrice)
                    {
                        auctionUI.transform.Find("InputField").GetComponent<InputField>().text = regPrice.ToString();
                        addToAuction();
                    }
                    else
                    {
                        int newBid = curBid + 50;
                        auctionUI.transform.Find("InputField").GetComponent<InputField>().text = newBid.ToString(); ;
                        addToAuction();
                    }
                }
                else
                {
                    removeFromAuction();
                }
            }
            if (missing == 1)
            {
                if (curMoney * minCost[2] > curBid)
                {
                    if (curBid < regPrice)
                    {
                        auctionUI.transform.Find("InputField").GetComponent<InputField>().text = regPrice.ToString();
                        addToAuction();
                    }
                    else
                    {
                        int newBid = calNearestInt(curMoney * minCost[2]);
                        newBid++;
                        auctionUI.transform.Find("InputField").GetComponent<InputField>().text = newBid.ToString();
                        addToAuction();
                    }
                }
                else
                {
                    removeFromAuction();
                }
            }
            if (missing >= 3)
            {
                if (curMoney * minCost[0] > curBid)
                {
                    if (curBid < regPrice)
                    {
                        auctionUI.transform.Find("InputField").GetComponent<InputField>().text = regPrice.ToString();
                        addToAuction();
                    }
                    else
                    {
                        int newBid = curBid + 50;
                        auctionUI.transform.Find("InputField").GetComponent<InputField>().text = newBid.ToString();
                        addToAuction();
                    }
                }
                else
                {
                    removeFromAuction();
                }
            }
        }
        else
        {
            removeFromAuction();
        }
    }

   
    
    /// <summary>
    /// 
    /// A method to determine if the AI will buy a property it lands on
    /// 
    /// </summary>
    private void aiBuy()
    {
        int curMoney = players[turn].money;
        double[] minCost = new double[] { 0.35, 0.55, 0.7, 0.4, 0.5 };


        TileClass currentTile = tileTransformList[players[turn].position].gameObject.GetComponent<TileClass>();
        if (currentTile.owner == null)//landed on unowned property
        {
            int curPrice = tileTransformList[players[turn].position].gameObject.GetComponent<TileClass>().price;
            bool allOwned = true;
            int missing = 0;
            //loops through the other proeprties in the tiles group to check if the player owns them all and if they are upgraded equally
            foreach (TileClass tile in currentTile.otherPropertiesInGroup)
            {
                if (tile.owner != players[turn])
                {
                    allOwned = false;
                    missing++;
                }
            }
            //checks if they dont own all in the set
            if (!allOwned)
            {
                //buys the property if it is below a threshold price based on the minCost array
                if (missing == 2)
                {
                    if (curMoney * minCost[1] > curPrice)
                    {
                        buyButton();
                    }
                    else
                    {
                        auctionSetup();
                    }
                }
                if (missing == 1)
                {
                    if (curMoney * minCost[2] > curPrice)
                    {
                        buyButton();
                    }
                    else
                    {
                        auctionSetup();
                    }
                }
                if (missing >= 3)
                {
                    if (curMoney * minCost[0] > curPrice)
                    {
                        buyButton();
                    }
                    else
                    {
                        auctionSetup();
                    }
                }
            }
        }
    }


    /// <summary>
    /// 
    /// A method for the AI to sell however much it needs to make the input amount of money
    /// 
    /// </summary>
    /// <param name="required"> How much money has to be made </param>
    /// <param name="rent"> If the money is for rent or not </param>
    private void sellToMake(int required, bool rent)
    {
        int made = 0;
        while (made < required)
        {
            if (players[turn].properties == null && rent)
            {
                lose();
                aiEndTurn();
            }
            foreach (TileClass owned in players[turn].properties)
            {
                bool allOwned = true;
                foreach (TileClass tile in owned.otherPropertiesInGroup)
                {
                    if (tile.owner != players[turn])
                    {
                        allOwned = false;
                    }
                }
                if(!allOwned)
                {
                    made += (owned.price / 2);
                    sellPropertyButton(owned);
                }
            }
            if (made < required)
            {
                foreach (TileClass owned in players[turn].properties)
                {
                    bool allOwned = true;
                    foreach (TileClass tile in owned.otherPropertiesInGroup)
                    {
                        if (tile.owner != players[turn])
                        {
                            allOwned = false;
                        }
                    }
                    if (allOwned && made < required)
                    {
                        made += owned.upgrade;
                        sellHouseButton(owned);
                    }
                }
            }
        }
    }


    /// <summary>
    /// 
    /// A method to close the card popup for AI 
    /// 
    /// </summary>
    public void cardOK()
    {
        textboxUI.SetActive(false);
        aiEndTurn();
    }

    /// <summary>
    /// 
    /// A method that determines if the AI can buy houses, if it wants to and on which properties
    /// 
    /// </summary>
    private void aiBuyHouses()
    {
        double[] minCost = new double[] { 0.35, 0.55, 0.7, 0.4, 0.5 };
        foreach (TileClass owned in players[turn].properties)
        {
            bool allOwned = true;
            bool allUpgraded = true;
            foreach (TileClass tile in owned.otherPropertiesInGroup)
            {
                if (tile.owner != players[turn])
                {
                    allOwned = false;
                }
                if (owned.houseNo - tile.houseNo > 1)
                {
                    allUpgraded = false;
                }
            }

            if (allOwned && allUpgraded)
            {
                if (owned.houseNo <= 4)
                {
                    if (owned.upgrade < players[turn].money)
                    {
                        buyHouseButton(owned);
                    }
                }
            }
        }
    }

}