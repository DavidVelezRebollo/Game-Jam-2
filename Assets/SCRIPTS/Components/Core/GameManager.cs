using UnityEngine;

namespace ANT.Components.Core {
    public enum GameStates {
        Menu,
        Paused,
        Playing
    }

    public class GameManager : MonoBehaviour {
        #region Singleton

        public static GameManager Instance;

        private void Awake() {
            if (Instance != null) return;
            Instance = this;

            _state = DebugMode ? GameStates.Playing : GameStates.Menu;
        }

        #endregion

        [SerializeField] private bool DebugMode;

        private GameStates _state;

        #region Getters & Setters

        public void SetState(GameStates state) { _state = state; }

        #endregion

        #region Methods

        public bool GamePaused() { return _state != GameStates.Playing; }

        public void EndGame() {
            // TODO - End the game
            _state = GameStates.Paused;
        }

        #endregion
    }
}
