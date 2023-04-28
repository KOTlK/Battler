using System;
using System.Text;
using TMPro;
using UnityEngine;

namespace Game.Runtime.MonoHell.Debug
{
    public class DebugText : MonoBehaviour
    {
        [SerializeField] private TMP_Text _text;

        private readonly StringBuilder _stringBuilder = new();
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }

        public static DebugText Instance { get; private set; }

        public void Append(string line)
        {
            _stringBuilder.Append(line);
            _stringBuilder.Append("\n");
        }

        private void Update()
        {
            _text.text = _stringBuilder.ToString();
            _stringBuilder.Clear();
        }
    }
}