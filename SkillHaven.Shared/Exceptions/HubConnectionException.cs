using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SkillHaven.Shared.Exceptions
{
    public class HubConnectionException:Exception
    {
        public HubConnectionException()
        {
            
        }

        public HubConnectionException(string message):base(message)
        {
            
        }

        public HubConnectionException(string message,Exception exception) : base(message,exception)
        {

        }

        protected HubConnectionException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
