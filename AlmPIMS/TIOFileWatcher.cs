using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AlmPIMS
{
    internal class TIOFileWatcher
    {
        private Form2 __pRootView = null;   // Form2를 타입으로 가지는 전역변수 __pRootView를 null로 선언 및 초기화

        private FileSystemWatcher __fsWatcher = null;   // FileSystemWatcher 클래스를 타입으로 가지는 전역변수 __fsWatcher null로 선언 및 초기화

        public static string __myCom = (System.Security.Principal.WindowsIdentity.GetCurrent().Name).Split('\\')[1];    // 클래스 string형 변수 __myCom에 Windows 계정 이름 할당

        private bool __flgInitComplete = false; // bool형 전역변수 __flgInitComplete false로 선언 및 false로 초기화

        static string __source_file = null; // 클래스 string형 변수 __source_file을 선언 및 null로 초기화
        static string __dest_file = System.IO.Path.Combine(Application.StartupPath, "Currentioconfig.io");  // 클래스 string형 변수 __dest_file을 선언 및 복사한 파일을 붙여넣기할 디렉토리로 초기화
        string __sIOFilePath = null;    // 전역 string형 변수 __sIOFilePath을 선언 및 null로 초기화

        public TIOFileWatcher(Form2 pRootView)  // Form2 클래스를 매개변수로 받는 클래스 생성자 생성
        {
            __pRootView = pRootView;    // 매개변수로 전달 받은 Form2 객체를 전역변수 __pRootView에 할당
        }

        private void ReadRegistry() // 레지스트리 읽는 메소드
        {
            string sourcefile = string.Empty;
            string sIOFilePath = string.Empty;

            RegistryKey rkey = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Services\ibaPDAService");    // 레지스트리 키 할당

            Object val = rkey.GetValue("Imagepath");    // Imagepath의 path 값 val에 할당

            string regPath = Convert.ToString(val); // val를 String으로 형변환하여 regPath에 할당

            FileInfo fi = new FileInfo(System.IO.Path.Combine(regPath, "..\\CurrentIoConfig.io"));  // 읽을 file의 디렉토리를 객체에 할당
            if (fi.Exists)  // 객체 디렉토리에 파일이 있을 경우
            {
                sourcefile = System.IO.Path.Combine(regPath, "..\\CurrentIoConfig.io");  // regPath의 전 디렉토리의 CurrentIoConfig.io를 소스 파일로 할당
                sIOFilePath = System.IO.Path.Combine(regPath, "..\\");   // regPath의 전 디렉토리를 소스 파일의 경로로 할당
            }
            else    // 객체 디렉토리에 파일이 없을 경우
            {
                rkey.Close();   // 레지스트리 키 종료
                rkey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Setup");    // 레지스트리 키 할당

                val = rkey.GetValue("BootDir"); // BootDir의 path값 val에 
                regPath = Convert.ToString(val);    // val를 String으로 형변환하여 regPath에 할당
                sourcefile = System.IO.Path.Combine(regPath, "ProgramData\\iba\\ibaPDA\\CurrentIoConfig.io");  // regPath 디렉토리의 ProgramData\\iba\\ibaPDA\\CurrentIoConfig.io를 소스 파일로 할당
                sIOFilePath = System.IO.Path.Combine(regPath, "ProgramData\\iba\\ibaPDA");   // regPath의 전 디렉토리를 소스 파일의 경로로 할당
            }
            //string sourcefile = System.IO.Path.Combine(regPath, "..\\CurrentIoConfig.io");  // regPath의 전 디렉토리의 CurrentIoConfig.io를 소스 파일로 할당

            __source_file = sourcefile; // 전역변수에 지역변수 할당
            __sIOFilePath = sIOFilePath;    // 전역변수에 지역변수 할당
        }

        public bool Activate()  // FileSystemWatcher 설정 메소드
        {
            bool flg_process = false;   // 지역벼누 flg_process 선언 및 false로 초기화

            try
            {
                ReadRegistry(); // 레지스트리 읽는 메소드 호출
                __fsWatcher = new FileSystemWatcher();  // FileSystemWatcher 객체 생성하여 전역변수에 할당
                __fsWatcher.Path = __sIOFilePath;   // FileSystemWatcher를 통해 감지할 Path 할당
                __fsWatcher.Filter = "*.io";    // 감지할 파일 타입 설정

                __fsWatcher.Changed += __fsWatcher_Changed; // 감지하는 디렉토리에서 변경이 될 때
                __fsWatcher.EnableRaisingEvents = true; // 실시간으로 감지 여부 설정

                //프로그램 시작시 강제로 이벤트 생성시켜서 IO 파일 읽기.
                FileSystemEventArgs fsea = new FileSystemEventArgs(WatcherChangeTypes.Changed, __fsWatcher.Path, string.Empty);   // 임의의 이벤트 생성
                __fsWatcher_Changed(this, fsea);  // Form2을 로드하면서 해당 클래스 실행

                __flgInitComplete = true;   // 복사 완료가 되면 __flgInitComplete 변수 true로 할당

                flg_process = true; // 변수 flg_process true로 할당
            }
            catch (Exception except)    // 예외 처리
            {
                System.Diagnostics.Trace.WriteLine(except.ToString());  // 예외 출력문에 출력
            }

            return flg_process; // 변수 flg_process 반환
        }
        private void __fsWatcher_Changed(object sender, FileSystemEventArgs e)  // 파일 변경 감지될 시 실행될 메소드
        {
            try
            {
                __pRootView.PendingUpdate = true;
                {
                    WaitForFile(__source_file);   // 복사할 파일이 제대로 생성될 때까지 대기
                    if (__flgInitComplete) Thread.Sleep(2000);  // __flgInitComplete가 true일 때, 쓰레드 지연
                    System.IO.File.Copy(__source_file, __dest_file, true);  // 파일이 제대로 생성되면 dest_file의 디렉토리로 복사
                    WaitForFile(__dest_file); // 복사하여 붙여넣기한 파일이 제대로 생성될 때까지 대기
                    if (__flgInitComplete) Thread.Sleep(1000);  // __flgInitComplete가 true일 때, 쓰레드 지연
                    __pRootView.UpdateDataGridWithIO(__dest_file);  // 복사된 파일을 인자로 UpdateDataGridWithIO() 메소드 호출
                }
            }
            catch (Exception ex)    // 예외 처리
            {
                MessageBox.Show(ex.Message);    // 예외 MessageBox로 출력
            }
            finally
            {
                __pRootView.PendingUpdate = false;
            }
        }

        public void WaitForFile(string filename) // 파일이 제대로 복사 및 생성 될 때까지 기다림
        {
            while (true)
            {
                try
                {
                    using (Stream stream = System.IO.File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.Read))   // 인자로 전달 받은 파일 읽기전용으로 열어서 stream에 할당
                    {
                        if (stream != null) // stream이 null이 아니면
                        {
                            System.Diagnostics.Trace.WriteLine(string.Format("Output file {0} ready.", filename));  // 출력창에 해당 코멘트 출력
                            break;  // 반복문 종료
                        }
                    }
                }
                catch (FileNotFoundException ex)    // FileNotFoundException 예외 처리
                {
                    System.Diagnostics.Trace.WriteLine(string.Format("Output file {0} not yet ready({1})", filename, ex.Message));  // 출력창에 해당 코멘트 및 예외 출력
                }
                catch (IOException ex)  // IOException 예외 처리
                {
                    System.Diagnostics.Trace.WriteLine(string.Format("Output file {0} not yet ready({1})", filename, ex.Message));  // 출력창에 해당 코멘트 및 예외 출력
                }
                catch (UnauthorizedAccessException ex)  // UnauthorizedAccessException 예외 처리
                {
                    System.Diagnostics.Trace.WriteLine(string.Format("Output file {0} not yet ready({1})", filename, ex.Message));  // 출력창에 해당 코멘트 및 예외 출력
                }
                catch (Exception ex)  // Exception 예외 처리
                {
                    System.Diagnostics.Trace.WriteLine(string.Format("Output file {0} not yet ready({1})", filename, ex.Message));  // 출력창에 해당 코멘트 및 예외 출력
                }
                System.Threading.Thread.Sleep(5000);    // 쓰레드 지연
            }
        }
    }
}
