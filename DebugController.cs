using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SurvivalProject
{
    public class DebugController : MonoBehaviour
    {
        private bool _showConsole;
        private string _input = "";
        private string _textAreaContent = "";
        private Vector2 _scrollPosition = Vector2.zero;
        InputManager _inputManager;
        [Header("Console Background Rect")]
        [SerializeField] private Rect _consoleBackground;

        [Header("TextField Rect")]
        [SerializeField] private Rect _inputTextFieldRect;

        [Space]
        [Header("Close Button Rect")]
        [SerializeField] private Rect _closeButtonRect;

        [Space]
        [Header("TextArea Rect")]
        [SerializeField] private Rect _textAreaRect;

        [Range(1f, 30f)]
        [SerializeField] private float _scrollOffset;

        [SerializeField] private Color _consoleBackgroundColor;
        public bool ConsoleIsOpen { get; private set; }
        private void Awake()
        {
            _inputManager = new InputManager();
        }
        private void OnEnable()
        {
            Application.logMessageReceived += HandleLog;
            _inputManager.Enable();
            _inputManager.Player.Console.started += OpenConsoleGUI;
        }
        private void OpenConsoleGUI(InputAction.CallbackContext context) => _showConsole = !_showConsole;
        private void OnGUI()
        {
            if (!_showConsole) { return; }
            Event currentEvent = Event.current;

            GUIStyle gUIStyle = new GUIStyle();

            GUI.Box(_consoleBackground, "");
            _scrollPosition = GUI.BeginScrollView(_textAreaRect, _scrollPosition, new Rect(0, 0, _textAreaRect.width * 1.5f, _textAreaContent.Length * _scrollOffset));
            GUI.Label(new Rect(0, 0, _textAreaRect.width, _textAreaContent.Length * 20), _textAreaContent, gUIStyle);
            GUI.EndScrollView();

            if (currentEvent != null && currentEvent.isKey && (currentEvent.keyCode == KeyCode.Return || currentEvent.keyCode == KeyCode.KeypadEnter))
            {
                string formattedText = ">" + _input;
                _textAreaContent += formattedText + "\n";
                _input = "";
                currentEvent.Use();
            }

            if (GUI.Button(_closeButtonRect, "Close"))
            {
                _showConsole = false;
                _input = "";
            }

            _input = GUI.TextField(_inputTextFieldRect, _input);
        }
        private void HandleLog(string logString, string stackTrace, LogType type)
        {
            string formattedLog = logString;

            if (type == LogType.Error)
            {
                formattedLog = "<color=red>" + ">" + formattedLog + "</color>";
            }
            else if (type == LogType.Warning)
            {
                formattedLog = "<color=yellow>" + ">" + formattedLog + "</color>";
            }
            else if (type == LogType.Log)
            {
                formattedLog = "<color=cyan>" + ">" + formattedLog + "</color>";
            }

            _textAreaContent += formattedLog + "\n";
        }
        private void OnDisable()
        {
            Application.logMessageReceived -= HandleLog;
            _inputManager.Disable();
            _inputManager.Player.Console.started -= OpenConsoleGUI;
        }
    }
}
