using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiraSAPRTest
{
  public interface ILiraTableRow
  {
    void LoadFromArray(object[,] array, int rowIndex);
  }
}
