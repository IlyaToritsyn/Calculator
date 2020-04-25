using System;
using System.Windows.Forms;

namespace Calculator
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Ввод цифры или запятой.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InputSymbol(object sender, EventArgs e)
        {
            if (Calculator.IsError)
            {
                return;
            }

            //Если количество символов на дисплее больше 15 и не ожидается нового числа, то отменяем ввод.
            else if (display.Text.Length > 15 && !Calculator.IsNewNumberWaiting)
            {
                return;
            }

            else if (Calculator.IsLastEqualButton)
            {
                display.Text = "0";

                Calculator.SetDefaultValues();
            }

            else
            {

            }

            Button buttonSymbol = (Button)sender;

            //Если повторно вводится запятая, то отменяем ввод.
            if (buttonSymbol.Text == "," && display.Text.Contains(","))
            {
                return;
            }

            //Если ожидается ввод нового числа или в дисплее "0", то стираем старое число.
            else if (Calculator.IsNewNumberWaiting || display.Text == "0")
            {
                display.Text = buttonSymbol.Text != "," ? "" : "0";
            }

            else
            {

            }

            display.Text += buttonSymbol.Text;
            Calculator.IsNewNumberWaiting = false;
        }

        private void ButtonEqual_Click(object sender, EventArgs e)
        {
            if (Calculator.IsNewNumberWaiting || Calculator.IsError)
            {
                return;
            }

            else
            {
                try
                {
                    display.Text = Calculator.Calculate(double.Parse(display.Text)).ToString();
                }

                catch (DivideByZeroException)
                {
                    display.Text = "На 0 делить нельзя";
                }

                Calculator.IsNewNumberWaiting = true;
                Calculator.IsLastEqualButton = true;
            }
        }

        private void Button16_Click(object sender, EventArgs e)
        {
            display.Text = "0";

            Calculator.SetDefaultValues();
        }

        private void Button15_Click(object sender, EventArgs e)
        {
            if (Calculator.IsError || display.Text == "0")
            {
                return;
            }

            //Если хранимое число и число на дисплее равны, то у хранимого числа тоже меняем знак.
            else if (Math.Abs(Calculator.Number - double.Parse(display.Text)) <= 0.00000000001)
            {
                display.Text = display.Text[0] != '-' ? "-" + display.Text : display.Text.Remove(0, 1);
                Calculator.Number = double.Parse(display.Text);
            }

            else
            {
                display.Text = display.Text[0] != '-' ? "-" + display.Text : display.Text.Remove(0, 1);
            }
        }

        private void buttonDeleteLast_Click(object sender, EventArgs e)
        {
            if (Calculator.IsError || Calculator.IsNewNumberWaiting)
            {
                return;
            }

            else
            {
                display.Text = display.Text.Remove(display.Text.Length - 1);

                if (display.Text == "" || display.Text == "-" || display.Text == "-0")
                {
                    display.Text = "0";
                }
            }
        }

        private void Button17_Click(object sender, EventArgs e)
        {
            if (Calculator.IsError)
            {
                return;
            }

            else
            {
                if (!Calculator.IsNewNumberWaiting)
                {
                    try
                    {
                        display.Text = Calculator.Calculate(double.Parse(display.Text)).ToString();
                    }

                    catch (DivideByZeroException)
                    {
                        display.Text = "На 0 делить нельзя";
                    }

                    Calculator.IsNewNumberWaiting = true;
                }

                else
                {

                }

                Calculator.Operation = 1;
                Calculator.IsLastEqualButton = false;
            }
        }

        private void ButtonMinus_Click(object sender, EventArgs e)
        {
            if (Calculator.IsError)
            {
                return;
            }

            else
            {
                if (!Calculator.IsNewNumberWaiting)
                {
                    try
                    {
                        display.Text = Calculator.Calculate(double.Parse(display.Text)).ToString();
                    }

                    catch (DivideByZeroException)
                    {
                        display.Text = "На 0 делить нельзя";
                    }

                    Calculator.IsNewNumberWaiting = true;
                }

                else
                {

                }

                Calculator.Operation = 2;
                Calculator.IsLastEqualButton = false;
            }
        }

        private void ButtonMultiplication_Click(object sender, EventArgs e)
        {
            if (Calculator.IsError)
            {
                return;
            }

            else
            {
                if (!Calculator.IsNewNumberWaiting)
                {
                    try
                    {
                        display.Text = Calculator.Calculate(double.Parse(display.Text)).ToString();
                    }

                    catch (DivideByZeroException)
                    {
                        display.Text = "На 0 делить нельзя";
                    }

                    Calculator.IsNewNumberWaiting = true;
                }

                else
                {

                }

                Calculator.Operation = 3;
                Calculator.IsLastEqualButton = false;
            }
        }

        private void ButtonDivision_Click(object sender, EventArgs e)
        {
            if (Calculator.IsError)
            {
                return;
            }

            else
            {
                if (!Calculator.IsNewNumberWaiting)
                {
                    try
                    {
                        display.Text = Calculator.Calculate(double.Parse(display.Text)).ToString();
                    }

                    catch (DivideByZeroException)
                    {
                        display.Text = "На 0 делить нельзя";
                    }

                    Calculator.IsNewNumberWaiting = true;
                }

                else
                {

                }

                Calculator.Operation = 4;
                Calculator.IsLastEqualButton = false;
            }
        }
    }

    public class Calculator
    {
        public static double Number { get; set; } = 0; //Обработанное число.
        public static int Operation { get; set; } = 1; //Номер операции: 1 - "+", 2 - "-", 3 - "*", 4 - "/".
        public static bool IsNewNumberWaiting { get; set; } = true; //Ожидается ли ввод нового числа (при вводе нового символа стирается старое число).
        public static bool IsError { get; set; } = false; //Произошла ли ошибка.
        public static bool IsLastEqualButton { get; set; } = false; //Нажата ли последней кнопка "=".

        public static void SetDefaultValues()
        {
            Number = 0;
            Operation = 1;
            IsError = false;
            IsNewNumberWaiting = true;
            IsLastEqualButton = false;
        }

        public static double Calculate(double additionalNumber)
        {
            switch (Operation)
            {
                case 1:
                    return Number += additionalNumber;

                case 2:
                    return Number -= additionalNumber;

                case 3:
                    return Number *= additionalNumber;

                case 4:
                    if (additionalNumber == 0)
                    {
                        IsError = true;

                        throw new DivideByZeroException();
                    }

                    else
                    {
                        return Number /= additionalNumber;
                    }

                default:
                    return double.NaN;
            }
        }
    }

    /// <summary>
    /// Исключение: деление на 0.
    /// </summary>
    public class DivideByZeroException : Exception
    {

    }
}
