using System;

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
        private static int operation;

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
        public static bool IsClearingOnly { get; private set; } = false;

        /// <summary>
        /// Проверка: последняя операция - "=" (игнорируя операции ввода чисел).
        /// </summary>
        public static bool IsEqualsLastOperation { get; set; } = true;

        /// <summary>
        /// Последнее 2 число, позволяющее повторить пред. операцию с данным значением.
        /// </summary>
        public static double LastSecond { get; set; } = 0;

        /// <summary>
        /// Восстановление умолчаний.
        /// </summary>
        public static void SetDefaultValues()
        {
            Result = 0;
            LastSecond = 0;
            IsEqualsLastOperation = true;
            IsClearingOnly = false;
            IsNewNumberExpected = true;
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
                    Result = Math.Round(Result + secondNumber, 13);

                    if (double.IsInfinity(Result))
                    {
                        IsClearingOnly = true;

                        throw new CustomExceptions.OverflowException();
                    }

                    return Result;
                case 2:
                    Result = Math.Round(Result - secondNumber, 13);

                    if (double.IsInfinity(Result))
                    {
                        IsClearingOnly = true;

                        throw new CustomExceptions.OverflowException();
                    }

                    return Result;
                case 3:
                    isNumberBeforeZero = Result == 0;

                    Result = Math.Round(Result * secondNumber, 13);

                    if (double.IsInfinity(Result) || ((Result == 0) && (!isNumberBeforeZero) && (secondNumber != 0)))
                    {
                        IsClearingOnly = true;

                        throw new CustomExceptions.OverflowException();
                    }

                    return Result;
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

                        Result = Math.Round(Result / secondNumber, 13);

                        if (double.IsInfinity(Result) || ((Result == 0) && !isNumberBeforeZero))
                        {
                            IsClearingOnly = true;

                            throw new CustomExceptions.OverflowException();
                        }

                        return Result;
                    }
                default:
                    return Result;
            }
        }
    }
}
