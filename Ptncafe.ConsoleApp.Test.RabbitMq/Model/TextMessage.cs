using System;
using System.Collections.Generic;
using System.Text;

namespace Ptncafe.ConsoleApp.Test.RabbitMq.Model
{
    public class TextMessage
    {
        public int Index { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string Message { get; set; }

    }
}
