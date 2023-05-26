using System.Collections.Generic;


/*
 * TODO: 
 * 1) Add a full selection of mate in {2,3,4} puzzles.
 * 2) Add a function to randomly grab a puzzle fen.
 * 3) Add puzzle solution parsing.
 * 
 */
namespace ChessBase {


	/*
	 *	Utility class for reading and writing FEN strings. 
	 */
	public static class FenString {

		// Container class that holds the game relevant 
		// information contained within a FEN string.
		[System.Serializable]
		public class GameInfo {
			public bool WhiteToMove;
			public bool WhiteCastleKingside;
			public bool WhiteCastleQueenside;
			public bool BlackCastleKingside;
			public bool BlackCastleQueenside;
			public int EnPassFile;
			public int Halfmoves;
			public int FullMoves;
			public int[] Squares;


			// Constructor
			public GameInfo() {
				Squares = new int[64];
			}
		}

		// Character symbol to piece id map for quick access
		public static Dictionary<char, int> PieceMap = new Dictionary<char, int>() {
			['K'] = PieceID.King,
			['Q'] = PieceID.Queen,
			['R'] = PieceID.Rook,
			['B'] = PieceID.Bishop,
			['N'] = PieceID.Knight,
			['P'] = PieceID.Pawn
		};

		// Piece id to character symbol map for quick access
		public static Dictionary<int, char> SymbolMap = new Dictionary<int, char>() {
			[PieceID.King]   = 'K',
			[PieceID.Queen]  = 'Q',
			[PieceID.Rook]   = 'R',
			[PieceID.Bishop] = 'B',
			[PieceID.Knight] = 'N',
			[PieceID.Pawn]   = 'P'
		};

		// Preselected FEN strings
		public const string NewGame = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
		public const string MateIn2 = "1rb4r/pkPp3p/1b1P3n/1Q6/N3Pp2/8/P1P3PP/7K w - - 1 1";
		public const string MateIn3 = "rn3rk1/p5pp/2p5/3Ppb2/2q5/1Q6/PPPB2PP/R3K1NR b - - 0 1";
		public const string MateIn4 = "r5rk/2p1Nppp/3p3P/pp2p1P1/4P3/2qnPQK1/8/R6R w - - 1 1";


		// Load game board info from a given fen string
		public static GameInfo Load(string Fen) {

			GameInfo LoadedGame = new GameInfo();
			string[] Sections = Fen.Split(' ');

			int File = 0;
			int Rank = 7;

			// Position section
			foreach (char Symbol in Sections[0]) {
				if (Symbol == '/') {
					File = 0;
					Rank--;
				}
				else {
					if (char.IsDigit(Symbol)) {
						File += (int)char.GetNumericValue(Symbol);
					}
					else {
						int PieceColor = (char.IsUpper(Symbol)) ? PieceID.White : PieceID.Black;
						int PieceType = PieceMap[char.ToUpper(Symbol)];
						LoadedGame.Squares[Rank * 8 + File] = PieceType | PieceColor;
						File++;
					}
				}
			}

			// Current move section - defaults to whites move
			LoadedGame.WhiteToMove = (Sections.Length > 2) ? (Sections[1] == "w") : true;

			// Castling rights section - defaults to all
			string CastlingRights = (Sections.Length > 2) ? Sections[2] : "KQkq";
			LoadedGame.WhiteCastleKingside = CastlingRights.Contains("K");
			LoadedGame.WhiteCastleQueenside = CastlingRights.Contains("Q");
			LoadedGame.BlackCastleKingside = CastlingRights.Contains("k");
			LoadedGame.BlackCastleQueenside = CastlingRights.Contains("q");

			// En Passant section
			if (Sections.Length > 3) {
				string enPassantFileName = Sections[3][0].ToString();
				if (BoardInfo.FileNames.Contains(enPassantFileName)) {
					LoadedGame.EnPassFile = BoardInfo.FileNames.IndexOf(enPassantFileName) + 1;
				}
			}

			// Half move count section
			if (Sections.Length > 4) {
				int.TryParse(Sections[4], out LoadedGame.Halfmoves);
			}

			// Full move count section
			if (Sections.Length > 5) {
				int.TryParse(Sections[5], out LoadedGame.FullMoves);
			}

			return LoadedGame;
		}


		// Generate FEN string for the current board layout
		public static string Save(GameBoard Board) {
			string Fen = "";

			// Go through ranks
			for (int Rank = 7; Rank >= 0; Rank--) {
				int EmptyFileNum = 0;

				// Go through files
				for (int File = 0; File < 8; File++) {
					int SquareIndex = Rank * 8 + File;
					int Piece = Board.Squares[SquareIndex];

					// Square has a valid piece
					if (Piece != 0) {
						if (EmptyFileNum != 0) {
							Fen += EmptyFileNum;
							EmptyFileNum = 0;
						}

						bool IsBlackPiece = !PieceID.IsWhite(Piece);
						int PieceType = PieceID.PieceType(Piece);

						// Decode piece type
						char PieceChar = ' ';
						switch (PieceType) {
							case PieceID.King:
								PieceChar = 'K';
								break;
							case PieceID.Queen:
								PieceChar = 'Q';
								break;
							case PieceID.Rook:
								PieceChar = 'R';
								break;
							case PieceID.Bishop:
								PieceChar = 'B';
								break;
							case PieceID.Knight:
								PieceChar = 'N';
								break;
							case PieceID.Pawn:
								PieceChar = 'P';
								break;
						}
						Fen += (IsBlackPiece) ? PieceChar.ToString().ToLower() : PieceChar.ToString();
					}
					else {
						EmptyFileNum++;
					}

				}
				if (EmptyFileNum != 0) {
					Fen += EmptyFileNum;
				}
				if (Rank != 0) {
					Fen += '/';
				}
			}

			// Side to move
			Fen += ' ';
			Fen += (Board.WhiteToMove) ? 'w' : 'b';

			// Castling
			bool WhiteKingside = (Board.CurrentGameState & 1) == 1;
			bool WhiteQueenside = (Board.CurrentGameState >> 1 & 1) == 1;
			bool BlackKingside = (Board.CurrentGameState >> 2 & 1) == 1;
			bool BlackQueenside = (Board.CurrentGameState >> 3 & 1) == 1;

			Fen += ' ';
			Fen += (WhiteKingside) ? "K" : "";
			Fen += (WhiteQueenside) ? "Q" : "";
			Fen += (BlackKingside) ? "k" : "";
			Fen += (BlackQueenside) ? "q" : "";
			Fen += ((Board.CurrentGameState & 15) == 0) ? "-" : "";

			// En-passant
			Fen += ' ';
			int epFile = (int)(Board.CurrentGameState >> 4) & 15;
			if (epFile == 0) {
				Fen += '-';
			}
			else {
				string FileName = BoardInfo.FileNames[epFile - 1].ToString();
				int epRank = (Board.WhiteToMove) ? 6 : 3;
				Fen += FileName + epRank;
			}

			// Half-move counter
			Fen += ' ';
			Fen += Board.HalfMoves;

			// Full-move count (should be one at start, and increase after each move by black)
			Fen += ' ';
			Fen += Board.FullMoves;

			return Fen;
		}

	}

}

