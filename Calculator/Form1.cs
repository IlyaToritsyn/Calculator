using System;
using System.Windows.Forms;
using ClassLibrary;

namespace CS_lab_5_Calculator
{
    public partial class Form1 : Form
    {
        /// <summary>
        /// Стандартное сообщение о непредвиденной ошибке.
        /// </summary>
        public static string Error { get; } = "Ошибка";

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
            buttonEqual.Focus();

            Button pressedButton = (Button)sender;

            if (Calculator.IsClearingOnly ||
                (((display.Text.Length >= 15 && pressedButton != buttonComma && display.Text[display.Text.Length - 1] != buttonComma.Text[0]) ||
                (pressedButton == buttonComma && display.Text.Contains(","))) && !Calculator.IsNewNumberExpected))
            {
                return;
            }

            //Если ожидается ввод нового числа или в дисплее "0", то стираем старое число.
            if (Calculator.IsNewNumberExpected || display.Text == "0")
            {
                display.Text = pressedButton != buttonComma ? "" : "0";
            }

            display.Text += pressedButton.Text;

            ChangeFontSize();

            if (Calculator.IsEqualsLastOperation)
            {
                Calculator.Result = Math.Round(double.Parse(display.Text), 13);
            }

            Calculator.IsNewNumberExpected = false;
        }

        /// <summary>
        /// Изменение размера шрифта в дисплее, чтобы содержимое полностью помещалось.
        /// </summary>
        private void ChangeFontSize()
        {
            display.Font = display.Text.Length > 15
                ? new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204)
                : new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
        }

        /// <summary>
        /// Кнопка "=".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonEqual_Click(object sender, EventArgs e)
        {
            if (Calculator.IsClearingOnly)
            {
                return;
            }

            try
            {
                if (Calculator.IsNewNumberExpected)
                {
                    display.Text = Calculator.Calculate(Calculator.LastSecond).ToString();
                }

                else
                {
                    if (!Calculator.IsEqualsLastOperation)
                    {
                        Calculator.LastSecond = double.Parse(display.Text);
                    }

                    display.Text = Calculator.Calculate(Calculator.LastSecond).ToString();
                }
            }

            catch (CustomExceptions exc)
            {
                display.Text = exc.Message;
            }

            catch
            {
                display.Text = Error;
            }

            ChangeFontSize();

            Calculator.IsNewNumberExpected = true;
            Calculator.IsEqualsLastOperation = true;
        }

        /// <summary>
        /// Кнопка "C".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonClear_Click(object sender, EventArgs e)
        {
            buttonEqual.Focus();

            display.Text = "0";

            ChangeFontSize();
            Calculator.SetDefaultValues();
        }

        /// <summary>
        /// Кнопка "+-".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonChangeSign_Click(object sender, EventArgs e)
        {
            buttonEqual.Focus();

            if (Calculator.IsClearingOnly || display.Text == "0")
            {
                return;
            }

            display.Text = display.Text[0] != '-' ? "-" + display.Text : display.Text.Remove(0, 1);

            ChangeFontSize();

            double displayNumber = double.Parse(display.Text);

            //Если число на дисплее было равно хранимому результату и последняя операция - "=", то меняем знак у хранимого результата.
            if (Math.Round(-displayNumber, 13) == Calculator.Result && Calculator.IsEqualsLastOperation)
            {
                Calculator.Result = displayNumber;
            }

            Calculator.IsNewNumberExpected = false;
        }

        /// <summary>
        /// Кнопка "⇦".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonDeleteLast_Click(object sender, EventArgs e)
        {
            buttonEqual.Focus();

            if (Calculator.IsClearingOnly || Calculator.IsNewNumberExpected)
            {
                return;
            }

            display.Text = display.Text.Remove(display.Text.Length - 1);

            if (display.Text == "" || display.Text == "-" || display.Text == "-0")
            {
                display.Text = "0";
            }

            ChangeFontSize();

            if (Calculator.IsEqualsLastOperation)
            {
                Calculator.Result = double.Parse(display.Text);
            }
        }

        /// <summary>
        /// Обработка нажатия кнопок с арифметическими операциями.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ProcessArithmeticOperation(object sender, EventArgs e)
        {
            buttonEqual.Focus();

            if (Calculator.IsClearingOnly)
            {
                return;
            }

            Calculator.LastSecond = double.Parse(display.Text);

            if (!Calculator.IsNewNumberExpected)
            {
                if (Calculator.IsEqualsLastOperation)
                {
                    Calculator.Operation = 0;
                }

                try
                {
                    display.Text = Calculator.Calculate(Calculator.LastSecond).ToString();
                }

                catch (CustomExceptions exc)
                {
                    display.Text = exc.Message;
                }

                catch
                {
                    display.Text = Error;
                }

                ChangeFontSize();

                Calculator.IsNewNumberExpected = true;
            }

            Calculator.IsEqualsLastOperation = false;
            Button pressedButton = (Button)sender;

            if (pressedButton == buttonPlus)
            {
                Calculator.Operation = 1;
            }

            else if (pressedButton == buttonMinus)
            {
                Calculator.Operation = 2;
            }

            else if (pressedButton == buttonMultiplication)
            {
                Calculator.Operation = 3;
            }

            else if (pressedButton == buttonDivision)
            {
                Calculator.Operation = 4;
            }
        }

        /// <summary>
        /// Обработка нажатия клавиш.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Key(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode) //Подключение кнопок.
            {
                case Keys.Enter: { ButtonEqual_Click(buttonEqual, e); break; }
                case Keys.NumPad0: case Keys.D0: { InputSymbol(button0, e); break; }
                case Keys.NumPad1: case Keys.D1: { InputSymbol(button1, e); break; }
                case Keys.NumPad2: case Keys.D2: { InputSymbol(button2, e); break; }
                case Keys.NumPad3: case Keys.D3: { InputSymbol(button3, e); break; }
                case Keys.NumPad4: case Keys.D4: { InputSymbol(button4, e); break; }
                case Keys.NumPad5: case Keys.D5: { InputSymbol(button5, e); break; }
                case Keys.NumPad6: case Keys.D6: { InputSymbol(button6, e); break; }
                case Keys.NumPad7: case Keys.D7: { InputSymbol(button7, e); break; }
                case Keys.NumPad8: case Keys.D8: { InputSymbol(button8, e); break; }
                case Keys.NumPad9: case Keys.D9: { InputSymbol(button9, e); break; }
                case Keys.Add: { ProcessArithmeticOperation(buttonPlus, e); break; }
                case Keys.Subtract: { ProcessArithmeticOperation(buttonMinus, e); break; }
                case Keys.Multiply: { ProcessArithmeticOperation(buttonMultiplication, e); break; }
                case Keys.Divide: { ProcessArithmeticOperation(buttonDivision, e); break; }
                case Keys.Oemcomma: case Keys.OemPeriod: { InputSymbol(buttonComma, e); break; }
                case Keys.Delete: { ButtonClear_Click(buttonClear, e); break; }
                case Keys.Back: { ButtonDeleteLast_Click(buttonDeleteLast, e); break; }
            }
        }
    }
}
