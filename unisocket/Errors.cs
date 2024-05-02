using System;

namespace LAB302
{
    public enum FailureType
    {
        CONNECTION_DISCONNECTED,
        RECEIVE_BUFFER_ERROR,
    }


    public static class Errors
    {
        public static void PrintMsg(string msg)
        {
            Console.WriteLine(msg);
        }

        public static void PrintError(string msg, bool needThrow = false)
        {
            if (needThrow)
                throw new Exception(msg);
            else
                Console.Error.WriteLine(msg);
        }

        public static void PrintFailure(string msg, FailureType failure)
        {
            Console.Error.WriteLine($"{msg} : {failure}");
        }
    }
}