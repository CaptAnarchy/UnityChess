using UnityEngine;


/*
 * TODO:
 * 1) Utilize the tint values to modify sprite color.
 * 
 */
namespace ChessBase {
	[CreateAssetMenu(menuName = "Theme/Pieces")]

	/*	This class stores the relevant sprite and color
	 *  information for a GUI representation of the chess pieces.
	 */
	public class PieceTheme : ScriptableObject {

		/*	Utility class for holding a players 
		 *	sprite representation
		 */
		[System.Serializable]
		public class PieceSprites {
			public Sprite King, Queen, Rook, Bishop, Knight, Pawn;
			public Color Tint = Color.white;

			public Sprite this[int i] {
				get {
					return new Sprite[] { King, Queen, Rook, Bishop, Knight, Pawn }[i];
				}
			}
		}

		// Sprite sheet texture for all pieces
		public Texture2D PieceTexture;

		// Sprite container for white
		public PieceSprites WhitePieces;

		// Sprite container for black
		public PieceSprites BlackPieces;


		// Automatically load the individual sprites from the texture sheet on enable
		private void OnEnable() {
			if (!PieceTexture) { return; }

			// This logic relies on the sprite sheet being in the order of
			// King, Queen, Bishop, Knight, Rook, Pawn with White on top 
			// and Black on the bottom

			Sprite[] Sprites = Resources.LoadAll<Sprite>(PieceTexture.name);

			if (Sprites.Length > 11) {
				WhitePieces.King = Sprites[0];
				WhitePieces.Queen = Sprites[1];
				WhitePieces.Bishop = Sprites[2];
				WhitePieces.Knight = Sprites[3];
				WhitePieces.Rook = Sprites[4];
				WhitePieces.Pawn = Sprites[5];

				BlackPieces.King = Sprites[6];
				BlackPieces.Queen = Sprites[7];
				BlackPieces.Bishop = Sprites[8];
				BlackPieces.Knight = Sprites[9];
				BlackPieces.Rook = Sprites[10];
				BlackPieces.Pawn = Sprites[11];
			}
		}
		

		// Gets the sprite associated with a particular PieceID
		public Sprite GetSprite(int Piece) {
			PieceSprites Sprites = PieceID.IsWhite(Piece) ? WhitePieces : BlackPieces;

			switch (PieceID.PieceType(Piece)) {
				case PieceID.King:
					return Sprites.King;
				case PieceID.Queen:
					return Sprites.Queen;
				case PieceID.Rook:
					return Sprites.Rook;
				case PieceID.Bishop:
					return Sprites.Bishop;
				case PieceID.Knight:
					return Sprites.Knight;
				case PieceID.Pawn:
					return Sprites.Pawn;
				default:
					if (Piece != 0) {
						Debug.Log(Piece);
					}
					return null;
			}
		}


		// Return the color to tint the sprite texture
		public Color GetTintColor(int Piece) {
			PieceSprites Sprites = PieceID.IsWhite(Piece) ? WhitePieces : BlackPieces;
			return Sprites.Tint;
		}


		// Return a random piece sprite for testing
		public Sprite RandomSprite() {
			PieceSprites Sprites = Random.Range(0, 100) > 50 ? WhitePieces : BlackPieces;
			int PieceIndex = Random.Range(0, 5);
			return Sprites[PieceIndex];
		}

	}

}

