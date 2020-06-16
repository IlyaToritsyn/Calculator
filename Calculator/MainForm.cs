using System;
using System.Drawing;
using System.Windows.Forms;
using ClassLibrary;

namespace CS_lab_6_Advanced_calculator
{
    public partial class MainForm : Form
    {
        /// <summary>
        /// Стандартное сообщение о непредвиденной ошибке.
        /// </summary>
        public static string Error { get; } = "Ошибка";

        /// <summary>
        /// Арифметическая строка для последующего вывода в журнал.
        /// </summary>
        public string CurrentEntry { get; set; } = "";

        public MainForm()
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

            //Если доступна только очистка ИЛИ
            //если количество символов в дисплее более 14 и вводимый символ вместе с пред. не являются запятыми и вместе с тем не ожидается новое число ИЛИ
            //если вводится запятая, которая уже есть в дисплее, и при этом не ожидается новое число,
            //то отклоняем ввод.
            if (Calculator.IsClearingOnly ||
                (((display.Text.Length >= 15 && pressedButton != buttonComma && display.Text[display.Text.Length - 1] != buttonComma.Text[0]) ||
                (pressedButton == buttonComma && display.Text.Contains(","))) && !Calculator.IsNewNumberExpected))
            {
                return;
            }

            Calculator.IsNewNumberBeingEntered = true; //Активируем режим ввода числа.

            //Ситуация: нажато "=", нажата кнопка допфункции (√, %, 1/x), и после этого вводится символ -
            //завершаем пред. операцию, выводим арифметическую строку в журнал.
            if (Calculator.IsAdditionalFunctionActive && Calculator.IsEqualsLastOperation)
            {
                ButtonEqual_Click(buttonEqual, e);
            }

            //Если ожидается ввод нового числа или в дисплее "0", то стираем старое число.
            if (Calculator.IsNewNumberExpected || display.Text == "0")
            {
                display.Text = pressedButton != buttonComma ? "" : "0";
            }

            display.Text += pressedButton.Text;

            ChangeFontSize();

            Calculator.IsNewNumberExpected = false;
            Calculator.IsAdditionalFunctionActive = false;
        }

        /// <summary>
        /// Изменение размера шрифта в дисплее, чтобы содержимое полностью помещалось.
        /// </summary>
        private void ChangeFontSize()
        {
            display.Font = display.Text.Length > 15
                ? new Font("Microsoft Sans Serif", 13F, FontStyle.Regular, GraphicsUnit.Point, 204)
                : new Font("Microsoft Sans Serif", 15F, FontStyle.Regular, GraphicsUnit.Point, 204);
        }

        /// <summary>
        /// Дополнение арифметической строки для последующего вывода в журнал.
        /// </summary>
        private void Log()
        {
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

                //Ситуация: нажато "=", затем возможный и необязательный ввод символов, и после этого вновь нажато "=" -
                //число с дисплея становится результатом и записывается в арифметическую строку для последующего вывода в журнал.
                if (Calculator.IsEqualsLastOperation && !Calculator.IsAdditionalFunctionActive)
                {
                    Calculator.Result = Math.Round(double.Parse(display.Text), 13);
                    CurrentEntry = Calculator.Result.ToString();
                }

                //Если после базовой операции (+, -, *, /) введено число и затем нажато "=" ИЛИ
                //если после базовой операции (+, -, *, /) сразу выбрана допфункция (√, %, 1/x), и затем нажато "=",
                //то число с дисплея становится последним 2 числом.
                if ((!Calculator.IsNewNumberExpected || Calculator.IsAdditionalFunctionActive) && !Calculator.IsEqualsLastOperation)
                {
                    Calculator.LastSecond = displayNumber;
                }

                //Блок не выполняется при ситуации: нажато "=", затем возможный необязательный ввод числа, потом выбрана допфункция (√, %, 1/x) и вводится символ.
                if (!Calculator.IsEqualsLastOperation || !Calculator.IsAdditionalFunctionActive || !Calculator.IsNewNumberBeingEntered)
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
                Calculator.IsClearingOnly = true;
            }

            if (!Calculator.IsClearingOnly)
            {
                if (CurrentEntry.Contains(" "))
                {
                    CurrentEntry += " = " + Calculator.Result;
                }

                log.SelectedIndex = log.Items.Add(CurrentEntry);

                if (CurrentEntry.Length >= 35)
                {
                    log.HorizontalScrollbar = true;
                }
            }

            CurrentEntry = "";

            ChangeFontSize();

            Calculator.IsNewNumberExpected = true;
            Calculator.IsEqualsLastOperation = true;
            Calculator.IsAdditionalFunctionActive = false;
            Calculator.IsNewNumberBeingEntered = false;
        }

        /// <summary>
        /// Кнопка "C".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonClear_Click(object sender, EventArgs e)
        {
            buttonEqual.Focus();

            //Ситуация: нажата "=", возможный и необязательный ввод числа, выбрана допфункция (√, %, 1/x) и нажата "C" -
            //завершаем операцию и выводим результат в журнал.
            if (Calculator.IsAdditionalFunctionActive && Calculator.IsEqualsLastOperation)
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
            }
        }

        /// <summary>
        /// Обработка нажатия кнопок с базовыми арифметическими операциями (+, -, *, /).
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

            double displayNumber = double.Parse(display.Text);

            if (Calculator.IsEqualsLastOperation && !Calculator.IsAdditionalFunctionActive)
            {
                Calculator.Result = Math.Round(displayNumber, Calculator.DigitsAfterPoint);
                CurrentEntry = Calculator.Result.ToString();
            }

            Calculator.LastSecond = displayNumber;

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
                    Calculator.IsClearingOnly = true;
                }

                ChangeFontSize();

                Calculator.IsNewNumberExpected = true;
            }

            Calculator.IsEqualsLastOperation = false;
            Calculator.IsAdditionalFunctionActive = false;
            Calculator.IsNewNumberBeingEntered = false;
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
        /// Если нажата кнопка "√".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
                display.Text = Error;
                Calculator.IsClearingOnly = true;
            }

            ChangeFontSize();

            Calculator.IsNewNumberExpected = true;
            Calculator.IsNewNumberBeingEntered = false;
        }

        /// <summary>
        /// Если нажата кнопка "%".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

            double answer = Calculator.CalculatePercent(double.Parse(display.Text));
            display.Text = answer.ToString();

            if (Calculator.IsEqualsLastOperation)
            {
                Calculator.Result = answer;
                CurrentEntry = display.Text;
            }

            ChangeFontSize();

            Calculator.IsNewNumberExpected = true;
            Calculator.IsNewNumberBeingEntered = false;
        }

        /// <summary>
        /// Если нажата кнопка "1/x".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
                display.Text = Error;
                Calculator.IsClearingOnly = true;
            }

            ChangeFontSize();

            Calculator.IsNewNumberExpected = true;
            Calculator.IsNewNumberBeingEntered = false;
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

        /// <summary>
        /// Если выбран пункт меню "Вид" - "Обычный".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// Если выбран пункт меню "Вид" - "Расширенный".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AdvancedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MinimumSize = new Size(370, MinimumSize.Height);
            MaximumSize = new Size(370, MaximumSize.Height);
            buttonSqrt.Location = new Point(290, Size.Height - 295);
            buttonPercent.Location = new Point(290, Size.Height - 205);
            buttonOneDividedX.Location = new Point(290, Size.Height - 115);

            Controls.Add(buttonSqrt);
            Controls.Add(buttonPercent);
            Controls.Add(buttonOneDividedX);

            normalToolStripMenuItem.CheckState = CheckState.Unchecked;
            advancedToolStripMenuItem.CheckState = CheckState.Checked;
        }

        /// <summary>
        /// Если выбран пункт меню "Вид" - "Журнал".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Если включаем отображение журнала.
            if (logToolStripMenuItem.Checked)
            {
                MinimumSize = new Size(MinimumSize.Width, 490);
                log.Size = new Size(MinimumSize.Width - 35, 115);

                Controls.Add(log);
            }

            //Если выключаем отображение журнала.
            else
            {
                MaximumSize = new Size(MaximumSize.Width, 370);

                Controls.Remove(log);
            }
        }

        /// <summary>
        /// Если выбран пункт меню "Опции" - "Очистить журнал".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClearLogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            log.Items.Clear();
        }

        /// <summary>
        /// Если выбран пункт меню "Опции" - "О программе".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// Если выбран пункт меню "Опции" - "Выход".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        /// <summary>
        /// Если нажата кнопка мыши в журнале.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Log_MouseDown(object sender, MouseEventArgs e)
        {
            if (log.Items.Count != 0)
            {
                log.DoDragDrop(log.SelectedItem, DragDropEffects.Copy);
            }
        }

        /// <summary>
        /// Если наводим перетаскиваемым элементом на дисплей.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Display_DragEnter(object sender, DragEventArgs e)
        {
            string candidate = (string)e.Data.GetData(DataFormats.UnicodeText);

            if (Calculator.ArithmeticExpressionRegex.IsMatch(candidate))
            {
                e.Effect = DragDropEffects.Copy;
            }
        }

        /// <summary>
        /// Если бросаем элемент на дисплей.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Display_DragDrop(object sender, DragEventArgs e)
        {
            string candidate = (string)e.Data.GetData(DataFormats.UnicodeText);

            if (Calculator.ArithmeticExpressionRegex.IsMatch(candidate))
            {
                int lastOperation = Calculator.Operation;
                Calculator.IsEqualsLastOperation = true;
                Calculator.IsAdditionalFunctionActive = false;
                Calculator.IsClearingOnly = false;
                Calculator.IsNewNumberExpected = true;
                CurrentEntry = "";
                display.Text = Calculator.Calculate(candidate).ToString();

                ChangeFontSize();

                Calculator.Operation = lastOperation;
            }
        }
    }
}
