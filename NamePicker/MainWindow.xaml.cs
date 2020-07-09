using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace NamePicker
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int NameLength = 6;
        private string SelectedColor = "";
        public MainWindow()
        {
            InitializeComponent();
        }
        private void ClrPcker_Background_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            lb_color.Content = SelectedColor = "#" + ClrPcker_Background.SelectedColor.Value.R.ToString() + ClrPcker_Background.SelectedColor.Value.G.ToString() + ClrPcker_Background.SelectedColor.Value.B.ToString();
        }
        private void btn_convert_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(SelectedColor))
            {
                lb_result.Content = ColorToName(SelectedColor, NameLength, (bool)cb_vocal.IsChecked);
            }
        }
        private void btn_clip_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(lb_result.Content.ToString());
        }
        public static string ColorToName(string input, int length, bool makeSecondLetterVocal = true)
        {
            StringBuilder strIn = new StringBuilder(input.Trim('#'));
            StringBuilder strOut = new StringBuilder(length);
            // vocals for second letter substitution
            char[] vocals = { 'a', 'e', 'i', 'o', 'u' };

            // names shouldn't be 1 letter long
            length = (length < 2) ? 2 : length;

            // build a key to manipulate with from the selected color
            // -> multiplied by the length to change the result completely based on lenght
            int key =  input.Select(x => (int)x).Sum() * length;
            // Add input to input until it's at least longer than the desired lenght
            while(strIn.Length < length)
            {
                strIn.Append(strIn);
            }
            // char that will be substituted with
            char sub;
            for (int i = 0; i < length; i++)
            {
                // current character
                sub = strIn[i];
                // XOR Comparing bits of the char "sub" with bits of the "key" (times the iterator to avoid double letters a bit (+1 because "times 0" is stupid.))
                sub = (char)(sub ^ (key * (i + 1)));
                // make sure resulting chars are in the "letter" range of unicode
                sub = (char)((sub % 26) + 97);
                strOut.Append(sub);
            }
            // capitalise the first letter
            strOut[0] = char.ToUpper(strOut[0]);
            // Make the second letter a vocal. Just looks better for names
            if (makeSecondLetterVocal)
            {
                strOut[1] = vocals[(strOut[1] % vocals.Length)];
            }
            // a lil loggin'
            Console.WriteLine("Selected Color: #" + strIn);
            Console.WriteLine("Selected Lenght: " + length);
            Console.WriteLine("Output: " + strOut);
            return strOut.ToString();
        }
        
        private void tb_length_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (int.TryParse(tb_length.Text, out NameLength))
            {
                if (NameLength < 2)
                {
                    NameLength = 2;
                }
                if (NameLength > 20)
                {
                    NameLength = 20;
                }
                tb_length.Text = NameLength.ToString();
            }
        }
    }
}
