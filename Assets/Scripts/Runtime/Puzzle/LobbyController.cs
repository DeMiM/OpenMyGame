using UnityEngine;

namespace Test.Puzzle
{

    public sealed class LobbyController : MonoBehaviour
    {
        [SerializeField]
        private LobbyView _lobbyView;

        [SerializeField]
        private SetupController _setupController;

        private void OnChooseImageHandler( Sprite sprite )
        {
            _setupController.Initialize( sprite );
        }

        private void OnEnable()
        {
            _lobbyView.OnChooseImage += OnChooseImageHandler;
        }

        private void OnDisable()
        {
            _lobbyView.OnChooseImage -= OnChooseImageHandler;
        }

    }

}
