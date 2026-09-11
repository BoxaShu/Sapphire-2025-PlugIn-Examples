using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LiraSapphire;
using System.Runtime.InteropServices;

namespace LiraSAPRTestSelection
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
      // Запускаем ВИЗОР
      LiraApplication App = new LiraApplication();
      // Создаём в нём новую расчётную схему:
      LiraDocument Doc = App.ActiveDocument;

      ////Запускаем ВИЗОР
      ////LiraSapphire.LiraApplication App = new LiraSapphire.LiraApplication();


      ////Get-ChildItem "REGISTRY::HKEY_CLASSES_ROOT" | Where-Object { $_.PSChildName -like "*Lira*" } | Select-Object PSChildName

      //string progId = "LiraSapphire.Application";
      //LiraSapphire.LiraApplication m_pSapApp = null;

      //try
      //{
      //  Console.WriteLine("Пытаемся подключиться к открытому Sapphire...");

      //  // 1. Получаем GUID (идентификатор класса) по текстовому ProgID
      //  int hr = CLSIDFromProgIDEx(progId, out Guid clsid);
      //  if (hr < 0) Marshal.ThrowExceptionForHR(hr);

      //  // 2. Запрашиваем ссылку на уже запущенный процесс Sapphire
      //  GetActiveObject(ref clsid, IntPtr.Zero, out object comObject);
      //  m_pSapApp = (LiraSapphire.LiraApplication)comObject; // неявно приводится к dynamic

      //  Console.WriteLine("Успешно подключились!");
      //}
      //catch (Exception ex)
      //{
      //  Console.WriteLine(ex.Message);
      //  //throw;
      //}
    }
  }
}
