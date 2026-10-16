using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace LiraSAPRTest
{
  /// <summary>
  /// Строка таблицы типов и узлов конечных элементов.
  /// </summary>
  [DebuggerDisplay("ID: {ID}, Type: {Type}, NodeIds: {NodeIds}")]
  public class kLiraTable_Elements_TypeAndNodes_TableRow : ILiraTableRow
  {
    /// <summary>Номер конечного элемента</summary>
    public int ID { get; set; }

    /// <summary>Тип конечного элемента</summary>
    public int Type { get; set; }

    /// <summary>Узлы конечного элемента</summary>
    public List<int> NodeIds { get; set; } = new List<int>();

    /// <summary>Конструктор без параметров</summary>
    public kLiraTable_Elements_TypeAndNodes_TableRow() { }

    /// <summary>Полный конструктор</summary>
    public kLiraTable_Elements_TypeAndNodes_TableRow(int id, int type, List<int> nodeIds)
    {
      ID = id;
      Type = type;
      NodeIds = nodeIds ?? new List<int>();
    }

    /// <summary>
    /// Создаёт строку из строки двумерного массива <paramref name="array"/>.
    /// Ожидаемый порядок столбцов: 0 — ID, 1 — Type, 2 — NodeIds (строка вида "1, 2, 3").
    /// </summary>
    public static kLiraTable_Elements_TypeAndNodes_TableRow FromArray(object[,] array, int rowIndex)
    {
      var row = new kLiraTable_Elements_TypeAndNodes_TableRow();
      row.LoadFromArray(array, rowIndex);
      return row;
    }

    // А это — реализация интерфейса
    public void LoadFromArray(object[,] array, int rowIndex)
    {
      ID = Convert.ToInt32(array[rowIndex, 0]);
      Type = Convert.ToInt32(array[rowIndex, 1]);
      NodeIds = ParseNodeIds(array[rowIndex, 2]?.ToString());
    }



    /// <summary>
    /// Разбирает строку вида "1, 2, 3" в список целых чисел.
    /// Некорректные элементы пропускаются.
    /// </summary>
    public static List<int> ParseNodeIds(string? input)
    {
      var result = new List<int>();
      if (string.IsNullOrWhiteSpace(input))
        return result;

      foreach (var part in input.Split(','))
      {
        if (int.TryParse(part.Trim(), out int value))
          result.Add(value);
      }

      return result;
    }

    public override bool Equals(object? obj)
        => obj is kLiraTable_Elements_TypeAndNodes_TableRow o
           && ID == o.ID
           && Type == o.Type
           && NodeIds.SequenceEqual(o.NodeIds);

    public override int GetHashCode()
    {
      var hash = new HashCode();
      hash.Add(ID);
      hash.Add(Type);
      foreach (var nodeId in NodeIds)
        hash.Add(nodeId);
      return hash.ToHashCode();
    }

    public override string ToString()
        => $"ID: {ID}, Type: {Type}, NodeIds: [{string.Join(", ", NodeIds)}]";
  }
}