using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Test.Puzzle
{

    public sealed class SetupView : MonoBehaviour
    {
        [SerializeField]
        private Button _closeButton;

        [SerializeField]
        private Button _playButtonFree;

        [SerializeField]
        private Button _playButtonCoins;

        [SerializeField]
        private Button _playButtonAds;

        [SerializeField]
        private Image _sourceImage;

        [SerializeField]
        private Image _sourceMask;

        [SerializeField]
        private ToggleGroup _typeSwitcher;

        [SerializeField]
        private Toggle[] _typeVariants;

        [SerializeField]
        private Sprite[] _maskVariants;

        public event System.Action<PuzzleType> OnSwitchType;
        public event System.Action<int> OnTryPlayPuzzle;
        public event System.Action OnTryCloseWindow;

        private Dictionary<Toggle, PuzzleData> _switching;

        public void SetSourceImage( Sprite sprite )
        {
            _sourceImage.sprite = sprite;

            if ( sprite != null )
            {
                Toggle toggle = _typeVariants[ 0 ];

                toggle.isOn = true;

                _typeSwitcher.NotifyToggleOn( toggle );

                _sourceMask.sprite = _maskVariants[ 0 ];
                _sourceMask.enabled = true;
            }
            else
            {
                _typeSwitcher.SetAllTogglesOff( false );

                _sourceMask.sprite = null;
                _sourceMask.enabled = false;
            }
        }

        private void OnClickCloseButtonHandler()
        {
            OnTryCloseWindow?.Invoke();
        }

        private void OnClickPlayButtonFreeHandler()
        {
            OnTryPlayPuzzle?.Invoke( 0 );
        }

        private void OnClickPlayButtonCoinsHandler()
        {
            OnTryPlayPuzzle?.Invoke( 1 );
        }

        private void OnClickPlayButtonAdsHandler()
        {
            OnTryPlayPuzzle?.Invoke( 2 );
        }

        private void OnTypeSwitchHandler( bool isOn )
        {
            if ( isOn == true )
            {
                Toggle activeToggle = _typeSwitcher.GetFirstActiveToggle();

                if ( _switching.TryGetValue( activeToggle, out PuzzleData puzzleData ) )
                {
                    _sourceMask.sprite = puzzleData.puzzleMask;

                    OnSwitchType?.Invoke( puzzleData.puzzleType );
                }
            }
        }

        private void Awake()
        {
            PuzzleType[] puzzleTypes = ( PuzzleType[] ) System.Enum.GetValues( typeof( PuzzleType ) );

            int amount = Mathf.Min( Mathf.Min( puzzleTypes.Length, _typeVariants.Length ), _maskVariants.Length );

            _switching = new Dictionary<Toggle, PuzzleData>( amount );

            for ( int i = 0; i < amount; ++i )
            {
                Toggle toggle = _typeVariants[ i ];

                toggle.onValueChanged.AddListener( OnTypeSwitchHandler );
                toggle.group = _typeSwitcher;

                _typeSwitcher.RegisterToggle( toggle );

                _switching.Add( toggle, new PuzzleData( puzzleTypes[ i ], _maskVariants[ i ] ) );
            }

            for ( int i = amount; i < _typeVariants.Length; ++i )
            {
                _typeVariants[ i ].gameObject.SetActive( false );
            }
        }

        private void OnEnable()
        {
            _closeButton.onClick.AddListener( OnClickCloseButtonHandler );
            _playButtonFree.onClick.AddListener( OnClickPlayButtonFreeHandler );
            _playButtonCoins.onClick.AddListener( OnClickPlayButtonCoinsHandler );
            _playButtonAds.onClick.AddListener( OnClickPlayButtonAdsHandler );
        }

        private void OnDisable()
        {
            _closeButton.onClick.RemoveListener( OnClickCloseButtonHandler );
            _playButtonFree.onClick.RemoveListener( OnClickPlayButtonFreeHandler );
            _playButtonCoins.onClick.RemoveListener( OnClickPlayButtonCoinsHandler );
            _playButtonAds.onClick.RemoveListener( OnClickPlayButtonAdsHandler );
        }

        private struct PuzzleData
        {
            public readonly PuzzleType puzzleType;
            public readonly Sprite puzzleMask;

            public PuzzleData( PuzzleType puzzleType, Sprite puzzleMask )
            {
                this.puzzleType = puzzleType;
                this.puzzleMask = puzzleMask;
            }
        }

    }

}