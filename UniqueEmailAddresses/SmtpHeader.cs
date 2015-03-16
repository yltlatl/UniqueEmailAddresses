using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;


namespace UniqueEmailAddresses
{
    internal class SmtpHeader
    {
        #region Constructors

        public SmtpHeader(string header, string newline)
        {
            Fields = new List<Tuple<string, string>>(ParseHeader(header, newline));
        }

        #endregion

        #region Properties
        private List<Tuple<string, string>> Fields { get; set; }

        
        #endregion

        #region Public Methods
        public List<string> GetCustomField(string fieldName)
        {
            var lFieldName = fieldName.ToLowerInvariant();
            return GetField(lFieldName);
        }

        public List<string> GetFrom()
        {
            return GetField("from:");
        }

        public List<string> GetTo()
        {
            return GetField("to:");
        }

        public List<string> GetCc()
        {
            return GetField("cc:");
        }

        public List<string> GetBcc()
        {
            return GetField("bcc:");
        }

        public List<string> GetDate()
        {
            return GetField("date:");
        }

        public List<string> GetInReplyTo()
        {
            return GetField("in-reply-to:");
        }

        public List<string> GetMessageId()
        {
            return GetField("message-id:");
        }

        public List<string> GetReceived()
        {
            return GetField("received:");
        }

        public List<string> GetReplyTo()
        {
            return GetField("reply-to:");
        }

        public List<string> GetReturnPath()
        {
            return GetField("return-path:");
        }

        public List<string> GetSender()
        {
            return GetField("sender:");
        }

        public List<string> GetSubject()
        {
            return GetField("subject:");
        }

        #endregion

        #region Methods

        private static List<Tuple<string, string>> ParseHeader(string header, string newline)
        {
            string[] nlArray = {newline};
            var explodedHeader = header.Split(nlArray, StringSplitOptions.RemoveEmptyEntries);
            var headerFields = new List<string>();
            foreach (var t in explodedHeader)
            {
                var str = new StringBuilder();
                str.Append(t);
                var initialCharacter = t[0];
                if (char.IsWhiteSpace(initialCharacter))
                {
                    if (headerFields.Count > 0)
                    {
                        str.Insert(0, headerFields.Last());
                        headerFields.RemoveAt(headerFields.Count - 1);
                    }
                }
                headerFields.Add(str.ToString());
            }
            var retVal = new List<Tuple<string, string>>();
            var components = headerFields.Select(headerField => headerField.Split(new[] {": "}, 2, StringSplitOptions.None)).ToList();
            foreach (var component in components)
            {
                switch (component.Length)
                {
                    case 1:
                    {
                        var field = Tuple.Create(string.Format("{0}:", component[0]), string.Empty);
                        retVal.Add(field);
                    }
                        break;
                    case 2:
                    {
                        var field = Tuple.Create(string.Format("{0}:", component[0]), component[1]);
                        retVal.Add(field);
                    }
                        break;
                    default:
                        throw new InvalidDataException("Failed to parse field into appropriate parts.");
                }
            }
            return retVal;
        }

        protected List<string> GetField(string fieldName)
        {
            var fieldInstances = Fields.Where(t => t.Item1.ToLowerInvariant().Equals(fieldName)).Select(tuple => tuple.Item2).ToList();
            return fieldInstances;
        }

        #endregion
    }
}
