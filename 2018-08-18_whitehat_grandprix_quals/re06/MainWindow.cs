using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace WpfApp1
{
    public class MainWindow : Window, IComponentConnector
    {
        internal TextBox tb_key;

        internal Button btn_check;

        private bool _contentLoaded;

        public MainWindow()
        {
            this.InitializeComponent();
        }

        private void btn_check_Click(object sender, RoutedEventArgs e)
        {
            if (MainWindow.Enc(this.tb_key.Text, 9157, 41117) != "iB6WcuCG3nq+fZkoGgneegMtA5SRRL9yH0vUeN56FgbikZFE1HhTM9R4tZPghhYGFgbUeHB4tEKRRNR4Ymu0OwljQwmRRNR4jWBweOKRRyCRRAljLGQ=")
            {
                MessageBox.Show("Try again!");
            }
            else
            {
                MessageBox.Show("Correct!! You found FLAG");
            }
        }

        public static string Enc(string s, int e, int n)
        {
            int i;
            int[] numArray = new int[s.Length];
            for (i = 0; i < s.Length; i++)
            {
                numArray[i] = s[i];
            }
            int[] numArray1 = new int[(int)numArray.Length];
            for (i = 0; i < (int)numArray.Length; i++)
            {
                numArray1[i] = MainWindow.mod(numArray[i], e, n);
            }
            string str = "";
            for (i = 0; i < (int)numArray.Length; i++)
            {
                str = string.Concat(str, (char)numArray1[i]);
            }
            return Convert.ToBase64String(Encoding.Unicode.GetBytes(str));
        }

        [DebuggerNonUserCode]
        [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent()
        {
            if (!this._contentLoaded)
            {
                this._contentLoaded = true;
                Application.LoadComponent(this, new Uri("/WpfApp1;component/mainwindow.xaml", UriKind.Relative));
            }
        }

        public static int mod(int m, int e, int n)
        {
            int[] numArray = new int[100];
            int num = 0;
            do
            {
                numArray[num] = e % 2;
                num++;
                e /= 2;
            }
            while (e != 0);
            int num1 = 1;
            for (int i = num - 1; i >= 0; i--)
            {
                num1 = num1 * num1 % n;
                if (numArray[i] == 1)
                {
                    num1 = num1 * m % n;
                }
            }
            return num1;
        }

        [DebuggerNonUserCode]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target)
        {
            int num = connectionId;
            if (num == 1)
            {
                this.tb_key = (TextBox)target;
            }
            else if (num == 2)
            {
                this.btn_check = (Button)target;
                this.btn_check.Click += new RoutedEventHandler(this.btn_check_Click);
            }
            else
            {
                this._contentLoaded = true;
            }
        }
    }
}
