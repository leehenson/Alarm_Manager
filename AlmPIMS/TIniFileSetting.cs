using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AlmPIMS
{
    internal class TIniFileSetting
    {
        private Form2 __pRootView = null;   // Form2를 타입으로 가지는 전역변수 __pRootView null로 선언 및 초기화

        [DllImport("kernel32")] public static extern bool WritePrivateProfileString(string section, string key, string val, string filePath);   // ini 파일 사용을 위한 Kernel32 WritePrivateProfileString 모듈 Import
        [DllImport("kernel32")] private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath); // ini 파일 사용을 위한 Kernel32 GetPrivateProfileString 모듈 Import


        public TIniFileSetting(Form2 pRootView) // Form2 타입을 매개변수로 받는 TIniFileSetting 클래스 생성자 생성
        {
            __pRootView = pRootView;    // 매개변수로 전달 받은 Form2 정보를 전역변수 __pRootView에 할당
        }

        public void openiniFile()   // 프로그램 시작될 시 읽기 위한 ini 가져오기
        {
            TSignalSetting signalSetting = __pRootView.m_sigSetting;    // 지역변수 signalSetting에 Form2의 m_sigSetting 객체 할당

            try
            {
                string sIniPath = Application.StartupPath + @"\\Setting.ini";   // 읽을 ini 파일 경로를 string형 sIniPath 변수에 할당
                signalSetting.IPAddress1 = GetIniValue(__pRootView.Name, "IPaddress1", sIniPath);   // m_sigSetting 객체 IPAddress1에 읽은 ini파일의 IPaddress1의 Value 할당
                signalSetting.Port1 = Int32.Parse(GetIniValue(__pRootView.Name, "Port1", sIniPath));   // m_sigSetting 객체 Port1 읽은 ini파일의 Port1 Value 할당
                signalSetting.IPAddress2 = GetIniValue(__pRootView.Name, "IPaddress2", sIniPath);   // m_sigSetting 객체 IPAddress2에 읽은 ini파일의 IPaddress2의 Value 할당
                signalSetting.Port2 = Int32.Parse(GetIniValue(__pRootView.Name, "Port2", sIniPath));   // m_sigSetting 객체 Port2 읽은 ini파일의 Port2 Value 할당
                signalSetting.IPAddress3 = GetIniValue(__pRootView.Name, "IPaddress3", sIniPath);   // m_sigSetting 객체 IPAddress2에 읽은 ini파일의 IPaddress2의 Value 할당
                signalSetting.Port3 = Int32.Parse(GetIniValue(__pRootView.Name, "Port3", sIniPath));   // m_sigSetting 객체 Port2 읽은 ini파일의 Port2 Value 할당
                signalSetting.ModuleNum = Int32.Parse(GetIniValue(__pRootView.Name, "ModuleNumber", sIniPath)); // m_sigSetting 객체 ModuleNum 읽은 ini파일의 ModuleNum Value 할당
                signalSetting.FactoryCode = GetIniValue(__pRootView.Name, "FactoryCode", sIniPath); // m_sigSetting 객체 FactoryCode 읽은 ini파일의 FactoryCode Value 할당
                signalSetting.SendTime = GetIniValue(__pRootView.Name, "SendTime", sIniPath); // m_sigSetting 객체 SendTime 읽은 ini파일의 SendTime Value 할당
                System.Diagnostics.Trace.WriteLine("ModuleNumber:(" + signalSetting.ModuleNum.ToString() + ")"); // 출력창에 모듈번호 출력

                for (int i = 0; i < __pRootView.dataGridView1.Rows.Count; i++)  // dataGridView Row의 수만큼 반복
                {
                    __pRootView.dataGridView1[1, i].Value = GetIniValue(__pRootView.Name, "ck1 " + i.ToString(), sIniPath).ToLower() == "true" ? true : false;   // dataGridView 1열 i행 Value에 ck1 + i Value bool형으로 할당
                }
            }
            catch (Exception ex)    // 예외 처리
            {
                MessageBox.Show(ex.Message);    // 예외 MessageBox에 출력
            }
        }

        public void saveiniFile()   // 매초마다 혹은 프로그램 종료될 시 ini 저장하기
        {
            //__pRootView.__invokeState = false;
            TSignalSetting signalSetting = __pRootView.m_sigSetting;    // 지역변수 signalSetting에 Form2의 m_sigSetting 객체 할당


            string sIniPath = Application.StartupPath + @"\\Setting.ini";   // 저장할 ini 파일 경로를 string형 sIniPath 변수에 할당
            WritePrivateProfileString(__pRootView.Name, "IPaddress1", signalSetting.IPAddress1, sIniPath);    // 저장할 ini 파일에 IPAddress1 이름으로 m_sigSetting 객체 IPAddress1 데이터 저장
            WritePrivateProfileString(__pRootView.Name, "Port1", signalSetting.Port1.ToString(), sIniPath);   // 저장할 ini 파일에 Port1 이름으로 m_sigSetting 객체 Port1 데이터 저장
            WritePrivateProfileString(__pRootView.Name, "IPaddress2", signalSetting.IPAddress2, sIniPath);    // 저장할 ini 파일에 IPAddress2 이름으로 m_sigSetting 객체 IPAddress2 데이터 저장
            WritePrivateProfileString(__pRootView.Name, "Port2", signalSetting.Port2.ToString(), sIniPath);   // 저장할 ini 파일에 Port2 이름으로 m_sigSetting 객체 Port2 데이터 저장
            WritePrivateProfileString(__pRootView.Name, "IPaddress3", signalSetting.IPAddress3, sIniPath);    // 저장할 ini 파일에 IPAddress2 이름으로 m_sigSetting 객체 IPAddress2 데이터 저장
            WritePrivateProfileString(__pRootView.Name, "Port3", signalSetting.Port3.ToString(), sIniPath);   // 저장할 ini 파일에 Port2 이름으로 m_sigSetting 객체 Port2 데이터 저장
            WritePrivateProfileString(__pRootView.Name, "ModuleNumber", signalSetting.ModuleNum.ToString(), sIniPath);   // 저장할 ini 파일에 ModuleNum 이름으로 m_sigSetting 객체 ModuleNum 데이터 저장
            WritePrivateProfileString(__pRootView.Name, "FactoryCode", signalSetting.FactoryCode, sIniPath);   // 저장할 ini 파일에 FactoryCode 이름으로 m_sigSetting 객체 FactoryCode 데이터 저장
            WritePrivateProfileString(__pRootView.Name, "SendTime", signalSetting.SendTime, sIniPath);   // 저장할 ini 파일에 SendTime 이름으로 m_sigSetting 객체 SendTime 데이터 저장
            //System.Diagnostics.Trace.WriteLine("ModuleNumber:(" + signalSetting.ModuleNum.ToString() + ")");

            foreach (TSignalUnit obj in signalSetting.m_lstSignals)  // m_sigSetting 객체의 m_lstSignlas 객체를 TSignalUnit형의 obj으로 할당
            {
                WritePrivateProfileString(__pRootView.Name, "ck1 " + obj.Id.ToString(), obj.Checked1.ToString(), sIniPath);   // 저장할 ini 파일에 ck1 + Id 이름으로 m_sigSetting 객체 Checked1 데이터 저장
            }
        }

        public string GetIniValue(string section, string key, string filePath)  // filePath의 파일을 읽고, 해당 section의 key의 Value를 반환하는 메소드
        {
            StringBuilder temp = new StringBuilder(255);    // temp라는 이름으로 StringBuilder 객체 생성
            GetPrivateProfileString(section, key, "", temp, 255, filePath); // ini 파일 읽기
            return temp.ToString(); // temp를 string 타입으로 변환 후 반환
        }

        internal void saveAsiniFile()   // 다른 이름으로 ini파일 저장하는 메소드
        {
            TSignalSetting signalSetting = __pRootView.m_sigSetting;    // 지역변수 signalSetting에 Form2의 m_sigSetting 객체 할당

            SaveFileDialog saveFileDialog = new SaveFileDialog();   // SaveFileDialog 객체 생성
            saveFileDialog.InitialDirectory = Application.StartupPath;  // 초기 디렉토리 설정
            saveFileDialog.Title = "다른 이름으로 저장";    // dialog title 설정
            saveFileDialog.OverwritePrompt = true;  // 같은 이름이 파일이 존재할 경우 경고창 띄움
            saveFileDialog.DefaultExt = "ini";  // 저장할 파일을 기본 포맷 설정
            saveFileDialog.Filter = "Initialization File(*.ini)|*.ini"; // 저장할 파일 포맷 옵션 설정

            if (saveFileDialog.ShowDialog() == DialogResult.OK) // OK 버튼을 눌렀을 때
            {
                string sIniPath = saveFileDialog.FileName;  // saveFileDialog에 작성된 파일 이름을 sIniPath 변수에 할당

                WritePrivateProfileString(__pRootView.Name, "IPaddress1", signalSetting.IPAddress1, sIniPath);    // 저장할 ini 파일에 IPAddress1 이름으로 m_sigSetting 객체 IPAddress1 데이터 저장
                WritePrivateProfileString(__pRootView.Name, "Port1", signalSetting.Port1.ToString(), sIniPath);   // 저장할 ini 파일에 Port1 이름으로 m_sigSetting 객체 Port1 데이터 저장
                WritePrivateProfileString(__pRootView.Name, "IPaddress2", signalSetting.IPAddress2, sIniPath);    // 저장할 ini 파일에 IPAddress2 이름으로 m_sigSetting 객체 IPAddress2 데이터 저장
                WritePrivateProfileString(__pRootView.Name, "Port2", signalSetting.Port2.ToString(), sIniPath);   // 저장할 ini 파일에 Port2 이름으로 m_sigSetting 객체 Port2 데이터 저장
                WritePrivateProfileString(__pRootView.Name, "IPaddress3", signalSetting.IPAddress3, sIniPath);    // 저장할 ini 파일에 IPAddress2 이름으로 m_sigSetting 객체 IPAddress2 데이터 저장
                WritePrivateProfileString(__pRootView.Name, "Port3", signalSetting.Port3.ToString(), sIniPath);   // 저장할 ini 파일에 Port2 이름으로 m_sigSetting 객체 Port2 데이터 저장
                WritePrivateProfileString(__pRootView.Name, "ModuleNumber", signalSetting.ModuleNum.ToString(), sIniPath);   // 저장할 ini 파일에 ModuleNum 이름으로 m_sigSetting 객체 ModuleNum 데이터 저장
                WritePrivateProfileString(__pRootView.Name, "FactoryCode", signalSetting.FactoryCode, sIniPath);   // 저장할 ini 파일에 FactoryCode 이름으로 m_sigSetting 객체 FactoryCode 데이터 저장
                WritePrivateProfileString(__pRootView.Name, "SendTime", signalSetting.SendTime, sIniPath);   // 저장할 ini 파일에 SendTime 이름으로 m_sigSetting 객체 SendTime 데이터 저장
                //System.Diagnostics.Trace.WriteLine("ModuleNumber:(" + signalSetting.ModuleNum.ToString() + ")");

                foreach (TSignalUnit obj in signalSetting.m_lstSignals)  // m_sigSetting 객체의 m_lstSignlas 객체를 TSignalUnit형의 obj으로 할당
                {
                    WritePrivateProfileString(__pRootView.Name, "ck1 " + obj.Id.ToString(), obj.Checked1.ToString(), sIniPath);   // 저장할 ini 파일에 ck1 + Id 이름으로 m_sigSetting 객체 Checked1 데이터 저장
                }
            }
        }

        internal void openAsiniFile()   // 저장된 ini파일 불러오는 메소드
        {
            TSignalSetting signalSetting = __pRootView.m_sigSetting;    // 지역변수 signalSetting에 Form2의 m_sigSetting 객체 할당

            OpenFileDialog openFileDialog = new OpenFileDialog();   // OpenFileDialog 객체 생성
            openFileDialog.InitialDirectory = Application.StartupPath;  // 초기 디렉토리 설정
            openFileDialog.Title = "저장된 파일 불러오기";       // dialog title 설정
            openFileDialog.Filter = "Initialization File(*.ini)|*.ini"; // 가져올 파일 포맷 옵션 설정
            openFileDialog.Multiselect = false; // 다수의 파일 선택 불가능하게 설정

            if (openFileDialog.ShowDialog() == DialogResult.OK) // OK 버튼 눌렀을 때
            {
                string sIniPath = openFileDialog.FileName;  // 선택한 File의 Path를 sIniPath에 할당
                try
                {
                    signalSetting.IPAddress1 = GetIniValue(__pRootView.Name, "IPaddress1", sIniPath);  // m_sigSetting 객체 IPAddress1에 읽은 ini파일의 IPaddress1의 Value 할당
                    signalSetting.Port1 = Int32.Parse(GetIniValue(__pRootView.Name, "Port1", sIniPath));   // m_sigSetting 객체 Port1 읽은 ini파일의 Port1 Value 할당
                    signalSetting.IPAddress2 = GetIniValue(__pRootView.Name, "IPaddress2", sIniPath);  // m_sigSetting 객체 IPAddress2에 읽은 ini파일의 IPaddress2의 Value 할당
                    signalSetting.Port2 = Int32.Parse(GetIniValue(__pRootView.Name, "Port2", sIniPath));   // m_sigSetting 객체 Port2 읽은 ini파일의 Port2 Value 할당
                    signalSetting.IPAddress3 = GetIniValue(__pRootView.Name, "IPaddress3", sIniPath);   // m_sigSetting 객체 IPAddress2에 읽은 ini파일의 IPaddress2의 Value 할당
                    signalSetting.Port3 = Int32.Parse(GetIniValue(__pRootView.Name, "Port3", sIniPath));   // m_sigSetting 객체 Port2 읽은 ini파일의 Port2 Value 할당
                    signalSetting.ModuleNum = Int32.Parse(GetIniValue(__pRootView.Name, "ModuleNumber", sIniPath)); // m_sigSetting 객체 ModuleNum 읽은 ini파일의 ModuleNum Value 할당
                    signalSetting.FactoryCode = GetIniValue(__pRootView.Name, "FactoryCode", sIniPath); // m_sigSetting 객체 FactoryCode 읽은 ini파일의 FactoryCode Value 할당
                    signalSetting.SendTime = GetIniValue(__pRootView.Name, "SendTime", sIniPath); // m_sigSetting 객체 SendTime 읽은 ini파일의 SendTime Value 할당
                    System.Diagnostics.Trace.WriteLine("ModuleNumber:(" + signalSetting.ModuleNum.ToString() + ")"); // 출력창에 모듈번호 출력

                    for (int i = 0; i < __pRootView.dataGridView1.Rows.Count; i++)  // dataGridView Row의 수만큼 반복
                    {
                        __pRootView.dataGridView1[1, i].Value = GetIniValue(__pRootView.Name, "ck1 " + i.ToString(), sIniPath).ToLower() == "true" ? true : false;   // dataGridView 1열 i행 Value에 ck1 + i Value bool형으로 할당
                    }
                }
                catch (Exception ex)    // 예외 처리
                {
                    MessageBox.Show(ex.Message);    // 예외 MessageBox에 출력
                }
            }
        }
    }
}
