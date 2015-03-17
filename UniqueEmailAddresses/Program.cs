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
                        var header = new CitiSmtpHeader(currentField, "®");
                        currentAddresses.AddRange(header.XZantazRecipValues);
                    }
                    else
                    {
                        currentAddresses = DelimitedFile.ParseMultiValueField(currentField, ';', true, true).ToList();
                    }
                    foreach (var address in currentAddresses)
                    {
                        //if (!uniqueAddresses.Contains(address)) uniqueAddresses.Add(address);
                        //TODO: see if can get comparison working faster than this at this stage (real hit is the comparing every single string in the list)
                        if (uniqueAddresses.Count == 0)
                        {
                            uniqueAddresses.Add(address);
                            continue;
                        }
                        var minimumStringComparison = string.Compare(address, uniqueAddresses[0], StringComparison.InvariantCultureIgnoreCase);
                        if (minimumStringComparison == 0) continue;
                        if (minimumStringComparison < 0)
                        {
                            uniqueAddresses.Insert(0, address);
                            continue;
                        }
                        var maximumStringComparison = string.Compare(address, uniqueAddresses.Last(), StringComparison.InvariantCultureIgnoreCase);
                        if (maximumStringComparison == 0) continue;
                        if (maximumStringComparison > 0)
                        {
                            uniqueAddresses.Add(address);
                            continue;
                        }
                        var lowerBound = 0;
                        var upperBound = uniqueAddresses.Count - 1;
                        var located = false;
                        while (!located)
                        {
                            if (upperBound - lowerBound <= 1)
                            {
                                located = true;
                                uniqueAddresses.Insert(upperBound, address);
                                upperBound = uniqueAddresses.Count - 1;
                                continue;
                            }
                            var midPoint = (int)Math.Round((double)((upperBound - lowerBound) / 2) + lowerBound, 0);
                            if (string.Compare(address, uniqueAddresses[midPoint], StringComparison.InvariantCultureIgnoreCase) == 0)
                            {
                                located = true;
                                continue;
                            }
                            if (string.Compare(address, uniqueAddresses[lowerBound], StringComparison.InvariantCultureIgnoreCase) > 0 && string.Compare(address, uniqueAddresses[midPoint], StringComparison.InvariantCultureIgnoreCase) < 0)
                            {
                                upperBound = midPoint;
                                continue;
                            }
                            if (string.Compare(address, uniqueAddresses[midPoint], StringComparison.InvariantCultureIgnoreCase) > 0 && string.Compare(address, uniqueAddresses[upperBound], StringComparison.InvariantCultureIgnoreCase) < 0)
                            {
                                lowerBound = midPoint;
                                continue;
                            }
                        }
                        /*if (uniqueAddresses.Count == 0)
                        {
                            uniqueAddresses.Add(address);
                            continue;
                        }
                        var stringRelationship = string.Compare(address, uniqueAddresses[0],
                            StringComparison.InvariantCultureIgnoreCase);
                        if (stringRelationship == 0)
                        {
                            continue;
                        }
                        if (stringRelationship < 0)
                        {
                            uniqueAddresses.Insert(0, address);
                            continue;
                        }
                        if (stringRelationship > 0)
                        {
                            var firstNextAddress = uniqueAddresses.FirstOrDefault(a => string.Compare(address, a) <= 0);
                            if (firstNextAddress == null)
                            {
                                uniqueAddresses.Add(address);
                                continue;
                            }
                            var location = uniqueAddresses.IndexOf(firstNextAddress);
                            if (string.Compare(address, uniqueAddresses[location], StringComparison.InvariantCultureIgnoreCase) == 0) continue;
                            uniqueAddresses.Insert(location, address);
                        }*/
                    }
                }
                iDf.GetNextRecord();
            }
            //uniqueAddresses.Sort();
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
