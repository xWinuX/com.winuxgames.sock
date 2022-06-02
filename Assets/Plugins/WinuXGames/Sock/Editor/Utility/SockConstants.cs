namespace WinuXGames.Sock.Editor.Utility
{
    public static class SockConstants
    {
        // General
        public const int IterationLimit = 1000;

        public const int SockLineNodeWidth = 350;
        
        // Tags
        public const string SockTagPrefix                 = "__PRIV_SOCK_";
        public const string SockPositionTag               = SockTagPrefix + "POSITION";
        public const string SockLineMergerNodePositionTag = SockTagPrefix + "POSITION_LINE-MERGER-NODE";
        public const string SockStartNodePositionTag      = SockTagPrefix + "POSITION_START-NODE";
    }
}