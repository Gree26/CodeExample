using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviour
{
    //Singleton get/set
    public static GameManager Instance { get; private set; }

    //Mesh info
    private MeshGenerator mg;
    private List<Cell> blocks = new List<Cell>();
    public List<GameObject> prefabs = new List<GameObject>();
    public GameObject heighlight;

    //Item id identifier
    Dictionary<int, string> items = new Dictionary<int, string>();

    //User's map id
    private int mapId = 0;

    //Unity Default Update Method
    private void Update()
    {
        mapId = DBManager.mapId;
    }

    //Unity Default Awake Method
    private void Awake()
    {
        //Singleton Pattern
        if (Instance != null)
        {
            Debug.Log("Error: " + this.gameObject + " has a duplicate GameManager Component Attached!");
            Destroy(this);
        }
        //DontDestroyOnLoad(this);  // prevents this from being destroyed on scene load
        // store the singleton reference
        Instance = this;



        mg = this.GetComponent<MeshGenerator>();
    }

    //Unity Default Start Method
    private void Start()
    {
        items.Add(1, "Block");
        
        for(int i = 0; i < prefabs.Count; i++)
        {
            items.Add(i+2, prefabs[i].gameObject.name);
        }

        CallGetMap();
    }

    /// <summary>
    /// Public method so that other methods can get the mesh genererator
    /// </summary>
    /// <returns>The mesh generator</returns>
    public MeshGenerator getMeshGenerator()
    {
        
        return mg;

    }

    
    /// <summary>
    /// Check if an object exists at the given location
    /// if so, exit the method
    /// If not, add it to the list and call the mesh generator
    /// </summary>
    /// <param name="c">Data and location of block/Game object</param>
    public void placeObject(Cell c)
    {
        string type = items[c.Id()];


        //If Block
        if (type=="Block")
        {
            placeBlock(c);
        } else
        {
            for(int i = 0; i < prefabs.Count; i++)
            {
                if (type == prefabs[i].gameObject.name)
                {
                    placeGameObject(c, prefabs[i]);
                }
            }
        }
    }

    /// <summary>
    /// Load a new scene
    /// </summary>
    /// <param name="sc">The name of the scene to be loaded</param>
    public void LoadScene(string sc)
    {
        SceneManager.LoadScene(sc, LoadSceneMode.Single);
        Destroy(GetComponent<Mesh>());
    }

    /// <summary>
    /// Place a block using the given cell data
    /// </summary>
    /// <param name="c"></param>
    private void placeBlock(Cell c)
    {
        Vector3 gridPos = c.Position();

        //If block exists exit method
        for (int i = 0; i < blocks.Count; i++)
        {
            if (blocks[i].Position() == c.Position())
            {
                Debug.Log("Cell Exists at location: " + c.Position());
                return;
            }
        }

        blocks.Add(c);

        //Call the meshgenerator create method
        mg.newCube(gridPos);
    }


    private List<GameObject> createdObjects = new List<GameObject>();
    /// <summary>
    /// 
    /// </summary>
    /// <param name="c"></param>
    /// <param name="g"></param>
    private void placeGameObject(Cell c, GameObject g)
    {
        //If block exists exit method
        for (int i = 0; i < blocks.Count; i++)
        {
            if (blocks[i].Position() == c.Position())
            {
                return;
            }
        }

        blocks.Add(c);

        createdObjects.Add(Instantiate(g,c.Position(),g.transform.rotation));

        heightLights.Add(Instantiate(heighlight, createdObjects[createdObjects.Count - 1].transform));
    }

    /// <summary>
    /// Remove a block from a given position
    /// </summary>
    /// <param name="pos">Remove block from this position</param>
    public void removeObject(Vector3 pos)
    {
        Cell c = new Cell();

        //Parse through the blocks array and remove a block if there exists one at that position
        for(int i = 0; i < blocks.Count; i++)
        {
            if (blocks[i].Position() == pos)
            {
                c = blocks[i];
                break;
            }
        }

        //If found nothing
        if (c.Id() == 0)
            return;

        Debug.Log(c.Id());

        string type = items[c.Id()];


        //If it is a Block
        if (type == "Block")
        {
            removeBlock(c);
        }
        else
        {
            //Delete game object
            for (int i = 0; i < prefabs.Count; i++)
            {
                if (type == prefabs[i].gameObject.name)
                {
                    removeGameObject(c);
                }
            }
        }
    }

    /// <summary>
    /// Remove a game object from the scene based on the given cell
    /// </summary>
    /// <param name="c">The cell that the game object should be removed from</param>
    private void removeGameObject(Cell c)
    {
        for(int i = 0; i < createdObjects.Count; i++)
        {
            //If there exists an object at that position, remove from array and delete game object
            if (createdObjects[i].transform.position==c.Position())
            {
                blocks.Remove(c);
                Destroy(createdObjects[i]);
                createdObjects.RemoveAt(i);
                return;
            }
        }

        //Debug.LogError("Cell Doesnt Exist in this Context! - " + this.gameObject.name);
    }

    /// <summary>
    /// Remove a block when the user decides to
    /// </summary>
    /// <param name="c">The cell from which to remove a block</param>
    private void removeBlock(Cell c)
    {
        //Find the block with the same position
        for (int i = 0; i < blocks.Count; i++)
        {
            if (blocks[i].Position() == c.Position())
            {
                //Clear block from array and then call on the mesh generator to clear the block from the mesh
                blocks.Remove(c);
                mg.removeCube(c.Position());
                return;
            }
        }

        Debug.LogError("Block Doesnt Exist in this Context! - " + this.gameObject.name);
    }

    private List<GameObject> heightLights = new List<GameObject>();
    /// <summary>
    /// Update the height the player is currently interacting with
    /// </summary>
    /// <param name="yHeight">The new height to move to</param>
    public void updateHeight(int yHeight)
    {
        //Clear current highlights 
        for(int i = 0; i < heightLights.Count; i++)
        {
            Destroy(heightLights[i]);
        }

        heightLights.Clear();

        //Populate new highlights 
        for (int i = 0; i < createdObjects.Count; i++)
        {
            if (createdObjects[i].transform.position.y == yHeight)
            {
                heightLights.Add(Instantiate(heighlight,createdObjects[i].transform));
            }

        }
    }

    //Load saved map 
    public void CallGetMap()
    {
        StartCoroutine(GetMap());
    }

    /// <summary>
    /// Get the map from the database by calling on the php communication script, then convert into usable map data
    /// Intended to be called by a Coroutine
    /// </summary>
    /// <returns>IEnumerator - Lets the coroutine know when the method has successfully executed</returns>
    IEnumerator GetMap()
    {
        Debug.Log("User ID: " + DBManager.username + ", Map ID: " + DBManager.mapId);
        WWWForm form = new WWWForm();
        //Get user's map ID from database manager
        form.AddField("mapId", DBManager.mapId);

        WWW url = new WWW("http://localhost/sqlconnect/getMap.php", form);
        yield return url;

        
        //Convert map back from string to a set of Vector3 points with block data
        if (url.text.Split(',')[0] == "0" && url.text != "0,")
        {
            string[] mapArray = url.text.Split(',');

            for (int i = 1; i < (mapArray.Length); i++)
            {
                Debug.Log("MAP: " + url.text);

                //Place the object based on input
                placeObject(new Cell(new Vector3(float.Parse(mapArray[i]), float.Parse(mapArray[i + 1]), float.Parse(mapArray[i + 2])), int.Parse(mapArray[i + 3])));
                i += 3;
            }
        }
        else
        {
            //Error, was there a valid map response 
            Debug.Log("MAP DONWLOAD FAILED. ERROR #" + url.text);
        }
    }

    //Saved map
    public void CallSaveMap()
    {
        StartCoroutine(SaveMap());
    }

    /// <summary>
    /// Convert the method into a string and call on the php script to save the string into the database
    /// Intended to be called by a Coroutine
    /// </summary>
    /// <returns>IEnumerator - Lets the coroutine know when the method has successfully executed</returns>
    IEnumerator SaveMap()
    {
        string temp = "";
        //Convert block location's into a string to send to DB
        for(int i = 0; i < blocks.Count; i++)
        {
            //Put a comma after if there are more blocks to come 
            if (i + 1 != blocks.Count)
                temp += blocks[i].toDBString() + ",";
            else
                temp += blocks[i].toDBString();
        }
        Debug.Log("SAVED MAP: " + temp);

        WWWForm form = new WWWForm();
        //Add all info into form
        form.AddField("mapId", DBManager.mapId);
        form.AddField("cellInfo", temp);
        //Execute form with PHP script
        WWW url = new WWW("http://localhost/sqlconnect/setMap.php", form);
        yield return url;
        
    }
}
