using System;

namespace LAB302
{
    public enum FailureType
    {
        ACCEPT_ERROR,
        CONNECTION_ERROR,
        SEND_ERROR,
        RECEIVE_ERROR,
        IP_PARSE_ERROR,
    }
    
    public class UniSocketErrors : Exception
    {
        public static event Action<string> ErrorRaised;

        private string _msg;

        public string RaiseError()
        {
            ErrorRaised?.Invoke(_msg);

            return _msg;
        }

        public static void RaiseMsg(string msg)
        {
            ErrorRaised?.Invoke(msg);
        }

        public UniSocketErrors(object msg) : base(message: msg.ToString())
        {
            _msg = msg.ToString();
        }

        public UniSocketErrors(FailureType failureType) : base(message: failureType.ToString())
        {
            _msg = failureType.ToString();
        }

        public UniSocketErrors(FailureType failureType, object msg) : base(message: $"{failureType} : {msg}")
        {
            _msg = $"{failureType} : {msg}";
        }
    }

    public static class DebuggerExtensions
    {
        public static void Print(this UniSocketErrors errors, bool needThrow = false)
        {
            if (needThrow)
                throw errors;
            else
                Console.WriteLine(errors.RaiseError());
        }

        public static void Print(this Exception exception, bool needThrow = false)
        {
            if (needThrow)
                throw exception;
            else
            {
                Console.WriteLine(exception);
                UniSocketErrors.RaiseMsg(exception.Message);
            }
        }
    }
}