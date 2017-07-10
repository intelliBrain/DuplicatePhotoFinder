using System;
using System.Collections.Generic;

namespace DupeFinder
{
    public class FileMatch
    {
        public FileMatch(string fullName, string matchName, int copyNumber, string extension, Dictionary<string, string> testProps)
        {
            FullName = fullName;
            MatchName = matchName;
            CopyNumber = copyNumber;
            Extension = extension;
               
            TestProps = testProps;
        }

        public string Extension { get; }
        public Dictionary<string, string> TestProps { get; }
        public string FullName { get; }
        public string MatchName { get; }
        public int CopyNumber { get; }

        public bool IsDuplicateOf(FileMatch other)
        {
            if (FullName == other.FullName)
                return false;

            return String.Equals(Extension, other.Extension) 
                   && String.Equals(MatchName.ToLower(), other.MatchName.ToLower()) 
                   && CopyNumber != other.CopyNumber
                   && AllTestPropsMatch(other);
        }

        private bool AllTestPropsMatch(FileMatch other)
        {
            if (TestProps.Count != other.TestProps.Count)
                return false;

            bool allTestPropsMatch = true;
            foreach (var kvp in TestProps)
            {
                var thisTestProp = kvp.Value;
                if (other.TestProps.TryGetValue(kvp.Key, out string otherTestProp))
                {
                    if (otherTestProp != thisTestProp)
                        return false;
                }
                else
                {
                    return false;
                }
            }
            return allTestPropsMatch;
        }
    }
}