using System;
using UnityEngine;


namespace ChessBase {

	/*
	 *	Implementation of the graphical game piece
	 *	for use by the GameBoardUI.
	 */
	[System.Serializable]
	public class GamePiece : MonoBehaviour {

		//public delegate void DragEndedDelegate(GamePiece Piece);
		//public DragEndedDelegate DragEndedCallback;

		const float PieceDepth = -0.1f;
		const float MovementSpeed = 15f;

		private SpriteRenderer Renderer;
		private BoxCollider2D Collider;

		public int Id = 0;
		public Ordinate BoardPosition;
		public bool IsDragging = false;
		public System.Nullable<Vector3> TargetLocation = null;


		// Generates a new game piece GameObject
		public static GamePiece CreatePiece(int Pid, PieceTheme Theme) {
			if (Pid == 0) { return null; }

			// Create new game object and set up
			GameObject NewObject = new GameObject("Piece", typeof(ChessBase.GamePiece));
			NewObject.transform.localScale = Vector3.one * 100 / (2000 / 6f);

			// Grab the GamePiece component and set up
			GamePiece NewPiece = NewObject.GetComponent<GamePiece>();

			// Setup the renderer
			NewPiece.Id = Pid;
			NewPiece.Renderer = NewObject.AddComponent<SpriteRenderer>();
			NewPiece.Renderer.sprite = Theme.GetSprite(NewPiece.Id);

			// Setup the collider
			NewPiece.Collider = NewObject.AddComponent<BoxCollider2D>();
			NewPiece.Collider.size = new Vector2(3.34f, 3.34f);

			return NewPiece;
		}


		// Sets the board position and world location of the piece
		public void SetPosition(Ordinate Ord) {
			BoardPosition = Ord;

			GameBoardUI Board = GameObject.FindObjectOfType<GameBoardUI>();
			if(!Board) { return; }

			transform.position = Board.GetSquarePosition(Ord, PieceDepth);
			Board.PieceSelectedCallback += Test;
		}


		// Delegate called when a piece is selected
		public void Test(GamePiece Test) {
			if(Test != this) { return; }
			Debug.Log(PieceID.GetColor(Id) + " " + PieceID.GetName(Id));
		}


		// Handles lerp'ing the piece to the desired location
		private void FixedUpdate() {
			if (TargetLocation.HasValue) {
				if (IsDragging) {
					TargetLocation = null;
					return;
				}

				if (transform.position == TargetLocation) {
					TargetLocation = null;
					//DragEndedCallback(this);
				}
				else {
					transform.position = Vector3.Lerp(
						transform.position, 
						TargetLocation.Value, 
						MovementSpeed * Time.fixedDeltaTime
					);
				}
			}
		}

	}

}

