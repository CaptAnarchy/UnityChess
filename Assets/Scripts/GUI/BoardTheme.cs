using UnityEngine;


namespace ChessBase {
	[CreateAssetMenu(menuName = "Theme/Board")]

	/*
	 *	This class stores the relevant color information 
	 *  for a GUI representation of the chess board.
	 */
	public class BoardTheme : ScriptableObject {

		// This structure contains all possible square color states
		[System.Serializable]
		public struct SquareColors {
			public Color Normal;
			public Color Selected;
			public Color Legal;
			public Color MoveFrom;
			public Color MoveTo;
		}

		// Light square color scheme
		public SquareColors LightColors;
	
		// Dark square color theme
		public SquareColors DarkColors;


		// Constructors sets default colors
		public BoardTheme() {
			LightColors = new SquareColors();
			LightColors.Normal = Color.white;
			LightColors.Selected = Color.green;
			LightColors.Legal = Color.gray;
			LightColors.MoveFrom = Color.gray;
			LightColors.MoveTo = Color.gray;

			DarkColors = new SquareColors();
			DarkColors.Normal = Color.black;
			DarkColors.Selected = Color.red;
			DarkColors.Legal = Color.gray;
			DarkColors.MoveFrom = Color.gray;
			DarkColors.MoveTo = Color.gray;
		}
	}

}

