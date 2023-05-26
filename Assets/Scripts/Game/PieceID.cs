

using System.Collections.Generic;

namespace ChessBase {

	/*
	 *	Encapsulation of the piece color and type information into
	 *	a 32 bit integer with some accompanying utility functions.
	 */
	public static class PieceID {

		public const int None = 0;  // 0b00000
		public const int King = 1;  // 0b00001
		public const int Pawn = 2;  // 0b00010
		public const int Knight = 3;// 0b00011
		public const int Bishop = 5;// 0b00101
		public const int Rook = 6;  // 0b00110
		public const int Queen = 7; // 0b00111

		public const int White = 8; // 0b01000
		public const int Black = 16;// 0b10000

		// Masks for extracting the information
		const int TypeMask = 0b00111;
		const int WhiteMask = 0b01000;
		const int BlackMask = 0b10000;
		const int ColorMask = WhiteMask | BlackMask;


		// Piece id to piece name map for quick access
		public static Dictionary<int, string> NameMap = new Dictionary<int, string>() {
			[PieceID.King]   = "King",
			[PieceID.Queen]  = "Queen",
			[PieceID.Rook]   = "Rook",
			[PieceID.Bishop] = "Bishop",
			[PieceID.Knight] = "Knight",
			[PieceID.Pawn]   = "Pawn"
		};


		// Returns the color id of the given piece
		public static int PieceColor(int Piece) {
			return Piece & ColorMask;
		}


		// Returns the type id of the given piece
		public static int PieceType(int Piece) {
			return Piece & TypeMask;
		}


		// True is given piece is white, false otherwise
		public static bool IsWhite(int Piece) {
			return (Piece & ColorMask) == White;
		}


		// True if the given piece is a rook or queen, false otherwise
		public static bool IsRookOrQueen(int Piece) {
			return (Piece & 0b110) == 0b110;
		}


		// True if the given piece is a bishop or queen, false otherwise
		public static bool IsBishopOrQueen(int Piece) {
			return (Piece & 0b101) == 0b101;
		}


		// True if the given piece uses sliding moves, false otherwise
		public static bool IsSlidingPiece(int Piece) {
			return (Piece & 0b100) == 0b100;
		}


		// Converts the piece id to a FEN compliant representation
		public static string ToString(int Piece) {
			if (Piece == 0) { return "None"; }

			bool WhitePiece = IsWhite(Piece);

			string PieceName = "";
			char Symbol = FenString.SymbolMap[PieceType(Piece)];
			PieceName += (IsWhite(Piece) ? Symbol : char.ToLower(Symbol));

			return PieceName;
		}


		// Returns the piece full name string
		public static string GetName(int Pid) {
			int Type = PieceType(Pid);
			if (NameMap.ContainsKey(Type)) {
				return NameMap[Type];
			}
			return "";
		}


		// Returns the piece full color string
		public static string GetColor(int Pid) {
			return IsWhite(Pid) ? "White" : "Black";
		}

	}

}

