using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class Program
    {
        String finale = "iB6WcuCG3nq+fZkoGgneegMtA5SRRL9yH0vUeN56FgbikZFE1HhTM9R4tZPghhYGFgbUeHB4tEKRRNR4Ymu0OwljQwmRRNR4jWBweOKRRyCRRAljLGQ=";
        string alphabet = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUV";


        static void Main(string[] args)
        {
            String ding = Console.ReadLine();
            int test = checkString(Program.Enc(ding, 9157, 41117), "iB6WcuCG3nq+fZkoGgneegMtA5SRRL9yH0vUeN56FgbikZFE1HhTM9R4tZPghhYGFgbUeHB4tEKRRNR4Ymu0OwljQwmRRNR4jWBweOKRRyCRRAljLGQ=");
            Console.WriteLine(test);
        }

        private static String btn_check_Click(String input)
        {
            if (Program.Enc(input, 9157, 41117) != "iB6WcuCG3nq+fZkoGgneegMtA5SRRL9yH0vUeN56FgbikZFE1HhTM9R4tZPghhYGFgbUeHB4tEKRRNR4Ymu0OwljQwmRRNR4jWBweOKRRyCRRAljLGQ=")
            {
                return "Try again!";
            }
            else
            {
                return "Correct!! You found FLAG";
            }
        }

        public static int checkString(String neuer, String alter)
        {
            int index = 0;
            for (int i = 0; i < (int)neuer.Length; i++)
            {
                if (neuer[i] == alter[i])
                {
                    index += 1;
                }
                else
                {
                    return index;
                }
            }
            return index;
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
                numArray1[i] = Program.Mod(numArray[i], e, n);
            }
            string str = "";
            for (i = 0; i < (int)numArray.Length; i++)
            {
                str = string.Concat(str, (char)numArray1[i]);
            }

            // Console.WriteLine(str);

            //Console.WriteLine(Convert.ToBase64String(Encoding.Unicode.GetBytes(str)));


            return Convert.ToBase64String(Encoding.Unicode.GetBytes(str));
        }

        public static int Mod(int m, int e, int n)
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

    }
}

