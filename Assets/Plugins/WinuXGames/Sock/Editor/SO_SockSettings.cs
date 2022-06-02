using UnityEngine;

namespace WinuXGames.Sock.Editor
{
    [CreateAssetMenu(menuName = "Create SO_SockSettings", fileName = "SO_SockSettings", order = 0)]
    public class SO_SockSettings : ScriptableObject
    {
        [SerializeField] private SO_SockNodeSettings _nodeSettings;
        
        internal SO_SockNodeSettings NodeSettings { get => _nodeSettings; set => _nodeSettings = value; }
    }
}