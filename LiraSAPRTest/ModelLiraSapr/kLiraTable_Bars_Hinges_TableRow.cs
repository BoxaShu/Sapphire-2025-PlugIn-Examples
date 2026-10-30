using System;
using System.Diagnostics;
using System.Globalization;

namespace LiraSAPRTest.ModelLiraSapr
{
  /// <summary>
  /// Строка таблицы шарниров стержней (<c>kLiraTable_Bars_Hinges</c>).
  /// Имена свойств соответствуют заголовкам столбцов таблицы:
  /// КЭ,
  /// X1(H), Y1(H), Z1(H) — т/м; UX(H), UY(H), UZ(H) — т*м/рад; W(H);
  /// X1(K), Y1(K), Z1(K) — т/м; UX(K), UY(K), UZ(K) — т*м/рад; W(K).
  /// Все столбцы, кроме первого (КЭ), — <see cref="double"/>.
  /// (H) — начало стержня, (K) — конец стержня.
  /// </summary>
  [DebuggerDisplay("ElementNumber: {ElementNumber}, X1H: {X1H}, Y1H: {Y1H}, Z1H: {Z1H}, UXH: {UXH}, UYH: {UYH}, UZH: {UZH}, WH: {WH}, X1K: {X1K}, Y1K: {Y1K}, Z1K: {Z1K}, UXK: {UXK}, UYK: {UYK}, UZK: {UZK}, WK: {WK}")]
  public class kLiraTable_Bars_Hinges_TableRow : ILiraTableRow
  {
    /// <summary>КЭ — номер конечного элемента (стержня)</summary>
    public int ElementNumber { get; set; }

    /// <summary>X1(H) — жёсткость связи по X в начале стержня, т/м</summary>
    public double X1H { get; set; }

    /// <summary>Y1(H) — жёсткость связи по Y в начале стержня, т/м</summary>
    public double Y1H { get; set; }

    /// <summary>Z1(H) — жёсткость связи по Z в начале стержня, т/м</summary>
    public double Z1H { get; set; }

    /// <summary>UX(H) — жёсткость связи по UX в начале стержня, т*м/рад</summary>
    public double UXH { get; set; }

    /// <summary>UY(H) — жёсткость связи по UY в начале стержня, т*м/рад</summary>
    public double UYH { get; set; }

    /// <summary>UZ(H) — жёсткость связи по UZ в начале стержня, т*м/рад</summary>
    public double UZH { get; set; }

    /// <summary>W(H) — жёсткость связи по W в начале стержня</summary>
    public double WH { get; set; }

    /// <summary>X1(K) — жёсткость связи по X в конце стержня, т/м</summary>
    public double X1K { get; set; }

    /// <summary>Y1(K) — жёсткость связи по Y в конце стержня, т/м</summary>
    public double Y1K { get; set; }

    /// <summary>Z1(K) — жёсткость связи по Z в конце стержня, т/м</summary>
    public double Z1K { get; set; }

    /// <summary>UX(K) — жёсткость связи по UX в конце стержня, т*м/рад</summary>
    public double UXK { get; set; }

    /// <summary>UY(K) — жёсткость связи по UY в конце стержня, т*м/рад</summary>
    public double UYK { get; set; }

    /// <summary>UZ(K) — жёсткость связи по UZ в конце стержня, т*м/рад</summary>
    public double UZK { get; set; }

    /// <summary>W(K) — жёсткость связи по W в конце стержня</summary>
    public double WK { get; set; }

    /// <summary>Конструктор без параметров</summary>
    public kLiraTable_Bars_Hinges_TableRow() { }

    /// <summary>Полный конструктор</summary>
    public kLiraTable_Bars_Hinges_TableRow(
        int elementNumber,
        double x1h,
        double y1h,
        double z1h,
        double uxh,
        double uyh,
        double uzh,
        double wh,
        double x1k,
        double y1k,
        double z1k,
        double uxk,
        double uyk,
        double uzk,
        double wk)
    {
      ElementNumber = elementNumber;
      X1H = x1h;
      Y1H = y1h;
      Z1H = z1h;
      UXH = uxh;
      UYH = uyh;
      UZH = uzh;
      WH = wh;
      X1K = x1k;
      Y1K = y1k;
      Z1K = z1k;
      UXK = uxk;
      UYK = uyk;
      UZK = uzk;
      WK = wk;
    }

    /// <summary>
    /// Создаёт строку из строки двумерного массива <paramref name="array"/>.
    /// Ожидаемый порядок столбцов: 0 — ElementNumber (КЭ),
    /// 1 — X1H, 2 — Y1H, 3 — Z1H, 4 — UXH, 5 — UYH, 6 — UZH, 7 — WH,
    /// 8 — X1K, 9 — Y1K, 10 — Z1K, 11 — UXK, 12 — UYK, 13 — UZK, 14 — WK.
    /// </summary>
    public static kLiraTable_Bars_Hinges_TableRow FromArray(object[,] array, int rowIndex)
    {
      var row = new kLiraTable_Bars_Hinges_TableRow();
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
      UXH = ToDouble(array[rowIndex, 4]);
      UYH = ToDouble(array[rowIndex, 5]);
      UZH = ToDouble(array[rowIndex, 6]);
      WH = ToDouble(array[rowIndex, 7]);
      X1K = ToDouble(array[rowIndex, 8]);
      Y1K = ToDouble(array[rowIndex, 9]);
      Z1K = ToDouble(array[rowIndex, 10]);
      UXK = ToDouble(array[rowIndex, 11]);
      UYK = ToDouble(array[rowIndex, 12]);
      UZK = ToDouble(array[rowIndex, 13]);
      WK = ToDouble(array[rowIndex, 14]);
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
        => obj is kLiraTable_Bars_Hinges_TableRow o
           && ElementNumber == o.ElementNumber
           && X1H.Equals(o.X1H)
           && Y1H.Equals(o.Y1H)
           && Z1H.Equals(o.Z1H)
           && UXH.Equals(o.UXH)
           && UYH.Equals(o.UYH)
           && UZH.Equals(o.UZH)
           && WH.Equals(o.WH)
           && X1K.Equals(o.X1K)
           && Y1K.Equals(o.Y1K)
           && Z1K.Equals(o.Z1K)
           && UXK.Equals(o.UXK)
           && UYK.Equals(o.UYK)
           && UZK.Equals(o.UZK)
           && WK.Equals(o.WK);

    public override int GetHashCode()
        => HashCode.Combine(
               HashCode.Combine(ElementNumber, X1H, Y1H, Z1H),
               HashCode.Combine(UXH, UYH, UZH, WH),
               HashCode.Combine(X1K, Y1K, Z1K, UXK),
               HashCode.Combine(UYK, UZK, WK)
           );

    public override string ToString()
        => $"КЭ: {ElementNumber}, " +
           $"X1(H): {X1H}, Y1(H): {Y1H}, Z1(H): {Z1H}, UX(H): {UXH}, UY(H): {UYH}, UZ(H): {UZH}, W(H): {WH}, " +
           $"X1(K): {X1K}, Y1(K): {Y1K}, Z1(K): {Z1K}, UX(K): {UXK}, UY(K): {UYK}, UZ(K): {UZK}, W(K): {WK}";
  }
}
