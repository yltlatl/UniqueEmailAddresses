using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniqueEmailAddresses
{
    class Program
    {
        static void Main(string[] args)
        {
            var inPath = args[0];
            var outPath = args[1];
            var iDf = new DelimitedFile(inPath, "Windows-1252", '\n', ',', ';', '"');
            iDf.GetNextRecord();
            var headerRecord = iDf.HeaderRecord.ToList();
            for (var i = 0; i < headerRecord.Count; i++)
            {
                Console.WriteLine("{0}\t{1}", i, headerRecord[i]);
            }
            Console.WriteLine("Enter numbers of fields to process separated by commas.");
            string fieldChoicesString;
            do
            {
                fieldChoicesString = Console.ReadLine();
            } while (string.IsNullOrEmpty(fieldChoicesString));
            var fieldChoices = fieldChoicesString.Split(new[] {','}).Select(c => Convert.ToInt16(c)).ToList();
            Console.WriteLine("Enter number of the email header field, if any, or -1, if none.");
            short? headerField = null;
            do
            {
                try
                {
                    headerField = Convert.ToInt16(Console.ReadLine());
                }
                catch
                {
                    Console.WriteLine("Enter an integer corresponding to the header field, if any, or -1, if none.");
                    headerField = null;
                }
            } while (headerField == null);
            var uniqueAddresses = new List<string>();
            while (!iDf.EndOfFile)
            {
                Console.WriteLine("Processing line {0}.", iDf.CurrentLineNumber);
                foreach (var field in fieldChoices)
                {
                    var currentAddresses = new List<string>();
                    var currentField = iDf.GetFieldByPosition(field);
                    if (field == headerField)
                    {
                        var header = new SmtpHeader(currentField, "®");
                        foreach (var customHeader in CustomHeaders.Values)
                        {
                            var currentHeaderFieldList = header.GetCustomField(customHeader);
                            //most of the time where will only be one item in the currentHeaderFieldList
                            foreach (var currentHeaderField in currentHeaderFieldList)
                            {
                                currentAddresses.AddRange(DelimitedFile.ParseMultiValueField(currentHeaderField, ',', true, true));
                            }
                        }
                    }
                    else
                    {
                        currentAddresses = DelimitedFile.ParseMultiValueField(currentField, ';', true, true).ToList();
                    }
                    foreach (var address in currentAddresses)
                    {
                        if (uniqueAddresses.Count == 0)
                        {
                            uniqueAddresses.Add(address);
                            break;
                        }
                        var stringRelationship = string.Compare(address, uniqueAddresses[0],
                            StringComparison.InvariantCultureIgnoreCase);
                        if (stringRelationship == 0)
                        {
                            break;
                        }
                        if (stringRelationship < 0)
                        {
                            uniqueAddresses.Insert(0, address);
                            break;
                        }
                        if (stringRelationship > 0)
                        {
                            uniqueAddresses.Add(address);
                        }
                    }
                }
                iDf.GetNextRecord();
            }
            using (var str = new StreamWriter(outPath))
            {
                foreach (var address in uniqueAddresses)
                {
                    str.WriteLine(address);
                }
            }
        }
    }

    public struct CustomHeaders
    {
        public static readonly string[] Values = {"X-Zantaz-Recip:", "X-Zantaz-Bcc:", "X-Zantaz-MailboxOwner:"};
    }
}
