using System;
using UnityEngine;


namespace ChessBase {

	/*
	 *	Utility class for storing, comparing, and converting
	 *	piece coordinates (i.e., file and rank).
	 */
	[System.Serializable]
	public struct Ordinate : IComparable<Ordinate> {

		public int FileIndex => _FileIndex;
		public int RankIndex => _RankIndex;
		
		private int _FileIndex;
		private int _RankIndex;


		// Constructor
		public Ordinate(int InFile, int InRank = 0) {
			_FileIndex = InFile;
			_RankIndex = InRank;
		}


		// Constructor
		public Ordinate(int SquareIndex) {
			this = BoardInfo.GetOrdinate(SquareIndex);
		}


		// True if coordinate represents a light square
		public bool IsLightSquare() {
			return (FileIndex + RankIndex) % 2 != 0;
		}


		// True if coordinate represents a valid board position
		public bool IsValid() {
			if (FileIndex < 0 || RankIndex < 0) {
				return false;
			}
			if (FileIndex > 7 || RankIndex > 7) {
				return false;
			}
			return true;
		}


		// Returns -1, 0, 1 depending on sort order, 0 indicates identical locations.
		public int CompareTo(Ordinate Other) {
			int MyIdx = ToBoardIndex();
			int OtherIdx = Other.ToBoardIndex();

			if(MyIdx == OtherIdx) { return 0; }
			return (MyIdx < OtherIdx) ? -1 : 1;
		}


		// Returns the linear index of this coordinate
		public int ToBoardIndex() {
			return (RankIndex * 8) + FileIndex;
		}


		// Returns a string representing this coordinate
		override public string ToString() {
			string StringName = "";
			if (FileIndex < 0 || RankIndex < 0) {
				StringName = "Invalid";
			}
			else {
				StringName += BoardInfo.FileNames[FileIndex];
				StringName += BoardInfo.RankNames[RankIndex];
			}
			return StringName;
		}

	}

}