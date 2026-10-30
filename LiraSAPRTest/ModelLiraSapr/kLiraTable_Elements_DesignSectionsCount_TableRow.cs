using System;
using System.Diagnostics;

namespace LiraSAPRTest.ModelLiraSapr
{
  /// <summary>
  /// Строка таблицы «Количество расчётных сечений» (<c>kLiraTable_Elements_DesignSectionsCount</c>).
  /// Имена свойств соответствуют заголовкам столбцов таблицы:
  /// КЭ, Количество расчётных сечений.
  /// Оба столбца — целые.
  /// </summary>
  [DebuggerDisplay("ElementNumber: {ElementNumber}, DesignSectionsCount: {DesignSectionsCount}")]
  public class kLiraTable_Elements_DesignSectionsCount_TableRow : ILiraTableRow
  {
    /// <summary>КЭ — номер конечного элемента</summary>
    public int ElementNumber { get; set; }

    /// <summary>Количество расчётных сечений</summary>
    public int DesignSectionsCount { get; set; }

    /// <summary>Конструктор без параметров</summary>
    public kLiraTable_Elements_DesignSectionsCount_TableRow() { }

    /// <summary>Полный конструктор</summary>
    public kLiraTable_Elements_DesignSectionsCount_TableRow(int elementNumber, int designSectionsCount)
    {
      ElementNumber = elementNumber;
      DesignSectionsCount = designSectionsCount;
    }

    /// <summary>
    /// Создаёт строку из строки двумерного массива <paramref name="array"/>.
    /// Ожидаемый порядок столбцов: 0 — ElementNumber (КЭ), 1 — DesignSectionsCount.
    /// </summary>
    public static kLiraTable_Elements_DesignSectionsCount_TableRow FromArray(object[,] array, int rowIndex)
    {
      var row = new kLiraTable_Elements_DesignSectionsCount_TableRow();
      row.LoadFromArray(array, rowIndex);
      return row;
    }

    // А это — реализация интерфейса
    public void LoadFromArray(object[,] array, int rowIndex)
    {
      ElementNumber = ToInt(array[rowIndex, 0]);
      DesignSectionsCount = ToInt(array[rowIndex, 1]);
    }

    private static int ToInt(object value)
    {
      if (value == null || value is DBNull) return 0;
      return value is int i ? i : (int.TryParse(value.ToString(), out int parsed) ? parsed : 0);
    }

    public override bool Equals(object? obj)
        => obj is kLiraTable_Elements_DesignSectionsCount_TableRow o
           && ElementNumber == o.ElementNumber
           && DesignSectionsCount == o.DesignSectionsCount;

    public override int GetHashCode()
        => HashCode.Combine(ElementNumber, DesignSectionsCount);

    public override string ToString()
        => $"КЭ: {ElementNumber}, Количество расчётных сечений: {DesignSectionsCount}";
  }
}
