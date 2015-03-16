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
        {
            XZantazRecipValues = new List<string>(ParseXZantazRecip());
        }

        #endregion

        #region Public Properties and Methods

        public List<string> XZantazRecipValues { get; private set; }
        
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

        #endregion

        #region Methods

        private List<string> ParseXZantazRecip()
        {
            var wholeField = GetXZantazRecip();
            var retVal = new List<string>();
            foreach (var fieldInstance in wholeField)
            {
                var cleanFieldInstance = fieldInstance.Replace("\t", string.Empty);
                var explodedCleanFieldInstance = cleanFieldInstance.Split(new[] { ',' }).ToList();
                var buffer = new StringBuilder();
                for (var i = 0; i < explodedCleanFieldInstance.Count; i++)
                {
                    if (explodedCleanFieldInstance[i].StartsWith(@"""") && explodedCleanFieldInstance[i].EndsWith(","))
                    {
                        buffer.Append(explodedCleanFieldInstance);
                        continue;
                    }
                    if (buffer.Length > 0)
                    {
                        buffer.Append(explodedCleanFieldInstance[i]);
                        retVal.Add(buffer.ToString());
                        buffer.Clear();
                        continue;
                    }
                    retVal.Add(explodedCleanFieldInstance[i]);
                }
            }
            return retVal;
        }

        #endregion
    }
}
