using System;
using System.Diagnostics;

namespace LiraSAPRTest.ModelLiraSapr
{
  /// <summary>
  /// Строка таблицы нелинейных диаграмм (<c>kLiraTable_NonlinearDiagrams</c>) —
  /// законы нелинейного деформирования (σ–ε и аналогичные).
  /// Имена свойств соответствуют заголовкам столбцов таблицы:
  /// Вид нелинейного закона, Номер записи, Параметры.
  /// Замечание: в тестовой модели таблица была пуста, поэтому типы столбцов
  /// определены по смыслу заголовков.
  /// </summary>
  [DebuggerDisplay("LawType: {LawType}, RecordNumber: {RecordNumber}, Parameters: {Parameters}")]
  public class kLiraTable_NonlinearDiagrams_TableRow : ILiraTableRow
  {
    /// <summary>Вид нелинейного закона (например "0 - Линейная", "1 - ...")</summary>
    public string LawType { get; set; } = string.Empty;

    /// <summary>Номер записи</summary>
    public int RecordNumber { get; set; }

    /// <summary>Параметры закона (текстовое представление набора значений)</summary>
    public string Parameters { get; set; } = string.Empty;

    /// <summary>Конструктор без параметров</summary>
    public kLiraTable_NonlinearDiagrams_TableRow() { }

    /// <summary>Полный конструктор</summary>
    public kLiraTable_NonlinearDiagrams_TableRow(string lawType, int recordNumber, string parameters)
    {
      LawType = lawType;
      RecordNumber = recordNumber;
      Parameters = parameters;
    }

    /// <summary>
    /// Создаёт строку из строки двумерного массива <paramref name="array"/>.
    /// Ожидаемый порядок столбцов: 0 — LawType (Вид нелинейного закона),
    /// 1 — RecordNumber (Номер записи), 2 — Parameters (Параметры).
    /// </summary>
    public static kLiraTable_NonlinearDiagrams_TableRow FromArray(object[,] array, int rowIndex)
    {
      var row = new kLiraTable_NonlinearDiagrams_TableRow();
      row.LoadFromArray(array, rowIndex);
      return row;
    }

    // А это — реализация интерфейса
    public void LoadFromArray(object[,] array, int rowIndex)
    {
      LawType = array[rowIndex, 0]?.ToString() ?? string.Empty;
      RecordNumber = ToInt(array[rowIndex, 1]);
      Parameters = array[rowIndex, 2]?.ToString() ?? string.Empty;
    }

    private static int ToInt(object value)
    {
      if (value == null || value is DBNull) return 0;
      return value is int i ? i : (int.TryParse(value.ToString(), out int parsed) ? parsed : 0);
    }

    public override bool Equals(object? obj)
        => obj is kLiraTable_NonlinearDiagrams_TableRow o
           && LawType == o.LawType
           && RecordNumber == o.RecordNumber
           && Parameters == o.Parameters;

    public override int GetHashCode()
        => HashCode.Combine(LawType, RecordNumber, Parameters);

    public override string ToString()
        => $"Вид нелинейного закона: {LawType}, Номер записи: {RecordNumber}, Параметры: {Parameters}";
  }
}
