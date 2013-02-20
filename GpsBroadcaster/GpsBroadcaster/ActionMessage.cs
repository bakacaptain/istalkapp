using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GpsBroadcaster
{
    public class ActionMessage
    {
        public String Message { get; set; }

        public ActionMessage(String message)
        {
            Message = message;
        }

        public static implicit operator ActionMessage(String msg)
        {
            return new ActionMessage(msg);
        }

        public override string ToString()
        {
            return Message;
        }
    }
}
