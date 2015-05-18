using System;
using System.IO;
using System.Text;

namespace EleFunGames
{
	internal static class Program
	{
		/// <summary>
		///     Точка входа в приложение
		/// </summary>
		/// <param name="args">Аргументы командной строки</param>
		private static void Main(string[] args)
		{
			ParseArguments(args);
			Message.Show("Работа программы завершена, нажмите любую клавишу");
			Console.ReadKey();
		}

		/// <summary>
		///     Парсит аргументы, на основе результатов выполняет соответствующие действия
		/// </summary>
		/// <param name="args">Аргументы командной строки</param>
		private static void ParseArguments(string[] args)
		{
			// реализуем регистронезависимость, исправляем тире и дефисы на минусы
			string[] argsWork = new string[args.Length];
			for (int i = 0; i < args.Length; i++)
			{
				argsWork[i] = args[i].ToUpperInvariant().Replace('–', '-').Replace('—', '-');
			}

			// проверям первый аргумент run
			if (argsWork.Length == 0 || argsWork[0] != "RUN")
			{
				Message.ShowError(Message.WRONG_RUN_ARG, args);
				return;
			}

			// проверяем второй аргумент
			if (argsWork.Length <= 1)
			{
				Message.ShowError(Message.WRONG_NOT_COMMAND, args);
				return;
			}
			switch (argsWork[1])
			{
				// Работа с файлом
				case "-F":
					ParseFilename(args, argsWork);
					return;
				// Вывод справки
				case "-H":
					ParseHelp();
					return;
				// Неверный аргумент
				default:
					ParseWrong(args);
					return;
			}
		}

		/// <summary>
		///     Обработчик для команды -F
		/// </summary>
		/// <param name="args">Список аргументов</param>
		/// <param name="argsWork">Cписок аргументов после обработки</param>
		private static void ParseFilename(string[] args, string[] argsWork)
		{
			// проверяем правильность указания файла
			int positionArgM = -1;
			var fileNameBuilder = new StringBuilder();
			for (int i = 2; i < argsWork.Length; i++)
			{
				if (argsWork[i] == "-M")
				{
					positionArgM = i;
					break;
				}
				fileNameBuilder.Append(argsWork[i] + " ");
			}
			string fileName = fileNameBuilder.ToString().TrimEnd();

			// Выводим ошибки связанные с указанием комманд после -f
			if (fileName.Length == 0)
			{
				Message.ShowError(Message.WRONG_FILENAME_NOT_SPECIFIED, args);
				return;
			}
			if (positionArgM == -1 || positionArgM + 1 >= argsWork.Length)
			{
				Message.ShowError(Message.WRONG_FILENAME_COMMAND, args);
				return;
			}
			if (!File.Exists(fileName))
			{
				Message.ShowError(Message.WRONG_FILE_NOT_EXIST, args);
				return;
			}

			switch (argsWork[positionArgM + 1])
			{
				case "FIND":
					const int NUMBER_OF_ARGS_AFTER_M = 3;
					if (positionArgM + NUMBER_OF_ARGS_AFTER_M >= argsWork.Length || argsWork[positionArgM + 2] != "-S")
					{
						Message.ShowError(Message.WRONG_FILENAME_COMMAND, args);
						return;
					}
					var lineBuilder = new StringBuilder();
					for (int i = positionArgM + NUMBER_OF_ARGS_AFTER_M; i < args.Length; i++)
					{
						lineBuilder.Append(args[i] + " ");
					}
					string line = lineBuilder.ToString().TrimEnd();
					Message.Show(FindFilenameOffset(fileName, line));
					return;
				case "CHECKSUM":
					Message.Show(CalculateChecksum(fileName));
					return;
			}
		}

		/// <summary>
		///     Обработчик для команды -H
		/// </summary>
		private static void ParseHelp()
		{
			Message.ShowHelp();
		}

		/// <summary>
		///     Обработчик для невалидной команды
		/// </summary>
		/// <param name="args"></param>
		private static void ParseWrong(string[] args)
		{
			Message.ShowError(Message.WRONG_NOT_COMMAND, args);
		}

		/// <summary>
		///     Находит смещения в байтах в файле в кодировке UTF-8, где находится указаная строка
		/// </summary>
		/// <param name="filename">Имя файла</param>
		/// <param name="inputLine">Строка для поиска</param>
		private static string FindFilenameOffset(string filename, string inputLine)
		{
			StringBuilder result = new StringBuilder();

			// Вначале определим наличие BOM, который занимает 3 байта
			BinaryReader binaryReader = new BinaryReader(File.OpenRead(filename));
			byte[] data = binaryReader.ReadBytes((int) Math.Min(binaryReader.BaseStream.Length, 3));
			binaryReader.Close();
			bool bom = (data.Length >= 3 && data[0] == 0xef && data[1] == 0xbb && data[2] == 0xbf);
			const int BOOM_SIZE = 3;
			int caretPosition = bom ? BOOM_SIZE : 0;
			StreamReader sr = new StreamReader(filename);

			// находим все вхождения указанной строки и узнаем их смещение от начала файла
			int counter = 0;
			const int LINE_BREAK_SIZE = 2;
			while (sr.EndOfStream == false)
			{
				string s = sr.ReadLine();
				if (s == inputLine)
				{
					result.Append(caretPosition + " ");
					counter++;
				}
				caretPosition += Encoding.UTF8.GetByteCount(s) + LINE_BREAK_SIZE;
			}

			return (bom ? "Кодировка использует BOM (+3 байта в начале)\n" : null)
				+ "Размер файла в байтах: " + caretPosition + "\n"
				+ "Найдено вхождений строки: " + counter + "\nСмещения: " + result;
		}

		/// <summary>
		///     Производит подсчет CRC-32
		/// </summary>
		/// <param name="fileName">Имя файла для которого нужно произвести подсчет</param>
		/// <returns></returns>
		private static string CalculateChecksum(string fileName)
		{
			Crc32 crc32 = new Crc32();
			string hash = String.Empty;
			using (FileStream fs = File.Open(fileName, FileMode.Open))
			{
				foreach (byte b in crc32.ComputeHash(fs))
				{
					hash += b.ToString("x2").ToLower();
				}
			}

			return "CRC-32: 0x" + hash;
		}
	}
}