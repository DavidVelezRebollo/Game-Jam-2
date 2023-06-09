using ANT.Input;
using ANT.Components.Core;

using UnityEngine;
using System.Collections.Generic;
using Cinemachine;

namespace ANT.Components.Ants
{
    public class AntsManager : MonoBehaviour {
        #region Singleton

        public static AntsManager Instance;
        
        private void Awake() {
            if (Instance != null) return;
            Instance = this;

            DontDestroyOnLoad(this);
        }

        #endregion
        
        [SerializeField] private List<AntComponent> InitialAnts;
        [SerializeField] private CinemachineVirtualCamera AntsCamera;

        private readonly List<AntComponent> _currentAnts = new();
        private InputManager _input;
        private GameManager _gameManager;

        private int _auxIndex;

        private bool _onSelectMode;
        private bool _antsProtected;

        #region Unity Events

        private void Start() {
            _input = InputManager.Instance;
            _gameManager = GameManager.Instance;

            InitialAnts.ForEach(ant => {
                _currentAnts.Add(ant);

                if (_currentAnts.Count > 1) {
                    ant.SetAttachedAnt(InitialAnts[_currentAnts.Count - 2]);
                    ant.SetAttachedSpeed(_currentAnts[0].GetSpeed());
                } else if (_currentAnts.Count == 1) {
                    ant.SetPlayableState(true);
                    ant.gameObject.tag = "Game/PlayableAnt";
                }
            });
        }

        private void Update() {
            if (_onSelectMode) {
                SelectAnt();
                return;
            }
            
            _onSelectMode = _input.AntSelectFlag();

            if (!_onSelectMode) return;
            
            _gameManager.SetState(GameStates.Paused);
            _currentAnts[0].Highlight(new Color(0, 0, 0, 0.8f));
            _auxIndex = 0;
        }

        #endregion

        #region Getters & Setters

        public int CurrentAntsCount() { return _currentAnts.Count; }

        public AntComponent GetAnt(int index) { return _currentAnts[index]; }

        public bool IsAntsProtected() { return _antsProtected; }

        public void ProtectAnts() { _antsProtected = true; }

        #endregion

        #region Methods

        public void AddAnt(AntComponent ant) {
            int direction = Mathf.FloorToInt(_currentAnts[0].GetAntDirection());
            int offset = direction == -1 ? 3 : -3;
            
            ant.SetAttachedAnt(_currentAnts[^1]);
            ant.SetAttachedSpeed(_currentAnts[0].GetSpeed());
            ant.SetAntPosition(_currentAnts[0].GetAntCurrentPosition() + new Vector3(offset, 0f));
            ant.SetAntLayer(3);
            
            _currentAnts.Add(ant);
        }

        public void RemoveAnt(AntComponent ant) {
            ant.SetAttachedAnt(null);
            ant.SetAttachedSpeed(0);

            _currentAnts.Remove(ant);
        }

        #endregion

        #region Auxiliar Methods

        private void SelectAnt() {
            int direction = (int) _currentAnts[0].GetAntDirection();
            if (_input.RightFlag()) {
                _currentAnts[_auxIndex].Dehighlight();
                if (direction >= 0) _auxIndex = _auxIndex == 0 ? _currentAnts.Count - 1 : (_auxIndex - 1) % _currentAnts.Count;
                else _auxIndex = _auxIndex == _currentAnts.Count - 1 ? 0 : (_auxIndex + 1) % _currentAnts.Count;
                _currentAnts[_auxIndex].Highlight(new Color(0, 0, 0, 0.8f));
            }
                
            if (_input.LeftFlag()) {
                _currentAnts[_auxIndex].Dehighlight();
               if (direction >= 0) _auxIndex = _auxIndex == _currentAnts.Count - 1 ? 0 : (_auxIndex + 1) % _currentAnts.Count;
               else _auxIndex = _auxIndex == 0 ? _currentAnts.Count - 1 : (_auxIndex - 1) % _currentAnts.Count;
                _currentAnts[_auxIndex].Highlight(new Color(0, 0, 0, 0.8f));
            }

            if (_input.AntSelectFlag()) {
                // Ant position change in the list
                _currentAnts[0].gameObject.tag = "Game/Ant";
                
                (_currentAnts[_auxIndex].transform.position, _currentAnts[0].transform.position)
                    = (_currentAnts[0].transform.position, _currentAnts[_auxIndex].transform.position);
                (_currentAnts[_auxIndex], _currentAnts[0]) = (_currentAnts[0], _currentAnts[_auxIndex]);

                // Attachs the ants
                _currentAnts[0].Dehighlight();
                _currentAnts[0].SetAttachedAnt(null);
                _currentAnts[0].gameObject.tag = "Game/PlayableAnt";
                _currentAnts[0].PlayAntSound();
                AntsCamera.m_Follow = _currentAnts[0].transform;

                for (int i = 1; i < _currentAnts.Count; i++) {
                    _currentAnts[i].SetAttachedAnt(_currentAnts[i - 1]);
                    _currentAnts[i].SetAttachedSpeed(_currentAnts[0].GetSpeed());
                }

                // Resume the game
                _gameManager.SetState(GameStates.Playing);
                _onSelectMode = false;
            }
        }

        #endregion
    }
}
