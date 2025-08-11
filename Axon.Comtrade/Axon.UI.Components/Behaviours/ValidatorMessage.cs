using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Axon.UI.Components.Behaviours
{

    public class ValidatorMessage
    {
        public ValidatorMessage(bool isError, string parent, string reference, string parameter, string message)
        {
            this.IsError = IsError;
            this.Parent = parent;
            this.Reference = reference;
            this.Parameter = parameter;
            this.Message = message;
        }

        public bool IsError { get; set; }
        public string Parent { get; set; }
        public string Reference { get; set; }
        public string Parameter { get; set; }
        public string Message { get; set; }
    }

}
