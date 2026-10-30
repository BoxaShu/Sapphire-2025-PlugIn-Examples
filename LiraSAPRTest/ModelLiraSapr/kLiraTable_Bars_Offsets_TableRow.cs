using System;
using System.Diagnostics;
using System.Globalization;

namespace LiraSAPRTest.ModelLiraSapr
{
  /// <summary>
  /// Строка таблицы «Жёсткие вставки стержней» (<c>kLiraTable_Bars_Offsets</c>).
  /// Имена свойств соответствуют заголовкам столбцов таблицы:
  /// КЭ, X1(H) м, Y1(H) м, Z1(H) м, X1(K) м, Y1(K) м, Z1(K) м, Rotate (+ -).
  /// Смещения (столбцы 1–6) хранятся как <see cref="double"/> (в данных приходят как System.Single),
  /// последний столбец — строка (например "-").
  /// </summary>
  [DebuggerDisplay("ElementNumber: {ElementNumber}, X1H: {X1H}, Y1H: {Y1H}, Z1H: {Z1H}, X1K: {X1K}, Y1K: {Y1K}, Z1K: {Z1K}, Rotate: {Rotate}")]
  public class kLiraTable_Bars_Offsets_TableRow : ILiraTableRow
  {
    /// <summary>КЭ — номер конечного элемента (стержня)</summary>
    public int ElementNumber { get; set; }

    /// <summary>X1(H) — смещение начала стержня по X, м</summary>
    public double X1H { get; set; }

    /// <summary>Y1(H) — смещение начала стержня по Y, м</summary>
    public double Y1H { get; set; }

    /// <summary>Z1(H) — смещение начала стержня по Z, м</summary>
    public double Z1H { get; set; }

    /// <summary>X1(K) — смещение конца стержня по X, м</summary>
    public double X1K { get; set; }

    /// <summary>Y1(K) — смещение конца стержня по Y, м</summary>
    public double Y1K { get; set; }

    /// <summary>Z1(K) — смещение конца стержня по Z, м</summary>
    public double Z1K { get; set; }

    /// <summary>Rotate (+ -) — признак/угол поворота вставки (текстовое значение, например "-")</summary>
    public string Rotate { get; set; } = string.Empty;

    /// <summary>Конструктор без параметров</summary>
    public kLiraTable_Bars_Offsets_TableRow() { }

    /// <summary>Полный конструктор</summary>
    public kLiraTable_Bars_Offsets_TableRow(
        int elementNumber,
        double x1h,
        double y1h,
        double z1h,
        double x1k,
        double y1k,
        double z1k,
        string rotate)
    {
      ElementNumber = elementNumber;
      X1H = x1h;
      Y1H = y1h;
      Z1H = z1h;
      X1K = x1k;
      Y1K = y1k;
      Z1K = z1k;
      Rotate = rotate;
    }

    /// <summary>
    /// Создаёт строку из строки двумерного массива <paramref name="array"/>.
    /// Ожидаемый порядок столбцов: 0 — ElementNumber (КЭ),
    /// 1 — X1H, 2 — Y1H, 3 — Z1H, 4 — X1K, 5 — Y1K, 6 — Z1K, 7 — Rotate.
    /// </summary>
    public static kLiraTable_Bars_Offsets_TableRow FromArray(object[,] array, int rowIndex)
    {
      var row = new kLiraTable_Bars_Offsets_TableRow();
      row.LoadFromArray(array, rowIndex);
      return row;
    }

    // А это — реализация интерфейса
    public void LoadFromArray(object[,] array, int rowIndex)
    {
      ElementNumber = ToInt(array[rowIndex, 0]);
      X1H = ToDouble(array[rowIndex, 1]);
      Y1H = ToDouble(array[rowIndex, 2]);
      Z1H = ToDouble(array[rowIndex, 3]);
      X1K = ToDouble(array[rowIndex, 4]);
      Y1K = ToDouble(array[rowIndex, 5]);
      Z1K = ToDouble(array[rowIndex, 6]);
      Rotate = array[rowIndex, 7]?.ToString() ?? string.Empty;
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
        => obj is kLiraTable_Bars_Offsets_TableRow o
           && ElementNumber == o.ElementNumber
           && X1H.Equals(o.X1H)
           && Y1H.Equals(o.Y1H)
           && Z1H.Equals(o.Z1H)
           && X1K.Equals(o.X1K)
           && Y1K.Equals(o.Y1K)
           && Z1K.Equals(o.Z1K)
           && Rotate == o.Rotate;

    public override int GetHashCode()
        => HashCode.Combine(
               HashCode.Combine(ElementNumber, X1H, Y1H, Z1H),
               HashCode.Combine(X1K, Y1K, Z1K, Rotate)
           );

    public override string ToString()
        => $"КЭ: {ElementNumber}, X1(H): {X1H}, Y1(H): {Y1H}, Z1(H): {Z1H}, " +
           $"X1(K): {X1K}, Y1(K): {Y1K}, Z1(K): {Z1K}, Rotate: {Rotate}";
  }
}
