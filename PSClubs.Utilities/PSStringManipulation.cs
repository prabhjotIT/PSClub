using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;

namespace PSClubsUtilities
{
    public static class PSStringManipulation
    {
       public static string PSExtractDigits(string input)
        {
            return new String(input.Where(Char.IsDigit).ToArray());
        }
        public static bool PSPostalCodeIsValid(string postalCode,string postalRegex)
        {
            Regex pattern = new Regex(postalRegex, RegexOptions.IgnoreCase);
            if (postalCode == null || pattern.IsMatch(postalCode.ToString()))
                return true;
            return false;
                        
        }
        public static string PSCapitalize(string input)
        {
            if (input == null)
            {
                return "it is an empty string. ";
            }
            else
            {
                TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;

                input = input.Trim().ToLower();
                string output=textInfo.ToTitleCase(input);
                return output;
            }

        }
    }
}
//var listOfWords = input.Split(' ').ToList();
//foreach (string word in listOfWords)
//{
//                    //var wordChar=word.ToCharArray();
//                    = char.ToUpper(word[0]);//convert first letter to its upper case equalent :)
//                                            //simply write in one line when you came back 

//}
//return string.Join(" ", output);//conjoining the list into single string and returning
