using System;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace ClassLibrary
{
    /// <summary>
    /// Класс калькулятора.
    /// </summary>
    public static class Calculator
    {
        /// <summary>
        /// Номер операции: 0 - нет, 1 - "+", 2 - "-", 3 - "*", 4 - "/".
        /// </summary>
        private static int operation = 0;

        private static int digitsAfterPoint = 13;
        /// <summary>
        /// Посчитанный результат.
        /// </summary>
        public static double Result { get; set; } = 0;

        /// <summary>
        /// Номер операции: 0 - нет, 1 - "+", 2 - "-", 3 - "*", 4 - "/".
        /// </summary>
        public static int Operation
        {
            get => operation;

            set
            {
                if (value < 0 || value >= 5)
                {
                    throw new CustomExceptions.NotSupportedOperationException();
                }

                operation = value;
            }
        }

        /// <summary>
        /// Ожидается ли ввод нового числа (при вводе нового символа стирается старое число).
        /// </summary>
        public static bool IsNewNumberExpected { get; set; } = true;

        /// <summary>
        /// Есть возможность только сбросить калькулятор.
        /// </summary>
        public static bool IsClearingOnly { get; set; } = false;

        /// <summary>
        /// Проверка: последняя операция - "=" (игнорируя операции ввода чисел).
        /// </summary>
        public static bool IsEqualsLastOperation { get; set; } = true;

        public static bool IsAdditionalFunctionActive { get; set; } = false;

        //public static bool IsSqrtLast { get; set; } = false;

        public static bool IsInputActive { get; set; } = false;

        /// <summary>
        /// Последнее 2 число, позволяющее повторить пред. операцию с данным значением.
        /// </summary>
        public static double LastSecond { get; set; } = 0;

        /// <summary>
        /// Фильтр для арифметических выражений (примеры данных, успешно проходящих фильтр: "1", "1 + 3", "4 * 5 - 3", "3 - 5 * 7 = -14", "5E+2 - 7 = -3,5E-9").
        /// </summary>
        public static Regex ArithmeticExpressionRegex { get; } = new Regex(@"^(?:-?\d+(,\d+)?(E[+-]\d+)?)(\s[*+\/-]\s(-?\d+(,\d+)?(E[+-]\d+)?))*(\s=\s-?\d+(,\d+)?(E[+-]\d+)?)?$");

        /// <summary>
        /// Восстановление умолчаний.
        /// </summary>
        public static void SetDefaultValues()
        {
            Operation = 0;
            Result = 0;
            LastSecond = 0;
            //ResultAfterEquals = 0;
            IsEqualsLastOperation = true;
            IsClearingOnly = false;
            IsNewNumberExpected = true;
            IsAdditionalFunctionActive = false;
            //IsSqrtLast = false;
            IsInputActive = false;
        }

        /// <summary>
        /// Вычисление результата.
        /// </summary>
        /// <param name="secondNumber">2 число</param>
        /// <returns>Результат</returns>
        public static double Calculate(double secondNumber)
        {
            bool isNumberBeforeZero;

            switch (Operation)
            {
                case 1:
                    Result = Math.Round(Result + secondNumber, digitsAfterPoint);

                    if (double.IsInfinity(Result))
                    {
                        IsClearingOnly = true;

                        throw new CustomExceptions.OverflowException();
                    }

                    break;
                case 2:
                    Result = Math.Round(Result - secondNumber, digitsAfterPoint);

                    if (double.IsInfinity(Result))
                    {
                        IsClearingOnly = true;

                        throw new CustomExceptions.OverflowException();
                    }

                    break;
                case 3:
                    isNumberBeforeZero = Result == 0;

                    if (IsAdditionalFunctionActive)
                    {
                        Result = Math.Round(Result * secondNumber, 11);
                    }

                    else
                    {
                        Result = Math.Round(Result * secondNumber, digitsAfterPoint);
                    }

                    if (double.IsInfinity(Result) || ((Result == 0) && (!isNumberBeforeZero) && (secondNumber != 0)))
                    {
                        IsClearingOnly = true;

                        throw new CustomExceptions.OverflowException();
                    }

                    break;
                case 4:
                    if (secondNumber == 0)
                    {
                        IsClearingOnly = true;

                        if (Result == 0)
                        {
                            throw new CustomExceptions.UndefinedResultException();
                        }

                        throw new CustomExceptions.DivideByZeroException();
                    }

                    else
                    {
                        isNumberBeforeZero = Result == 0;

                        Result = Math.Round(Result / secondNumber, digitsAfterPoint);

                        if (double.IsInfinity(Result) || ((Result == 0) && !isNumberBeforeZero))
                        {
                            IsClearingOnly = true;

                            throw new CustomExceptions.OverflowException();
                        }

                        break;
                    }
                default:
                    break;
            }

            IsAdditionalFunctionActive = false;
            //IsSqrtLast = false;

            return Result;
        }

        /// <summary>
        /// Получение ответа для арифметического выражения, заданного строкой.
        /// </summary>
        /// <param name="expression">Арифметическое выражение в виде строки</param>
        /// <returns>Результат вычисления</returns>
        public static double Calculate(string expression)
        {
            expression = expression.Trim();

            if (!ArithmeticExpressionRegex.IsMatch(expression))
            {
                throw new CustomExceptions.InvalidArithmeticStringFormatException();
            }

            if (expression.Contains("="))
            {
                expression = expression.Remove(expression.IndexOf('=') - 1);
            }

            string[] data = expression.Split(' ');
            Operation = 0;
            Result = Math.Round(double.Parse(data[0]), digitsAfterPoint);

            foreach (string element in data)
            {
                if (double.TryParse(element, out double number))
                {
                    Calculate(number);
                }

                else
                {
                    switch (element)
                    {
                        case "+":
                            Operation = 1;

                            break;
                        case "-":
                            Operation = 2;

                            break;
                        case "*":
                            Operation = 3;

                            break;
                        case "/":
                            Operation = 4;

                            break;
                        default:
                            break;
                    }
                }
            }

            return Result;
        }

        public static double CalculateSqrt(double number)
        {
            if (number < 0)
            {
                IsClearingOnly = true;

                throw new CustomExceptions.InvalidInput();
            }

            IsAdditionalFunctionActive = true;
            //IsSqrtLast = true;

            return Math.Round(Math.Sqrt(number), digitsAfterPoint);
        }

        public static double CalculatePercent(double number)
        {
            IsAdditionalFunctionActive = true;
            //IsSqrtLast = false;

            return Operation == 0 ? 0 : Result / 100 * number;
        }

        public static double CalculateOneDividedX(double number)
        {
            if (number == 0)
            {
                IsClearingOnly = true;

                throw new CustomExceptions.DivideByZeroException();
            }

            double result = Math.Round(1 / number, digitsAfterPoint);

            if (result == 0)
            {
                IsClearingOnly = true;

                throw new CustomExceptions.OverflowException();
            }

            IsAdditionalFunctionActive = true;
            //IsSqrtLast = false;

            return result;
        }
    }
}
