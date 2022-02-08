using System;
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
        private string field;
        private string fieldToShow;
        private string expression;
        private byte openBrCounter = 0;
        private byte closeBrCounter = 0;
        private char[] operators = { '+', '-', '*', '/', '(' };
        private char[] numbers = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
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
        public string FieldToShow
        {
            get
            {
                return fieldToShow;
            }
            set
            {
                fieldToShow = value;
                OnPropertyChanged();
            }
        }
        #endregion Properties

        #region NumCommand
        public ICommand NumCommand { get; }
        private void OnNumCommandExecute(object p)
        {
            if ((string)p == "," && Field.LastIndexOfAny(numbers) == Field.Length - 1)
            {
                Field += p.ToString();
                FieldToShow += p.ToString();
            }
            else if (Field == "0")
            {
                Field = "";
                Field += p.ToString();
                FieldToShow += p.ToString();
            }
            else if (!(Field.EndsWith(")") || (string)p == ","))
            {
                Field += p.ToString();
                FieldToShow += p.ToString();
            }
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
                FieldToShow = (string)p;
                openBrCounter++;
            }
            else if ((string)p == ")" && (Field.LastIndexOfAny(numbers) == Field.Length - 1 || Field.Substring(Field.Length - 1) == ")") && closeBrCounter < openBrCounter)
            {
                FieldToShow += (string)p;
                int openBrInd = Field.LastIndexOf('(');
                string expr = Field.Substring(openBrInd + 1);
                if (expr.Contains("+"))
                    Field = Field.Remove(openBrInd).Insert(openBrInd, Calc.Add(expr));
                else if (expr.Contains("-"))
                    Field = Field.Remove(openBrInd).Insert(openBrInd, Calc.Sub(expr));
                else if (expr.Contains("*"))
                    Field = Field.Remove(openBrInd).Insert(openBrInd, Calc.Mult(expr));
                else if (expr.Contains("/"))
                    Field = Field.Remove(openBrInd).Insert(openBrInd, Calc.Div(expr));
                closeBrCounter++;
            }
            else if ((string)p == "(" && !(Field.EndsWith(",") || Field.EndsWith(")") || (Field.LastIndexOfAny(numbers) == Field.Length - 1)))
            {
                Field += (string)p;
                FieldToShow += (string)p;
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
            if ((string)p == "*" || (string)p == "/")
            {
                int openBrInd = Field.LastIndexOf('(') == -1 ? 0 : Field.LastIndexOf('(');
                expression = Field.Substring(Field.LastIndexOf('(') + 1);
                if (Field.Substring(openBrInd).Contains("*"))
                    Field = Field.Remove(openBrInd).Insert(openBrInd, Calc.Mult(expression));
                else if (Field.Substring(openBrInd).Contains("/"))
                    Field = Field.Remove(openBrInd).Insert(openBrInd, Calc.Div(expression));
            }
            else if ((string)p == "+" || (string)p == "-")
            {
                int openBrInd = Field.LastIndexOf('(') == -1 ? 0 : Field.LastIndexOf('(');
                expression = Field.Substring(Field.LastIndexOf('(') + 1);
                if (Field.Substring(openBrInd).Contains("+"))
                    Field = Field.Remove(openBrInd).Insert(openBrInd, Calc.Add(expression));
                else if (Field.Substring(openBrInd).Contains("-"))
                    Field = Field.Remove(openBrInd).Insert(openBrInd, Calc.Sub(expression));
            }
            if (Field.LastIndexOfAny(numbers) == Field.Length - 1)
            {
                Field += (string)p;
                FieldToShow += (string)p;
            }
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
            else if (Field.Contains("^"))
            {

            }

        }
        private bool CanCountCommandExecuted(object p)
        {
            if ((Field != null || Field != "0") && openBrCounter == closeBrCounter)
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
                FieldToShow = null;
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
                    FieldToShow = FieldToShow.Remove(FieldToShow.Length - 1, 1);
                }
                else
                {
                    Field = "0";
                    FieldToShow = null;
                }
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
            {
                Double.TryParse(Field.Substring(Field.LastIndexOfAny(operators) + 1), out double v);
                Field += "^(";
                FieldToShow += "^(";
            }
            else
            {
                Double.TryParse(Field.Substring(Field.LastIndexOfAny(operators) + 1), out double val);
                val = Math.Sqrt(val);
                Field = Field.Remove(Field.LastIndexOfAny(operators) + 1) + val.ToString();
            }

        }
        private bool CanPowCommandExecuted(object p)
        {
            if (Field.LastIndexOfAny(numbers) == Field.Length - 1)
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
