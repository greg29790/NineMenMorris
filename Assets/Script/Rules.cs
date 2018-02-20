using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Rules : MonoBehaviour {
	enum EPhase{
		PLACING,
		MOVING,
		REMOVING,
		FLYING,
		ENDING
	}

	enum EPlayer{
		EMPTY = -1,
		WHITE = 0,
		BLACK = 1,
	}
	private const int NB_PIECES_MAX = 9;
	public GameObject Selector;
	public GameObject MovingSelector;
	public GameObject[] Black;
	public GameObject[] White;
	public GameObject Ending;
	
	private float timeT;
	private Text textPhase;
	private Text textPlayer;
	private Text textInfoFly;
	private Text info;
	private bool isPLayerWhite = true;
	private int nbWhitePiecePlaced = 0;
	private int nbBlackPiecePlaced = 0;
	private Vector2 pieceSelected = Vector2.zero;
	private EPhase phase = EPhase.PLACING;

	private Dictionary<Vector2, EPlayer> dicoOfPlayerPosition = new Dictionary<Vector2, EPlayer>();
	private Dictionary<Vector2, List<int>> dicoOfAdjacent = new Dictionary<Vector2, List<int>>();
	private List<List<int>> listOfMillsCombinaison = new List<List<int>>();
	private List<Vector2> listWhiteStartPiecesPosition = new List<Vector2>();
	private List<Vector2> listBlackStartPiecesPosition = new List<Vector2>();
	private ArrayList arrayPosition = new ArrayList
	{
		new Vector2(-2.99f, 2.91f),
		new Vector2(-0.15f, 2.91f),
		new Vector2(2.72f, 2.91f),
		new Vector2(-2.1f, 2.07f),
		new Vector2(-0.15f, 2.04f),
		new Vector2(1.77f, 2.03f),
		new Vector2(-1.11f, 1.17f),
		new Vector2(-0.15f, 1.15f),
		new Vector2(0.81f, 1.15f),
		new Vector2(-3.11f, 0.27f),
		new Vector2(-2.12f, 0.23f),
		new Vector2(-1.11f, 0.23f),
		new Vector2(0.84f, 0.26f),
		new Vector2(1.8f, 0.23f),
		new Vector2(2.77f, 0.23f),
		new Vector2(-1.15f, -0.7f),
		new Vector2(-0.16f, -0.68f),
		new Vector2(0.85f, -0.68f),
		new Vector2(-2.15f, -1.63f),
		new Vector2(-0.15f, -1.65f),
		new Vector2(1.77f, -1.65f),
		new Vector2(-3.11f, -2.6f),
		new Vector2(-0.16f, -2.6f),
		new Vector2(2.81f, -2.6f),
	};
	// Use this for initialization
	void Start () {
		textPhase = GameObject.Find("Phase").GetComponent<Text>();
		textPlayer = GameObject.Find("Player").GetComponent<Text>();
		textInfoFly = GameObject.Find("InfoFly").GetComponent<Text>();
		info = GameObject.Find("Info").GetComponent<Text>();

		textPlayer.text = "White Player";

		InitStartPosition();
		InitDicoOfPlayersPosition();
		InitDicoOfAdjacent();
		InitListOfMillsCombinaison();
		Selector.SetActive(false);
		MovingSelector.SetActive(false);
		Ending.SetActive(false);

	}
	
	// Update is called once per frame
	void Update () {
		timeT += Time.deltaTime;
		if (isPLayerWhite){
			textPlayer.text = "White Player";
			textPlayer.color = Color.white;
		} else {
			textPlayer.text = "Black Player";
			textPlayer.color = Color.black;
		}
		Selector.SetActive(false);
		MovingSelector.SetActive(false);

		Vector2 mousePosition = MouseSelector();

		if (phase != EPhase.PLACING && ImpossibleToMove(GetPlayer())){
			phase = EPhase.ENDING;
			isPLayerWhite = !isPLayerWhite;
		}

		switch (phase){
			case EPhase.PLACING : Place(mousePosition);
			break;
			case EPhase.MOVING : Move(mousePosition);
			break;
			case EPhase.REMOVING : RemoveOpponentPiece();
			break;
			case EPhase.FLYING : Fly(mousePosition);
			break;
			case EPhase.ENDING : End();
			break;
		}
	}

	// Initialize list of original positions of black and white pieces
	private void InitStartPosition(){
		foreach (GameObject piece in White){
			listWhiteStartPiecesPosition.Add(piece.transform.position);
		}
		foreach (GameObject piece in Black){
			listBlackStartPiecesPosition.Add(piece.transform.position);
		}
	}	

	// Put black and white pieces in their original positions
	private void InitPiecesPosition(){
		for(int i = 0; i < NB_PIECES_MAX; i++){
			White[i].transform.position = listWhiteStartPiecesPosition[i];
			Black[i].transform.position = listBlackStartPiecesPosition[i];
		}
	}
	// Initialize board with empty square
	private void InitDicoOfPlayersPosition(){
		foreach (Vector2 pos in arrayPosition){
			if (dicoOfPlayerPosition.ContainsKey(pos)){
				dicoOfPlayerPosition[pos] = EPlayer.EMPTY;
			}else{
				dicoOfPlayerPosition.Add(pos, EPlayer.EMPTY);	
			}
			
		}
	}
	/*
	Board :

	00-------01-------02
	| 	     |	       |
	|  03----04----05  |
	|  |     |      |  |
	|  |  06-07-08  |  |
	|  |  |      |  |  |
	09-10-11    12-13-14
	|  |  |		 |  |  |
	|  |  15-16-17  |  |
	|  |     |      |  |
	|  18----19----20  |
	|  	     |         |
	21-------22-------23	 
	
	 */
	 // Initialize dictionnary of adjacent square for each square
	private void InitDicoOfAdjacent()
	{
		dicoOfAdjacent.Add((Vector2)arrayPosition[0], new List<int>{1,9});
		dicoOfAdjacent.Add((Vector2)arrayPosition[1], new List<int>{0,2,4});
		dicoOfAdjacent.Add((Vector2)arrayPosition[2], new List<int>{1,14});
		dicoOfAdjacent.Add((Vector2)arrayPosition[3], new List<int>{4,10});
		dicoOfAdjacent.Add((Vector2)arrayPosition[4], new List<int>{1,3,5,7});
		dicoOfAdjacent.Add((Vector2)arrayPosition[5], new List<int>{4,13});
		dicoOfAdjacent.Add((Vector2)arrayPosition[6], new List<int>{7,11});
		dicoOfAdjacent.Add((Vector2)arrayPosition[7], new List<int>{4,6,8});
		dicoOfAdjacent.Add((Vector2)arrayPosition[8], new List<int>{7,12});
		dicoOfAdjacent.Add((Vector2)arrayPosition[9], new List<int>{0,10,21});
		dicoOfAdjacent.Add((Vector2)arrayPosition[10], new List<int>{3,9,11,18});
		dicoOfAdjacent.Add((Vector2)arrayPosition[11], new List<int>{6,10,15});
		dicoOfAdjacent.Add((Vector2)arrayPosition[12], new List<int>{8,13,17});
		dicoOfAdjacent.Add((Vector2)arrayPosition[13], new List<int>{5,12,14,20});
		dicoOfAdjacent.Add((Vector2)arrayPosition[14], new List<int>{2,13,23});
		dicoOfAdjacent.Add((Vector2)arrayPosition[15], new List<int>{11,16});
		dicoOfAdjacent.Add((Vector2)arrayPosition[16], new List<int>{15,17,19});
		dicoOfAdjacent.Add((Vector2)arrayPosition[17], new List<int>{12,16});
		dicoOfAdjacent.Add((Vector2)arrayPosition[18], new List<int>{10,19});
		dicoOfAdjacent.Add((Vector2)arrayPosition[19], new List<int>{16,18,20,22});
		dicoOfAdjacent.Add((Vector2)arrayPosition[20], new List<int>{13,19});
		dicoOfAdjacent.Add((Vector2)arrayPosition[21], new List<int>{9,22});
		dicoOfAdjacent.Add((Vector2)arrayPosition[22], new List<int>{19,21,23});
		dicoOfAdjacent.Add((Vector2)arrayPosition[23], new List<int>{14,22});
	}

	// Initialize list of all Mills combinaison on board
	private void InitListOfMillsCombinaison(){
		 listOfMillsCombinaison.Add(new List<int>{0, 1, 2});
		 listOfMillsCombinaison.Add(new List<int>{0, 9, 21});
		 listOfMillsCombinaison.Add(new List<int>{1, 4, 7});
		 listOfMillsCombinaison.Add(new List<int>{2, 14, 23});
		 listOfMillsCombinaison.Add(new List<int>{3, 4, 5});
		 listOfMillsCombinaison.Add(new List<int>{3, 10, 18});
		 listOfMillsCombinaison.Add(new List<int>{5, 13, 20});
		 listOfMillsCombinaison.Add(new List<int>{6, 7, 8});
		 listOfMillsCombinaison.Add(new List<int>{6, 11, 15});
		 listOfMillsCombinaison.Add(new List<int>{8, 12, 17});
		 listOfMillsCombinaison.Add(new List<int>{9, 10, 11});
		 listOfMillsCombinaison.Add(new List<int>{12, 13, 14});
		 listOfMillsCombinaison.Add(new List<int>{15, 16, 17});
		 listOfMillsCombinaison.Add(new List<int>{16, 19, 22});
		 listOfMillsCombinaison.Add(new List<int>{18, 19, 20});
		 listOfMillsCombinaison.Add(new List<int>{21, 22, 23});
	 }

	// Return where the mouse is on the board
	 private Vector2 MouseSelector()
	{
		float camDis = Camera.main.transform.position.y - GetComponent <Transform> ().position.y;
		Vector2 mouse = Camera.main.ScreenToWorldPoint (new Vector3 (Input.mousePosition.x, Input.mousePosition.y, camDis));

		foreach (Vector2 position in arrayPosition){
			if (IsCloseToPosition(mouse, position))
				return position;
		}
		return Vector2.zero;
	}

	// Get the current player
	private EPlayer GetPlayer(){
		return (isPLayerWhite ? EPlayer.WHITE : EPlayer.BLACK);	
	}

	// Get the current player to string
	private string GetPlayerToString(){
		return (isPLayerWhite ? "White" : "Black");
	}

	// Get the opponent player
	private EPlayer GetOpponent(){
		return (isPLayerWhite ? EPlayer.BLACK : EPlayer.WHITE);	
	}

	// Check if current player can't play
	private bool ImpossibleToMove(EPlayer player){
		foreach (Vector2 position in arrayPosition){
			if (dicoOfPlayerPosition[position] == player){
				foreach (int indexOfAdjacent in dicoOfAdjacent[position]){
					if (GetPlayerInSquare(indexOfAdjacent) == EPlayer.EMPTY){
						return false;
					}
				}
			}
		}
		return true;
	}
	// How many pieces has the current player on the board
	private int GetNbPieceOnBoardCurrentPlayer(){
		return isPLayerWhite ? nbWhitePiecePlaced : nbBlackPiecePlaced;
	}

	// Who is on a specific square
	private EPlayer GetPlayerInSquare(int squareIndex){
		Vector2 squarePosition = (Vector2)arrayPosition[squareIndex];
		return dicoOfPlayerPosition[squarePosition];
	}

	// Get the gameobject piece at specific position for specific player
	private GameObject FindPieceWithPosition(Vector2 position, EPlayer player)
	{
		if (player == EPlayer.WHITE){
			foreach (GameObject piece in White){
				if ((Vector2)piece.transform.position == position)
					return piece;
			}
		} else {
			foreach (GameObject piece in Black){
				if ((Vector2)piece.transform.position == position)
					return piece;
			}
		}
		return null;
	}

	// Check is mouse is close to specific square on the board
	private bool IsCloseToPosition(Vector2 mousePosition, Vector2 squarePosition){
			float xmin = squarePosition.x - 0.2f;
			float xmax = squarePosition.x + 0.2f;
			float ymin = squarePosition.y - 0.2f;
			float ymax = squarePosition.y + 0.2f;

			return (mousePosition.x >= xmin && mousePosition.x <= xmax && mousePosition.y >= ymin && mousePosition.y <= ymax);
	}

	// Placing Phase
	private void Place(Vector2 mousePosition){
		if (nbWhitePiecePlaced == NB_PIECES_MAX && nbBlackPiecePlaced == NB_PIECES_MAX)
		{
			phase = EPhase.MOVING;
			return;
		}
		
		textPhase.text = "Placing piece";
		textInfoFly.text = "";
		Selector.SetActive(false);

		if (mousePosition != Vector2.zero && dicoOfPlayerPosition[mousePosition] == EPlayer.EMPTY){
			Selector.SetActive(true);
			Selector.transform.position = mousePosition;
			if (isPLayerWhite)
			{
				
				if (Input.GetMouseButtonDown(0) && nbWhitePiecePlaced < NB_PIECES_MAX){
					White[nbWhitePiecePlaced].transform.position = mousePosition;
					dicoOfPlayerPosition[mousePosition] = EPlayer.WHITE;
					isPLayerWhite = false;
					nbWhitePiecePlaced++;
				}
			} else {
				if (Input.GetMouseButtonDown(0) && nbBlackPiecePlaced < NB_PIECES_MAX){
					Black[nbBlackPiecePlaced].transform.position = mousePosition;
					dicoOfPlayerPosition[mousePosition] = EPlayer.BLACK;
					isPLayerWhite = true;
					nbBlackPiecePlaced++;
				}
			}
		}
	}

	// Moving Phase
	private void Move(Vector2 mousePosition){
		if (GetNbPieceOnBoardCurrentPlayer() == 3){
			phase = EPhase.FLYING;
			return;
		}
		textPhase.text = "Moving piece";
		textInfoFly.text = "";

		if (pieceSelected == Vector2.zero)
		{
			pieceSelected = SelectPiece(mousePosition, GetPlayer());
		}
		if (pieceSelected != Vector2.zero){
			if (MovePiece(mousePosition, GetPlayer())){
				if (MillCreated()){
					phase = EPhase.REMOVING;
					
				}else{
					isPLayerWhite = !isPLayerWhite;
					pieceSelected = Vector2.zero;
					Selector.SetActive(false);
				}
			}
		}
	}

	// First step of moving or flying is to select piece
	private Vector2 SelectPiece(Vector2 mousePosition, EPlayer player){
		if (mousePosition != Vector2.zero && dicoOfPlayerPosition[mousePosition] == player ){
			Selector.transform.position = mousePosition;
			Selector.SetActive(true);
			if (Input.GetMouseButtonDown(0))
			{
				foreach (int indexOfAdjacent in dicoOfAdjacent[mousePosition]){
					if (GetPlayerInSquare(indexOfAdjacent) == EPlayer.EMPTY || phase == EPhase.FLYING){
						timeT = 0f;
						info.text = "Piece selected. Click again on it to deselect.";
						return (mousePosition);
					}
				}
				info.text = "Impossible to move this piece.";
			}
		}
		return Vector2.zero;
	}

	// Second step of moving is to move the piece
	private bool MovePiece(Vector2 mousePosition, EPlayer player){
		GameObject pieceToMove = null;
		
		Selector.SetActive(true);
		MovingSelector.SetActive(false);
		
		foreach (int indexOfAdjacent in dicoOfAdjacent[pieceSelected]){
			Vector2 adjacent = (Vector2)arrayPosition[indexOfAdjacent];			
			if (IsCloseToPosition(mousePosition, adjacent) && GetPlayerInSquare(indexOfAdjacent) == EPlayer.EMPTY){
				MovingSelector.transform.position = adjacent;
				MovingSelector.SetActive(true);
				if (Input.GetMouseButtonDown(0)){
					pieceToMove = FindPieceWithPosition(pieceSelected, player);
					if (pieceToMove != null){
						pieceToMove.transform.position = adjacent;
						dicoOfPlayerPosition[pieceSelected] = EPlayer.EMPTY;
						dicoOfPlayerPosition[adjacent] = player;
						pieceSelected = adjacent;
						info.text = "";
						return true;
					}
				}
			}
		}
		
		if (mousePosition == pieceSelected && Input.GetMouseButtonDown(0) && timeT > 0.5f){
			timeT = 0f;
			pieceSelected = Vector2.zero;
			info.text = "";
		}
		return false;
	}

	// After moving piece, check if mill is created
	private bool MillCreated(){
		int index = arrayPosition.IndexOf(pieceSelected);
		List<List<int>> listTmp = listOfMillsCombinaison.FindAll(list => list.Contains(index));
		
		foreach (List<int> list in listTmp){
			int count = 0;
			foreach (int i in list){
				Vector2 squarePosition = (Vector2)arrayPosition[i];
				if (dicoOfPlayerPosition[squarePosition] == GetPlayer()){
					count++;
				}
			}
			if (count == 3)
				return true;
		}
		return false;
	}

	// Select and remove Opponent piece
	private void RemoveOpponentPiece(){
		GameObject pieceToRemove = null;

		info.text = "Remove Opponent's piece";
		Vector2 mousePosition = MouseSelector();
		if (mousePosition != Vector2.zero && dicoOfPlayerPosition[mousePosition] == GetOpponent()){
			Selector.transform.position = mousePosition;
			Selector.SetActive(true);
			if (Input.GetMouseButtonDown(0))
			{
				pieceToRemove = FindPieceWithPosition(mousePosition, GetOpponent());
				if (pieceToRemove != null){
					dicoOfPlayerPosition[mousePosition] = EPlayer.EMPTY;
					if (GetPlayer() == EPlayer.WHITE){
						pieceToRemove.transform.position = listWhiteStartPiecesPosition[nbBlackPiecePlaced - 1];
						nbBlackPiecePlaced--;
					}else{
						pieceToRemove.transform.position = listBlackStartPiecesPosition[nbWhitePiecePlaced - 1];
						nbWhitePiecePlaced--;
					}
					if (nbWhitePiecePlaced == 2 || nbBlackPiecePlaced == 2){
						phase = EPhase.ENDING;
					}
					else{
						phase = EPhase.MOVING;
						isPLayerWhite = !isPLayerWhite;
						pieceSelected = Vector2.zero;
					}
					info.text = "";
					MovingSelector.SetActive(false);
					Selector.SetActive(false);
				}
			}
		}
	}

	// Flying phase
	private void Fly(Vector2 mousePosition){
		if (GetNbPieceOnBoardCurrentPlayer() > 3){
			phase = EPhase.MOVING;
			return;
		}		
		textPhase.text = "Flying piece";
		textInfoFly.text = "Player "+ GetPlayerToString() +" can move in any empty square";
		if (pieceSelected == Vector2.zero)
		{
			pieceSelected = SelectPiece(mousePosition, GetPlayer());
		}
		if (pieceSelected != Vector2.zero){
			if (FlyingPiece(mousePosition, GetPlayer())){
				if (MillCreated()){
					phase = EPhase.REMOVING;
				}else{
					isPLayerWhite = !isPLayerWhite;
					pieceSelected = Vector2.zero;
					Selector.SetActive(false);
				}
			}
		}
	}

	// Second step of flying is to move the piece
	private bool FlyingPiece(Vector2 mousePosition, EPlayer player){
		GameObject pieceToMove = null;

		Selector.SetActive(true);
		MovingSelector.SetActive(false);
		if (mousePosition != Vector2.zero && dicoOfPlayerPosition[mousePosition] == EPlayer.EMPTY){
			MovingSelector.SetActive(true);
			MovingSelector.transform.position = mousePosition;
			if (Input.GetMouseButtonDown(0)){
				pieceToMove = FindPieceWithPosition(pieceSelected, player);
				pieceToMove.transform.position = mousePosition;
				dicoOfPlayerPosition[mousePosition] = player;
				dicoOfPlayerPosition[pieceSelected] = EPlayer.EMPTY;
				pieceSelected = mousePosition;
				info.text = "";
				return true;
			}
		}
		if (mousePosition == pieceSelected && Input.GetMouseButtonDown(0) && timeT > 1f){
			Debug.Log(timeT);
			timeT = 0f;
			pieceSelected = Vector2.zero;
			info.text = "";
		}
		return false;
	}

	// Ending phase
	private void End(){
		Ending.SetActive(true);
		Ending.transform.Find("Text").GetComponent<Text>().text = "Player "+ GetPlayerToString() +" wins\nClick Enter to Restart";
		textInfoFly.text = "";
		if (Input.GetKeyDown(KeyCode.Return)){
			Restart();
		}
	}

	private void Restart(){
		InitPiecesPosition();
		InitDicoOfPlayersPosition();
		Selector.SetActive(false);
		MovingSelector.SetActive(false);
		Ending.SetActive(false);

		pieceSelected = Vector2.zero;
		textPlayer.text = "White Player";
		isPLayerWhite = true;
		nbBlackPiecePlaced = 0;
		nbWhitePiecePlaced = 0;
		phase = EPhase.PLACING;
		timeT = 0;
	}
}
