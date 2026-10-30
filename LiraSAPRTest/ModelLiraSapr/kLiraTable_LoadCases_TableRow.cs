using System;
using System.Diagnostics;

namespace LiraSAPRTest.ModelLiraSapr
{
  /// <summary>
  /// Строка таблицы «Загружения» (<c>kLiraTable_LoadCases</c>).
  /// Имена свойств соответствуют заголовкам столбцов таблицы:
  /// Загружение, Наименование загружения, Подзадача, Вид загружения, Формула.
  /// Первый столбец — целый, остальные — строки.
  /// </summary>
  [DebuggerDisplay("LoadCaseNumber: {LoadCaseNumber}, Name: {Name}, Subtask: {Subtask}, Kind: {Kind}, Formula: {Formula}")]
  public class kLiraTable_LoadCases_TableRow : ILiraTableRow
  {
    /// <summary>Загружение — номер загружения</summary>
    public int LoadCaseNumber { get; set; }

    /// <summary>Наименование загружения</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Подзадача (например "1. Основная задача: C1. Основная задача; D1. Основная задача")</summary>
    public string Subtask { get; set; } = string.Empty;

    /// <summary>Вид загружения (например "Постоянное ( P)")</summary>
    public string Kind { get; set; } = string.Empty;

    /// <summary>Формула (может быть пустой)</summary>
    public string Formula { get; set; } = string.Empty;

    /// <summary>Конструктор без параметров</summary>
    public kLiraTable_LoadCases_TableRow() { }

    /// <summary>Полный конструктор</summary>
    public kLiraTable_LoadCases_TableRow(
        int loadCaseNumber,
        string name,
        string subtask,
        string kind,
        string formula)
    {
      LoadCaseNumber = loadCaseNumber;
      Name = name;
      Subtask = subtask;
      Kind = kind;
      Formula = formula;
    }

    /// <summary>
    /// Создаёт строку из строки двумерного массива <paramref name="array"/>.
    /// Ожидаемый порядок столбцов: 0 — LoadCaseNumber (Загружение), 1 — Name,
    /// 2 — Subtask, 3 — Kind, 4 — Formula.
    /// </summary>
    public static kLiraTable_LoadCases_TableRow FromArray(object[,] array, int rowIndex)
    {
      var row = new kLiraTable_LoadCases_TableRow();
      row.LoadFromArray(array, rowIndex);
      return row;
    }

    // А это — реализация интерфейса
    public void LoadFromArray(object[,] array, int rowIndex)
    {
      LoadCaseNumber = ToInt(array[rowIndex, 0]);
      Name = array[rowIndex, 1]?.ToString() ?? string.Empty;
      Subtask = array[rowIndex, 2]?.ToString() ?? string.Empty;
      Kind = array[rowIndex, 3]?.ToString() ?? string.Empty;
      Formula = array[rowIndex, 4]?.ToString() ?? string.Empty;
    }

    private static int ToInt(object value)
    {
      if (value == null || value is DBNull) return 0;
      return value is int i ? i : (int.TryParse(value.ToString(), out int parsed) ? parsed : 0);
    }

    public override bool Equals(object? obj)
        => obj is kLiraTable_LoadCases_TableRow o
           && LoadCaseNumber == o.LoadCaseNumber
           && Name == o.Name
           && Subtask == o.Subtask
           && Kind == o.Kind
           && Formula == o.Formula;

    public override int GetHashCode()
        => HashCode.Combine(LoadCaseNumber, Name, Subtask, Kind, Formula);

    public override string ToString()
        => $"Загружение: {LoadCaseNumber}, Наименование: {Name}, Подзадача: {Subtask}, " +
           $"Вид: {Kind}, Формула: {Formula}";
  }
}
