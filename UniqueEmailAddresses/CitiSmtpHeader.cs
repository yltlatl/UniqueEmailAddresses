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
            var buffer = new StringBuilder();
            var openQuote = false;
            foreach (var fieldInstance in wholeField)
            {
                for (var i = 0; i < fieldInstance.Length; i++)
                {
                    var character = fieldInstance[i];
                    char? trailingCharacterOne = null;
                    if (i > 0) trailingCharacterOne = fieldInstance[i - 1];
                    char? trailingCharacterTwo = null;
                    if (i > 1) trailingCharacterTwo = fieldInstance[i - 2];
                    if (character != '"' && character != ',')
                    {
                        buffer.Append(character);
                        continue;
                    }
                    if (character == '"')
                    {
                        buffer.Append(character);
                        if (trailingCharacterOne == '"' && openQuote)
                        {
                            openQuote = false;
                            continue;
                        }
                        if (trailingCharacterOne == '"' && trailingCharacterTwo == '"')
                        {
                            continue;
                        }
                        if (trailingCharacterOne == '"')
                        {
                            openQuote = true;
                            continue;
                        }
                    }
                    if (character == ',' && openQuote)
                    {
                        buffer.Append(character);
                        continue;
                    }
                    if (character == ',')
                    {
                        retVal.Add(buffer.ToString());
                        buffer.Clear();
                    }
                }
                if (buffer.Length > 0)
                {
                    retVal.Add(buffer.ToString());
                    buffer.Clear();
                }
            }
            return retVal.Select(v => v.Trim()).Select(u => u.Replace("\t", " ")).ToList();
        }

        #endregion
    }
}
