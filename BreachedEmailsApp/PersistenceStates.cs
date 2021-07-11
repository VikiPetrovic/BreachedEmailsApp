using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BreachedEmailsApp.Helpers;

namespace BreachedEmailsApp
{
    [Serializable]
    public class EmailsState
    {
        // serializable class, ki vsebuje state grain-a za breached emaile
        public HashSet<string> BreachedEmails { get; set; }

        public EmailsState()
        {
            this.BreachedEmails = new HashSet<string>();
        }
    }
}
