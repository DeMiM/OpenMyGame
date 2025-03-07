using UnityEngine;
using UnityEngine.UI;

namespace Test.Puzzle
{

    public sealed class LobbyView : MonoBehaviour
    {
        [SerializeField]
        private Button _button;

        [SerializeField]
        private Image _image;

        public event System.Action<Sprite> OnChooseImage;

        private void OnClickButtonHandler()
        {
            OnChooseImage?.Invoke( _image.sprite );
        }

        private void OnEnable()
        {
            _button.onClick.AddListener( OnClickButtonHandler );
        }

        private void OnDisable()
        {
            _button.onClick.RemoveListener( OnClickButtonHandler );
        }

    }

}
