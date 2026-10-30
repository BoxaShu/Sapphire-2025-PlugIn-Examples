using System;
using System.Diagnostics;
using System.Globalization;

namespace LiraSAPRTest.ModelLiraSapr
{
  /// <summary>
  /// Строка таблицы «Элементы - материалы» (<c>kLiraTable_Elements_Materials</c>).
  /// Имена свойств соответствуют заголовкам столбцов таблицы:
  /// КЭ, Код материала, Список компонентов материала.
  /// Первый столбец — <see cref="int"/>, остальные — строки.
  /// </summary>
  [DebuggerDisplay("Element: {ElementNumber}, Material: {MaterialCode}, Components: {MaterialComponents}")]
  public class kLiraTable_Elements_Materials_TableRow : ILiraTableRow
  {
    /// <summary>КЭ (номер конечного элемента)</summary>
    public int ElementNumber { get; set; }

    /// <summary>Код материала (например "RC")</summary>
    public string MaterialCode { get; set; } = string.Empty;

    /// <summary>Список компонентов материала (например "3.2.2")</summary>
    public string MaterialComponents { get; set; } = string.Empty;

    /// <summary>Конструктор без параметров</summary>
    public kLiraTable_Elements_Materials_TableRow() { }

    /// <summary>Полный конструктор</summary>
    public kLiraTable_Elements_Materials_TableRow(
        int elementNumber, string materialCode, string materialComponents)
    {
      ElementNumber = elementNumber;
      MaterialCode = materialCode;
      MaterialComponents = materialComponents;
    }

    /// <summary>
    /// Создаёт строку из строки двумерного массива <paramref name="array"/>.
    /// Ожидаемый порядок столбцов: 0 — ElementNumber, 1 — MaterialCode, 2 — MaterialComponents.
    /// </summary>
    public static kLiraTable_Elements_Materials_TableRow FromArray(object[,] array, int rowIndex)
    {
      var row = new kLiraTable_Elements_Materials_TableRow();
      row.LoadFromArray(array, rowIndex);
      return row;
    }

    // А это — реализация интерфейса
    public void LoadFromArray(object[,] array, int rowIndex)
    {
      ElementNumber = ToInt(array[rowIndex, 0]);
      MaterialCode = ToText(array[rowIndex, 1]);
      MaterialComponents = ToText(array[rowIndex, 2]);
    }

    private static string ToText(object value)
        => value == null || value is DBNull ? string.Empty : value.ToString() ?? string.Empty;

    /// <summary>
    /// Приводит значение столбца к <see cref="int"/>.
    /// Пустые строки/null/DBNull дают 0.
    /// </summary>
    private static int ToInt(object value)
    {
      if (value == null || value is DBNull) return 0;
      switch (value)
      {
        case int n: return n;
        case long l: return (int)l;
        case double d: return (int)d;
        case float f: return (int)f;
        default:
          var text = value.ToString();
          if (string.IsNullOrWhiteSpace(text)) return 0;
          return int.TryParse(text, NumberStyles.Any, CultureInfo.CurrentCulture, out int current)
              ? current
              : (int.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out int invariant) ? invariant : 0);
      }
    }

    public override bool Equals(object? obj)
        => obj is kLiraTable_Elements_Materials_TableRow o
           && ElementNumber == o.ElementNumber
           && MaterialCode == o.MaterialCode
           && MaterialComponents == o.MaterialComponents;

    public override int GetHashCode()
        => HashCode.Combine(ElementNumber, MaterialCode, MaterialComponents);

    public override string ToString()
        => $"КЭ: {ElementNumber}, Код материала: {MaterialCode}, Компоненты: {MaterialComponents}";
  }
}