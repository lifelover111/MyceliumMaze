#if UNITY_EDITOR

using System.Text.RegularExpressions;

namespace MBS.Utilities.Helpers
{
    public  static class Text_Helper
    {
        public  static string TrimAndRemoveDoubleSpaces( string text )
        {
            var clearedText = text.Trim( );
            clearedText = Regex.Replace( clearedText, @"\s+", " " );
            return clearedText;
        }

        public  static bool IsTextLengthOk( string name, int minCharNumber = 2 )
        {
            if ( name.Length < minCharNumber ) return false;

            return true;
        }

        public  static bool IsTextContainsOnlyLettersAndDigits( string text )
        {
            var isValid = true;

            for ( var c = 0; c < text.Length; c++ )
                if ( text[ c ] != '(' && text[ c ] != ')' && text[ c ] != '_' && text[ c ] != '.' &&
                     !char.IsLetterOrDigit( text[ c ] ) && !char.IsWhiteSpace( text[ c ] ) )
                    isValid = false;

            return isValid;
        }
    }
}

#endif