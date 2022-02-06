﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mod1_Final.Models
{
    static class Calc
    {
        public static string Add(string expr)
        {
            string[] members= expr.Split('+');
            string x = (Convert.ToDouble(members[0]) + Convert.ToDouble(members[1])).ToString();
            return x;
        }
        public static string Sub(string expr)
        {
            string[] members = expr.Split('-');
            string x = (Convert.ToDouble(members[0]) - Convert.ToDouble(members[1])).ToString();
            return x;
        }
        public static double Mult(string expr)
        {
            string[] members = expr.Split('*');
            double x = Convert.ToDouble(members[0]) * Convert.ToDouble(members[1]);
            return x;
        }
        public static double Div(string expr)
        {
            string[] members = expr.Split('/');
            double x = Convert.ToDouble(members[0]) / Convert.ToDouble(members[1]);
            return x;
        }
        public static bool BracketsCheck(string s, out int[,] indArray, int a = 0) //Метод определяет правильность расстановки скобок и определяет очерёдность их раскрытия
        {
            if (a == 0)
            {
                indArray = null;
                return true;
            }
            else
            {
                Stack<char> brackets = new Stack<char>();
                Stack<int> brInd = new Stack<int>();
                indArray = new int[a, 2];
                int r = 0; //Номер строки в массиве для индексов открывающих и закрывающих скобок
                for (int i = 0; i < s.Length; i++)
                {
                    if (s[i] == '(')
                    {
                        brackets.Push(')');
                        brInd.Push(i);
                    }
                    else if (brackets.Count != 0)
                    {
                        if (s[i] == brackets.Peek())
                        {
                            brackets.Pop();
                            indArray[r, 0] = brInd.Pop();
                            indArray[r, 1] = i;
                            r++;
                        }
                    }
                }
                if (brackets.Count == 0)
                    return true;
                else
                    return false;
            }


        }
    }
}
