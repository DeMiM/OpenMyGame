using Test.UI;
using UnityEngine;

namespace Test.Puzzle
{

    public sealed class SetupController : MonoBehaviour
    {
        public const string SetupWindowId = "PUZZLE";

        [SerializeField]
        private SetupView _setupView;

        private PuzzleInputData puzzleInputData;

        public void Initialize( Sprite sprite )
        {
            puzzleInputData = new PuzzleInputData( sprite );

            _setupView.SetSourceImage( sprite );

            WindowManager.Show( SetupWindowId, null );
        }

        private void OnSwitchTypeHandler( PuzzleType puzzleType )
        {
            puzzleInputData.puzzleType = puzzleType;
        }

        private void OnTryPlayPuzzleHandler( int mode )
        {
            switch ( mode )
            {
                case 0:
                    Debug.Log( $"Begin play puzzle from {( int ) puzzleInputData.puzzleType} element for image \"{puzzleInputData.sprite.name}\" as free" );
                    break;

                case 1:
                    Debug.Log( $"Begin play puzzle from {( int ) puzzleInputData.puzzleType} element for image \"{puzzleInputData.sprite.name}\" for 1000 coins" );
                    break;

                case 2:
                    Debug.Log( $"Begin play puzzle from {( int ) puzzleInputData.puzzleType} element for image \"{puzzleInputData.sprite.name}\" for ads view" );
                    break;
            }
        }

        private void OnTryCloseWindowHandler()
        {
            WindowManager.Hide( SetupWindowId );

            _setupView.SetSourceImage( null );
        }

        private void OnEnable()
        {
            _setupView.OnSwitchType += OnSwitchTypeHandler;
            _setupView.OnTryPlayPuzzle += OnTryPlayPuzzleHandler;
            _setupView.OnTryCloseWindow += OnTryCloseWindowHandler;
        }

        private void OnDisable()
        {
            _setupView.OnSwitchType -= OnSwitchTypeHandler;
            _setupView.OnTryPlayPuzzle -= OnTryPlayPuzzleHandler;
            _setupView.OnTryCloseWindow -= OnTryCloseWindowHandler;
        }

    }

}