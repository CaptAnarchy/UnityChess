

namespace ChessBase {

	/*
	 *	Utility class for converting the different
	 *	types of ordinate values and proper square names.
	 */
	public static class BoardInfo {

		// Name strings
		public const string FileNames = "abcdefgh";
		public const string RankNames = "12345678";

		// First rank list linear index
		public const int A1 = 0;
		public const int B1 = 1;
		public const int C1 = 2;
		public const int D1 = 3;
		public const int E1 = 4;
		public const int F1 = 5;
		public const int G1 = 6;
		public const int H1 = 7;

		// Last rank list linear index
		public const int A8 = 56;
		public const int B8 = 57;
		public const int C8 = 58;
		public const int D8 = 59;
		public const int E8 = 60;
		public const int F8 = 61;
		public const int G8 = 62;
		public const int H8 = 63;


		// Return file (0 to 7) of square given the linear index 
		public static int GetFile(int SquareIndex) {
			return SquareIndex & 0b000111;
		}


		// Return rank (0 to 7) of square given the linear index 
		public static int GetRank(int SquareIndex) {
			return SquareIndex >> 3;
		}


		// Return the linear index given file and rank
		public static int GetSquareIndex(int FileIndex, int RankIndex) {
			return RankIndex * 8 + FileIndex;
		}


		//  Return the linear index given the file and rank in ordinate format
		public static int GetSquareIndex(Ordinate Ord) {
			return GetSquareIndex(Ord.FileIndex, Ord.RankIndex);
		}


		// Return the ordinate format given the linear index
		public static Ordinate GetOrdinate(int SquareIndex) {
			return new Ordinate(GetFile(SquareIndex), GetRank(SquareIndex));
		}


		// Returns true if the square is light given the linear index
		public static bool IsLightSquare(int SquareIndex) {
			return IsLightSquare(GetOrdinate(SquareIndex));
		}

		// Returns true if the square is light given the ordinate format
		public static bool IsLightSquare(Ordinate Ord) {
			return IsLightSquare(Ord.FileIndex, Ord.RankIndex);
		}

		// Returns true if the square is light given the file and rank
		public static bool IsLightSquare(int FileIndex, int RankIndex) {
			return (FileIndex + RankIndex) % 2 != 0;
		}


		// Return the square name given the linear index
		public static string GetSquareName(int SquareIndex) {
			return GetSquareName(GetOrdinate(SquareIndex));
		}


		// Return the square name given the ordinate format
		public static string GetSquareName(Ordinate Ord) {
			return GetSquareName(Ord.FileIndex, Ord.RankIndex);
		}


		// Return the square name given the file and rank
		public static string GetSquareName(int FileIndex, int RankIndex) {
			return FileNames[FileIndex] + "" + (RankIndex + 1);
		}


		// Return the square file name given the ordinate format
		public static string GetFileName(Ordinate Ord) {
			return GetFileName(Ord.FileIndex);
		}


		// Return the square file name given the file index
		public static string GetFileName(int FileIndex) {
			return FileNames[FileIndex].ToString();
		}


		// Return the square rank name given the ordinate format
		public static string GetRankName(Ordinate Ord) {
			return GetRankName(Ord.RankIndex);
		}


		// Return the square rank name given the rank index
		public static string GetRankName(int RankIndex) {
			return (RankIndex + 1) + "";
		}

	}

}

