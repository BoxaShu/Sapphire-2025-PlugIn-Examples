using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;

namespace LiraSAPRTest.ModelLiraSapr
{
  /// <summary>
  /// Строка таблицы «Нагрузки» (<c>kLiraTable_Loads</c>).
  /// Имена свойств соответствуют заголовкам столбцов таблицы:
  /// Тип, Узел/Элемент, Комментарий, Параметры, Закон динамики, Параметры закона динамики.
  /// «Узел/Элемент» — <see cref="int"/>, остальные — строки.
  /// </summary>
  [DebuggerDisplay("Type: {Type}, NodeOrElement: {NodeOrElement}, DynamicsLaw: {DynamicsLaw}")]
  public class kLiraTable_Loads_TableRow : ILiraTableRow
  {
    /// <summary>Тип динамического воздействия или конечного элемента (например "B163")</summary>
    public string Type { get; set; } = string.Empty;

    /// <summary>Узел / Элемент — номер узла или конечного элемента расчётной схемы</summary>
    public int NodeOrElement { get; set; }

    /// <summary>Комментарий — произвольное пояснение пользователя</summary>
    public string Comment { get; set; } = string.Empty;

    /// <summary>
    /// Параметры воздействия (т, м, °С). Строка вида "P:0.440000 OnlyForFlexPart:0 dY1:0.000000 dZ1:0.000000".
    /// </summary>
    public string Parameters { get; set; } = string.Empty;

    /// <summary>Закон динамики — закон изменения нагрузки во времени</summary>
    public string DynamicsLaw { get; set; } = string.Empty;

    /// <summary>Параметры закона динамики — настройки закона (частота, период, затухание и т.п.)</summary>
    public string DynamicsLawParameters { get; set; } = string.Empty;

    /// <summary>Конструктор без параметров</summary>
    public kLiraTable_Loads_TableRow() { }

    /// <summary>Полный конструктор</summary>
    public kLiraTable_Loads_TableRow(
        string type, int nodeOrElement, string comment,
        string parameters, string dynamicsLaw, string dynamicsLawParameters)
    {
      Type = type;
      NodeOrElement = nodeOrElement;
      Comment = comment;
      Parameters = parameters;
      DynamicsLaw = dynamicsLaw;
      DynamicsLawParameters = dynamicsLawParameters;
    }

    /// <summary>
    /// Создаёт строку из строки двумерного массива <paramref name="array"/>.
    /// Ожидаемый порядок столбцов: 0 — Type, 1 — NodeOrElement, 2 — Comment,
    /// 3 — Parameters, 4 — DynamicsLaw, 5 — DynamicsLawParameters.
    /// </summary>
    public static kLiraTable_Loads_TableRow FromArray(object[,] array, int rowIndex)
    {
      var row = new kLiraTable_Loads_TableRow();
      row.LoadFromArray(array, rowIndex);
      return row;
    }

    // А это — реализация интерфейса
    public void LoadFromArray(object[,] array, int rowIndex)
    {
      Type = ToText(array[rowIndex, 0]);
      NodeOrElement = ToInt(array[rowIndex, 1]);
      Comment = ToText(array[rowIndex, 2]);
      Parameters = ToText(array[rowIndex, 3]);
      DynamicsLaw = ToText(array[rowIndex, 4]);
      DynamicsLawParameters = ToText(array[rowIndex, 5]);
    }

    private static string ToText(object value)
        => value == null || value is DBNull ? string.Empty : value.ToString() ?? string.Empty;

        /// <summary>
        /// Разбирает <see cref="Parameters"/> (строка вида "P:0.440000 OnlyForFlexPart:0 dY1:0.000000 dZ1:0.000000")
        /// в словарь «ключ → значение». Пары разделяются пробелом, ключ и значение — двоеточием.
        /// Токены без двоеточия пропускаются.
        /// </summary>
        public Dictionary<string, string> ParseParameters()
        {
          var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
          if (string.IsNullOrWhiteSpace(Parameters)) return result;

          foreach (var token in Parameters.Split(
                       new[] { ' ', '\t', '\r', '\n' },
                       StringSplitOptions.RemoveEmptyEntries))
          {
            var colon = token.IndexOf(':');
            if (colon <= 0 || colon >= token.Length - 1) continue;
            var key = token.Substring(0, colon).Trim();
            var value = token.Substring(colon + 1).Trim();
            if (key.Length > 0) result[key] = value;
          }

          return result;
        }

        /// <summary>
        /// Разбирает <see cref="Parameters"/> в словарь «ключ → число».
        /// Токены с нечисловым значением пропускаются; при повторяющихся ключах остаётся последний.
        /// </summary>
        public Dictionary<string, double> ParseNumericParameters()
        {
          var result = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase);
          foreach (var pair in ParseParameters())
          {
            if (double.TryParse(pair.Value, NumberStyles.Any, CultureInfo.CurrentCulture, out double current))
              result[pair.Key] = current;
            else if (double.TryParse(pair.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out double invariant))
              result[pair.Key] = invariant;
          }

          return result;
        }

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
        => obj is kLiraTable_Loads_TableRow o
           && Type == o.Type
           && NodeOrElement == o.NodeOrElement
           && Comment == o.Comment
           && Parameters == o.Parameters
           && DynamicsLaw == o.DynamicsLaw
           && DynamicsLawParameters == o.DynamicsLawParameters;

    public override int GetHashCode()
        => HashCode.Combine(Type, NodeOrElement, Comment, Parameters, DynamicsLaw, DynamicsLawParameters);

    public override string ToString()
        => $"Тип: {Type}, Узел/Элемент: {NodeOrElement}, Закон динамики: {DynamicsLaw}";
  }
}