using System;
using System.Collections.Generic;
using System.Text;

namespace EleFunGames
{
	/// <summary>
	///     Класс для вывода сообщений на экран
	/// </summary>
	internal static class Message
	{
		/// <summary>
		///     Заголовок сообщения об ошибке
		/// </summary>
		private const string _WRONG_HEAD = "Произошла следующая ошибка:\n\n";

		/// <summary>
		///     Подвал сообщения об ошибке
		/// </summary>
		private const string _WRONG_BOTTOM = "\n\nУбедитесь в правильности вашeй входной строки, и попробуйте снова"
			+ "\nВы ввели следующую строку:";

		/// <summary>
		///     Сообщение об отсутствии аргумента Run
		/// </summary>
		public const string WRONG_RUN_ARG = "В качестве первого аргумента должна выступать команда run";

		/// <summary>
		///     Сообщение об отсутствии допустимых команд
		/// </summary>
		public const string WRONG_NOT_COMMAND = "Не найдено ни одной допустимой команды во втором аргументе";

		/// <summary>
		///     Сообщение об отсутствии файла в списке аргументов при наличии команды -F
		/// </summary>
		public const string WRONG_FILENAME_NOT_SPECIFIED = "Наличие команды -f подразумевает указание файла после нее";

		/// <summary>
		///     Сообщение о несуществующем файле
		/// </summary>
		public const string WRONG_FILE_NOT_EXIST = "Указанный вами файл не существует, попробуйте ввести другое имя";

		/// <summary>
		///     Сообщение о неправильных коммандах после команды -f
		/// </summary>
		public const string WRONG_FILENAME_COMMAND = "Наличие команды -f filename подразумевает также наличие после нее одной из  следующих команд:" +
			"\n1) -m find -s hello" +
			"\n2) -m checksum";

		/// <summary>
		///     Справочная информация
		/// </summary>
		private const string _HELP = "\nДопустимые команды:" +
			"\n\n1) Run –f filename –m find –s hello" +
			"\nВыводит все смещения в байтах для файла filename, где расположена строка hello" +
			"\nИмя файла вводится без кавычек, в имени допускается не более одного пробела подряд" +
			"\n\n2) Run –f filename –m checksum" +
			"\nВыводит 32-хбитную чексумму, рассчитанную по сумме всех 32-хбитных слов в файле" +
			"\n\n3) Run –h" +
			"\nВыводит справку о командах и параметрах" +
			"\n\nКоманды вводятся без учета регистра";

		/// <summary>
		///     Выводит сообщение об ошибке
		/// </summary>
		/// <param name="inputMsg">Cообщение касательно ошибки</param>
		/// <param name="args">Параметры командной строки</param>
		public static void ShowError(string inputMsg, IEnumerable<string> args)
		{
			var argsBuilder = new StringBuilder();
			foreach (var a in args)
			{
				argsBuilder.Append(" " + a);
			}
			Console.WriteLine(_WRONG_HEAD + "\"" + inputMsg + "\"" + _WRONG_BOTTOM + argsBuilder + _HELP);
		}

		/// <summary>
		///     Выводит справочную информацию
		/// </summary>
		public static void ShowHelp()
		{
			Console.WriteLine(_HELP);
		}

		/// <summary>
		///     Выводит указанную строку
		/// </summary>
		public static void Show(string input)
		{
			Console.WriteLine(input);
		}
	}
}