using System;

namespace ClassLibrary
{
    /// <summary>
    /// Класс пользовательских исключений.
    /// </summary>
    public class CustomExceptions : Exception
    {
        /// <summary>
        /// Исключение: деление на 0 невозможно.
        /// </summary>
        public class DivideByZeroException : CustomExceptions
        {
            /// <summary>
            /// Сообщение об ошибке.
            /// </summary>
            public override string Message { get; } = "Деление на 0 невозможно";
        }

        /// <summary>
        /// Исключение: переполнение.
        /// </summary>
        public class OverflowException : CustomExceptions
        {
            /// <summary>
            /// Сообщение об ошибке.
            /// </summary>
            public override string Message { get; } = "Переполнение";
        }

        /// <summary>
        /// Исключение: результат не определён.
        /// </summary>
        public class UndefinedResultException : CustomExceptions
        {
            /// <summary>
            /// Сообщение об ошибке.
            /// </summary>
            public override string Message { get; } = "Результат не определён";
        }

        /// <summary>
        /// Исключение: обращение к несуществующему номеру операции.
        /// </summary>
        public class NotSupportedOperationException : CustomExceptions
        {
            /// <summary>
            /// Сообщение об ошибке.
            /// </summary>
            public override string Message { get; } = "Обращение к несуществующему номеру операции";
        }

        /// <summary>
        /// Исключение: неверный формат арифметической строки.
        /// </summary>
        public class InvalidArithmeticStringFormatException : CustomExceptions
        {
            /// <summary>
            /// Сообщение об ошибке.
            /// </summary>
            public override string Message { get; } = "Неверный формат арифметической строки";
        }

        /// <summary>
        /// Исключение: недопустимый ввод.
        /// </summary>
        public class InvalidInput : CustomExceptions
        {
            /// <summary>
            /// Сообщение об ошибке.
            /// </summary>
            public override string Message { get; } = "Недопустимый ввод";
        }
    }
}
