using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace LiraSAPRTest.ModelLiraSapr
{
  /// <summary>
  /// Строка таблицы АЖТ — абсолютно жёстких тел (<c>kLiraTable_PRBs</c>).
  /// Имена свойств соответствуют заголовкам столбцов таблицы:
  /// Номер АЖТ, Тип АЖТ, Номер ведущего узла АЖТ, Список ведомых узлов АЖТ.
  /// </summary>
  [DebuggerDisplay("Number: {Number}, Type: {Type}, MasterNodeId: {MasterNodeId}, SlaveNodeIds: {SlaveNodeIds}")]
  public class kLiraTable_PRBs_TableRow : ILiraTableRow
  {
    /// <summary>Номер АЖТ</summary>
    public int Number { get; set; }

    /// <summary>Тип АЖТ (например "Все степени свободы")</summary>
    public string Type { get; set; } = string.Empty;

    /// <summary>Номер ведущего узла АЖТ</summary>
    public int MasterNodeId { get; set; }

    /// <summary>Список ведомых узлов АЖТ</summary>
    public List<int> SlaveNodeIds { get; set; } = new List<int>();

    /// <summary>Конструктор без параметров</summary>
    public kLiraTable_PRBs_TableRow() { }

    /// <summary>Полный конструктор</summary>
    public kLiraTable_PRBs_TableRow(int number, string type, int masterNodeId, List<int> slaveNodeIds)
    {
      Number = number;
      Type = type;
      MasterNodeId = masterNodeId;
      SlaveNodeIds = slaveNodeIds ?? new List<int>();
    }

    /// <summary>
    /// Создаёт строку из строки двумерного массива <paramref name="array"/>.
    /// Ожидаемый порядок столбцов: 0 — Number, 1 — Type, 2 — MasterNodeId,
    /// 3 — SlaveNodeIds (строка вида "102075 102085 125908").
    /// </summary>
    public static kLiraTable_PRBs_TableRow FromArray(object[,] array, int rowIndex)
    {
      var row = new kLiraTable_PRBs_TableRow();
      row.LoadFromArray(array, rowIndex);
      return row;
    }

    // А это — реализация интерфейса
    public void LoadFromArray(object[,] array, int rowIndex)
    {
      Number = ToInt(array[rowIndex, 0]);
      Type = array[rowIndex, 1]?.ToString() ?? string.Empty;
      MasterNodeId = ToInt(array[rowIndex, 2]);
      SlaveNodeIds = ParseNodeIds(array[rowIndex, 3]?.ToString());
    }

    /// <summary>
    /// Разбирает строку вида "102075 102085 125908" (разделители — пробелы, запятые
    /// или точка с запятой) в список целых чисел. Некорректные элементы пропускаются.
    /// </summary>
    public static List<int> ParseNodeIds(string? input)
    {
      var result = new List<int>();
      if (string.IsNullOrWhiteSpace(input))
        return result;

      var parts = input.Split(new[] { ' ', ',', ';', '\t', '\r', '\n' },
                              StringSplitOptions.RemoveEmptyEntries);
      foreach (var part in parts)
      {
        if (int.TryParse(part, out int value))
          result.Add(value);
      }

      return result;
    }

    private static int ToInt(object value)
    {
      if (value == null || value is DBNull) return 0;
      return value is int i ? i : (int.TryParse(value.ToString(), out int parsed) ? parsed : 0);
    }

    public override bool Equals(object? obj)
        => obj is kLiraTable_PRBs_TableRow o
           && Number == o.Number
           && Type == o.Type
           && MasterNodeId == o.MasterNodeId
           && SlaveNodeIds.SequenceEqual(o.SlaveNodeIds);

    public override int GetHashCode()
    {
      var hash = new HashCode();
      hash.Add(Number);
      hash.Add(Type);
      hash.Add(MasterNodeId);
      foreach (var nodeId in SlaveNodeIds)
        hash.Add(nodeId);
      return hash.ToHashCode();
    }

    public override string ToString()
        => $"Номер АЖТ: {Number}, Тип АЖТ: {Type}, Ведущий узел: {MasterNodeId}, " +
           $"Ведомые узлы: [{string.Join(", ", SlaveNodeIds)}]";
  }
}
