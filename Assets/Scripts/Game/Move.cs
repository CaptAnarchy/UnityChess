

/*
 * TODO:
 * 1) Handle special moves (i.e., En Passant, Castle, Checks, etc.).
 * 2) Create move from PGN string.
 * 
 */
namespace ChessBase {

	/*
	 *	Utility structure encapsulating a single chess move.
	 */
	[System.Serializable]
	public struct Move {

		public Ordinate Origin;
		public GamePiece Piece;

		public Ordinate Target;
		public GamePiece TargetPiece;


		// Initialize piece, origin, and target
		public void Init(GamePiece InPiece) {
	
			Piece = InPiece;
			Origin = (Piece == null) ? new Ordinate(-1) : Piece.BoardPosition;

			TargetPiece = null;
			Target = Origin;
		}


		// Sets the move origin board position and returns the linear index
		public int SetOrigin(Ordinate InOrigin) {
			Origin = InOrigin;
			return OriginIndex();
		}


		// Sets the move target board position and returns the linear index
		public int SetTarget(Ordinate InTarget) {
			Target = InTarget;
			return TargetIndex();
		}


		// Return linear index of origin coordinate
		public int OriginIndex() {
			return BoardInfo.GetSquareIndex(Origin);
		}


		// Return linear index of target coordinate
		public int TargetIndex() {
			return BoardInfo.GetSquareIndex(Target);
		}


		// Determines the linear offset of the move
		public int GetOffset() {
			return TargetIndex() - OriginIndex();
		}


		// True if the origin piece and target location are valid
		public bool IsValid() {
			if (Piece == null) { return false; }
			if (Piece.Id == 0) { return false; }
			if (!Origin.IsValid()) { return false; }
			if (!Target.IsValid()) { return false; }

			return true;
		}


		// Returns the PGN string for the move
		override public string ToString() {
			if (!IsValid()) { return ""; }

			string MoveString = "";
			int PieceType = PieceID.PieceType(Piece.Id);

			// Parse pawn move / capture
			if(PieceType == PieceID.Pawn) {
				MoveString += BoardInfo.GetFileName(Origin);
				if(TargetPiece && TargetPiece.Id > 0) {
					MoveString += ("x" + BoardInfo.GetSquareName(Target));
				}
				else {
					MoveString += BoardInfo.GetRankName(Target);
				}
			}

			// Parse other pieces
			else {
				if (FenString.SymbolMap.ContainsKey(PieceType)) {
					MoveString += FenString.SymbolMap[PieceType];
					if (TargetPiece && TargetPiece.Id > 0) {
						MoveString += "x";
					}
					MoveString += BoardInfo.GetSquareName(Target);
				}
			}

			return MoveString;
		}


		// Creates a move based on the PGN string
		public Move FromString(string PgnMove) {
			// TODO
			return new Move();
		}

	}

}
