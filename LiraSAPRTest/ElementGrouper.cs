using LiraSAPRTest.ModelLiraSapr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiraSAPRTest
{


  public static class ElementGrouper
  {
    /// <summary>
    /// Делит элементы на группы связности: элементы попадают в одну группу,
    /// если они связаны через общие узлы (в т.ч. транзитивно — A↔B через узел 1,
    /// B↔C через узел 2 ⇒ A, B, C в одной группе).
    /// </summary>
    public static List<List<kLiraTable_Elements_TypeAndNodes_TableRow>> SplitIntoConnectedGroups(
        IReadOnlyList<kLiraTable_Elements_TypeAndNodes_TableRow> rows)
    {
      var result = new List<List<kLiraTable_Elements_TypeAndNodes_TableRow>>();
      if (rows == null || rows.Count == 0)
        return result;

      int n = rows.Count;

      // --- Union-Find ---
      var parent = new int[n];
      var rank = new int[n];
      for (int i = 0; i < n; i++) { parent[i] = i; rank[i] = 0; }

      int Find(int x)
      {
        // сжатие пути
        while (parent[x] != x)
        {
          parent[x] = parent[parent[x]];
          x = parent[x];
        }
        return x;
      }

      void Union(int a, int b)
      {
        int ra = Find(a), rb = Find(b);
        if (ra == rb) return;
        if (rank[ra] < rank[rb]) (ra, rb) = (rb, ra);
        parent[rb] = ra;
        if (rank[ra] == rank[rb]) rank[ra]++;
      }

      // Для каждого узла — индекс первого элемента, где он встретился.
      // Все последующие элементы с тем же узлом объединяем с этим первым.
      var nodeToFirstElement = new Dictionary<int, int>();

      for (int i = 0; i < n; i++)
      {
        var nodeIds = rows[i]?.NodeIds;
        if (nodeIds == null || nodeIds.Count == 0)
          continue;

        // Внутренние дубли узлов в одном элементе смысла не несут —
        // отсеиваем, чтобы не делать лишних обращений к словарю.
        var seenInElement = new HashSet<int>();

        foreach (int nodeId in nodeIds)
        {
          if (!seenInElement.Add(nodeId)) continue;

          if (nodeToFirstElement.TryGetValue(nodeId, out int firstIdx))
            Union(i, firstIdx);
          else
            nodeToFirstElement[nodeId] = i;
        }
      }

      // --- Сборка групп по корню, с сохранением порядка появления ---
      var groupsByRoot = new Dictionary<int, List<kLiraTable_Elements_TypeAndNodes_TableRow>>();
      var rootOrder = new List<int>();

      for (int i = 0; i < n; i++)
      {
        int root = Find(i);
        if (!groupsByRoot.TryGetValue(root, out var list))
        {
          list = new List<kLiraTable_Elements_TypeAndNodes_TableRow>();
          groupsByRoot[root] = list;
          rootOrder.Add(root);
        }
        list.Add(rows[i]);
      }

      result.Capacity = rootOrder.Count;
      foreach (int root in rootOrder)
        result.Add(groupsByRoot[root]);

      return result;
    }
  }
}
