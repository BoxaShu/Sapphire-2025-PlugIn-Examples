using System;
using System.Diagnostics;
using System.Globalization;

namespace LiraSAPRTest.ModelLiraSapr
{
  /// <summary>
  /// Строка таблицы «Коэффициенты к жёсткостям» (<c>kLiraTable_Stiffnesses_Factors</c>).
  /// Имена свойств соответствуют заголовкам столбцов таблицы:
  /// КЭ (номер КЭ), K_E (коэффициент к модулям упругости).
  /// Первый столбец — целый, второй — <see cref="double"/> (в данных приходит как System.Single).
  /// </summary>
  [DebuggerDisplay("ElementNumber: {ElementNumber}, KE: {KE}")]
  public class kLiraTable_Stiffnesses_Factors_TableRow : ILiraTableRow
  {
    /// <summary>КЭ — номер конечного элемента</summary>
    public int ElementNumber { get; set; }

    /// <summary>K_E — коэффициент к модулям упругости</summary>
    public double KE { get; set; }

    /// <summary>Конструктор без параметров</summary>
    public kLiraTable_Stiffnesses_Factors_TableRow() { }

    /// <summary>Полный конструктор</summary>
    public kLiraTable_Stiffnesses_Factors_TableRow(int elementNumber, double ke)
    {
      ElementNumber = elementNumber;
      KE = ke;
    }

    /// <summary>
    /// Создаёт строку из строки двумерного массива <paramref name="array"/>.
    /// Ожидаемый порядок столбцов: 0 — ElementNumber (КЭ), 1 — KE (K_E).
    /// </summary>
    public static kLiraTable_Stiffnesses_Factors_TableRow FromArray(object[,] array, int rowIndex)
    {
      var row = new kLiraTable_Stiffnesses_Factors_TableRow();
      row.LoadFromArray(array, rowIndex);
      return row;
    }

    // А это — реализация интерфейса
    public void LoadFromArray(object[,] array, int rowIndex)
    {
      ElementNumber = ToInt(array[rowIndex, 0]);
      KE = ToDouble(array[rowIndex, 1]);
    }

    private static int ToInt(object value)
    {
      if (value == null || value is DBNull) return 0;
      return value is int i ? i : (int.TryParse(value.ToString(), out int parsed) ? parsed : 0);
    }

    /// <summary>
    /// Приводит значение столбца к <see cref="double"/>.
    /// Пустые строки/null/DBNull дают 0; принимаются как текущая, так и инвариантная культура.
    /// </summary>
    private static double ToDouble(object value)
    {
      if (value == null || value is DBNull) return 0d;
      switch (value)
      {
        case double d: return d;
        case float f: return f;
        case decimal m: return (double)m;
        case int n: return n;
        default:
          var text = value.ToString();
          if (string.IsNullOrWhiteSpace(text)) return 0d;
          return double.TryParse(text, NumberStyles.Any, CultureInfo.CurrentCulture, out double current)
              ? current
              : (double.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out double invariant) ? invariant : 0d);
      }
    }

    public override bool Equals(object? obj)
        => obj is kLiraTable_Stiffnesses_Factors_TableRow o
           && ElementNumber == o.ElementNumber
           && KE.Equals(o.KE);

    public override int GetHashCode()
        => HashCode.Combine(ElementNumber, KE);

    public override string ToString()
        => $"КЭ: {ElementNumber}, K_E: {KE}";
  }
}
