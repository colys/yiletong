using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common
{

    public class JsonMessage
    {
        public string Message { get; set; }

        public object Result { get; set; }

        public void LogException(Exception ex)
        {
            Message = ex.Message;
            Result = null;
        }
        public void LogException(Exception ex,string appendMsg)
        {
            Message = ex.Message + " " + appendMsg;
            Result = null;
        }
    }

    public class JsonMessage<T>
    {
        public string Message { get; set; }

        public T Result { get; set; }

        public void LogException(Exception ex)
        {
            Message = ex.Message;            
        }
    }
}
