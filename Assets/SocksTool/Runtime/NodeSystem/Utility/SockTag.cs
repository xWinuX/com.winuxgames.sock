namespace SocksTool.Runtime.NodeSystem.Utility
{
    public static class SockTag
    {
        public const string SockTagPrefix                 = "__PRIV_SOCK_";
        public const string SockPositionTag               = SockTagPrefix + "POSITION";
        public const string SockLineMergerNodePositionTag = SockTagPrefix + "POSITION_LINE-MERGER-NODE";
        public const string SockStartNodePositionTag      = SockTagPrefix + "POSITION_START-NODE";
    }
}