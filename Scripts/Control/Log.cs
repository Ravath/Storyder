using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;

namespace Storyder
{
    public static class Log
    {
        public static void LogErr(string message, params object[] args)
        {
            if(args.Length == 0)
            {
                GD.PrintErr(message);
            }
            else
            {
                GD.PrintErr(string.Format(message, args));
            }
        }
        
        public static void LogInfo(string message, params object[] args)
        {
            if(args.Length == 0)
            {
                GD.Print(message);
            }
            else
            {
                GD.Print(string.Format(message, args));
            }
        }
    }
}