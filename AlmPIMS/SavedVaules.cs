using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AlmPIMS
{
    internal class SavedVaules
    {
        private Form2 __pRootView = null;
        static string fileName = DateTime.Now.ToString("yyyyMMddHH");
        static string directoryPath = Application.StartupPath + @"\Saved_Values";  // 저장할 txt 파일 경로를 string형 sIniPath 변수에 할당
        static string filePath = Application.StartupPath + @"\Saved_Values\"+ DateTime.Now.ToString("yyyyMMddHH") + ".txt";  // 저장할 txt 파일 경로를 string형 sIniPath 변수에 할당
        DirectoryInfo di = new DirectoryInfo(directoryPath);
        FileStream fs = null;
        StreamWriter sw = null;
        

        public SavedVaules(Form2 pRootView)
        {
            __pRootView = pRootView;
        }

        private void createFile()
        {
            if (di.Exists == false)
            {
                di.Create();
                File.Create(filePath);
            }
            else
            {
                File.Create(filePath);
            }
        }

        public void savedTextFileTC01()
        {
            if (File.Exists(filePath))
            {
                foreach (TSignalUnit obj in __pRootView.m_sigSetting.m_lstSignals)
                {
                    fs = new FileStream(filePath, FileMode.Append,FileAccess.Write);
                    sw = new StreamWriter(fs);

                    sw.Write("TC01" + Environment.NewLine);
                    sw.Write(DateTime.Now.ToString("yyyMMddHHmmss" + Environment.NewLine));
                    sw.Write(__pRootView.m_sigSetting.FactoryCode + Environment.NewLine);
                    sw.Write(obj.Expression + Environment.NewLine);
                    sw.Write(obj.Comment1 + Environment.NewLine);
                    sw.Write(obj.Name + Environment.NewLine);
                    sw.Write(Environment.NewLine);

                    sw.Close();
                    fs.Close();
                }
            }
            else
            {
                createFile();
            }
        }

        public void savedTextFileTC02()
        {
            if(File.Exists(filePath))
            {
                foreach (TSignalUnit obj in __pRootView.m_sigSetting.m_lstSignals)
                {
                    fs = new FileStream(filePath, FileMode.Append, FileAccess.Write);
                    sw = new StreamWriter(fs);

                    sw.Write("TC02" + Environment.NewLine);
                    sw.Write(DateTime.Now.ToString("yyyMMddHHmmss" + Environment.NewLine));
                    sw.Write(__pRootView.m_sigSetting.FactoryCode + Environment.NewLine);
                    sw.Write(obj.Expression + Environment.NewLine);
                    sw.Write(obj.Comment1 + Environment.NewLine);
                    sw.Write(obj.Name + Environment.NewLine);
                    sw.Write(Environment.NewLine);

                    sw.Close();
                    fs.Close();
                }
            }
            else
            {
                createFile();
            }
        }
    }
}
