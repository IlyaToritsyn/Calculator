using System;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
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

        public static string CurrentEntry { get; set; } = "";

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

            Calculator.IsInputActive = true;

            if (Calculator.IsAdditionalFunctionActive && Calculator.IsEqualsLastOperation)
            {
                ButtonEqual_Click(buttonEqual, e);

                //Calculator.Result = Math.Round(double.Parse(display.Text), 13);
                //CurrentEntry = Calculator.Result.ToString();
            }

            //Если ожидается ввод нового числа или в дисплее "0", то стираем старое число.
            if (Calculator.IsNewNumberExpected || display.Text == "0")
            {
                display.Text = pressedButton != buttonComma ? "" : "0";
            }

            display.Text += pressedButton.Text;

            ChangeFontSize();

            //if (Calculator.IsEqualsLastOperation && !Calculator.IsAdditionalFunctionActive)
            //{
            //    Calculator.Result = Math.Round(double.Parse(display.Text), 13);
            //    CurrentEntry = Calculator.Result.ToString();
            //}

            Calculator.IsNewNumberExpected = false;
            Calculator.IsAdditionalFunctionActive = false;
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

        private void Log()
        {
            //if (Calculator.IsNewNumberExpected && !Calculator.IsAdditionalFunctionActive && Calculator.IsEqualsLastOperation)
            //{
            //    CurrentEntry += Calculator.Result;
            //}

            switch (Calculator.Operation)
            {
                case 1:
                    CurrentEntry += " + ";
                    CurrentEntry += Calculator.LastSecond;

                    break;
                case 2:
                    CurrentEntry += " - ";
                    CurrentEntry += Calculator.LastSecond;

                    break;
                case 3:
                    CurrentEntry += " * ";
                    CurrentEntry += Calculator.LastSecond;

                    break;
                case 4:
                    CurrentEntry += " / ";
                    CurrentEntry += Calculator.LastSecond;

                    break;
                default:
                    break;
            }
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
                double displayNumber = double.Parse(display.Text);

                if (Calculator.IsEqualsLastOperation && !Calculator.IsAdditionalFunctionActive)
                {
                    Calculator.Result = Math.Round(double.Parse(display.Text), 13);
                    CurrentEntry = Calculator.Result.ToString();
                }

                if ((!Calculator.IsNewNumberExpected || Calculator.IsAdditionalFunctionActive) && !Calculator.IsEqualsLastOperation)
                {
                    Calculator.LastSecond = displayNumber;
                }

                if (Calculator.IsEqualsLastOperation && Calculator.IsAdditionalFunctionActive && Calculator.IsInputActive)
                {
                    int lastOperation = Calculator.Operation;
                    Calculator.Operation = 0;

                    Log();

                    if (Calculator.IsAdditionalFunctionActive && Calculator.IsEqualsLastOperation)
                    {
                        Calculator.Result = displayNumber;
                    }

                    display.Text = Calculator.Calculate(Calculator.LastSecond).ToString();

                    Calculator.Operation = lastOperation;
                }

                else
                {
                    Log();

                    if (Calculator.IsAdditionalFunctionActive && Calculator.IsEqualsLastOperation)
                    {
                        Calculator.Result = displayNumber;
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

            if (!Calculator.IsClearingOnly)
            {
                log.Items.Add(CurrentEntry);

                log.SelectedIndex = log.Items.Count - 1;
            }

            CurrentEntry = "";

            ChangeFontSize();

            Calculator.IsNewNumberExpected = true;
            Calculator.IsEqualsLastOperation = true;
            Calculator.IsAdditionalFunctionActive = false;
            Calculator.IsInputActive = false;
        }

        /// <summary>
        /// Кнопка "C".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonClear_Click(object sender, EventArgs e)
        {
            buttonEqual.Focus();

            if (Calculator.IsAdditionalFunctionActive)
            {
                Calculator.Operation = 0;
                ButtonEqual_Click(buttonEqual, e);
            }

            display.Text = "0";

            ChangeFontSize();

            Calculator.SetDefaultValues();

            CurrentEntry = "";
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
                CurrentEntry = Calculator.Result.ToString();
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

                Log();
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

            if (Calculator.IsEqualsLastOperation && !Calculator.IsAdditionalFunctionActive)
            {
                Calculator.Result = Math.Round(double.Parse(display.Text), 13);
                CurrentEntry = Calculator.Result.ToString();
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

                    Log();
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

            //else if (Calculator.IsEqualsLastOperation)
            //{
            //    CurrentEntry += Calculator.Result;
            //}

            Calculator.IsEqualsLastOperation = false;
            Calculator.IsAdditionalFunctionActive = false;
            Calculator.IsSqrtLast = false;
            Calculator.IsInputActive = false;
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

        private void AdvancedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MinimumSize = new System.Drawing.Size(370, MinimumSize.Height);
            MaximumSize = new System.Drawing.Size(370, MaximumSize.Height);
            buttonSqrt.Location = new System.Drawing.Point(290, Size.Height - 295);
            buttonPercent.Location = new System.Drawing.Point(290, Size.Height - 205);
            buttonOneDividedX.Location = new System.Drawing.Point(290, Size.Height - 115);

            Controls.Add(buttonSqrt);
            Controls.Add(buttonPercent);
            Controls.Add(buttonOneDividedX);

            normalToolStripMenuItem.CheckState = CheckState.Unchecked;
            advancedToolStripMenuItem.CheckState = CheckState.Checked;
        }

        private void LogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (logToolStripMenuItem.Checked)
            {
                MinimumSize = new Size(MinimumSize.Width, 490);
                log.Size = new Size(MinimumSize.Width - 35, 115);
                log.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));

                Controls.Add(log);
            }

            else
            {
                MaximumSize = new Size(MaximumSize.Width, 370);

                Controls.Remove(log);
            }
        }

        private void ClearLogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            log.Items.Clear();
        }

        private void Log_MouseDown(object sender, MouseEventArgs e)
        {
            if (log.Items.Count != 0)
            {
                log.DoDragDrop(log.SelectedItem, DragDropEffects.Copy);
            }
        }

        private void Display_DragEnter(object sender, DragEventArgs e)
        {
            string candidate = (string)e.Data.GetData(DataFormats.UnicodeText);

            if (Calculator.ArithmeticExpressionRegex.IsMatch(candidate))
            {
                e.Effect = DragDropEffects.Copy;
            }
        }

        private void Display_DragDrop(object sender, DragEventArgs e)
        {
            string candidate = (string)e.Data.GetData(DataFormats.UnicodeText);

            if (Calculator.ArithmeticExpressionRegex.IsMatch(candidate))
            {
                int lastOperation = Calculator.Operation;
                Calculator.IsEqualsLastOperation = true;
                Calculator.IsAdditionalFunctionActive = false;
                Calculator.IsSqrtLast = false;
                Calculator.IsClearingOnly = false;
                Calculator.IsNewNumberExpected = true;
                CurrentEntry = "";
                display.Text = Calculator.Calculate(candidate).ToString();

                ChangeFontSize();

                Calculator.Operation = lastOperation;
            }
        }

        private void AboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Разработать приложение, расширяющее функционал калькулятора.\n\n" +
                "Дополнительные функции:\n" +
                "1. Поддержка расширенного режима, т.е. увел. поля и появление дополнительных кнопок.\n" +
                "2. Ведение истории, т.е. списка значений (ListBox) результатов всех операций, которые добавляются автоматически.\n" +
                "3. Очистка истории.\n" +
                "4. Функция «Drag & Drop», позволяющая перетащить число из истории на табло.\n" +
                "5. Меню с минимум 2 пунктами: «Выход», «О программе».", "Лабораторная работа № 6. Калькулятор расширенный.\n");
        }

        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void ButtonSqrt_Click(object sender, EventArgs e)
        {
            buttonEqual.Focus();

            if (Calculator.IsClearingOnly)
            {
                return;
            }

            try
            {
                double answer = Calculator.CalculateSqrt(double.Parse(display.Text));
                display.Text = answer.ToString();

                if (Calculator.IsEqualsLastOperation)
                {
                    Calculator.Result = answer;
                    CurrentEntry = display.Text;
                }
            }

            catch (CustomExceptions exc)
            {
                display.Text = exc.Message;
            }

            catch
            {
                Calculator.IsClearingOnly = true;
                display.Text = Error;
            }

            ChangeFontSize();

            Calculator.IsNewNumberExpected = true;
            Calculator.IsInputActive = false;
        }

        private void ButtonPercent_Click(object sender, EventArgs e)
        {
            buttonEqual.Focus();

            if (Calculator.IsClearingOnly)
            {
                return;
            }

            if (Calculator.IsAdditionalFunctionActive)
            {
                ButtonEqual_Click(buttonEqual, e);
                Calculator.SetDefaultValues();
            }

            double displayNumber = double.Parse(display.Text);
            double answer = Calculator.CalculatePercent(displayNumber);
            display.Text = answer.ToString();
            //Calculator.Result = answer;

            if (Calculator.IsEqualsLastOperation)
            {
                Calculator.Result = answer;
                CurrentEntry = display.Text;

                //if (CurrentEntry == "")
                //{
                //    CurrentEntry = display.Text;
                //}

                //else
                //{
                //    CurrentEntry = display.Text;

                //    log.Items.Add(CurrentEntry);

                //    log.SelectedIndex = log.Items.Count - 1;
                //}
            }

            ChangeFontSize();

            Calculator.IsNewNumberExpected = true;
            Calculator.IsInputActive = false;
        }

        private void ButtonOneDividedX_Click(object sender, EventArgs e)
        {
            buttonEqual.Focus();

            if (Calculator.IsClearingOnly)
            {
                return;
            }

            try
            {
                double answer = Calculator.CalculateOneDividedX(double.Parse(display.Text));
                display.Text = answer.ToString();

                if (Calculator.IsEqualsLastOperation)
                {
                    Calculator.Result = answer;
                    CurrentEntry = display.Text;
                }
            }

            catch (CustomExceptions exc)
            {
                display.Text = exc.Message;
            }

            catch
            {
                Calculator.IsClearingOnly = true;
                display.Text = Error;
            }

            ChangeFontSize();

            Calculator.IsNewNumberExpected = false;
            Calculator.IsInputActive = false;
        }

        private void NormalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MinimumSize = new Size(300, MinimumSize.Height);
            MaximumSize = new Size(300, MaximumSize.Height);

            Controls.Remove(buttonSqrt);
            Controls.Remove(buttonPercent);
            Controls.Remove(buttonOneDividedX);

            normalToolStripMenuItem.CheckState = CheckState.Checked;
            advancedToolStripMenuItem.CheckState = CheckState.Unchecked;
        }
    }
}
