using System;
using System.Collections.Generic;
using UnityEngine;


namespace ChessBase {

	/*
	 *	Implementation for the computer representation 
	 *	of the chess board.
	 */
	[System.Serializable]
	public class GameBoard {

		// True if it is whites turn
		public bool WhiteToMove;

		// Number of moves since capture or pawn moved
		public int HalfMoves;

		// Total number of completed turns in the game
		public int FullMoves;

		// Bits 0-3 Store white and black king-side/queen-side castling legality
		// Bits 4-7 Store file of ep square (starting at 1, so 0 = no ep square)
		// Bits 8-12 Captured piece
		// Bits 13-20 Half-move counter
		// Bits 21-31 Full-move counter
		Stack<uint> GameStateHistory;
		public uint CurrentGameState;

		// Stores piece code for each square on the board.
		// Piece code is defined as (piece type | piece color)
		public int[] Squares;


		// Constructor
		public GameBoard() {
			Squares = new int[64];
			GameStateHistory = new Stack<uint>();
		}


		// Initializes the board with the starting position
		public void Initialize(string Fen = FenString.NewGame) {
			LoadPosition(Fen);

			// Debug
			Debug.Log("GameState: " + GameStateString(CurrentGameState));
			Debug.Log("FEN: " + SavePosition());
		}


		// Sets up the board according to the given FEN string
		public void LoadPosition(string Fen) {

			// Load position from string
			var LoadedPosition = FenString.Load(Fen);

			// Who's turn
			WhiteToMove = LoadedPosition.WhiteToMove;

			// Castling rights
			CurrentGameState = 0;
			CurrentGameState |= LoadedPosition.WhiteCastleKingside ? 1u : 0u;
			CurrentGameState |= LoadedPosition.WhiteCastleQueenside ? 2u : 0u;
			CurrentGameState |= LoadedPosition.BlackCastleKingside ? 4u : 0u;
			CurrentGameState |= LoadedPosition.BlackCastleQueenside ? 8u : 0u;

			// En Passant file
			CurrentGameState |= ((uint)LoadedPosition.EnPassFile << 4);

			// Counters
			HalfMoves = LoadedPosition.Halfmoves;
			CurrentGameState |= (uint)HalfMoves << 13;

			FullMoves = LoadedPosition.FullMoves;
			CurrentGameState |= (uint)FullMoves << 21;

			// Piece layout
			for (int SquareIndex = 0; SquareIndex < 64; SquareIndex++) {
				int Piece = LoadedPosition.Squares[SquareIndex];
				Squares[SquareIndex] = Piece;
			}
		}


		// Saves the current position as a FEN string
		public string SavePosition() {
			return FenString.Save(this);
		}


		// True if the given move is valid
		public bool IsValidMove(Move TheMove) {

			// Invalid piece
			if (!TheMove.Piece || TheMove.Piece.Id == 0) { return false; }

			// Not a move
			if (TheMove.Origin.CompareTo(TheMove.Target) == 0) { return false; }

			// Same color
			if (TheMove.TargetPiece) {
				if (PieceID.PieceColor(TheMove.Piece.Id) == PieceID.PieceColor(TheMove.TargetPiece.Id)) {
					return false; 
				}
			}

			// Origin is invalid
			if (!TheMove.Piece.BoardPosition.IsValid()) { return false; }

			// Target is invalid
			if (!TheMove.Target.IsValid()) { return false; } 
		
			/*
			 * Add more validation logic below
			 */


			return true;
		}


		// True if move was executed without error
		public bool ExecuteMove(Move TheMove) {

			// Validate move
			if(!IsValidMove(TheMove)) { return false; }

			// Store current state
			GameStateHistory.Push(CurrentGameState);
			
			// Clear out everything but castling rights
			CurrentGameState &= 15u;

			// Get the indices
			int Origin = TheMove.OriginIndex();
			int Target = TheMove.TargetIndex();
			int Offset = Target - Origin;

			// Get the piece id's
			int Piece = Squares[Origin];
			int Captured = Squares[Target];

			// Castling rights and En Passant
			switch (PieceID.PieceType(Piece)) {
				case PieceID.King:
					CurrentGameState &= PieceID.IsWhite(Piece) ? ~3u : ~12u;
					break;
				case PieceID.Rook:
					int File = TheMove.Origin.FileIndex;
					bool bIsWhite = PieceID.IsWhite(Piece);
					// Queen side
					if(File == 0) {
						CurrentGameState &= bIsWhite ? ~2u : ~8u;
					}
					// King side
					else if (File == 7) {
						CurrentGameState &= bIsWhite ? ~1u : ~4u;
					}
					break;
				case PieceID.Pawn:
					if(Offset > 8 || Offset < -8) {
						CurrentGameState |= (uint)(TheMove.Origin.FileIndex + 1) << 4;
					}
					else {
						CurrentGameState &= ~(15u << 4);
					}
					break;
				default:
					break;
			}

			// Captured piece
			if (Captured > 0) {
				CurrentGameState |= (uint)Captured << 8;
			}

			// Moving pawn or capturing
			if (Captured > 0 || PieceID.PieceType(Piece) == PieceID.Pawn) {
				HalfMoves = 0;
			}
			else {
				HalfMoves++;
			}
			CurrentGameState |= (uint)HalfMoves << 13;

			// Move the piece
			Squares[Target] = Squares[Origin];
			Squares[Origin] = 0;

			// Update stats
			WhiteToMove = !WhiteToMove;
			if (WhiteToMove) {
				FullMoves++;
			}
			CurrentGameState |= (uint)FullMoves << 21;

			// Debug strings
			Debug.Log("Move: " + TheMove.ToString());
			Debug.Log("GameState: " + GameStateString(CurrentGameState));
			Debug.Log("FEN: " + SavePosition());

			return true;
		}


		// Display game state as a binary number spaced at field cutoffs
		public static string GameStateString(uint GameState) {
			char[] Output = new char[36];
			int[] Spaces = new int[] { 11, 19, 24, 28 };
			
			int iSpace = 0;
			for (int idx = 0; idx < 32; ++idx) {
				if (iSpace < Spaces.Length && idx == Spaces[iSpace]) {
					Output[idx + iSpace] = ' ';
					iSpace++;
				}
				Output[idx + iSpace] = (char)('0' + (char)((GameState << idx) >> 31));
			}
			return new string(Output);
		}
	}

}

