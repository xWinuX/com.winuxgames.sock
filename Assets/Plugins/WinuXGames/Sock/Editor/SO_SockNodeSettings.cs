using UnityEngine;

namespace WinuXGames.Sock.Editor
{
    [CreateAssetMenu(menuName = "Create SO_SockNodeSettings", fileName = "SO_SockNodeSettings", order = 0)]
    public class SO_SockNodeSettings : ScriptableObject
    {
        [SerializeField] private SockNodeSettings _lineNodeSettings;
        [SerializeField] private SockNodeSettings _optionNodeSettings;
        [SerializeField] private SockNodeSettings _startNodeSettings;
        [SerializeField] private SockNodeSettings _lineMergerNodeSettings;
    }
}