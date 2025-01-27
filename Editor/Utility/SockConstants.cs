﻿namespace WinuXGames.Sock.Editor.Utility
{
    /// <summary>
    /// Holds all constants sock uses
    /// </summary>
    public static class SockConstants
    {
        // General
        public const int IterationLimit = 1000;

        // Tags
        public const string SockTagPrefix                 = "__PRIV_SOCK_";
        public const string SockPositionTag               = SockTagPrefix + "POSITION";
        public const string SockLineMergerNodePositionTag = SockTagPrefix + "POSITION_LINE-MERGER-NODE";
        public const string SockStartNodePositionTag      = SockTagPrefix + "POSITION_START-NODE";
        public const string SockStopNodePositionTag       = SockTagPrefix + "POSITION_STOP-NODE";
    }
}