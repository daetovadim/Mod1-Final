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
        private char[] primarly = { '*', '/' };
        private char[] secondary = { '+', '-' };
        private char[] numbers = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
        private int indOfSecondary;
        private int indOfPrimarly;
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
            else if (Field == "0" || Field == "+")
            {
                Field = "";
                Field += p.ToString();
                if (FieldToShow == "+")
                    FieldToShow = null;
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
                else if (Field.Contains("^"))
                {
                    int indToExp = Field.StartsWith("-") ? 0 : Field.LastIndexOfAny(new char[] { '+', '-', '*', '/' }) + 1;
                    Double.TryParse(Field.Substring(indToExp, Field.LastIndexOf('^') - indToExp), out double val);
                    Field = Field.Remove(indToExp).Insert(indToExp, Math.Pow(val, Convert.ToDouble(expr)).ToString());
                }
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
                indOfSecondary = Field.LastIndexOfAny(secondary);
                indOfPrimarly = Field.LastIndexOfAny(primarly);
                if (indOfPrimarly != -1 && indOfSecondary != -1 && indOfPrimarly > indOfSecondary && !Field.StartsWith("-"))
                {
                    expression = Field.Substring(indOfSecondary + 1);
                    if (expression.Contains("*"))
                        Field = Field.Remove(indOfSecondary + 1).Insert(indOfSecondary + 1, Calc.Mult(expression));
                    else if (expression.Contains("/"))
                        Field = Field.Remove(indOfSecondary + 1).Insert(indOfSecondary + 1, Calc.Div(expression));
                    int openBrInd = Field.LastIndexOf('(') == -1 ? 0 : Field.LastIndexOf('(');
                    expression = Field.Substring(Field.LastIndexOf('(') + 1);
                    if (Field.Substring(openBrInd).Contains("+"))
                        Field = Field.Remove(openBrInd).Insert(openBrInd, Calc.Add(expression));
                    else if (Field.Substring(openBrInd).Contains("-"))
                        Field = Field.Remove(openBrInd).Insert(openBrInd, Calc.Sub(expression));
                }
                else if (indOfPrimarly == -1)
                {
                    int openBrInd = Field.LastIndexOf('(') == -1 ? 0 : Field.LastIndexOf('(');
                    expression = Field.Substring(Field.LastIndexOf('(') + 1);
                    if (Field.Substring(openBrInd).Contains("+"))
                        Field = Field.Remove(openBrInd).Insert(openBrInd, Calc.Add(expression));
                    else if (Field.StartsWith("-"))
                    {

                    }
                    else if (Field.Substring(openBrInd).Contains("-"))
                        Field = Field.Remove(openBrInd).Insert(openBrInd, Calc.Sub(expression));

                }
                else if (indOfSecondary == -1)
                {
                    int openBrInd = Field.LastIndexOf('(') == -1 ? 0 : Field.LastIndexOf('(');
                    expression = Field.Substring(Field.LastIndexOf('(') + 1);
                    if (Field.Substring(openBrInd).Contains("*"))
                        Field = Field.Remove(openBrInd).Insert(openBrInd, Calc.Mult(expression));
                    else if (Field.Substring(openBrInd).Contains("/"))
                        Field = Field.Remove(openBrInd).Insert(openBrInd, Calc.Div(expression));
                }
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
            int openBrInd = Field.LastIndexOf('(') == -1 ? 0 : Field.LastIndexOf('(');
            expression = Field.Substring(Field.LastIndexOf('(') + 1);
            if (Field.Substring(openBrInd).Contains("*"))
                Field = Field.Remove(openBrInd).Insert(openBrInd, Calc.Mult(expression));
            else if (Field.Substring(openBrInd).Contains("/"))
                Field = Field.Remove(openBrInd).Insert(openBrInd, Calc.Div(expression));
            else if (Field.Substring(openBrInd).Contains("+"))
                Field = Field.Remove(openBrInd).Insert(openBrInd, Calc.Add(expression));
            else if (Field.Substring(openBrInd).Contains("-"))
                Field = Field.Remove(openBrInd).Insert(openBrInd, Calc.Sub(expression));
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
                openBrCounter++;
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

        #region PlusMinusCommand
        public ICommand PlusMinusCommand { get; }
        private void OnPlusMinusCommandExecute(object p)
        {
            indOfPrimarly = Field.LastIndexOfAny(primarly);
            indOfSecondary = Field.LastIndexOfAny(secondary);
            int plusInd = Field.LastIndexOf('+');
            int minusInd = Field.LastIndexOf('-');
            if (indOfPrimarly > indOfSecondary)
            {
                Field = Field.Insert(indOfPrimarly + 1, "-");
                FieldToShow = FieldToShow.Insert(FieldToShow.LastIndexOfAny(primarly), "-");
            }
            else if (plusInd > minusInd)
            {
                Field = Field.Remove(plusInd, 1).Insert(plusInd, "-");
                FieldToShow = FieldToShow.Remove(FieldToShow.LastIndexOf('+'), 1).Insert(FieldToShow.LastIndexOf('+'), "-");
            }
            else if (minusInd > plusInd)
            {
                Field = Field.Remove(minusInd, 1).Insert(minusInd, "+");
                FieldToShow = FieldToShow.Remove(FieldToShow.LastIndexOf('-'), 1).Insert(FieldToShow.LastIndexOf('-'), "+");
            }
            else
            {
                Field = Field == "0" ? "-" : Field.Insert(0, "-");
                FieldToShow = FieldToShow?.Insert(0, "-") ?? "-";
            }
        }
        private bool CanPlusMinusCommandExecuted(object p)
        {
            if (Field.LastIndexOfAny(numbers) == Field.Length - 1 || Field.EndsWith("-") || Field.EndsWith("+"))
                return true;
            else
                return false;
        }
        #endregion

        #region PercentCommand
        public ICommand PercentCommand { get; }
        private void OnPercentCommandExecute(object p)
        {
            int startInd = Field.LastIndexOfAny(operators) + 1;
            Double.TryParse(Field.Substring(startInd), out double result);
            Field = Field.Remove(startInd).Insert(startInd, (result / 100).ToString());
            FieldToShow += "%";
        }
        private bool CanPercentCommandExecuted(object p)
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
            PlusMinusCommand = new RelayCommand(OnPlusMinusCommandExecute, CanPlusMinusCommandExecuted);
            PercentCommand = new RelayCommand(OnPercentCommandExecute, CanPercentCommandExecuted);
        }
    }
}
