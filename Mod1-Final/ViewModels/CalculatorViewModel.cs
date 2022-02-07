﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Mod1_Final;
using Mod1_Final.Models;

namespace Mod1_Final.ViewModels
{
    class CalculatorViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        void OnPropertyChanged([CallerMemberName] string PropertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
        }

        #region Fields
        private double x;
        private double y;
        private double z;
        private string field;
        private byte openBrCounter = 0;
        private byte closeBrCounter = 0;
        private char[] operators = { '+', '-', '*', '/', '(' };
        #endregion Fields

        #region Properties
        public string Field
        {
            get
            {
                if (field == null)
                    return "0";
                else
                    return field;
            }
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }
        #endregion Properties

        #region NumCommand
        public ICommand NumCommand { get; }
        private void OnNumCommandExecute(object p)
        {
            if ((string)p == "," && Byte.TryParse(Field.Substring(Field.Length - 1), out byte r))
                Field += p.ToString();
            else if (Field == "0")
            {
                Field = "";
                Field += p.ToString();
            }
            else if (!(Field.EndsWith(")") || (string)p == ","))
                Field += p.ToString();
        }
        private bool CanNumCommandExecuted(object p)
        {
            return true;
        }
        #endregion
        #region BracketsCommand
        public ICommand BracketsCommand { get; }
        private void OnBracketsCommandExecute(object p)
        {
            if ((string)p == "(" && Field == "0")
            {
                Field = (string)p;
                openBrCounter++;
            }
            else if ((string)p == ")" && (Byte.TryParse(Field.Substring(Field.Length - 1), out byte r) || Field.Substring(Field.Length - 1) == ")") && closeBrCounter < openBrCounter)
            {
                Field += (string)p;
                closeBrCounter++;
            }
            else if ((string)p == "(" && !(Field.EndsWith(",") || Field.EndsWith(")")))
            {
                Field += (string)p;
                openBrCounter++;
            }
        }
        private bool CanBracketsCommandExecuted(object p)
        {
            return true;
        }
        #endregion
        #region OperationCommand
        public ICommand OperationCommand { get; }
        private void OnOperationCommandExecute(object p)
        {
            if (!(Field.EndsWith("+") || Field.EndsWith("-") || Field.EndsWith("*") || Field.EndsWith("/") || Field.EndsWith(",")))
                Field += (string)p;
        }
        private bool CanOperationCommandExecuted(object p)
        {
            if (Field != null)
                return true;
            else
                return false;
        }
        #endregion
        #region CountCommand
        public ICommand CountCommand { get; }
        private void OnCountCommandExecute(object p)
        {
            if (Field.Contains("("))
            {
                if (Calc.BracketsCheck(Field, out int[,] indArray, openBrCounter))
                {
                    for (int i = 0; i < indArray.Length / 2; i++)
                    {
                        int openBrInd = indArray[i, 0];
                        int closeBrInd = indArray[i, 1];
                        int expressionLength = closeBrInd - openBrInd - 1;
                        string undOperation = Field.Substring(openBrInd, expressionLength + 2);
                        string expression = Field.Substring(openBrInd + 1, expressionLength);
                        if (expression.Contains("+"))
                        {
                            undOperation = Calc.Add(expression).PadRight(expressionLength + 2);
                            Field = Field.Remove(openBrInd, expressionLength + 2).Insert(openBrInd, undOperation);
                        }
                        else if (undOperation.Contains("-"))
                        {
                            undOperation = Calc.Sub(expression).PadRight(expressionLength + 2);
                            Field = Field.Remove(openBrInd, expressionLength + 2).Insert(openBrInd, undOperation);
                        }
                        else if (undOperation.Contains("*"))
                        {
                            undOperation = Calc.Mult(expression).PadRight(expressionLength + 2);
                            Field = Field.Remove(openBrInd, expressionLength + 2).Insert(openBrInd, undOperation);
                        }
                        else if (undOperation.Contains("/"))
                        {
                            undOperation = Calc.Div(expression).PadRight(expressionLength + 2);
                            Field = Field.Remove(openBrInd, expressionLength + 2).Insert(openBrInd, undOperation);
                        }
                    }
                }
            }
            //else if (expression.Contains("+"))
            //{
            //    undOperation = Calc.Add(expression).PadRight(expressionLength + 2);
            //    Field = Field.Remove(openBrInd, expressionLength + 2).Insert(openBrInd, undOperation);
            //}
            //else if (undOperation.Contains("-"))
            //{
            //    undOperation = Calc.Sub(expression).PadRight(expressionLength + 2);
            //    Field = Field.Remove(openBrInd, expressionLength + 2).Insert(openBrInd, undOperation);
            //}
            //else if (undOperation.Contains("*"))
            //{
            //    undOperation = Calc.Mult(expression).PadRight(expressionLength + 2);
            //    Field = Field.Remove(openBrInd, expressionLength + 2).Insert(openBrInd, undOperation);
            //}
            //else if (undOperation.Contains("/"))
            //{
            //    undOperation = Calc.Div(expression).PadRight(expressionLength + 2);
            //    Field = Field.Remove(openBrInd, expressionLength + 2).Insert(openBrInd, undOperation);
            //}
        }
        private bool CanCountCommandExecuted(object p)
        {
            if (Field != null || Field != "0")
                return true;
            else
                return false;
        }
        #endregion
        #region DeleteCommand
        public ICommand DeleteCommand { get; }
        private void OnDeleteCommandExecute(object p)
        {
            if ((string)p == "ac")
            {
                Field = "0";
                openBrCounter = closeBrCounter = 0;
            }
            else
            {
                if (Field.Length > 1)
                {
                    if (Field.Substring(Field.Length - 1) == "(")
                        openBrCounter--;
                    else if (Field.Substring(Field.Length - 1) == ")")
                        closeBrCounter--;
                    Field = Field.Remove(Field.Length - 1, 1);
                }
                else
                    Field = "0";
            }
        }
        private bool CanDeleteCommandExecuted(object p)
        {
            return true;
        }
        #endregion
        #region PowCommand
        public ICommand PowCommand { get; }
        private void OnPowCommandExecute(object p)
        {
            if ((string)p == "^")
                Field += "^(";
            else
            {
                Double.TryParse(Field.Substring(Field.LastIndexOfAny(operators) + 1), out double val);
                val = Math.Sqrt(val);
                Field = Field.Remove(Field.LastIndexOfAny(operators) + 1) + val.ToString();
            }

        }
        private bool CanPowCommandExecuted(object p)
        {
            if (Byte.TryParse(Field.Substring(Field.Length - 1), out byte r))
                return true;
            else
                return false;
        }
        #endregion

        public CalculatorViewModel()
        {
            NumCommand = new RelayCommand(OnNumCommandExecute, CanNumCommandExecuted);
            BracketsCommand = new RelayCommand(OnBracketsCommandExecute, CanBracketsCommandExecuted);
            OperationCommand = new RelayCommand(OnOperationCommandExecute, CanOperationCommandExecuted);
            CountCommand = new RelayCommand(OnCountCommandExecute, CanCountCommandExecuted);
            DeleteCommand = new RelayCommand(OnDeleteCommandExecute, CanDeleteCommandExecuted);
            PowCommand = new RelayCommand(OnPowCommandExecute, CanPowCommandExecuted);
        }
    }
}
