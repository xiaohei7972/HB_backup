using System;
using System.IO;
using System.Text;
namespace HREngine.Bots
{
    public class AiTest : TestBase
    {
        public void Test()
        {
            Settings.Instance.test = true;
            // 自动检测 Silverfish 目录路径（基于程序集位置）
            string assemblyDir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            string mainPath = Path.Combine(assemblyDir, "Routines", "DefaultRoutine", "Silverfish") + Path.DirectorySeparatorChar;
            Settings.Instance.mainPath = mainPath;

            var testFilePath = Path.Combine(mainPath, "Test", "Data", "test.txt");
            var data = File.ReadAllText(testFilePath);
            Settings.Instance.logpath = Path.Combine(mainPath, "Test", "Data") + Path.DirectorySeparatorChar;
            Settings.Instance.path = Path.Combine(mainPath, "data") + Path.DirectorySeparatorChar; // 用于CardDB类构造，读取CardDefs.xml
            InitSetting();

            Ai ai = Ai.Instance;
            ai.botBase = new Behavior丨通用丨不设惩罚();  //根据卡组选择合适的策略

            ai.autoTester(true, data, 2);// 0：全做 1:只斩杀 2：正常
            Console.WriteLine("测试完毕，请去Logg.txt文件末尾查看Ai操作");
        }

        
        public static void main(string[] args)  //如果单独Run这个程序，main->Main
        {
            AiTest test = new AiTest();
            test.Test();
        }
    }
}