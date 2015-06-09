using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;



public class Tuple<T1, T2>
{
	public T1 First;
	public T2 Second;
	internal Tuple(T1 first, T2 second)
	{
		First = first;
		Second = second;
	}
}

[System.Serializable]
public class StupidVector3<T>
{
	public T x;
	public T y;
	public T z;
	internal StupidVector3 (T x, T y, T z)
	{
		this.x = x;
		this.y = y;
		this.z = z;
	}
}


// This script holds the main game api and contains main game loop.
public class GameController : MonoBehaviour 
{


	public enum UnitType {Mothership = 0, Destroyer, Cruiser, Fighter, Transporter, Shuttle, Unknown};
	
	public enum GameState {NotStarted, Playing, Paused, PlayerWon, EnemyWon, Draw};

	public enum Who {Player = 0, Enemy};

	public static string gameDataPath = "/gameData.dat";
	public static string presetsPath = "/presets.dat";
	public static int skirmishNumber = 16;

	// Unit type translation.
	public static UnitType getUnitType (GameObject ob)
	{
		if (ob == null)
		{
			return UnitType.Unknown;
		}
		if (ob.name == "Mothership")
		{
			return UnitType.Mothership;
		}
		else if (ob.name == "Destroyer")
		{
			return UnitType.Destroyer;
		}
		else if (ob.name == "Cruiser")
		{
			return UnitType.Cruiser;
		}
		else if (ob.name == "Fighter")
		{
			return UnitType.Fighter;
		}
		else if (ob.name == "Transporter")
		{
			return UnitType.Transporter;
		}
		else if (ob.name == "Shuttle")
		{
			return UnitType.Shuttle;
		}
		else
		{
			return UnitType.Unknown;
		}
	}

	// This class is used to represent players in-game variables.
	public class Player
	{
		// Unit type translation.
		public static UnitType getUnitType (GameObject ob)
		{
			if (ob == null)
			{
				return UnitType.Unknown;
			}
			if (ob.name == "Mothership")
			{
				return UnitType.Mothership;
			}
			else if (ob.name == "Destroyer")
			{
				return UnitType.Destroyer;
			}
			else if (ob.name == "Cruiser")
			{
				return UnitType.Cruiser;
			}
			else if (ob.name == "Fighter")
			{
				return UnitType.Fighter;
			}
			else if (ob.name == "Transporter")
			{
				return UnitType.Transporter;
			}
			else if (ob.name == "Shuttle")
			{
				return UnitType.Shuttle;
			}
			else
			{
				return UnitType.Unknown;
			}
		}
		// Points to victory of a player.
		private float points;

		// Credits for buying units.
		private float credits;
		
		// Now will be the list of units of some types.
		
		private const int typeOfUnits = 7;
		private List<GameObject>[] units;
		
		public Player () 
		{
			this.units = new List<GameObject>[typeOfUnits];
			for (int i = 0; i < typeOfUnits; i++)
			{
				this.units[i] = new List<GameObject>();
			}
		}
		
		
		
		// Some methods for accessing fields.
		
		// Should be called when creating new object for a player.
		public void addUnit(GameObject ob)
		{
			//Debug.Log ("Adding object " + ob);
			//Debug.Log (getUnitType(ob));
			if (ob != null)
			{
				UnitType t = getUnitType(ob); 
				this.units[(int)t].Add(ob);
				//Debug.Log("After addition " + this.units[(int)t].Count);
			}
		}
		
		// Should be called on destroying an object.
		public void removeUnit(GameObject ob)
		{
			if (ob != null)
			{
				UnitType t = getUnitType(ob); 
				this.units[(int)t].Remove(ob);
			}
		}

		public int getUnitCount (UnitType type)
		{
			//Debug.Log("Type is " + type);
			//Debug.Log("Size is " + this.units[(int) type].Count);
			return this.units[(int) type].Count;
		}

		
		public void setPoints (float p)
		{
			this.points = p;
		}
		
		public float getPoints ()
		{
			return this.points;
		}

		public void setCredits (float c)
		{
			this.credits = c;
		}

		public float getCredits ()
		{
			return this.credits;
		}

		
	}

	// The structure which holds player's
	// upgrades and unlock progression etc.
	[System.Serializable]
	public class GameData 
	{
		// The structure which holds the progression 
		// of player on global map.
		[System.Serializable]
		public class ProgressTree
		{
			[System.Serializable]
			public class MissionNode
			{
				// Is mission unlocked?
				private bool unlocked;
				// Sons of mission.
				private List<int> sons;

				public MissionNode (bool u, List<int> s)
				{
					this.unlocked = u;
					this.sons = new List<int>();
					foreach (int i in s)
					{
						this.sons.Add(i);
					}
				}

				public bool isUnlocked ()
				{
					return this.unlocked;
				}

				public List<int> getSons()
				{
					return this.sons;
				}

				public void setUnlocked (bool u)
				{
					this.unlocked = u;
				}

				public void setSons (List<int> s)
				{
					this.sons.Clear();
					foreach (int i in s)
					{
						this.sons.Add(i);
					}
				}


			}

			// The size of our tree.
			private const int treeSize = 16;
			// Which missions are unlocked.
			private MissionNode[] nodes;

			public ProgressTree (LinkedList<Tuple<bool, List<int>>> inputData)
			{
				this.nodes = new MissionNode[ProgressTree.treeSize];
				int i = 0;
				foreach (Tuple<bool, List<int>> t in inputData)
				{
					MissionNode n = new MissionNode (t.First, t.Second);
					this.nodes[i] = n;
					i++;
				}
			}

			public MissionNode[] getNodes ()
			{
				return this.nodes;
			}
			

		}

		// This class represent's upgrades done to units.
		[System.Serializable]
		public class Upgrades 
		{
			private Dictionary<int, int> levels;
			public Upgrades (List<Tuple<int, int>> l)
			{
				levels = new Dictionary<int, int> ();
				foreach (Tuple<int, int> t in l)
				{
					levels.Add(t.First, t.Second);
				}
			}
		}
		// How many resources player has gained.
		// Used for upgrades.
		private float resources;

		// The structure used to represent the data
		// about mission progression. So that they could
		// be non-linear.
		private ProgressTree progressTree;

		// Now should be some structure for unit's upgrade.
		private Upgrades upgrades;

		public GameData ()
		{
			resources = 0.0f;
			progressTree = null;
			upgrades = null;
		}

		public float getResources ()
		{
			return this.resources;
		}

		public void setResources (float f)
		{
			this.resources = f;
		}

		public ProgressTree getProgressTree ()
		{
			return this.progressTree;
		}

		public void loadProgressTree (LinkedList<Tuple<bool, List<int>>> l)
		{
			this.progressTree = new ProgressTree (l);
		}

		public Upgrades getUpgrades ()
		{
			return this.upgrades;
		}

		public void setUpgrades (List<Tuple<int, int>> l)
		{
			this.upgrades = new Upgrades (l);
		}

	}

	// Structure to save the presets for missions
	// and for creating skirmishes.
	[System.Serializable]
	public class Presets
	{
		// The amount of credits that every player gets on start.
		public float START_CREDITS;
		// How many points player gets for holding an asteroid.
		public float pointsForAsteroid;
		// How many points player gets for killing enemy units.
		public float pointsForKilling;
		// How many credits player gets for building on asteroid.
		public float creditsPerBuilding;
		// How many points one has to get to win.
		public float winPoints;
		// The constraints of map.
		public float minX;
		public float maxX;
		public float minZ;
		public float maxZ;
		public StupidVector3<float>[] asteroids;
		public StupidVector3<float> massP;
		public StupidVector3<float> massE;
		public Presets (float sc, float pa, float pk, float cb, float wp, 
		                float mix, float mx, float miz, float mz, StupidVector3<float>[] astero,
	                	StupidVector3<float> massP, StupidVector3<float> massE)
		{
			this.START_CREDITS = sc;
			this.pointsForAsteroid = pa;
			this.pointsForKilling = pk;
			this.creditsPerBuilding = cb;
			this.winPoints = wp;
			this.minX = mix;
			this.maxX = mx;
			this.minZ = miz;
			this.maxZ = mz;
			this.asteroids = astero;
			this.massP = massP;
			this.massE = massE;
		}
	}

	// Method that returns random location for player mass relay
	public static StupidVector3<float> getRandomMassRelayP (float size)
	{
		if (size == 500.0f)
		{
			return new StupidVector3<float>(100.0f + Random.Range(-50.0f, 50.0f),
			                   0.0f,
			                   100.0f + Random.Range(-50.0f, 50.0f));
		}
		else if (size == 1000.0f)
		{
			return new StupidVector3<float>(150.0f + Random.Range(-100.0f, 100.0f),
				                   0.0f,
				                   150.0f + Random.Range(-100.0f, 100.0f));
		}
		else if (size == 1500.0f)
		{
			return new StupidVector3<float>(200.0f + Random.Range(-150.0f, 150.0f),
				                   0.0f,
				                   200.0f + Random.Range(-150.0f, 150.0f));
		}
		else
		{
			Debug.Log("WRONG PARAMETER!");
			return new StupidVector3<float>(50.0f, 0.0f, 50.0f);
		}
	}
	// Method that returns random location for enemy mass relay
	public static StupidVector3<float> getRandomMassRelayE (float size)
	{
		if (size == 500.0f)
		{
			return new StupidVector3<float>(400.0f + Random.Range(-50.0f, 50.0f),
			                   0.0f,
			                   400.0f + Random.Range(-50.0f, 50.0f));
		}
		else if (size == 1000.0f)
		{
			return new StupidVector3<float>(850.0f + Random.Range(-100.0f, 100.0f),
				                   0.0f,
				                   850.0f + Random.Range(-100.0f, 100.0f));
		}
		else if (size == 1500.0f)
		{
			return new StupidVector3<float>(1300.0f + Random.Range(-150.0f, 150.0f),
				                   0.0f,
				                   1300.0f + Random.Range(-150.0f, 150.0f));
		}
		else
		{
			Debug.Log("WRONG PARAMETER!");
			return new StupidVector3<float>(450.0f, 0.0f, 450.0f);
		}
	}
	// Method that returns random locations for asteroids
	public static StupidVector3<float>[] getRandomAsteroids (float size, int numOfAstero)
	{
		float space = Mathf.Floor(size / numOfAstero);
		if (size == 500.0f)
		{
			StupidVector3<float>[] results = new StupidVector3<float>[numOfAstero];
			for (int i = 1; i <= numOfAstero; i++)
			{
				results[i-1] = new StupidVector3<float> ((i * space) + Random.Range(-50.0f, 50.0f),
				                          0.0f,
				                          Random.Range(50.0f, 450.0f));
			}
			return results;
		}
		else if (size == 1000.0f)
		{
			StupidVector3<float>[] results = new StupidVector3<float>[numOfAstero];
			for (int i = 1; i <= numOfAstero; i++)
			{
				results[i-1] = new StupidVector3<float> ((i * space) + Random.Range(-50.0f, 50.0f),
				                          0.0f,
				                          Random.Range(100.0f, 900.0f));
			}
			return results;
		}
		else if (size == 1500.0f)
		{
			StupidVector3<float>[] results = new StupidVector3<float>[numOfAstero];
			for (int i = 1; i <= numOfAstero; i++)
			{
				results[i-1] = new StupidVector3<float> ((i * space) + Random.Range(-50.0f, 50.0f),
				                          0.0f,
				                          Random.Range(150.0f, 1350.0f));
			}
			return results;
		}
		else
		{
			Debug.Log("WRONG PARAMETER!");
			StupidVector3<float>[] vec = new StupidVector3<float>[1];
			vec[0] = new StupidVector3<float>(450.0f, 0.0f, 450.0f);
			return vec;
		}
	}
	


	// Space for game variables.
	// The amount of credits that every player gets on start.
	public float START_CREDITS = 10000.0f;
	// How many points player gets for holding an asteroid.
	public float pointsForAsteroid = 1.0f;
	// How many points player gets for killing enemy units.
	public float pointsForKilling = 1.0f;
	// How many credits player gets for building on asteroid.
	public float creditsPerBuilding = 1.0f;
	// How many points one has to get to win.
	public float winPoints = 1000.0f;
	// Array of players. Player[0] - is our player. Player[1] - enemy.
	// This is only for know, because in future it will be possible to have
	// many players at once.
	private const int numberOfPlayers = 2;
	public Player[] players;
	// Array of Asteroids.
	public GameObject[] asteroids;
	// In which state of game are we?
	private GameState gameState = GameState.NotStarted;
	// Prefabs for asteroids and mass relays.
	public GameObject asteroidPrefab;
	public GameObject massRelayPrefab;
	public GameObject enemyMassRelayPrefab;

	// How many times we update the values?
	private float updateRate = 1.0f;
	private float timePassed = 0.0f;

	// Array of unit prefabs - gameObject to instantiate.
	public const int typeOfUnits = 7;
	public float[] unitCosts = new float[typeOfUnits];
	public float[] buildTime = new float[typeOfUnits];
	public GameObject[] unitPrefabs = new GameObject[typeOfUnits];
	public GameObject[] enemyPrefabs = new GameObject[typeOfUnits];

	// What are actually selected units? Are they a group?
	// We know that only player can select units for now
	// so any portability features will not be considered for now.
	public enum Selected {None, Single, Group}
	private List<GameObject> selectedUnits;
	private Selected selected;

	// This variables are the spawn points for the players.
	public GameObject[] massRelays = new GameObject[numberOfPlayers];
	// How much time is used for building?
	private float[] timeToSpawn = new float[numberOfPlayers];
	// This will help the gui, are the player building a unit?
	private bool[] isBuilded = new bool[numberOfPlayers];
	// What units are builded?
	private UnitType[] builded = new UnitType[numberOfPlayers];
	// The constraints of map.
	public float minX = 0.0f;
	public float maxX = 1000.0f;
	public float minZ = 0.0f;
	public float maxZ = 1000.0f;

	// Camera constraints
	public float minY = 50.0f;
	public float maxY = 200.0f;

	// Just for getting the mothership of a player.
	public GameObject mothership;

	public GameState getGameState() {
		return gameState;
	}

	public float[] TimeToSpawn {
		get { return timeToSpawn; }
	}


	private GUIclass gui;

	// End of game variables.


    // DEBUG VARS

	private bool first = false;
	private float ten = 0.0f;

    // END OF DEBUG VARS

	// Structure for saves.
	public GameData gameData {get; private set;}
	// How many resources are achieved for mission;
	private const float rewardedResources = 100.0f;
	// This should be the mission number.
	public int missionNumber = 0; 


	// This function will load all the presets for maps
	// e.g. map size, start credits.
	public void loadPresets ()
	{
		// Read.
		Presets presets;
		if (File.Exists(Application.persistentDataPath + presetsPath))
		{
			BinaryFormatter binaryFormatter = new BinaryFormatter();
			FileStream stream = File.Open(Application.persistentDataPath + presetsPath,
			                              FileMode.Open, FileAccess.Read);
			try
			{
				System.Object obj = (Presets)binaryFormatter.Deserialize(stream);
				presets = (Presets)obj; 
			}
			catch (IOException e)
			{
				presets = createPresets();
				Debug.Log (e.ToString());
			}
			finally
			{
				stream.Close();
			}
		}
		else
		{
			presets = createPresets();
		}

		// Now load to gamecontroller variables.
		this.START_CREDITS = presets.START_CREDITS;
		this.pointsForAsteroid = presets.pointsForAsteroid;
		this.pointsForKilling = presets.pointsForKilling;
		this.winPoints = presets.winPoints;
		this.minX = presets.minX;
		this.maxX = presets.maxX;
		this.minZ = presets.minZ;
		this.maxZ = presets.maxZ;
		foreach (StupidVector3<float> vec in presets.asteroids)
		{
			Vector3 newvec = new Vector3(vec.x, vec.y, vec.z);
			Instantiate(asteroidPrefab, newvec, Quaternion.identity);
		}
		Vector3 newvec1 = new Vector3(presets.massP.x, presets.massP.y, presets.massP.z);
		Vector3 newvec2 = new Vector3(presets.massE.x, presets.massE.y, presets.massE.z);
		massRelays[0] = Instantiate(massRelayPrefab, newvec1, Quaternion.identity) as GameObject;
		massRelays[1] = Instantiate(enemyMassRelayPrefab, newvec2, Quaternion.identity) as GameObject;
	}

	// Reads presets from file and returns the presets class.
	public static Presets readPresets ()
	{
		Presets presets;
		if (File.Exists(Application.persistentDataPath + presetsPath))
		{
			BinaryFormatter binaryFormatter = new BinaryFormatter();
			FileStream stream = File.Open(Application.persistentDataPath + presetsPath,
			                              FileMode.Open, FileAccess.Read);
			try
			{
				System.Object obj = (Presets)binaryFormatter.Deserialize(stream);
				presets = (Presets)obj; 
			}
			catch (IOException e)
			{
				presets = createPresets();
				Debug.Log (e.ToString());
			}
			finally
			{
				stream.Close();
			}
		}
		else
		{
			presets = createPresets();
		}
		return presets;
	}

	// Saves given preset.
	public static void savePresets (Presets presets)
	{
		BinaryFormatter binaryFormatter = new BinaryFormatter();
		FileStream stream = File.Open(Application.persistentDataPath + presetsPath, FileMode.OpenOrCreate);
		binaryFormatter.Serialize(stream, presets);
		stream.Close();
	}

	// Creates presets for 16 missions.
	public static Presets createPresets ()
	{
		BinaryFormatter binaryFormatter = new BinaryFormatter();
		FileStream stream = File.Open(Application.persistentDataPath + presetsPath, FileMode.OpenOrCreate);
		// How many missions.
		float creds;
		float pts_astr;
		float pts_kill;
		float b_creds;
		float win_pts;
		float mix;
		float mx;
		float miz;
		float mz;
		StupidVector3<float>[] asteros;
		StupidVector3<float> massP;
		StupidVector3<float> massE;
		// Skirmish. (Mission 17)
		creds = 10000.0f;
		pts_astr = 1.0f;
		pts_kill = 50.0f;
		b_creds = 1.0f;
		win_pts = 1000.0f;
		mix = 0.0f;
		mx = 1000.0f;
		miz = 0.0f;
		mz = 1000.0f;
		asteros = new StupidVector3<float>[3];
		asteros[0] = new StupidVector3<float>(50.0f, 0.0f, 50.0f);
		asteros[1] = new StupidVector3<float>(450.0f, 0.0f, 450.0f);
		asteros[2] = new StupidVector3<float>(250.0f, 0.0f, 250.0f);
		massP = new StupidVector3<float>(100.0f, 0.0f, 50.0f);
		massE = new StupidVector3<float>(400.0f, 0.0f, 450.0f);


		Presets presets = new Presets(creds, pts_astr, pts_kill, b_creds, win_pts,
		                              mix, mx, miz, mz, asteros, massP, massE);
		binaryFormatter.Serialize(stream, presets);
		stream.Close();
		return presets;
	}

	// This function loads all the parameters of player
	// ships: it's upgrades, points etc.
	private void loadData ()
	{
		if (File.Exists(Application.persistentDataPath + gameDataPath))
		{
			BinaryFormatter binaryFormatter = new BinaryFormatter();
			FileStream stream = File.Open(Application.persistentDataPath + gameDataPath,
			                              FileMode.Open, FileAccess.Read);
			try
			{
				System.Object obj = (GameData)binaryFormatter.Deserialize(stream);
				gameData = (GameData)obj; 
			}
			catch (IOException e)
			{
				firstRun();
				Debug.Log (e.ToString());
			}
			finally
			{
				stream.Close();
			}
		}
		else
		{
			firstRun();
		}
	}

	// Updates player data, saving his increased points
	// after win, and unlocking new map.
	private void saveData ()
	{
		BinaryFormatter binaryFormatter = new BinaryFormatter();
		FileStream stream = File.Open(Application.persistentDataPath + gameDataPath, FileMode.OpenOrCreate);
		binaryFormatter.Serialize(stream, gameData);
		stream.Close();
	}
	

	// On the first run we would like to initialize
	// our file after creation.
	private void firstRun ()
	{
		gameData = new GameData ();
		// Set resources.
		int res = 0;
		gameData.setResources(res);
		// Set upgrades.
		List<Tuple<int, int>> lev = new List<Tuple<int, int>>();
		for (int i = 0; i < typeOfUnits; i++)
		{
			Tuple<int, int> t = new Tuple<int, int>(i, 0);
			lev.Add(t);
		}
		gameData.setUpgrades(lev);
		// Create progress tree.
		LinkedList<Tuple<bool, List<int>>> progTree = new LinkedList<Tuple<bool, List<int>>>();
		// Now we have to create manually the progress graph by neighbours list.
		// First mission. (first = 0)
		Tuple<bool, List<int>> mission1 = new Tuple<bool, List<int>>(true, new List<int> ());
		mission1.Second.Add (1);
		mission1.Second.Add (2);
		mission1.Second.Add (3);
		// Second mission.
		Tuple<bool, List<int>> mission2 = new Tuple<bool, List<int>>(false, new List<int> ());
		mission2.Second.Add (4);
		// Third mission.
		Tuple<bool, List<int>> mission3 = new Tuple<bool, List<int>>(false, new List<int> ());
		mission3.Second.Add (1);
		mission3.Second.Add (4);
		mission3.Second.Add (5);
		// Fourth mission.
		Tuple<bool, List<int>> mission4 = new Tuple<bool, List<int>>(false, new List<int> ());
		mission4.Second.Add (6);
		// Fifth mission.
		Tuple<bool, List<int>> mission5 = new Tuple<bool, List<int>>(false, new List<int> ());
		mission1.Second.Add (10);
		mission1.Second.Add (11);
		mission1.Second.Add (3);
		// Sixth mission.
		Tuple<bool, List<int>> mission6 = new Tuple<bool, List<int>>(false, new List<int> ());
		mission6.Second.Add (12);
		// Seventh mission.
		Tuple<bool, List<int>> mission7 = new Tuple<bool, List<int>>(false, new List<int> ());
		mission7.Second.Add (4);
		mission7.Second.Add (7);
		// Eighth mission.
		Tuple<bool, List<int>> mission8 = new Tuple<bool, List<int>>(false, new List<int> ());
		mission8.Second.Add (8);
		mission8.Second.Add (9);
		// Nineth mission.
		Tuple<bool, List<int>> mission9 = new Tuple<bool, List<int>>(false, new List<int> ());
		// Tenth mission.
		Tuple<bool, List<int>> mission10 = new Tuple<bool, List<int>>(false, new List<int> ());
		// Eleventh mission.
		Tuple<bool, List<int>> mission11 = new Tuple<bool, List<int>>(false, new List<int> ());
		mission11.Second.Add (13);
		// Twelveth mission.
		Tuple<bool, List<int>> mission12 = new Tuple<bool, List<int>>(false, new List<int> ());
		mission12.Second.Add (14);
		// Thirteenth mission.
		Tuple<bool, List<int>> mission13 = new Tuple<bool, List<int>>(false, new List<int> ());
		mission13.Second.Add (11);
		// Fourteenth mission.
		Tuple<bool, List<int>> mission14 = new Tuple<bool, List<int>>(false, new List<int> ());
		mission14.Second.Add (15);
		// Fifteenth mission.
		Tuple<bool, List<int>> mission15 = new Tuple<bool, List<int>>(false, new List<int> ());
		mission15.Second.Add (15);
		// Sixteenth mission.
		Tuple<bool, List<int>> mission16 = new Tuple<bool, List<int>>(false, new List<int> ());
		// Now into the list. Using add first is O(1) -> faster than addLast.
		progTree.AddFirst(mission16);
		progTree.AddFirst(mission15);
		progTree.AddFirst(mission14);
		progTree.AddFirst(mission13);
		progTree.AddFirst(mission12);
		progTree.AddFirst(mission11);
		progTree.AddFirst(mission10);
		progTree.AddFirst(mission9);
		progTree.AddFirst(mission8);
		progTree.AddFirst(mission7);
		progTree.AddFirst(mission6);
		progTree.AddFirst(mission5);
		progTree.AddFirst(mission4);
		progTree.AddFirst(mission3);
		progTree.AddFirst(mission2);
		progTree.AddFirst(mission1);

		gameData.loadProgressTree (progTree);
		saveData();
	}



	void Start () 
	{
		loadData();
		if (missionNumber >= skirmishNumber)
		{
			loadPresets();
		}
		players = new Player[numberOfPlayers];
		timeToSpawn[0] = 0.0f;
		timeToSpawn[1] = 0.0f;
		isBuilded[0] = false;
		isBuilded[1] = false;
		// Initialize players variables.
		players[0] = new Player();
		players[1] = new Player();
		players[0].setPoints(0.0f);
		players[1].setPoints(0.0f);
		players[0].setCredits(START_CREDITS);
		players[1].setCredits(START_CREDITS);
		// Now should begin the dynamic asteroid handling - if skirmish mode.
		// Get all the asteroids so we can handle them.
		asteroids = GameObject.FindGameObjectsWithTag("Asteroid");
		// Set the game state to playing.
		gameState = GameState.Playing;

		gui = GameObject.FindGameObjectWithTag("GUI").GetComponent<GUIclass>();
		if (gui == null) {
			Debug.LogError("NOGUI");
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		timePassed += Time.deltaTime;
		if (this.timePassed > this.updateRate)
		{
			this.handleGame();
			this.timePassed = 0.0f;
			//Debug.Log ("Player points is :" + players[0].getPoints());
			//Debug.Log ("Enemy points is :" + players[1].getPoints());
		}

		//Debug.Log (timePassed);
		//Debug.Log ("Player points is :" + players[0].getPoints());
		//Debug.Log ("Enemy points is :" + players[1].getPoints());
		// Debug test.
		//if (!first)
		//{
			//this.navigateTo (GameObject.FindGameObjectWithTag ("Unit"), new Vector3 (50, 0, 100));
			//List<GameObject> lista = new List<GameObject>();
			//lista.Add(GameObject.FindGameObjectWithTag ("Player"));

			//this.buildUnit(Who.Player, UnitType.Mothership);

			//this.attackUnit(lista[0], GameObject.FindGameObjectWithTag("Cube"));
			//first = true;
		//}
		//ten++;
		//this.moveCamera(new Vector3(this.transform.position.x+(ten/10),50.0f,this.transform.position.z+(ten/10)));
		// End of debug test.
	}

	// Moving one unit into position.
	public void navigateTo (GameObject unit, Vector3 pos)
	{
		unit.GetComponent<UnitController> ().moveTo (pos);
	}

	// Moving many units to position.
	public void navigateTo (List<GameObject> units, Vector3 pos)
	{
		foreach (GameObject unit in units) 
		{
			this.navigateTo(unit, pos);
		}
	}

	// Unit attacks enemy unit.
	public void attackUnit (GameObject unit, GameObject enemy)
	{
		if (unit != null)
		{
			unit.GetComponent<UnitController> ().setTarget (enemy);
		}
	}

	// Many units attack enemy unit.
	public void attackUnit (List<GameObject> units, GameObject enemy)
	{
		foreach (GameObject unit in units) 
		{
			this.attackUnit (unit, enemy);
		}
	}

	// Moves main camera into desired position.
	public void moveCamera(Vector3 pos)
	{
		Debug.Log (minY);
		Debug.Log (maxY);
		Debug.Log (pos.y);
		if (pos.y > minY)
		{
			if (pos.y < maxY)
			{
				GameObject.FindGameObjectWithTag("MainCamera").transform.position = pos;
			}
		}
	
	}

	// Should be called before the destruction of enemy unit.
	public void givePointsForKilling (GameObject unit)
	{
		if (unit.tag == "Enemy")
		{
			players[0].setPoints(players[0].getPoints() + pointsForKilling);
		}
		else if (unit.tag == "Player")
		{
			players[1].setPoints(players[1].getPoints() + pointsForKilling);
		}
	}

	public void handleBuilding (AsteroidController ac)
	{
		AsteroidController.Building bld = ac.getBuilding();
		if (bld != null)
		{
			if (bld.whoControlls() == AsteroidController.Master.Player)
			{
				players[0].setCredits(players[0].getCredits() + (creditsPerBuilding * this.timePassed));
			}
			else if (bld.whoControlls() == AsteroidController.Master.Enemy)
			{
				players[1].setCredits(players[1].getCredits() + (creditsPerBuilding * this.timePassed));
			}
		}
	}


	public void handleAsteroid (GameObject asteroid)
	{
		AsteroidController asteroidController = asteroid.GetComponent<AsteroidController>();
		// Handle points for holding the asteroid.
		float morePoints = pointsForAsteroid * this.timePassed;
		if (asteroidController.belongsTo == AsteroidController.Master.Player)
		{
			//Debug.Log ("Here");
			players[0].setPoints(players[0].getPoints() + morePoints);
		}
		else if (asteroidController.belongsTo == AsteroidController.Master.Enemy)
		{
			players[1].setPoints(players[1].getPoints() + morePoints);
		}
		// Handle resources gained from asteroid.
		this.handleBuilding(asteroidController);
	}

	public void handleGame ()
	{
		if (gameState == GameState.Playing)
		{
			// Handle points for asteroids.
			// Every asteroid gives some points for winning.
			foreach (GameObject Asteroid in this.asteroids)
			{
				this.handleAsteroid(Asteroid);
			}

			// Handle builded unit.
			this.handleBuildedUnits();

			// Check if player has won the game.
			if ((players[0].getPoints() >= winPoints) && (players[1].getPoints() < players[0].getPoints()))
			{
				// Yaaay player has won the game. Set the game state to player won and announce it.
				gameState = GameState.PlayerWon;
			}
			// Check if enemy has won the game.
			else if ((players[1].getPoints() >= winPoints) && (players[0].getPoints() < players[1].getPoints()))
			{
				// :< Player has lost. Setting the game state and do something with it.
				gameState = GameState.EnemyWon;
			}
			// Check if the game result is draw.
			else if ((players[0].getPoints() >= winPoints) && (players[1].getPoints() >= winPoints))
			{
				// The game ended in draw. Do something about it.
				gameState = GameState.Draw;
			}
			// Else we are playing the game - no need to change the game state.
		}
		// Stop game and show some screens.
		else if (gameState == GameState.PlayerWon)
		{
			// Update resources.
			gameData.setResources(gameData.getResources() + rewardedResources);
			// Update progress.
			GameData.ProgressTree progTree = gameData.getProgressTree();
			GameData.ProgressTree.MissionNode[] nodes = progTree.getNodes();
			GameData.ProgressTree.MissionNode node = nodes[missionNumber];
			foreach (int neighbours in node.getSons())
			{
				nodes[neighbours].setUnlocked(true);
			}
			saveData();
			gui.ShowGameOverScreen("You have won!");
		}
		else if (gameState == GameState.EnemyWon)
		{
			gui.ShowGameOverScreen("Enemy has won!");
		}
		else if (gameState == GameState.Draw)
		{
			gui.ShowGameOverScreen("Draw!");
		}
		// The game is paused.
		else
		{
			// Do something or nothing.
		}


	}

	// Handles unit deletion from every list and set that it would be.
	public void deleteUnit (GameObject unit)
	{
		if (unit.tag == "Enemy")
		{
			this.players[1].removeUnit(unit);
		}
		else if (unit.tag == "Player")
		{
			this.players[0].removeUnit(unit);
		}
		foreach (GameObject astero in asteroids)
		{
			astero.GetComponent<AsteroidController>().unitKilled(unit);
		}
		GameObject[] AIs = GameObject.FindGameObjectsWithTag("AICore");
		foreach (GameObject ai in AIs)
		{
			ai.GetComponent<AIController>().deleteUnit(unit);
		}
		GameObject[] agents = GameObject.FindGameObjectsWithTag("EnemyAgent");
		foreach (GameObject agent in agents)
		{
			agent.GetComponent<EnemyController>().removeUnit(unit);
		}
		GameObject[] units = GameObject.FindGameObjectsWithTag("UnitMover");
		if ((unit.tag == "Player") && (unit.name != "Mothership"))
		{
			foreach (GameObject u in units)
			{
				if (u.transform.parent.gameObject.GetComponent<UnitController>().getTarget() == unit)
				{
					u.transform.parent.gameObject.GetComponent<UnitController>().setTarget(null);
				}
			}
			TouchScript touchsc = GameObject.FindGameObjectWithTag("TouchScript").GetComponent<TouchScript>();
			touchsc.DeselectUnit(unit);
		}

	}

	// Returns vector around some vector.
	private Vector3 getVectorAround (Vector3 vec)
	{
		Vector3 ret = new Vector3 (vec.x + Random.Range(-50.0f, 50.0f), vec.y, vec.z + Random.Range(-50.0f, 50.0f));
		if (ret.x < this.minX)
		{
			ret.x = minX;
		}
		else if (ret.x > this.maxX)
		{
			ret.x = maxX;
		}

		if (ret.z < this.minZ)
		{
			ret.z = minZ;
		}
		else if (ret.z > this.maxZ)
		{
			ret.z = maxZ;
		}
		return ret;
	}

	// Spawn units near it's mass relay.
	private void spawnUnit (int i)
	{
		timeToSpawn[i] = 0.0f;
		isBuilded[i] = false;
		GameObject unit;
		// If a player?
		if (i == 0)
		{
			unit = Instantiate (unitPrefabs[(int) builded[i]], 
			                    this.getVectorAround(massRelays[i].transform.position),
			                    Quaternion.identity) as GameObject;
			unit.name = unit.name.Replace("(Clone)", ""); 
			players[i].addUnit(unit);
			//Debug.Log (players[i].getUnitCount(getUnitType(unit)));
		}
		// If an enemy?
		else if (i == 1)
		{
			unit = Instantiate (enemyPrefabs[(int) builded[i]], 
			                    this.getVectorAround(massRelays[i].transform.position),
			                    Quaternion.identity) as GameObject;
			unit.name = unit.name.Replace("(Clone)", ""); 
			players[i].addUnit(unit);
			//Debug.Log (players[i].getUnitCount(getUnitType(unit)));
			GameObject.FindGameObjectWithTag("AICore").GetComponent<AIController>().units.Add(unit);
		}
	}

	/*
	private void fallingDownTemporaryFix() {
		if (mothership.transform.position.y != 0) {
			Debug.LogError("Falling down");
			mothership.transform.Translate(mothership.transform.position.x, 0, mothership.transform.position.z, Space.World);
			mothership.transform.Rotate (0, mothership.transform.rotation.x, 0);
		}
	}
	*/

	// We are handling here the updates on building units.
	private void handleBuildedUnits ()
	{
		for (int i = 0; i < numberOfPlayers; i++)
		{
			// Check if a player is building?
			if (isBuilded[i])
			{
				timeToSpawn[i] += this.timePassed;
				if (timeToSpawn[i] > buildTime[(int) builded[i]])
				{
					this.spawnUnit(i);
				}
			}
		}
	}

	// Builds a unit in desired position and of desired type of unit.
	public bool buildUnit (Who who, UnitType typeOfUnit)
	{
		if ((who == Who.Player) || (who == Who.Enemy))
		{
			if ((!isBuilded[(int) who]) && (players[(int) who].getCredits() >= unitCosts[(int) typeOfUnit]))
			{
				players[(int) who].setCredits(players[(int) who].getCredits() - unitCosts[(int) typeOfUnit]);
				// Set building to wanted unit and start counting down.
				timeToSpawn[(int) who] = 0.0f;
				isBuilded[(int) who] = true;
				builded[(int) who] = typeOfUnit;
				return true;
			}
			else
			{
				return false;
			}
		}
		return false;

	}


	// Deselects all currenlty selected units.
	public void deselectAllUnits ()
	{
		this.selectedUnits.Clear ();
		this.selected = Selected.None;
	}


	// Deselect given unit.
	public void deselectUnit (GameObject unit)
	{
		this.selectedUnits.Remove (unit);
		if (selectedUnits.Count == 0)
		{
			this.selected = Selected.None;
		}
		else if (selectedUnits.Count == 1)
		{
			this.selected = Selected.Single;
		}
		// Else still group selected.
	}


	// Selects one unit, deselects if any other units were currently selected.
	public void selectUnit (GameObject unit)
	{
		// We are discarding all currently selected units.
		this.deselectAllUnits ();
		this.selectedUnits.Add(unit);
		this.selected = Selected.Single;
	}

	// Adds to a group of selected units.
	public void addToGroup (GameObject unit)
	{
		this.selectedUnits.Add(unit);
		if (this.selectedUnits.Count == 1)
		{
			this.selected = Selected.Single;
		}
		else if (this.selectedUnits.Count > 1)
		{
			this.selected = Selected.Group;
		}
	}


	// Returns selected unit, if only one unit is selected.
	// Else it returns null.
	public GameObject getSelectedUnit ()
	{
		if (this.selected == Selected.None)
		{
			return null;
		}
		else if (this.selected == Selected.Single)
		{
			return this.selectedUnits.First();
		}
		else
		{
			// This is not included in Selected.None because maybe
			// in future we would want the first unit to get Selected.
			return null;
		}
	}

	// Returns list of all selected units.
	public List<GameObject> getSelectedGroup ()
	{
		return this.selectedUnits;
	}

	// You can ask if a player is building a unit
	public bool isPlayerBuilding (Who who)
	{
		return isBuilded[(int) who];
	}

	// You can get the unit he is building.
	public UnitType whatPlayerBuilds (Who who)
	{
		return builded[(int) who];
	}

	// You can get his time left to build.
	public float playerTimeLeftToBuild (Who who)
	{
		float res = buildTime[(int) builded[(int) who]] - timeToSpawn[(int) who];
		if (res < 0.0f)
		{
			res = 0.0f;
		}
		return res;
	}

	// This functions returns player's points
	public float getPlayerPoints (Who who)
	{
		return this.players[(int) who].getPoints();
	}

	// This functions returns player's credits
	public float getPlayerCredits (Who who)
	{
		return this.players[(int) who].getCredits();
	}

	// Returns the points for victory.
	public float getWinPoints ()
	{
		return winPoints;
	}

	// If mothership was killed - the player has ultimately lost.
	public void mothershipKilled(Who who)
	{
		if (who == Who.Enemy)
		{
			this.gameState = GameState.PlayerWon;
		}
		else if (who == Who.Player)
		{
			this.gameState = GameState.EnemyWon;
		}
	}

	public GameObject getMothership()
	{
		return this.mothership;
	}

	public int getPlayerAsteroids (Who who)
	{
		int count = 0;
		foreach (GameObject asteroid in asteroids)
		{
			AsteroidController.Master mas = (AsteroidController.Master) ((int) who);
			if (asteroid.GetComponent<AsteroidController>().belongsTo == mas)
			{
				count++;
			}
		}
		return count;
	}

}
