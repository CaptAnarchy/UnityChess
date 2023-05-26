using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ChessBase {

	/*
	 *	This class implements the chess board user interface,
	 *	including drag/drop and piece sprite storage.
	 */
	public class GameBoardUI : MonoBehaviour {

		// Delegates
		public delegate void PieceSelectedDelegate(GamePiece Piece);
		public PieceSelectedDelegate PieceSelectedCallback;

		// Drag / drop variables
		private bool IsDragActive = false;
		private Vector3 DragStart;
		private Vector3 PieceStart;
		private GamePiece SelectedPiece;
		private Move CurrentMove;
		private Move LastValidMove;
		private Stack<Move> MoveHistory = new Stack<Move>();

		// Display
		private MeshRenderer[,] SquareRenderers;

		// Configuration
		public bool WhiteOnBottom = true;
		public BoardTheme Theme = null;
		public PieceTheme Pieces = null;

		[SerializeField] private GameBoard Board = new GameBoard();
		[SerializeField] private List<GamePiece> PieceList;
		[SerializeField] private List<GamePiece> CapturedList;

		// Variables for text testing
		private Text text;
		private GameObject CanvasGO;


		// Start is called before the first frame update
		void Awake() {
			Application.SetStackTraceLogType(LogType.Log, StackTraceLogType.None);
			CanvasGO = GameObject.Find("Canvas");

			// There can be only one
			GameBoardUI[] GameBoards = FindObjectsOfType<GameBoardUI>();
			if (GameBoards.Length > 1) {
				Destroy(gameObject);
			}
			GameBoards[0].PieceSelectedCallback += SelectPiece;

			// Initialization
			InitializeBoard();
			InitializePieces(); //FenUtility.MateIn3);
			InitializeLabels();

		}


		// Update is called once per frame, handles drag and drop
		void Update() {

			// Left button released while dragging
			if (IsDragActive && Input.GetMouseButtonUp(0)) {
				OnDragEnd();
				return;
			}

			// Left button not being held down
			if (!Input.GetMouseButton(0)) {
				return;
			}

			// Process drag
			if (IsDragActive) {
				OnDrag();
			}

			// Determine if a new piece is being dragged
			else {
				Vector3 WorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
				RaycastHit2D HitInfo = Physics2D.Raycast(WorldPosition, Vector2.zero);
				if (HitInfo.collider != null) {
					GamePiece DragItem = HitInfo.transform.gameObject.GetComponent<GamePiece>();
					if (DragItem && (Board.WhiteToMove ^ !PieceID.IsWhite(DragItem.Id))) {
						PieceSelectedCallback(DragItem);
						OnDragBegin();
					}
				}
			}

			UpdateSquareColors();
		}


		// Generate and initialize the graphical chess board
		private void InitializeBoard() {

			// Create the squares container
			GameObject Container = GameObject.Find("Squares");
			if (!Container) {
				Container = new GameObject("Squares");
				Container.transform.parent = transform;
			}

			// Default theme
			if (Theme == null) {
				Theme = ScriptableObject.CreateInstance<BoardTheme>();
			}

			// Initialize the board mesh renderers
			Shader SquareShader = Shader.Find("Unlit/Color");
			SquareRenderers = new MeshRenderer[8, 8];

			// Loop through file and rank
			for (int Rank = 0; Rank < 8; Rank++) {
				for (int File = 0; File < 8; File++) {

					// Create the current square
					Transform Square = GameObject.CreatePrimitive(PrimitiveType.Quad).transform;
					Square.parent = Container.transform;
					Square.name = BoardInfo.GetSquareName(File, Rank);
					
					// Update renderers
					SquareRenderers[File, Rank] = Square.gameObject.GetComponent<MeshRenderer>();
					SquareRenderers[File, Rank].material = new Material(SquareShader);
				}
			} 

			// Initialize and set color
			SetPerspective(WhiteOnBottom);
			UpdateSquareColors();
		}


		// Generates and initializes pieces
		public void InitializePieces(string Fen = FenString.NewGame) {

			// Create the pieces container
			GameObject Container = GameObject.Find("Pieces");
			if (!Container) {
				Container = new GameObject("Pieces");
				Container.transform.parent = transform;
			}


			// Initialize the computer representation
			Board.Initialize(Fen);

			// Create the necessary pieces
			PieceList.Clear();
			for (int idx = 0; idx < 64; idx++) {
				GamePiece NewPiece = GamePiece.CreatePiece(Board.Squares[idx], Pieces);
				if (NewPiece) {
					NewPiece.transform.parent = Container.transform;
					NewPiece.name = PieceID.ToString(NewPiece.Id);
					NewPiece.SetPosition(BoardInfo.GetOrdinate(idx));
					PieceList.Add(NewPiece);
				}
			}
		}


		// Generate file and rank labels
		public void InitializeLabels() {

			// Load the Arial font from Unity resources
			Font Arial;
			Arial = (Font)Resources.GetBuiltinResource(typeof(Font), "Arial.ttf");

			// Container game object for the file labels
			GameObject FileLabelsGO = new GameObject("FileLabels");
			FileLabelsGO.transform.SetParent(CanvasGO.transform);
			RectTransform rectTransform = FileLabelsGO.AddComponent<RectTransform>();
			rectTransform.localScale = Vector3.one;
			rectTransform.sizeDelta = Vector2.zero;
			rectTransform.anchorMin = Vector2.zero;
			rectTransform.anchorMax = Vector2.one;
			rectTransform.localPosition = Vector3.zero;

			// Loop through file
			for (int File = 0; File < 8; File++) {
				string FileLabel = BoardInfo.FileNames[File].ToString();

				// Create the file label text GameObject
				GameObject TextGO = new GameObject(FileLabel);
				TextGO.transform.SetParent(FileLabelsGO.transform);
				rectTransform = TextGO.AddComponent<RectTransform>();
				rectTransform.localScale = Vector3.one;
				rectTransform.sizeDelta = Vector2.zero;
				rectTransform.anchorMin = Vector2.one * 0.5f;
				rectTransform.anchorMax = Vector2.one * 0.5f;
				rectTransform.localPosition = Vector3.zero;

				// Set Text component properties
				text = TextGO.AddComponent<Text>();
				text.font = Arial;
				text.text = FileLabel;
				text.fontSize = 30;
				text.alignment = TextAnchor.MiddleCenter;
				text.horizontalOverflow = HorizontalWrapMode.Overflow;
				text.verticalOverflow = VerticalWrapMode.Overflow;
				Vector3 WorldPos = GetSquarePosition(new Ordinate(File, 0));
				text.rectTransform.localPosition = new Vector3(WorldPos.x * 72.5f, -4.5f * 72.5f, 0);
			}

			// Container game object for the rank labels
			GameObject RankLabelsGO = new GameObject("RankLabels");
			RankLabelsGO.transform.SetParent(CanvasGO.transform);
			rectTransform = RankLabelsGO.AddComponent<RectTransform>();
			rectTransform.localScale = Vector3.one;
			rectTransform.sizeDelta = Vector2.zero;
			rectTransform.anchorMin = Vector2.zero;
			rectTransform.anchorMax = Vector2.one;
			rectTransform.localPosition = Vector3.zero;

			// Loop through rank
			for (int Rank = 0; Rank < 8; Rank++) {
				string RankLabel = BoardInfo.RankNames[Rank].ToString();

				// Create the file label text GameObject
				GameObject TextGO = new GameObject(RankLabel);
				TextGO.transform.SetParent(RankLabelsGO.transform);
				rectTransform = TextGO.AddComponent<RectTransform>();
				rectTransform.localScale = Vector3.one;
				rectTransform.sizeDelta = Vector2.zero;
				rectTransform.anchorMin = Vector2.one * 0.5f;
				rectTransform.anchorMax = Vector2.one * 0.5f;
				rectTransform.localPosition = Vector3.zero;

				// Set Text component properties
				text = TextGO.AddComponent<Text>();
				text.font = Arial;
				text.text = RankLabel;
				text.fontSize = 30;
				text.alignment = TextAnchor.MiddleCenter;
				text.horizontalOverflow = HorizontalWrapMode.Overflow;
				text.verticalOverflow = VerticalWrapMode.Overflow;
				Vector3 WorldPos = GetSquarePosition(new Ordinate(0, Rank));
				text.rectTransform.localPosition = new Vector3(-4.5f * 72.5f, WorldPos.y * 72.5f, 0);
			}
		}


		// Delegate called when a piece is selected
		public void SelectPiece(GamePiece Piece) {
			SelectedPiece = Piece;

			Debug.Log(
				PieceID.ToString(SelectedPiece.Id) + 
				" selected at " + 
				BoardInfo.GetSquareName(SelectedPiece.BoardPosition)
			);
		}


		// Actually performs the move
		bool ExecuteMove(Move TheMove) {

			// The board handles internal representation logic
			if(!Board.ExecuteMove(TheMove)) { return false; }

			// Remove any graphical piece on the target square
			if (TheMove.TargetPiece) {
				Debug.Log("Capture: " + PieceID.ToString(TheMove.TargetPiece.Id) + " at " + TheMove.Target.ToString());
				PieceList.Remove(TheMove.TargetPiece);
				Destroy(TheMove.TargetPiece.gameObject);
				TheMove.TargetPiece = null;
			}

			// Animate the move
			Vector3 SquareLocation = GetSquare(TheMove.Target).transform.position;
			TheMove.Piece.TargetLocation = SquareLocation;
			TheMove.Piece.BoardPosition = TheMove.Target;

			LastValidMove = CurrentMove;
			MoveHistory.Push(LastValidMove);
			CurrentMove.Init(null);

			UpdateSquareColors();

			return true;
		}


		// Returns the piece on the given square coordinate
		GamePiece GetPiece(Ordinate Ord) {
			foreach (var Piece in PieceList) {
				if (Piece.BoardPosition.CompareTo(Ord) == 0) {
					return Piece;
				}
			}
			return null;
		}


		// Sets the chess board perspective, default is white on the bottom
		public void SetPerspective(bool whitePOV = true) {
			WhiteOnBottom = whitePOV;
			ResetSquarePositions();
		}


		// Return the ordinate of the given square renderer
		public Ordinate GetSquareOrdinate(MeshRenderer Renderer) {
			for (int File = 0; File < 8; File++) {
				for (int Rank = 0; Rank < 8; Rank++) {
					if(Renderer == SquareRenderers[File, Rank]) {
						return new Ordinate(File, Rank);
					}
				}
			}
			return new Ordinate(-1);
		}


		// Adjusts square location based on player POV
		void ResetSquarePositions() {
			for (int File = 0; File < 8; File++) {
				for (int Rank = 0; Rank < 8; Rank++) {
					SquareRenderers[File, Rank].transform.position = GetSquarePosition(File, Rank);
				}
			}
		}


		// Gets the square mesh renderer at the given coordinate
		public MeshRenderer GetSquare(Ordinate Ord) {
			return GetSquare(Ord.FileIndex, Ord.RankIndex);
		}


		// Gets the square mesh renderer at the given file and rank
		public MeshRenderer GetSquare(int File, int Rank) {
			return SquareRenderers[File, Rank];
		}


		// Gets a 3D world position of a square based on ordinate representation
		public Vector3 GetSquarePosition(Ordinate Ord, float Depth = 0) {
			return GetSquarePosition(Ord.FileIndex, Ord.RankIndex, Depth);
		}


		// Gets a 3D world position of a square based on file and rank.
		public Vector3 GetSquarePosition(int File, int Rank, float Depth = 0) {
			if (WhiteOnBottom) {
				return new Vector3(-3.5f + File, -3.5f + Rank, Depth);
			}
			return new Vector3(-3.5f + 7 - File, 7 - Rank - 3.5f, Depth);
		}


		// Resets all board squares to their normal color
		public void UpdateSquareColors() {

			// Reset entire board
			for (int Idx = 0; Idx < 64; Idx++) {
				SetSquareColor(new Ordinate(Idx), Theme.LightColors.Normal, Theme.DarkColors.Normal);
			}

			// Highlight the last move
			if (LastValidMove.IsValid()) {
				SetSquareColor(LastValidMove.Origin, Theme.LightColors.MoveFrom, Theme.DarkColors.MoveFrom);
				SetSquareColor(LastValidMove.Target, Theme.LightColors.MoveTo, Theme.DarkColors.MoveTo);
			}

			// Highlight the selected square
			if (SelectedPiece && SelectedPiece.Id > 0) {
				SetSquareColor(SelectedPiece.BoardPosition, Theme.LightColors.Selected, Theme.DarkColors.Selected);
			}

		}


		// Sets a particular board square color
		void SetSquareColor(Ordinate Ord, Color LightColor, Color DarkColor) {
			if (!Ord.IsValid()) { return; }
			GetSquare(Ord).material.color = (Ord.IsLightSquare()) ? LightColor : DarkColor;
		}


		// Called to begin a drag operation
		void OnDragBegin() {		
			SelectedPiece.TargetLocation = null;
			IsDragActive = SelectedPiece.IsDragging = true;
			DragStart = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			PieceStart = SelectedPiece.transform.position;

			CurrentMove.Init(SelectedPiece);
		}


		// Called every frame during a drag operation
		private void OnDrag() {
			if (!IsDragActive) { return; }
			Vector3 DragCurrent = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			SelectedPiece.transform.localPosition = PieceStart + (DragCurrent - DragStart);
		}


		// Called at the end of a drag operation
		private void OnDragEnd() {
			IsDragActive = SelectedPiece.IsDragging = false;
			SelectedPiece.TargetLocation = PieceStart;
			SelectedPiece = null;

			// Determine which square piece was dropped on
			RaycastHit HitInfo;
			Ray RayOrigin = Camera.main.ScreenPointToRay(Input.mousePosition);
			if (Physics.Raycast(RayOrigin, out HitInfo)) {
				MeshRenderer Square = HitInfo.transform.GetComponent<MeshRenderer>();
				if (Square) {
					CurrentMove.SetTarget(GetSquareOrdinate(Square));
					CurrentMove.TargetPiece = GetPiece(CurrentMove.Target);
					ExecuteMove(CurrentMove);
				}
			}

		}

	}

}