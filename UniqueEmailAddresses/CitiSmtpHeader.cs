using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniqueEmailAddresses
{
    internal class CitiSmtpHeader : SmtpHeader
    {
        #region Constructors

        public CitiSmtpHeader(string header, string newline)
            : base(header, newline)
        { }

        #endregion

        #region Public Properties and Methods

        public List<string> GetXZantazRecip()
        {
            return GetField("x-zantaz-recip:");
        }

        public List<string> GetXZantazBcc()
        {
            return GetField("x-zantaz-bcc:");
        }

        public List<string> GetXOriginalQueueid()
        {
            return GetField("x-original-queueid:");
        }

        public List<string> XZantazSequence()
        {
            return GetField("x-zantaz-sequence:");
        }

        public List<string> XEnvSender()
        {
            return GetField("x-env-sender:");
        }

        public List<string> XMailboxOwnerNames()
        {
            return GetField("x-mailbox-owner-names:");
        }


        public static List<string> ParseField(string field)
        {
            
        }
        #endregion
    }
}
