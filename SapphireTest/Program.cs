using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using System.IO;
using System.Runtime.InteropServices;

using SapphireLib;

namespace SapphireTest
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
      string progId = "Sapphire.Application"; //"Excel.Application";
      SapphireLib.Application m_pSapApp = null;
      SapphireLib.AutoDocument activeDoc = null;
      SapphireLib.AutoProject project = null;
      SapphireLib.AutoStorey storey = null;
      try
      {
        Console.WriteLine("Пытаемся подключиться к открытому Excel...");
        
        // 1. Получаем GUID (идентификатор класса) по текстовому ProgID
        int hr = CLSIDFromProgIDEx(progId, out Guid clsid);
        if (hr < 0) Marshal.ThrowExceptionForHR(hr);

        // 2. Запрашиваем ссылку на уже запущенный процесс Excel
        GetActiveObject(ref clsid, IntPtr.Zero, out object comObject);
        m_pSapApp = (SapphireLib.Application)comObject; // неявно приводится к dynamic

        Console.WriteLine("Успешно подключились!");

        // 3. получаем активный документ

        activeDoc = (SapphireLib.AutoDocument)m_pSapApp.GetActiveDoc();
        if (activeDoc == null)
        {
          activeDoc = (SapphireLib.AutoDocument)m_pSapApp.NewDocument();  // создаем новый документ
        }


        project = (SapphireLib.AutoProject)activeDoc.GetActiveProject();
        if (project == null)
        {
          project = (SapphireLib.AutoProject)activeDoc.NewProject();     // создаем новое здание
        }
        const int toUp = 1;          // toUp- движемся по этажам снизу вверх
                                     // const int toDn = 0;    // toDn- движемся по этажам сверху вниз


        storey = (SapphireLib.AutoStorey)project.GetFirstStorey(toUp); // Берем самый нижний этаж
        if (storey == null)
          storey = (SapphireLib.AutoStorey)project.NewStorey("Новый этаж");



        const int UPDATE_MODELS = 0x1000;      // обновить (обновить неактуальные модели) модель, структуру модели, и перерисовать все
        const int UPDATE_TREE_VIEW = 0x20000;  // Отработать изменение в диалоге структура проекта


        //-----------Сетка строительных осей-----------------
        MakeAxesGrid(project);
        //----------------Одиночная ось  --------------------
        MakeSingleAxes(project);
        //----------------- Стена----------------------------
        MakeWall(storey);
        //----------------- Плита перекрытия ----------------
        MakeSlab(storey);
        //----------------- Колонна ЖБ-----------------------
        MakeColumn(storey);
        //----------------- Колонна стальная ----------------
        //MakeSteelColumn(storey);
        //----------------- ЖБ - балка ----------------------
        MakeBeam(storey);
        //----------------- Балка стальная ----------------------
        //MakeSteelBeam(Storey);
        //------------ Доступ к объектам проекта ------------
        GetObject(project);


        //--------------- обновить и нарисовать модели, обновить дерево проекта ------
        activeDoc.DrawModel(UPDATE_MODELS | UPDATE_TREE_VIEW);
        m_pSapApp.ClearOutputWnd(1); //очистить: 1-окно процессов; 2 - окно ошибок; 3 - окно предупреждений

      }
      catch (COMException ex) when ((uint)ex.HResult == 0x800401E3) // MK_E_UNAVAILABLE
      {
        Console.WriteLine("Ошибка: Excel в данный момент вообще не запущен на компьютере.");
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


    static void MakeSingleAxes(SapphireLib.IAutoProject Project)
    {
      if (Project != null)
      {
        const int TM_DIMENSION_8 = 524308; // Обозначение Ось
        SapphireLib.IAutoObjDim Axes = (SapphireLib.IAutoObjDim)Project.NewModel(TM_DIMENSION_8);
        var Points = new Object[] { -4.0, 0.0, 0.0, -4.0, 10.0, 0.0 };  // объявляем массив точек  {x1,y1,z1, x2,y2,z2}
        Axes.SetPoints(Points);
        Axes.Parameter["M_MARK"] = "K1";  // марка оси
      }
    }

    /// <summary>
    /// Создание сетки осей
    /// </summary>
    /// <param name="Project"></param>
    static void MakeAxesGrid(SapphireLib.IAutoProject Project)
    {
      if (Project != null)
      {
        const int TM_AXES = 17;
        const int CNET_RECTANGULAR = 1;
        const int CNET_RADIAL = 2;
        SapphireLib.IAutoAxes AxesRect = (SapphireLib.IAutoAxes)Project.NewModel(TM_AXES);
        var Arr_1 = new Object[] { 3.0, 3.0, 3.0, 3.0 };  // объявляем массив 1
        var Arr_2 = new Object[] { 3.0, 4.0, 4.0, 3.0 }; // объявляем массив 2
        AxesRect.SetParamIntervals(CNET_RECTANGULAR, Arr_1, Arr_2); // задаем параметры для прямоугольной сетки
                                                                    // устанавливаем позицию сетки
        AxesRect.Parameter["M_POS_X"] = -1.5;
        AxesRect.Parameter["M_POS_Y"] = 2.5;
        AxesRect.Parameter["M_ANGLE_Z"] = 30;
        SapphireLib.IAutoAxes AxesRad = (SapphireLib.IAutoAxes)Project.NewModel(TM_AXES);
        var Arr_U = new Object[] { 15.0, 15.0, 30.0, 30.0, 15.0, 15.0 }; // объявляем массив 2
        AxesRad.SetParamIntervals(CNET_RADIAL, Arr_U, Arr_1); // задаем параметры для прямоугольной сетки
                                                              // устанавливаем позицию сетки
        AxesRad.Parameter["M_POS_X"] = 15;
        AxesRad.Parameter["M_POS_Y"] = 0.0;
        AxesRad.Parameter["M_EXTRADIUS"] = 20;
      }
    }


    /// <summary>
    /// Создание полилинии по точкам
    /// </summary>
    /// <param name="PolyLine"></param>
    static void CreatePolyLineByPoints(ref SapphireLib.IAutoPolyLine PolyLine)
    {
      if (PolyLine != null)
      {
        //       var Pnt = new object[9]; // объявляем массив точек (такой массив используется и для других данных, например, массив параметров)
        //       double[] pnt = { 0, 0, 0, 4, 0, 0, 4, 3, 0 }; // координаты точек осевой линии стены {x1,y1,z1, x2,y2,z2, x3,y3,z3...}
        //       for (int i = 0; i < 9; i++)
        //         Pnt[i] = pnt[i];   // заносим координаты точек в массив
        var Pnt = new Object[] { 0.0, 0.0, 0.0, 4.0, 0.0, 0.0, 4.0, 3.0, 0.0 }; // объявляем массив точек
        PolyLine.SetPoints(Pnt); // занесли точки в линию
      }
    }



    /// <summary>
    /// Создать плиту
    /// </summary>
    /// <param name="Storey"></param>
    /// <returns></returns>
    static SapphireLib.IAutoModel MakeSlab(SapphireLib.IAutoStorey Storey)
    {
      if (Storey != null)
      {
        const int TM_SLAB = 11;         // Плита
                                        // const int TM_SLAB_FND = 65547;  // Фундаментная плита
        SapphireLib.IAutoModel Slab = (SapphireLib.IAutoModel)Storey.NewModel(TM_SLAB);
        
        if (Slab != null)
        {
          Slab.InitDefault(0); // инициализация плиты параметрами по умолчанию (см. диалог "Настройки/Параметры элементов по умолчанию")
          SapphireLib.IAutoPolyLine Line = (SapphireLib.IAutoPolyLine)Slab.GetAxisLine(); // получили линию внешнего контура плиты
                                                                                          //       var Pnt = new object[15]; // объявляем массив точек (такой массив используется и для других данных, например, массив параметров)
                                                                                          //       double[] pnt = { 0.0, 0.0, 0.0,  4.0, 0.0, 0.0,  4.0, 3.0, 0.0,  0.0, 3.0, 0.0,  0.0, 0.0, 0.0 }; // координаты точек осевой линии стены {x1,y1,z1, x2,y2,z2, x3,y3,z3...}
                                                                                          //       for (int i = 0; i < 15; i++)
                                                                                          //         Pnt[i] = pnt[i];   // заносим координаты точек в массив
          var Pnt = new Object[] { 0.0, 0.0, 0.0, 4.0, 0.0, 0.0, 4.0, 3.0, 0.0, 0.0, 3.0, 0.0, 0.0, 0.0, 0.0 }; // объявляем массив точек
          Line.SetPoints(Pnt);                          // занесли точки в линию
          
          
          Slab.set_Parameter("M_THICKNESS", 0.18);      // Толщина плиты [м]
                                                        // (имена параметров можно посмотреть в окне свойств объекта, если включить опцию "Настройки САПФИР\редактирование\таблица параметров\показ мнемоник", или см. документацию)
          Slab.MakeModel();  // создать модель по параметрам
          return Slab;
        }
      }
      return null;
    }




    // Создать по заданному контуру проем в плите перекрытия
    static SapphireLib.IAutoModel MakeHoleInSlab(SapphireLib.IAutoModel Slab)
    {
      const int TM_RECESS = 29; // Ниша
      const int TM_HOLE = 30; // Проем
      var PointHole = new Object[] { 1.6, 1.0, 0.0, 2.4, 1.0, 0.0, 2.4, 2.0, 0.0, 2.0, 2.3, 0.0, 1.6, 2.0, 0.0, 1.6, 1.0, 0.0 }; // контур проема в глобальной СК
      SapphireLib.IAutoModel Hole = Slab.NewHoleGlb(TM_HOLE, PointHole);
      return Hole;
    }

    /// <summary>
    /// Получить ID всех проемов в стене/плите
    /// </summary>
    /// <param name="Wall"></param>
    /// <param name="IdHole"></param>
    /// <returns></returns>

  static int GetIdHole(SapphireLib.IAutoModel Wall, ref List<int> IdHole)
    {
      const int TM_DOOR = 32;
      const int TM_WINDOW = 31;

      int cnt = Wall.CountChild;
      for (int i = 0; i < cnt; i++)
      {
        SapphireLib.IAutoModel Model = Wall.GetChildByIndex(i);
        int type = Model.TypeModel;
        if (type == TM_WINDOW || type == TM_DOOR)
          IdHole.Add(Model.ID);
      }
      return IdHole.Count;
    }




/// <summary>
/// Удалить заданные проемы в стене
/// </summary>
/// <param name="Wall"></param>
/// <param name="IdHole"></param>


  static void RemoveHole(SapphireLib.IAutoModel Wall, List<int> IdHole)
    {
      SapphireLib.IAutoModelHolder MH = Wall.GetModelHolder();
      foreach (int ID in IdHole)
        MH.DelModelByID(ID);
    }






    /// <summary>
    /// Создать по заданному контуру проем/окно/дверь в стене
    /// </summary>
    /// <param name="Wall"></param>
    /// <returns></returns>
    static SapphireLib.IAutoModel MakeContourHoleInWall(SapphireLib.IAutoModel Wall)
    {
      const int TM_WINDOW = 31; // Окно
      const int TM_DOOR = 32;   // Дверь
      var PointHole = new Object[] { 1.6, 0.0, 0.8, 2.4, 0.0, 0.8, 2.4, 0.0, 2.1, 2.0, 0.0, 2.3, 1.6, 0.0, 2.1, 1.6, 0.0, 0.8 }; // контур проема в глобальной СК
      SapphireLib.IAutoModel Hole = Wall.NewHoleGlb(TM_WINDOW, PointHole);
      return Hole;
    }



    /// <summary>
    /// Создать стену
    /// </summary>
    /// <param name="Storey"></param>
    /// <returns></returns>
    static SapphireLib.IAutoModel MakeWall(SapphireLib.IAutoStorey Storey)
    {
      if (Storey != null)
      {
        const int TM_WALL = 10;         // Несущая стена
                                        //        const int TM_WALL_PER = 65546;  // Перегородка

        SapphireLib.IAutoModel Wall = (SapphireLib.IAutoModel)Storey.NewModel(TM_WALL);
        if (Wall != null)
        {
          Wall.InitDefault(0); // инициализация стены параметрами по умолчанию (см. диалог "Настройки/Параметры элементов по умолчанию")
          SapphireLib.IAutoPolyLine Line = (SapphireLib.IAutoPolyLine)Wall.GetAxisLine(); // получили осевую линию стены
                                                                                          //       var Pnt = new object[9]; // объявляем массив точек (такой массив используется и для других данных, например, массив параметров)
                                                                                          //       double[] pnt = { 0, 0, 0, 4, 0, 0, 4, 3, 0 }; // координаты точек осевой линии стены {x1,y1,z1, x2,y2,z2, x3,y3,z3...}
                                                                                          //       for (int i = 0; i < 9; i++)
                                                                                          //         Pnt[i] = pnt[i];   // заносим координаты точек в массив
          var Pnt = new Object[] { 0.0, 0.0, 0.0, 4.0, 0.0, 0.0, 4.0, 3.0, 0.0 }; // объявляем массив точек
          Line.SetPoints(Pnt); // занесли точки в линию
          Wall.set_Parameter("M_THICKNESS", 0.25); // Толщина стены [м] (имена параметров можно посмотреть в окне свойств объекта, если включить опцию "Настройки САПФИР\редактирование\таблица параметров\показ мнемоник", или см. документацию)
                                                   // или
                                                   // Wall.Parameter["M_THICKNESS"] = 0.25;
          Wall.MakeModel();  // создать модель по параметрам
          return Wall;
        }
      }
      return null;
    }
    
    
    /// <summary>
    /// Создать колонну
    /// </summary>
    /// <param name="Storey"></param>
    static void MakeColumn(SapphireLib.IAutoStorey Storey)
    {
      if (Storey != null)
      {
        const int TM_COLUMN = 12;              // Колонна
        var B = 0.3;  // параметры сечения колонны
        var H = 0.3;  //
        SapphireLib.IAutoModel Column = (SapphireLib.IAutoModel)Storey.NewModel(TM_COLUMN);
        if (Column != null)
        {
          Column.InitDefault(0); // инициализация колонны параметрами по умолчанию (см. диалог "Настройки/Параметры элементов по умолчанию")
          SapphireLib.IAutoMultiContour contour = (SapphireLib.IAutoMultiContour)Column.GetMultiContour();   // У объектов балка получаем объект сечение. Он хранит готовые контуры, и может формировать параметрические контуры
          var param = new Object[] { "7FF54307-9220-40a6-9A50-F9A5EFEA3E2B" };  // GUID Прямоугольного контура
          contour.InitDefault(param);                      // Инициируем необходимые данные для данного контура
          contour.Parameter["b"] = B;                      // Заносим параметры в контур
          contour.Parameter["h"] = H;
          Column.SetPosition(2.0, 3.0, 0.0);                   // задаем позицию колонны x, y, z
          Column.Parameter["M_TYPE_LEVEL"] = "M_LEVEL_DN";     // тип привязки низа колонны - низ текущего этажа
          Column.Parameter["M_LEVEL"] = 0.5;                   // смещение от заданного уровня
          Column.Parameter["M_TYPE_TOPLEVEL"] = "M_LEVEL_UP";  // тип привязки верха колонны - верх текущего этажа
          Column.Parameter["M_TOPLEVEL"] = -0.5;               // смещение от заданного уровня
          Column.MakeModel();                                  // Создать модель колонны
        }
      }
    }

    /// <summary>
    /// Создать балку
    /// </summary>
    enum TypeBasePntCont { BP_BASE_POINT = 0, BP_CENTR_BOX, BP_CENTR_GRAVITY, BP_LEFT, BP_TOP, BP_RIGHT, BP_BOTTOM, BP_TOP_LEFT, BP_BOTTOM_LEFT, BP_TOP_RIGHT, BP_BOTTOM_RIGHT }
    static void MakeBeam(SapphireLib.IAutoStorey Storey)
    {
      if (Storey != null)
      {
        const int TM_BEAM = 13;              // Балка
        SapphireLib.IAutoModel Beam = (SapphireLib.IAutoModel)Storey.NewModel(TM_BEAM);
        if (Beam != null)
        {
          Beam.InitDefault(0); // инициализация балки параметрами по умолчанию (см. диалог "Настройки/Параметры элементов по умолчанию")
          SapphireLib.IAutoMultiContour contour = (SapphireLib.IAutoMultiContour)Beam.GetMultiContour();   // У объектов балка получаем объект сечение. Он хранит готовые контуры, и может формировать параметрические контуры
          var param = new Object[] { "81A83117-6CB5-44f1-919B-FCEE909CB770" };  // GUID контура Тавр
          contour.InitDefault(param);                      // Инициируем необходимые данные для данного контура
          var B = 0.1;  // параметры сечения колонны
          var H = 0.3;  //
          var B1 = 0.3;  //
          var H1 = 0.1;  //
          contour.Parameter["b"] = B;                      // Заносим параметры в контур
          contour.Parameter["h"] = H;
          contour.Parameter["b1"] = B1;
          contour.Parameter["h1"] = H1;
          SapphireLib.IAutoPolyLine Line = (SapphireLib.IAutoPolyLine)Beam.GetAxisLine(); // получили линию внешнего контура плиты
                                                                                          //       var Pnt = new object[15]; // объявляем массив точек (такой массив используется и для других данных, например, массив параметров)
                                                                                          //       double[] pnt = { 0.0, 0.0, 0.0,  4.0, 0.0, 0.0,  4.0, 3.0, 0.0,  0.0, 3.0, 0.0,  0.0, 0.0, 0.0 }; // координаты точек осевой линии стены {x1,y1,z1, x2,y2,z2, x3,y3,z3...}
                                                                                          //       for (int i = 0; i < 15; i++)
                                                                                          //         Pnt[i] = pnt[i];   // заносим координаты точек в массив
          var Pnt = new Object[] { 0.0, 0.0, 0.0, 0.0, 3.0, 0.0, 4.0, 3.0, 0.0 }; // объявляем массив точек
          Line.SetPoints(Pnt);                          // занесли точки в линию
          Beam.Parameter["M_TYPE_LEVEL"] = "M_LEVEL_UP";     // тип привязки уровня балки - верх текущего этажа
          Beam.Parameter["M_TYPE_BASE"] = TypeBasePntCont.BP_BOTTOM; // привязка сечения в физической модели балки
          Beam.Parameter["M_TYPE_BASE_A"] = TypeBasePntCont.BP_BOTTOM; // привязка сечения в аналитической модели балки
                                                                       //       Beam.Parameter["M_LEVEL"] = 0.25;                // дополнительное смещение от заданного уровня
                                                                       // (имена параметров можно посмотреть в окне свойств объекта, если включить опцию "Настройки САПФИР\редактирование\таблица параметров\показ мнемоник", или см. документацию)
          Beam.MakeModel();  // создать модель по параметрам
        }
      }
    }


    /// <summary>
    /// Доступ к объектам проекта
    /// </summary>
    /// <param name="Project"></param>
    static void GetObject(SapphireLib.IAutoProject Project)
    {
      if (Project != null)
      {
        const int TM_WALL = 10;         // стена
        const int TM_SLAB = 11;         // Плита
        const int TM_COLUMN = 12;       // колонна
        const int TM_BEAM = 13;         // Балка
        const int toUp = 1;             // toUp- движемся по этажам снизу вверх
                                        //          const int toDn = 0; // toDn- движемся по этажам сверху вниз
        SapphireLib.IAutoStorey Storey = (SapphireLib.IAutoStorey)Project.GetFirstStorey(toUp); // берем самый нижний этаж
        if (Storey != null)
        {
          int num_Obj = Storey.CountModel;
          for (int i = 0; i < num_Obj; i++)
          {
            SapphireLib.IAutoModel Obj = (SapphireLib.IAutoModel)Storey.GetModelByIndex(i);
            if (Obj != null)
            {
              int type_Obj = Obj.TypeModel;
              switch (type_Obj)
              {
                case TM_WALL:
                  Console.WriteLine("Стена");
                  break;
                case TM_SLAB:
                  Console.WriteLine("Плита");
                  break;
                case TM_COLUMN:
                  Console.WriteLine("Колонна");
                  break;
                case TM_BEAM:
                  Console.WriteLine("Балка");
                  break;
                default:
                  continue;
              }
              GetAllParam(Obj);          // получаем значения всех параметров объекта
              ObjMaterial(Obj);          // материал объекта
            }
          }
          Storey = (SapphireLib.IAutoStorey)Project.GetNextStorey(Storey, toUp); // берем следующий верхний этаж
        }
      }
    }


    /// <summary>
    /// Получаем значения всех параметров объекта 
    /// </summary>
    /// <param name="Obj"></param>
    static void GetAllParam(SapphireLib.IAutoModel Obj)
    {
      if (Obj != null)
      {
        const int OBP_VALUES = 0x10;  // значение параметров
        const int OBP_NAMES = 0x4;    // название параметров
        const int OBP_COMMENTS = 0x8; // Описание параметров
        object refpar = new Object[] { "" };
        Obj.GetMnParam(ref refpar, OBP_VALUES); // получаем значения параметров
        var par = (object[])refpar;
        int num = par.Length;       // размер массива параметров. Количество параметров = num/3
        for (int i = 0; i < num; i += 3)
        {
          var mnemo = par[i];    // мнемоника параметра
          var value = par[i + 1];  // значение параметра
          var flag = par[i + 2];  // флаги: тип параметра, скрытый, только для чтения, ....
        }
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

    /// <summary>
    /// Сосредоточенная сила 
    /// </summary>
    /// <param name="Storey"></param>
    static void MakeLoad_1(SapphireLib.IAutoStorey Storey)
    {
      if (Storey != null)
      {
        const int TM_LOAD1 = 65557;  // нагрузка сила
        SapphireLib.IAutoModel Load = (SapphireLib.IAutoModel)Storey.NewModel(TM_LOAD1);
        if (Load != null)
        {
          Load.set_Parameter("M_NAME", "Сосредоточенная сила");
          Load.Parameter["M_LOAD_1"] = 0.8;                 // величина нагрузки в [тс]
          Load.SetPosition(2.0, 0.0, 3.0);                  // позиция нагрузки
          object vect = new object[] { 0.0, 0.0, -1.0 };
          Load.SetDirectionLoad(vect);                      // направление действия
          Load.MakeModel();  // создать модель по параметрам
        }
      }
    }


    /// <summary>
    /// Нагрузка линейно распределённая 
    /// </summary>
    /// <param name="Storey"></param>
    static void MakeLoad_2(SapphireLib.IAutoStorey Storey)
    {
      if (Storey != null)
      {
        const int TM_LOAD2 = 131093;  // нагрузка сила
        SapphireLib.IAutoModel Load = (SapphireLib.IAutoModel)Storey.NewModel(TM_LOAD2);
        if (Load != null)
        {
          Load.set_Parameter("M_NAME", "Нагрузка линейно распределённая");
          // Задаем неравномерную нагрузку
          Load.Parameter["M_LOAD_1"] = 0.4;                 // величина нагрузки в начале [тс]
          Load.Parameter["M_LOAD_2"] = 0.7;                 // величина нагрузки в конце  [тс]
                                                            // задаем линию приложения нагрузки
          var Pnt = new Object[] { 0.0, 0.0, 3.0, 0.0, 3.0, 3.0 }; // объявляем массив точек
          SapphireLib.AutoPolyLine LoadAxis = (SapphireLib.AutoPolyLine)Load.GetAxisLine();     // Получаем объект — осевая линия
          LoadAxis.SetPoints(Pnt);
          object vect = new object[] { 0.0, 0.0, -1.0 };
          Load.SetDirectionLoad(vect);                      // направление действия нагрузки
          Load.MakeModel();  // создать модель по параметрам
        }
      }
    }

    /// <summary>
    /// Нагрузка распределённая по площади
    /// </summary>
    /// <param name="Storey"></param>
    static void MakeLoad_3(SapphireLib.IAutoStorey Storey)
    {
      if (Storey != null)
      {
        const int TM_LOAD3 = 262165;  // Нагрузка распределённая по площади
        SapphireLib.IAutoModel Load = (SapphireLib.IAutoModel)Storey.NewModel(TM_LOAD3);
        Load.set_Parameter("M_NAME", "Нагрузка распределённая по площади");
        // Задаем неравномерную нагрузку
        Load.Parameter["M_LOAD_1"] = 0.4;               // величина нагрузки в начале вектора градиента [тс]
        Load.Parameter["M_LOAD_2"] = 0.05;              // величина нагрузки в конце вектора градиента  [тс]
                                                        // задаем контур нагрузки
        var Pnt = new Object[] { 0.5, 0.125, 0.0, 3.5, 0.125, 0.0, 3.5, 1.5, 0.0, 0.5, 1.5, 0.0, 0.5, 0.5, 0.0 }; // объявляем массив точек
        SapphireLib.AutoPolyLine LoadAxis = (SapphireLib.AutoPolyLine)Load.GetAxisLine();     // Получаем объект — осевая линия
        LoadAxis.SetPoints(Pnt);
        // задаем направление градиента нагрузки
        Load.SetPosition(1.0, 0.5, 0.0);             // позиция нагрузки M_LOAD_1
        Load.SetPosition2(1.0, 1.5, 0.0);            // позиция нагрузки M_LOAD_2
        object vect = new object[] { 0.0, 0.0, -1.0 };
        Load.SetDirectionLoad(vect);                      // направление действия нагрузки
        Load.MakeModel();  // создать модель по параметрам
      }
    }

    /// <summary>
    /// Загружения
    /// </summary>
    enum LC_TYPE { LOADCASE_DEFAULT = 0, LOADCASE_MONT, LOADCASE_ADDMONT, LOADCASE_SPECSLAB, LOADCASE_WEIGHT, LOADCASE_SPECSLAB_TMP, LOADCASE_SPECSLAB_CONST }; // типы загружений
    enum LC_KIND { LCK_CONST = 0, LCK_LONGTIME, LCK_TEMPORAL, LCK_CRANE, LCK_BRAKING, LCK_SEISMIC, LCK_SPECIAL, LCK_INSTANT, LCK_NO_USE, LCK_NO_ACTIVE }; // виды загружений
    static void LoadCase(SapphireLib.AutoDocument ActiveDoc)
    {
      if (ActiveDoc != null)
      {
        SapphireLib.IAutoLoadCaseHolder LCH = (SapphireLib.IAutoLoadCaseHolder)ActiveDoc.GetLoadCaseHolder();  // получаем массив загружений
        if (LCH != null)
        {
          int numLC = LCH.CountLoadCase; // количество имеющихся загружений
          for (int i = 0; i < numLC; i++)
          {
            SapphireLib.IAutoLoadCase LC = (SapphireLib.IAutoLoadCase)LCH.GetLoadCaseByIndex(i); // берем очередное загружение
                                                                                                 // получаем параметры разными способами
            var name = LC.name;                    // название загружения
            int ID = LC.ID;                        // идентификатор загружения
            var name2 = LC.Parameter["M_NAME"];    // название загружения
            var ID2 = LC.Parameter["M_ID"];        // идентификатор загружения
                                                   // получаем значения всех параметров загружения
            object refpar = new Object[] { "" };
            LC.GetParameters(ref refpar);
            var par = (object[])refpar;
            int num = par.Length;            // размер массива параметров. Количество параметров = num/3
            for (int j = 0; j < num; j += 3)
            {
              var mnemo = par[j];        // мнемоника параметра
              var value = par[j + 1];    // значение параметра
              var flag = par[j + 2];     // флаги: тип параметра...
            }
          }
          // создание нового загружения 1
          SapphireLib.IAutoLoadCase LC_1 = (SapphireLib.IAutoLoadCase)LCH.AddLoadCase("Новое загружение 1");
          LC_1.Parameter["M_CR_FILL"] = 0x00ff00;                // цвет загружения
          LC_1.Parameter["M_LC_KIND"] = LC_KIND.LCK_LONGTIME;    // Вид загружения
          LC_1.Parameter["M_LC_TYPE"] = LC_TYPE.LOADCASE_WEIGHT; // Тип загружения
          LC_1.Parameter["M_LC_DLIT"] = 1.0;                     // к-ент длительности
          LC_1.Parameter["M_LC_ENGNELIN"] = 0.5;                 // к-ент инженерной нелинейности
          LC_1.Parameter["M_LC_ZNAKOPER_ZAG"] = 1;               // установить признак знакопеременности ZAG
                                                                 // создание нового загружения 2
          SapphireLib.IAutoLoadCase LC_2 = (SapphireLib.IAutoLoadCase)LCH.AddLoadCase("Новое загружение 2");
          var param = new Object[] { "M_CR_FILL", 0x0000ff, 0, "M_LC_KIND", LC_KIND.LCK_TEMPORAL, 0, "M_LC_TYPE", LC_TYPE.LOADCASE_MONT, 0, "M_LC_DLIT", 0.4, 0, "M_LC_ZNAKOPER_ZAG", 0, 0 }; // объявляем массив параметров
          LC_2.SetParameters(param);  // устанавливаем параметры
        }
      }
    }
    /// <summary>
    /// Диалог выбора материа
    /// </summary>
    /// <param name="pSapDoc"></param>
    /// <returns></returns>
    static string SelectMaterial(SapphireLib.AutoDocument pSapDoc)
    {
      string GuidMat = "";
      if (pSapDoc != null)
      {
        SapphireLib.IAutoMaterialDlg LibMatDlg = pSapDoc.GetMaterialDlg();
        if (LibMatDlg != null)
        {
          var ret = LibMatDlg.DoModal();
          if (ret == 1)
            GuidMat = LibMatDlg.GetMaterial();
        }
      }
      return GuidMat;
    }

    /// <summary>
    /// Создать призму
    /// </summary>
    /// <param name="Storey"></param>
    /// <returns></returns>
    static SapphireLib.IAutoModel MakePrism(SapphireLib.IAutoStorey Storey)
    {
      if (Storey != null)
      {
        const int TM_PRISM = 62;         // Призма
        SapphireLib.IAutoModel Prism = (SapphireLib.IAutoModel)Storey.NewModel(TM_PRISM);
        if (Prism != null)
        {
          Prism.Parameter["M_NAME"] = "Призма 1";
          Prism.Parameter["M_HEIGHT"] = 0.5;
          SapphireLib.IAutoVectContour Contour = (SapphireLib.IAutoVectContour)Prism.GetContour();
          SapphireLib.IAutoPolyLine LineOut = (SapphireLib.IAutoPolyLine)Contour.NewPolyLine();  // Полилиния внешнего контура
          Object[] pointsOut = new Object[] { 0, 0, 0, 6, 0, 0, 6, 6, 0, 0, 6, 0, 0, 0, 0 };
          LineOut.SetPoints(pointsOut);
          SapphireLib.IAutoPolyLine LineHole = (SapphireLib.IAutoPolyLine)Contour.NewPolyLine(); // Полилиния отверстия
          Object[] pointsHole = new Object[] { 2, 2, 0, 3, 2, 0, 3, 4, 0, 2, 4, 0, 2, 2, 0 };
          LineHole.SetPoints(pointsHole);
          SapphireLib.IAutoMatrix Matrix = Prism.GetMatrix();
          Matrix.Rotate(90, 0, 0, 0);    // ax, ay, az, 0 - устанавливаем параметры поворота в матрицу, 0 - режим установки параметров преобразования
          Matrix.Translate(0, 2, 0, 1);  // x, y, z, добавляем смещение по Y на 2 метра, 1 - режим добавления параметров преобразования
          Prism.SetMatrix(Matrix);       // заносим матрицу в призму
          Prism.MakeModel();  // создать модель по параметрам
          return Prism;
        }
      }
      return null;
    }


  }
}
