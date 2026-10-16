
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using System.IO;
using System.Runtime.InteropServices;

using SapphireLib;

namespace SapphirLookup
{
  internal class Program
  {
    // Импортируем нативные функции Windows для работы с COM-таблицей запущенных объектов (ROT)
    [DllImport("ole32.dll", CharSet = CharSet.Unicode)]
    private static extern int CLSIDFromProgIDEx(string lpszProgID, out Guid lpclsid);

    [DllImport("oleaut32.dll", PreserveSig = false)]
    private static extern void GetActiveObject(ref Guid rclsid, IntPtr pvReserved, [MarshalAs(UnmanagedType.IUnknown)] out object ppunk);


    static void Main(string[] args)
    {
      string progId = "Sapphire.Application";
      SapphireLib.Application m_pSapApp = null;
      SapphireLib.AutoDocument activeDoc = null;
      SapphireLib.AutoProject project = null;
      SapphireLib.AutoStorey storey = null;
      try
      {
        // Задаем ширину (в количестве символов)
        Console.WindowWidth = 150;

        // Задаем высоту (в строках)
        Console.WindowHeight = 70;

        // Рекомендуется также расширить экранный буфер, 
        // чтобы не появлялись полосы прокрутки, если таблица широкая
        Console.BufferWidth = 150;
        Console.BufferHeight = 1000;





        Console.WriteLine("Пытаемся подключиться к открытому Sapphire...");

        // 1. Получаем GUID (идентификатор класса) по текстовому ProgID
        int hr = CLSIDFromProgIDEx(progId, out Guid clsid);
        if (hr < 0) Marshal.ThrowExceptionForHR(hr);

        // 2. Запрашиваем ссылку на уже запущенный процесс Sapphire
        GetActiveObject(ref clsid, IntPtr.Zero, out object comObject);
        m_pSapApp = (SapphireLib.Application)comObject; // неявно приводится к dynamic

        Console.WriteLine("Успешно подключились!");

        // 3. получаем активный документ

        activeDoc = (SapphireLib.AutoDocument)m_pSapApp.GetActiveDoc();
        if (activeDoc == null)
        {
          activeDoc = (SapphireLib.AutoDocument)m_pSapApp.NewDocument();  // создаем новый документ
        }

        Console.WriteLine("Активный документ получен!");

        ////Получение и установка активного этажа
        //storey = activeDoc.GetActiveStorey();
        //activeDoc.SetActiveStorey(1);
        ////Получить выбранный в Сапфире объект

        Console.WriteLine($"В Sapphire выбрано моделей: {activeDoc.CountSelectedModel}");

        for (int i = 0; i < activeDoc.CountSelectedModel; i++)
        {
          SapphireLib.IAutoModel model1 = activeDoc.GetSelectedModel(i);

          Console.WriteLine("\n");
          Console.WriteLine($"Тип модели: {model1.TypeModel} - {model1.TypeModelName}");
          Console.WriteLine(new string('-', 150));

          GetObject(model1);
        }


        const int UPDATE_MODELS = 0x1000;      // обновить (обновить неактуальные модели) модель, структуру модели, и перерисовать все
        const int UPDATE_TREE_VIEW = 0x20000;  // Отработать изменение в диалоге структура проекта


        //--------------- обновить и нарисовать модели, обновить дерево проекта ------
        activeDoc.DrawModel(UPDATE_MODELS | UPDATE_TREE_VIEW);
        m_pSapApp.ClearOutputWnd(1); //очистить: 1-окно процессов; 2 - окно ошибок; 3 - окно предупреждений

      }
      catch (COMException ex) when ((uint)ex.HResult == 0x800401E3) // MK_E_UNAVAILABLE
      {
        Console.WriteLine("Ошибка: Sapphire в данный момент вообще не запущен на компьютере.");
      }
      catch (PlatformNotSupportedException)
      {
        // Этот блок сработает на Linux/macOS или в Windows Terminal,
        // где программно изменить размер окна системно запрещено
        Console.WriteLine("Окружение не поддерживает принудительное изменение размера окна.");
      }
      catch (Exception ex)
      {
        Console.WriteLine($"Произошла ошибка: {ex.Message}");
      }
      finally
      {
        // 5. Очищаем ссылки за собой
        if (storey != null) { Marshal.ReleaseComObject(storey); storey = null; }
        if (project != null) { Marshal.ReleaseComObject(project); project = null; }
        if (activeDoc != null) { Marshal.ReleaseComObject(activeDoc); activeDoc = null; }
        if (m_pSapApp != null) { Marshal.ReleaseComObject(m_pSapApp); m_pSapApp = null; }

        GC.Collect();
        GC.WaitForPendingFinalizers();
      }
      Console.WriteLine("Успешно закончили работу!");
      Console.ReadKey();

    }







    static void GetObject(SapphireLib.IAutoModel Model)
    {
      const int TM_WALL = 10;         // стена
      const int TM_SLAB = 11;         // Плита
      const int TM_COLUMN = 12;       // колонна
      const int TM_BEAM = 13;         // Балка
      if (Model != null)
      {
        //int type_Obj = Model.TypeModel;
        //switch (type_Obj)
        //{
        //  case TM_WALL:
        //    Console.WriteLine($"Стена ID:{Model.ID} TmpId:{Model.TmpId}");
        //    break;
        //  case TM_SLAB:
        //    Console.WriteLine($"Плита ID:{Model.ID} TmpId:{Model.TmpId}");
        //    break;
        //  case TM_COLUMN:
        //    Console.WriteLine($"Колонна ID:{Model.ID} TmpId:{Model.TmpId}");
        //    break;
        //  case TM_BEAM:
        //    Console.WriteLine($"Балка ID:{Model.ID} TmpId:{Model.TmpId}");
        //    break;
        //  default:
        //    Console.WriteLine($"Данный тип модели пока не обрабатывается! ID:{Model.ID} TmpId:{Model.TmpId}");
        //    return;
        //}

        try
        {
          GetAllParam(Model);          // получаем значения всех параметров объекта
        }
        catch (Exception ex)
        {
          Console.WriteLine($"Ошибка: Произошла ошибка при получение параметров!");
          Console.WriteLine($"Ошибка: {ex.Message}");

          //throw;
        }
      }
    }



    /// <summary>
    /// Получаем значения всех параметров объекта 
    /// </summary>
    /// <param name="Obj"></param>
    static void GetAllParam(SapphireLib.IAutoModel Obj)
    {
      if (Obj == null) return;

      // 1. Создаем экземпляр класса Random
      Random rand = new Random();

      var value = new Dictionary<string, List<string>>();
      var names = new Dictionary<string, string>();


      const int OBP_VALUES = 0x10;  // значение параметров
      const int OBP_NAMES = 0x4;    // название параметров
      const int OBP_COMMENTS = 0x8; // Описание параметров

      object refparNames = new Object[] { "" };
      object refparValue = new Object[] { "" };
      //object refparComments = new Object[] { "" };


      Obj.GetMnParam(ref refparNames, OBP_NAMES); // получаем значения параметров
      Obj.GetMnParam(ref refparValue, OBP_VALUES); // получаем значения параметров
                                                   //Obj.GetMnParam(ref refparValue, OBP_COMMENTS); // получаем значения параметров


      var parNames = (object[])refparNames;
      var parValue = (object[])refparValue;
      //var parComments = (object[])refparComments;


      for (int i = 0; i < parNames.Length; i += 3)
      {
        var mnemoNames = parNames[i];    // мнемоника параметра
        var valueNames = parNames[i + 1];  // значение параметра
                                           //var flagNames = parNames[i + 2];  // флаги: тип параметра, скрытый, только для чтения, ....
        names.Add(mnemoNames.ToString(), valueNames.ToString());
      }



      //int num = parValue.Length;       // размер массива параметров. Количество параметров = num/3
      for (int i = 0; i < parValue.Length; i += 3)
      {
        var mnemoValue = parValue[i];    // мнемоника параметра
        var valueValue = parValue[i + 1];  // значение параметра
        var flagValue = parValue[i + 2];  // флаги: тип параметра, скрытый, только для чтения, ....

        if (value.ContainsKey(mnemoValue.ToString()))
        {
          value.Add($"{mnemoValue.ToString()}____{rand.Next(1, 101)}", new List<string>() { valueValue.ToString(), flagValue.ToString() });

        }
        else
        {
          value.Add(mnemoValue.ToString(), new List<string>() { valueValue.ToString(), flagValue.ToString() });
        }
      }




      foreach (var v in value)
      {
        string name = " --- ";
        if (names.Any(q => q.Key == v.Key))
        {
          name = names.Where(q => q.Key == v.Key).First().Value.ToString();
        }

        int i = 0;
        if (v.Value[1] != "")
        {
          i = int.Parse(v.Value[1]);
        }

        string s = $"{i:X}";
        Console.WriteLine($"{v.Key,25}   |{name,50}   |{v.Value[0],40}   |0x{s:,40}");
      }



    }

    /// <summary>
    /// Материалы объекта 
    /// </summary>
    /// <param name="Obj"></param>
    static void ObjMaterial(SapphireLib.IAutoModel Obj)
    {
      if (Obj != null)
      {
        var guid_material = Obj.Parameter["M_MATERIAL"]; // получаем GUID материала
        SapphireLib.IAutoMaterial material = (SapphireLib.IAutoMaterial)Obj.GetMaterial();              // получаем указатель не материал
        var GUID = material.GUID;          // получаем GUID материала
        var name = material.name;          // название материала
        var Density = material.Density;    // объемный вес материала
        const int TM_WALL = 10;         // стена
        if (Obj.TypeModel == TM_WALL)
        {
          SapphireLib.AutoDocument ActiveDoc = (SapphireLib.AutoDocument)Obj.GetActiveDoc();                           // получаем указатель на документ
          SapphireLib.IAutoSetMaterials SetMaterials = (SapphireLib.IAutoSetMaterials)ActiveDoc.GetSetMaterials(); // получаем библиотеку материалов документа
          material = (SapphireLib.IAutoMaterial)SetMaterials.GetMaterialByName("Кирпич керамический");           // получаем материал "Кирпич керамический"
          if (material != null)
            Obj.Parameter["M_MATERIAL"] = material.GUID;        // назначаем материал объекту
        }
      }
    }
  }
}
