using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using System.Xml;

namespace AlmPIMS
{
    public partial class Form2 : Form
    {
        public volatile bool __flgDataTransmit = true;        // 데이터 송신 상태, 전역변수 __flgDataTransmit 선언 및 true로 초기화
        private volatile bool __flgPendingUpdate = false;   // DataGridView 업데이트 상태, 전역변수 __flgPendingUpdate 선언 및 false로 초기화
        public volatile bool __flginvokeStop = false;   // Invoke 정지, 이 변수가 true일 경우, Invoke를 정지해 데이터 전송을 못하도록 함. 전역변수 __flginvokeStop 선언 및 false로 초기화
        //public volatile bool __invokeState = true; // Invoke 상태, 전역변수 __invokeState 선언 및 true로 초기화

        TDataBuffer m_rcvBuffer = new TDataBuffer();    // TDataBuffer 클래스를 m_rcvBuffer 이름으로 객체 생성
        TIOFileWatcher m_ioWatcher = null;  // TIOFileWatcher 클래스를 m_ioWatcher 이름으로 선언 및 null로 초기화
        internal TSignalSetting m_sigSetting = null; // TSignalSetting 클래스를 m_sigSetting 이름으로 접근 제어 internal로 선언 및 null로 초기화
        internal TIniFileSetting m_iniSetting = null;    // TIniFileSetting 클래스를 m_iniSetting 이름으로 접근 제어 internal로 선언 및 null로 초기화
        TSetTimer m_setTimer = null;    // TSetTimer 클래스를 m_setTimer 이름으로 선언 및 null로 초기화
        TSocketConnect m_socketConnect = null;  // TSocketConnect 클래스를 m_socketConnect 이름으로 선언 및 null로 초기화
        internal SavedVaules m_SavedVaules = null;  // SavedVaules 클래스를 m_SavedVaules 이름으로 선언 및 null로 초기화
        //private readonly object m_lckSigSetting = new object();  // object 클래스를 __lckSigSetting 이름으로 접근제어 internal로 객체 생성
        //internal TDataGirdSetting m_dataGridSetting = null;   // TDataGirdSetting 클래스를 m_dataGridSetting 이름으로 접근 제어 internal로 선언 및 null로 초기화

        private const int CI_MAX_PDA_CAPACITY = 64;  // 상수 CI_MAX_PDA_CAPACITY 64로 초기화


        public bool PendingUpdate   // PendigUpdate Value를 저장하는 프로퍼티 생성
        {
            get { return __flgPendingUpdate; }
            set { __flgPendingUpdate = value; }
        }

        public Form2()
        {
            InitializeComponent();

            DoubleBuffered = true;  // DoubleBuffered true로 할당
            typeof(DataGridView).InvokeMember("DoubleBuffered", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.SetProperty, null, dataGridView1, new object[] { true }); // DataGridView의 알람(색깔 변동)을 한 번에 변동될 수 있도록 DoubleBuffer 사용
        }

        internal class TDataBuffer  // TDataBuffer 클래스 생성
        {
            public byte[] m_lstBuffer = new byte[8192]; // 8192 바이트 크기의 배열을 m_lstBuffer 이름으로 선언
            public int m_nOffset = 0;   // 인스턴스 변수 m_nOffset을 선언 및 0으로 초기화
        }

        private void Form2_Load(object sender, EventArgs e) // Form2가 로드할 때 실행되는 메소드
        {
            if (DesignMode) return; // DesignMode가 true일 경우, 반환
            //Control.CheckForIllegalCrossThreadCalls = false;    // 크로스 쓰레드 허용 (권장 방법 아님)
            m_ioWatcher = new TIOFileWatcher(this); // m_ioWatcher에 Form2를 생성자 인자로 하여 객체 생성
            m_sigSetting = new TSignalSetting(this);    // m_sigSetting Form2를 생성자 인자로 하여 객체 생성
            m_iniSetting = new TIniFileSetting(this);   // m_iniSetting Form2를 생성자 인자로 하여 객체 생성
            m_setTimer = new TSetTimer(this);   // m_setTimer Form2를 생성자 인자로 하여 객체 생성
            m_socketConnect = new TSocketConnect(this); // m_socketConnect Form2를 생성자 인자로 하여 객체 생성
            m_SavedVaules = new SavedVaules(this); // m_SavedVaules Form2를 생성자 인자로 하여 객체 생성
            //m_dataGridSetting = new TDataGirdSetting(this); // m_dataGridSetting Form2를 생성자 인자로 하여 객체 생성

            m_iniSetting.openiniFile(); // TiniFileSetting 클래스 인스턴스 메소드 openiniFile() 호출

            if (m_ioWatcher.Activate() == false)    // m_ioWatcher.Activate가 false일 때,
            {
                MessageBox.Show("IO reading failed.");  // MessageBox 출력
            }

            m_setTimer.SetTimer();

            m_socketConnect.serverConnect();    // TSocketConnect 클래스 인스턴스 메소드 serverConnect() 호출

            m_socketConnect.clientConnect();    // TSocketConnect 클래스 인스턴스 메소드 clientConnect() 호출

            if (!String.IsNullOrEmpty(m_sigSetting.IPAddress3)) // IPAddress3가 null이 아니면
            {
                m_socketConnect.clientConnectICT(); // 통합 모니터링 시스템 소켓 연결
            }
        }

        private void writeRichTextbox(string data)  // richTextBox2에 매개변수로 받은 string형 데이터를 출력하는 메소드 생성
        {
            richTextBox2.Invoke((MethodInvoker)delegate { richTextBox2.AppendText(DateTime.Now + "     " + data + "\r\n"); });  // richTextBox2에 실시간과 함께 매개변수를 통해 전달 받은 인자 출력
            richTextBox2.Invoke((MethodInvoker)delegate { richTextBox2.ScrollToCaret(); }); // richTextBox2의 스크롤 제일 밑으로 이동
        }

        //private void writeRichTextbox2(string data)  // string형 매개변수를 받는 메소드 생성
        //{
        //    richTextBox2.Invoke((MethodInvoker)delegate { richTextBox2.AppendText(data + "\r\n"); }); // richTextBox2에 실시간과 함께 매개변수를 통해 전달 받은 인자 출력
        //    richTextBox2.Invoke((MethodInvoker)delegate { richTextBox2.ScrollToCaret(); });   // richTextBox2의 스크롤 제일 밑으로 이동
        //}

        public void ConcreteInvoke(List<bool> lstValues)    // 크로스 쓰레드 사용을 위한 bool형 리스트를 매개 변수로 받는 메소드 생성
        {
            if (__flgPendingUpdate) return;// __flgPendingUpdate가 true이면 datagridview 항목을 업데이트 중.

            if (InvokeRequired) // InvokeRequired가 True일 때, 쓰레드 자원 경쟁 상태를 없애줌.
            {
                //Invoke(ConcreteInvoke, new object[] { lstValues }); // lstValues를 Invoke
                if (!__flginvokeStop) Invoke(new MethodInvoker(() => { ConcreteInvoke(lstValues); }));  // __flginvokeStop이 false가 아닐 때, ConcreteInvoke를 재호출
            }
            else    // InvokeRequired가 false일 때
            {
                try
                {
                    if (!__flgPendingUpdate)  // __flgPendingUpdate가 true가 아닐 때
                    {
                        //lock (m_lckSigSetting)  // 특정 블럭의 코드(Critical Section)를 한번에 하나의 쓰레드만 실행할 수 있도록 함.
                        {
                            string s_value = string.Empty;  // s_value 변수의 힙을 비움.
                            for (int i = 0; i < m_sigSetting.m_lstSignals.Count; i++)   // lstValues 수만큼 i를 증카시키면서 반복
                            {
                                s_value += string.Format("{0}", lstValues[i] ? '1' : '0');  // lstValues[i]가 true이면 1, false이면 0으로 s_value에 할당

                                __UpdateAlmBackColor(i, lstValues[i]);  // __UpdateAlmBackColor 메소드에 i와 lstValues를 인자로 전달

                                m_sigSetting.m_lstSignals[i].SavedValue = lstValues[i]; // TSignalUnit 클래스 SavedValue에 lstValues[i] 값 할당
                            }

                            m_socketConnect.__WriteData();  // TSocketConnect 클래스의 __WrieData() 메소드 호출
                        }
                    }
                }
                catch (Exception except)    // 예외 처리
                {
                    System.Diagnostics.Debug.WriteLine(except.ToString());  // 예외를 Debug창에 출력
                }
                finally
                {
                }
            }
        }

        internal void SaveIniFile()
        {
            if (__flgPendingUpdate) return;// __flgPendingUpdate가 true이면 datagridview 항목을 업데이트 중.

            m_iniSetting.saveiniFile();
        }

        private void __UpdateAlmBackColor(int nIDX, bool flgValue)  // 알람 발생 시, DataGridView의 색상을 바꾸는 메소드, int형, bool형 두 개의 매개 변수를 받는 메소드 생성
        {
            if (flgValue == true)   // 매개변수로 받은 flgValue가 true일 때
            {
                dataGridView1.Rows[nIDX].DefaultCellStyle.ForeColor = Color.White;  // 매개변수로 받은 nIDX에 해당하는 Row의 글자색 흰색
                dataGridView1.Rows[nIDX].DefaultCellStyle.BackColor = Color.Firebrick;  // 매개변수로 받은 nIDX에 해당하는 Row의 배경색 빨간색
            }
            else    // 매개변수로 받은 flgValue가 false일 때
            {
                dataGridView1.Rows[nIDX].DefaultCellStyle.ForeColor = Color.Black;  // 매개변수로 받은 nIDX에 해당하는 Row의 글자색 검정색
                dataGridView1.Rows[nIDX].DefaultCellStyle.BackColor = Color.White;  // 매개변수로 받은 nIDX에 해당하는 Row의 배경색 흰색
            }
        }

        internal void UpdateDataGridWithIO(string sPathIO)  // DataGridView 업데이트하는 메소드 생성, 읽어올 file path를 매개변수로 받음
        {
            if (InvokeRequired) // InvokeRequired가 True일 때, 쓰레드 자원 경쟁 상태를 없애줌.
            {
                Invoke(new MethodInvoker(() => { UpdateDataGridWithIO(sPathIO); }));    // UpdateDataGridWithIO() 메소드 재호출
            }
            else    // InvokeRequired가 false일 때
            {
                try
                {
                    __flgPendingUpdate = true;  // DataGridView 업데이트 상태, __flgPendingUpdate true로 할당
                    Thread.Sleep(100);  // Thead를 0.1초동안 지연시킴
                    //lock (m_lckSigSetting)  // 특정 블럭의 코드(Critical Section)를 한번에 하나의 쓰레드만 실행할 수 있도록 함.
                    {
                        System.Diagnostics.Debug.WriteLine("UpdateDataGridWithIO-1");   // Debug창에 코멘트 출력
                        XmlDocument xml = new XmlDocument();    // Xml 파일을 읽기위한 XmlDocument 클래스를 xml 이름으로 객체 생성

                        xml.Load(sPathIO);    // 매개변수로 전달 받은 file path 인자를 XmlDocument 클래스 Load() 메소드에 인자로 호출

                        XmlNodeList xnList = xml.SelectNodes("/IOConfiguration/Modules/Module/ModuleNr[text()='" + m_sigSetting.ModuleNum + "']/../Links/Link/Digital/Signal");  // 읽은 Xml 파일에서 읽을 노드를 필터링, 노드 경로를 인자로 XmlDocument 클래스의 SelectNodes() 메소드를 호출하고, XmlList형의 xnList 변수에 할당

                        if (xnList != null) // xnList가 null이 아닐 때
                        {
                            List<TSignalUnit> lst_units = new List<TSignalUnit>();  // TSignalUnit 클래스를 타입으로 가지는 List 객체 생성
                            int n_index = 0;    // 지역변수 n_index 선언 및 0으로 초기화
                            foreach (XmlNode xn in xnList)  // xnList의 개수만큼 반복하며, xnList의 각 Signal 노드를 XmlNode형의 xn으로 할당
                            {
                                if (xn["Expression"].InnerText != string.Empty) // Signal 노드의 자식 노드 중 Expression 노드의 이너텍스트가 빈 문자열이 아닐 때
                                {
                                    TSignalUnit obj = new TSignalUnit();    // TSignalUnit 클래스를 obj 이름으로 객체 생성

                                    obj.Id = n_index++; // TSignalUnit 클래스의 프로퍼티 Id에 지역변수 n_index를 할당, n_index는 foreach문이 반복할 때마다 1씩 증가
                                    obj.Name = xn["Name"].InnerText;   // TSignalUnit 클래스의 프로퍼티 Name에 Signal 노드의 자식 노드 중 Name 노드의 이너텍스트 할당
                                    obj.Expression = xn["Expression"].InnerText; // TSignalUnit 클래스의 프로퍼티 Expression에 Signal 노드의 자식 노드 중 Expression 노드의 이너텍스트 할당
                                    if (xn["Comment1"] == null) obj.Comment1 = string.Empty;    // Signal 노드의 자식 노드 중 Comment1 노드가 없을 때, 빈 문자열 할당
                                    else obj.Comment1 = xn["Comment1"].InnerText;   // Signal 노드의 자식 노드 중 Comment1 노드가 있을 때, TSignalUnit 클래스의 프로퍼티 Comment1에 Signal 노드의 자식 노드 중 Comment1 노드의 이너텍스트 할당
                                    if (xn["Comment2"] == null) obj.Comment2 = string.Empty;    // Signal 노드의 자식 노드 중 Comment2 노드가 없을 때, 빈 문자열 할당
                                    else obj.Comment2 = xn["Comment2"].InnerText;   // Signal 노드의 자식 노드 중 Comment2 노드가 있을 때, TSignalUnit 클래스의 프로퍼티 Comment2에 Signal 노드의 자식 노드 중 Comment2 노드의 이너텍스트 할당

                                    lst_units.Add(obj); // TSignalUnit형을 가지는 리스트에 TSignalUnit의 객체 obj 할당
                                }
                            }

                            m_sigSetting.InitContainer(lst_units);  // TSignalSetting 클래스의 IniContainer() 인스턴스 메소드에 Xml 파일에서 읽은 데이터가 담긴 lst_units 리스트를 인자로 호출
                        }

                        dataGridView1.Rows.Clear();  // dataGridView1 모든 열 지우기

                        foreach (TSignalUnit obj in m_sigSetting.m_lstSignals)  // m_sigSetting 객체의 m_lstSignlas 객체를 TSignalUnit형의 obj으로 할당
                        {
                            dataGridView1.Rows.Add(obj.Id, obj.Checked1, obj.Expression, obj.Name, obj.Comment1, obj.Comment2);  // m_lstSignals의 프로퍼티 데이터를 dataGridView Rows에 할당
                        }
                        System.Diagnostics.Debug.WriteLine("UpdateDataGridWithIO-2");   // Debug창에 코멘트 출력
                    }
                }
                catch (Exception ex)    // 예외처리
                {
                    MessageBox.Show("XML 문제 발생\r\n" + ex);  // MessageBox로 예외처리된 에러 디스플레이
                }
                finally
                {
                    Thread.Sleep(100);  // thread를 0.1초 지연시킴
                    __flgPendingUpdate = false; // DataGridView 업데이트 상태, false로 할당
                }
            }
        }

        private void Form2_FormClosed(object sender, FormClosedEventArgs e) // 프로그램이 꺼질 때 실행되는 메소드
        {
            m_iniSetting.saveiniFile();  // TIniFileSetting 클래스의 인스턴스 메소드 saveiniFile() 호출

            if (tDriverSocket1.Activated) tDriverSocket1.ShutdownDriver();  // tDriverSocket1이 연결되어 있다면, tDriverSocket1 연결 종료
            if (tDriverSocket2.Activated) tDriverSocket2.ShutdownDriver();  // tDriverSocket2이 연결되어 있다면, tDriverSocket2 연결 종료
        }

        private void Form2_Shown(object sender, EventArgs e)    // 프로그램이 켜질 때 실행되는 메소드
        {
            m_iniSetting.openiniFile();  // TIniFileSetting 인스턴스 메소드 openiniFile() 호출
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e) // dataGridView의 Cell이 클릭될 시 실행되는 메소드
        {
            dataGridView1.CommitEdit(DataGridViewDataErrorContexts.Commit); // 현재 셀의 변경 내용을 데이터 캐시에 커밋
        }

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e) // dataGridView의 CheckBox 상태가 변경될 시 실행되는 메소드
        {
            try
            {
                bool flg_checked1 = false;  // 지역변수 flg_checked1 선언 및 false로 초기화
                //bool flg_checked2 = false;  // flg_checked2 선언 및 false 할당
                int i_signal = 0;   // 지역변수 i_siganl 선언 및 0으로 초기화

                if (dataGridView1.CurrentCell != null)  // 현재 셀이 null이 아닐 때
                {
                    for (int i = 0; i < dataGridView1.Rows.Count; i++)  // dataGrieView의 Row 수만큼 반복하며 i 증가
                    {
                        flg_checked1 = (bool)dataGridView1[1, i].Value; // flg_checked1에 dataGridView 1열 i행 Value 할당
                        //flg_checked2 = (bool)dataGridView1[2, i].Value; // flg_checked2에 dataGridView 2열 i행 Value 할당
                        i_signal = (int)dataGridView1[0, i].Value;  // i_signal에 dataGridView 0열 i행 Value 할당

                        //System.Diagnostics.Debug.WriteLine(i_signal + " " + flg_checked1);   // s_signal을 Debug 창에 띄움.

                        foreach (TSignalUnit obj in m_sigSetting.m_lstSignals)  // m_sigSetting 객체의 m_lstSignlas 객체를 TSignalUnit형의 obj으로 할당
                        {
                            if (obj.Id == i_signal) // obj.Name과 i_signal이 같다면
                            {
                                obj.Checked1 = flg_checked1;  // obj.Checked1에 flg_checked1 할당
                                //obj.Checked2 = flg_checked2;  // obj.Checked2에 flg_checked2 할당
                                break;  // 반복문 종료.
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.StackTrace); // 에외 처리
            }
        }

        private void tDriverSocket1_Connected(object sender, Delta.IODriver.TDriverSocketEventArgs pevent)  // tDriverSocket1 연결될 시 실행되는 메소드
        {
            //richTextBox1.Text = "tDriverSocket1_Connected" + Environment.NewLine + richTextBox1.Text; ;
            writeRichTextbox("클라이언트 연결됨.");   // richTextBox2에 코멘트 디스플레이
        }

        private void tDriverSocket1_DeltaAlmInformation(object sender, Delta.Common.TDeltaEventArgs pevent) // tDriverSocket1의 알람 정보가 있을 시 실행되는 메소드
        {
        }

        private void tDriverSocket1_Disconnected(object sender, Delta.IODriver.TDriverSocketEventArgs pevent)   // tDriverSocket1 연결 끊길 시 실행되는 메소드
        {
            //richTextBox1.Text = "tDriverSocket1_Disconnected" + Environment.NewLine + richTextBox1.Text; ;
            writeRichTextbox("클라이언트 연결 종료"); // richTextBox2에 코멘트 디스플레이
        }

        private void tDriverSocket1_Received(object sender, Delta.IODriver.TDriverSocketEventArgs pevent)   // tDriverSocket1을 통해 받는 데이터 처리 메소드
        {
            Buffer.BlockCopy(pevent.IOBytes, 0, m_rcvBuffer.m_lstBuffer, m_rcvBuffer.m_nOffset, pevent.IOBytes.Length); // 매개변수를 통해 전달 받은 인자 IOBytes 배열 0번째 IOBytes.Length까지 m_rcvBuffer.m_lstBuffer에 m_rcvBuffer.m_nOffset부터 복사
            m_rcvBuffer.m_nOffset += pevent.IOBytes.Length; // m_rcvBuffer.m_nOffset는 pevent.IOBytes.Length를 더함
            byte[] local_buffer = new byte[CI_MAX_PDA_CAPACITY];  // CI_MAX_PDA_CAPACITY의 크기를 갖는 local_buffer 이름의 바이트 배열 선언
            while (m_rcvBuffer.m_nOffset >= CI_MAX_PDA_CAPACITY)    // m_rcvBuffer.m_nOffset이 CI_MAX_PDA_CAPACITY보다 크거나 같을 때
            {
                Buffer.BlockCopy(m_rcvBuffer.m_lstBuffer, 0, local_buffer, 0, CI_MAX_PDA_CAPACITY);   // m_rcvBuffer.m_lstBuffer 배열 0번째부터 CI_MAX_PDA_CAPACITY까지 local_buffer에 0번째부터 복사
                Buffer.BlockCopy(m_rcvBuffer.m_lstBuffer, CI_MAX_PDA_CAPACITY, m_rcvBuffer.m_lstBuffer, 0, m_rcvBuffer.m_nOffset - CI_MAX_PDA_CAPACITY);    // m_rcvBuffer.m_lstBuffer 배열 CI_MAX_PDA_CAPACITY의 크기부터 m_rcvBuffer.m_nOffset - CI_MAX_PDA_CAPACITY까지  m_rcvBuffer.m_lstBuffer에 0번째부터 복사
                m_rcvBuffer.m_nOffset -= CI_MAX_PDA_CAPACITY; // m_rcvBuffer.m_nOffset는  CI_MAX_PDA_CAPACITY 만큼 감소

                TDataProcess p = new TDataProcess(this);    // TDataProcess 객체 p에 Form2를 생성자 인자로 하여 객체 생성
                Stopwatch sw = new Stopwatch(); // Stopwatch 객체 sw에 Form2를 생성자 인자로 하여 객체 생성
                sw.Start(); // Stopwatch 클래스의 인스턴스 메소드 Start() 호출
                p.ProcessPackage(local_buffer); // TDataProcess 클래스의 인스턴스 메소드 ProcessPackage()에 BlockCopy된 local_buffer를 인자로 호출
                sw.Stop();  // Stopwatch 클래스의 인스턴스 메소드 Stop() 호출
                Trace.WriteLine("Elapsed-time:" + sw.ElapsedMilliseconds);  // 출력창에 처리시간 출력

                if(tDriverSocket3.Activated)
                {
                    tDriverSocket3.Write(pevent.IOBytes);    // ibaPDA TCP Outputs 64 Byte 데이터를 Socket3로 그대로 바이패스
                }

            }
        }

        private void tDriverSocket2_Connected(object sender, Delta.IODriver.TDriverSocketEventArgs pevent)  // tDriverSocket2 연결될 시 실행되는 메소드
        {
            //richTextBox2.Text = "tDriverSocket2_Connected" + Environment.NewLine + richTextBox2.Text; ;
            writeRichTextbox("서버 연결됨."); // 코멘트 디스플레이
            //dataTransmit();

            if (__flgDataTransmit)  // 데이터 송신 중일 때,
            {
                startToolStripMenuItem.Enabled = false; // start 버튼 비활성화
                stopToolStripMenuItem.Enabled = true;   // stop 버튼 활성화
                writeRichTextbox("알람 전송 시작.");  // richTextBox2에 코멘트 출력
                textBox5.Invoke((MethodInvoker)delegate { textBox5.Clear(); textBox5.AppendText("알람 전송중."); });  // textBox5을 비우고, "알람 전송중." 코멘트 출력
            }
        }

        private void tDriverSocket2_DeltaAlmInformation(object sender, Delta.Common.TDeltaEventArgs pevent) // tDriverSocket2의 알람 정보가 있을 시 실행되는 메소드
        {
            System.Diagnostics.Debug.WriteLine(pevent.Information); // 매개변수 pevent의 정보 Debug창에 출력
        }

        private void tDriverSocket2_Disconnected(object sender, Delta.IODriver.TDriverSocketEventArgs pevent)   // tDriverSocket2 연결 끊길 시 실행되는 메소드
        {
            //richTextBox2.Text = "tDriverSocket2_Disconnected" + Environment.NewLine + richTextBox2.Text; ;
            writeRichTextbox("서버 연결종료."); // 코멘트 디스플레이
        }

        private void tDriverSocket2_Received(object sender, Delta.IODriver.TDriverSocketEventArgs pevent)   // tDriverSocket2을 통해 받는 데이터 처리 메소드
        {

        }

        private void tDriverSocket3_Connected(object sender, Delta.IODriver.TDriverSocketEventArgs pevent)
        {
            writeRichTextbox("통합 모니터링 시스템 연결됨."); // 코멘트 디스플레이
        }

        private void tDriverSocket3_DeltaAlmInformation(object sender, Delta.Common.TDeltaEventArgs pevent)
        {
            System.Diagnostics.Debug.WriteLine(pevent.Information); // 매개변수 pevent의 정보 Debug창에 출력
        }

        private void tDriverSocket3_Disconnected(object sender, Delta.IODriver.TDriverSocketEventArgs pevent)
        {
            writeRichTextbox("통합 모니터링 시스템 연결종료."); // 코멘트 디스플레이
        }

        private void tDriverSocket3_Received(object sender, Delta.IODriver.TDriverSocketEventArgs pevent)
        {

        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)    // MenuItem의 save 버튼 클릭 시 실행되는 메소드
        {
            m_iniSetting.saveAsiniFile();  // 메뉴바 Save 버튼 클릭 시, m_iniSettingsaveAsiniFile() 메소드 호출
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)    // MenuItem의 exit 버튼 클릭 시 실행되는 메소드
        {
            Application.Exit(); // 메뉴바 Exit 버튼 누를 시, 애플리케이션 종료
        }

        private void startToolStripMenuItem_Click(object sender, EventArgs e)   // MenuItem의 start 버튼 클릭 시 실행되는 메소드
        {
            __flgDataTransmit = true;   // 메뉴바 Configure Start 버튼 누르면 전역변수 __flgDataTransmit true로 할당 (데이터 전송 시작)

            writeRichTextbox("알람 전송 시작."); // richTextBox2에 코멘트 출력

            if (__flgDataTransmit)  // 데이터 전송 중이면
            {
                startToolStripMenuItem.Enabled = false; // start 버튼 비활성화
                stopToolStripMenuItem.Enabled = true;   // stop 버튼 활성화
                textBox5.Text = "알람 전송중.";  // textBox5에 해당 코멘트 출력
            }
            else    // 데이터 전송 정지면
            {
                startToolStripMenuItem.Enabled = true;  // start 버튼 활성화
                stopToolStripMenuItem.Enabled = false;  // stop 버튼 비활성화
                textBox5.Text = "알람 전송 정지됨.";   // textBox5에 해당 코멘트 출력
            }
        }

        private void stopToolStripMenuItem_Click(object sender, EventArgs e)    // MenuItem의 stop 버튼 클릭 시 실행되는 메소드
        {
            __flgDataTransmit = false;  // 전역변수 __flgDataTransmit false로 할당 (데이터 전송 정지)

            writeRichTextbox("알람 전송 정지."); // 코멘트 디스플레이

            if (__flgDataTransmit)  // 데이터 전송 중이면
            {
                startToolStripMenuItem.Enabled = false; // start 버튼 비활성화
                stopToolStripMenuItem.Enabled = true;   // stop 버튼 활성화
                textBox5.Text = "알람 전송중.";  // textBox5에 해당 코멘트 출력
            }
            else    // 데이터 전송 정지면
            {
                startToolStripMenuItem.Enabled = true;  // start 버튼 활성화
                stopToolStripMenuItem.Enabled = false;  // stop 버튼 비활성화
                textBox5.Text = "알람 전송 정지됨.";   // textBox5에 해당 코멘트 출력
            }


        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)   // MenuItem의 about 버튼 클릭 시 실행되는 메소드
        {
            Form3 moadal = new Form3(); // Form3 객체 modal 이름으로 생성
            moadal.StartPosition = FormStartPosition.CenterParent;  // Form3의 시작 위치를 Form2의 가운데로 지정
            moadal.Show();  // Form3 실행
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)    // MenuItem의 open 버튼 클릭 시 실행되는 메소드
        {
            dataGridView1.CurrentCell = null;   // 현재 셀 null로 없앰
            m_iniSetting.openAsiniFile();  // openAsiniFile() 메소드 호출
            try
            {
                bool flg_checked1 = false;  // flg_checked1 선언 및 false로 초기화
                //bool flg_checked2 = false;  // flg_checked2 선언 및 false로 초기화
                int i_signal = 0;   // i_siganl 선언 및 0으로 초기화
                
                for (int i = 0; i < dataGridView1.Rows.Count; i++)  // dataGridView Row만큼 반복
                {
                    flg_checked1 = (bool)dataGridView1[1, i].Value; // flg_checked1에 dataGridView 1열 i행 Value 할당
                    //flg_checked2 = (bool)dataGridView1[2, i].Value; // flg_checked2에 dataGridView 2열 i행 Value 할당
                    i_signal = (int)dataGridView1[0, i].Value;  // i_signal에 dataGridView 0열 i행 Value 할당

                    //System.Diagnostics.Debug.WriteLine(i_signal + " " + flg_checked1);   // s_signal을 Debug 창에 띄움.

                    foreach (TSignalUnit obj in m_sigSetting.m_lstSignals)  // m_sigSetting 객체의 m_lstSignlas 객체를 TSignalUnit형의 obj으로 할당
                    {
                        if (obj.Id == i_signal) // obj.Id과 i_signal이 같다면
                        {
                            obj.Checked1 = flg_checked1;  // obj.Checked1에 flg_checked1 할당
                            //obj.Checked2 = flg_checked2;  // obj.Checked2에 flg_checked2 할당
                            break;  // 반복문 종료.
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.StackTrace); // 에외 처리
            }
        }

        private void Form2_FormClosing(object sender, FormClosingEventArgs e)   // 프로그램이 꺼지는 중에 실행되는 메소드
        {
            __flginvokeStop = true; // 전역변수 __flginvokeStop true로 할당
        }
    }

#if ACTIVATE_DEPRECATED
        private void button1_Click(object sender, EventArgs e)
        {
            __flgCloseThread1 = false;  // 무한루프에 돌도록 함
            __thread1 = new Thread(ThreadProcessServer);   // Thread 객채 생성, Form과는 별도 쓰레드에서 ThreadProcessServer 함수가 실행됨
            __thread1.IsBackground = true; // Form이 종료되면 thread1도 종료.
            __thread1.Start(); // thread1 시작.

            button1.Visible = false;    // button1 클릭 시 button1 숨기기
            button2.Visible = true; // button1 클릭 시 button2 숨김 해제
        }

        private void button2_Click(object sender, EventArgs e)
        {
            __flgCloseThread1 = true;   // 무한루프에 빠져나오게 함

            for (int i = 0; i < dataGridView1.Rows.Count; i++) // 알람으로 인한 변한 리스트 색상을 다시 원래대로 되돌려놓음
            {
                dataGridView1.Rows[i].DefaultCellStyle.ForeColor = Color.Black; // 글자색 검정색으로 변경
                dataGridView1.Rows[i].DefaultCellStyle.BackColor = Color.White; // 배경색 흰색으로 변경
            }

            button2.Visible = false;    // button2 클릭 시 button2 숨기기
            button1.Visible = true; // button2 클릭 시 button1 숨김 해제
        }

        private void button3_Click(object sender, EventArgs e)
        {
            __flgCloseThread2 = false;  // 무한루프에 돌도록 함
            __thread2 = new Thread(ThreadProcessClient);   // Thread 객채 생성, Form과는 별도 쓰레드에서 ThreadProcessServer 함수가 실행됨
            __thread2.IsBackground = true; // Form이 종료되면 thread2도 종료.
            __thread2.Start(); // thread2 시작.

            button3.Visible = false;    // button3 클릭 시 button3 숨기기
            button4.Visible = true; // button3 클릭 시 button4 숨김 해제
        }

        private void button4_Click(object sender, EventArgs e)
        {
            __flgCloseThread2 = true;   // 무한루프에 빠져나오게 함

            button4.Visible = false;    // button4 클릭 시 button4 숨기기
            button3.Visible = true; // button4 클릭 시 button3 숨김 해제
        }
#else
        //private void dataTransmit()
        //{
        //    __flgDataTransmit = true;   // 데이터 송신상태 ON

        //    button5.Visible = false;    // button5 클릭 시 button5 숨기기
        //    button6.Visible = true; // button5 클릭 시 button6 숨김 해제

        //    writeRichTextbox("데이터 송신 ON..."); // 코멘트 디스플레이
        //}

        //private void button1_Click(object sender, EventArgs e)
        //{
        //    serverConnect();
        //}

        //private void button2_Click(object sender, EventArgs e)
        //{
        //    if (tDriverSocket1.Activated) tDriverSocket1.ShutdownDriver();

        //    for (int i = 0; i < dataGridView1.Rows.Count; i++) // 알람으로 인한 변한 리스트 색상을 다시 원래대로 되돌려놓음
        //    {
        //        dataGridView1.Rows[i].DefaultCellStyle.ForeColor = Color.Black; // 글자색 검정색으로 변경
        //        dataGridView1.Rows[i].DefaultCellStyle.BackColor = Color.White; // 배경색 흰색으로 변경
        //    }

        //    button2.Visible = false;    // button2 클릭 시 button2 숨기기
        //    button1.Visible = true; // button2 클릭 시 button1 숨김 해제
        //}

        //private void button3_Click(object sender, EventArgs e)
        //{
        //    clientConnect();
        //}

        //private void button4_Click(object sender, EventArgs e)
        //{
        //    if (tDriverSocket2.Activated) tDriverSocket2.ShutdownDriver();

        //    button4.Visible = false;    // button4 클릭 시 button4 숨기기
        //    button3.Visible = true; // button4 클릭 시 button3 숨김 해제
        //}

        //private void button5_Click(object sender, EventArgs e)
        //{
        //    dataTransmit();
        //}

        //private void button6_Click(object sender, EventArgs e)
        //{
        //    __flgDataTransmit = false;   // 데이터 송신상태 OFF

        //    button6.Visible = false;    // button6 클릭 시 button6 숨기기
        //    button5.Visible = true; // button4 클릭 시 button6 숨김 해제

        //    writeRichTextbox("데이터 송신 OFF..."); // 코멘트 디스플레이
        //}

#endif //#if ACTIVATE_DEPRECATED

    //private void ThreadProcessServer()
    //{
    //    try
    //    {
    //        TcpListener tcpListener1 = new TcpListener(IPAddress.Parse(textBox1.Text), int.Parse(textBox2.Text)); // 서버 객체 생성 및 IP주소와 Port번호를 할당
    //        tcpListener1.Start();  // 서버 시작
    //        writeRichTextbox("서버 준비...클라이언트 기다리는 중...");    // 코멘트 디스플레이

    //        TcpClient tcpClient1 = tcpListener1.AcceptTcpClient(); // 클라이언트 접속 확인
    //        writeRichTextbox("클라이언트 연결됨...");   // 코멘트 디스플레이

    //        nsReader = tcpClient1.GetStream();  // 읽기 스트림 연결
    //        nsWriter = tcpClient1.GetStream();  // 쓰기 스트림 연결

    //        while (tcpClient1.Connected)  // 클라이언트가 연결되어 있는 동안
    //        {
    //            while (nsReader.CanRead)    // nsReader 읽기 가능한지 여부를 확인
    //            {
    //                int n_data = 0;
    //                byte[] local_buffer = new byte[CI_MAX_PDA_CAPACITY];  // 4바이트 단위로 local_buffer 변수 선언
    //                while (-1 != (n_data = nsReader.ReadByte()))
    //                {
    //                    if (__flgCloseThread1) goto EXIT_LOOP;  // __figCloseThread1가 True일 때, EXIT_LOOP로 흐름을 바꿔줌
    //                    m_rcvBuffer.m_lstBuffer[m_rcvBuffer.m_nOffset] = (byte)n_data;  // NetworkStream을 통해 읽은 바이트를 n_data 변수에 넣어줌
    //                    m_rcvBuffer.m_nOffset++;    // m_nOffset 하니씩 늘어남.

    //                    while (m_rcvBuffer.m_nOffset >= CI_MAX_PDA_CAPACITY)
    //                    {
    //                        Buffer.BlockCopy(m_rcvBuffer.m_lstBuffer, 0, local_buffer, 0, CI_MAX_PDA_CAPACITY);   // m_rcvBuffer 배열 0번째부터 local_buffer에 0번째부터 4개 복사
    //                        Buffer.BlockCopy(m_rcvBuffer.m_lstBuffer, CI_MAX_PDA_CAPACITY, m_rcvBuffer.m_lstBuffer, 0, m_rcvBuffer.m_nOffset - CI_MAX_PDA_CAPACITY);    // m_rcvBuffer 배열 4번째부터 m_rcvBuffer에 0번째부터 -4개 복사
    //                        m_rcvBuffer.m_nOffset -= CI_MAX_PDA_CAPACITY; // m_rcvBuffer.m_nOffset 4 바이트 삭제

    //                        TDataProcess p = new TDataProcess(this);    // TDataProcess에 Form2를 넣어줌
    //                        Stopwatch sw = new Stopwatch();
    //                        sw.Start();
    //                        p.ProcessPackage(local_buffer); // 4바이트가 채워진 local_buffer 바이트 값을 ProcessPackage에 전송
    //                        sw.Stop();
    //                        Trace.WriteLine("Elapsed-time:" + sw.ElapsedMilliseconds);
    //                    }
    //                }
    //            }
    //        }
    //        System.Diagnostics.Debug.WriteLine("Hello!!!");
    //    EXIT_LOOP:
    //        if (null != tcpClient1) tcpClient1.Close();  // tcpClient1이 null이 아니면 tcpClient1 종료
    //        if (null != tcpListener1) tcpListener1.Stop();   // tcpListener1이 null이 아니면 tcpListener1 종료
    //        __thread1.Join(1000);   // 쓰레드 종료
    //        System.Diagnostics.Debug.WriteLine("Thread clean complete.");   // 쓰레드가 종료되었음을 Debug창에서 알림
    //        writeRichTextbox("클라이언트 연결 종료..."); // RichTextBox에 클라이언트 종료를 알려줌
    //    }

    //    catch (Exception ex)    // 에러 발생 시 예외 처리
    //    {
    //        MessageBox.Show(ex.Message);    // 에러 메세지 MessageBox로 띄움
    //        button2.Visible = false;    // button2 클릭 시 button2 숨기기
    //        button1.Visible = true; // button2 클릭 시 button1 숨김 해제
    //    }

    //}

    //private bool __SendIntermediateServer(NetworkStream netStream)
    //{
    //    bool flg_process = true;

    //    byte[] NameByte = new byte[64];    // NameByte 128 바이트로 선언
    //    byte[] Comment1Byte = new byte[64];    // Commnet1Byte 128 바이트로 선언
    //    byte[] Comment2Byte = new byte[64];    // Comment2Byte 128 바이트로 선언
    //    byte[] ValueByte = new byte[1]; // ValueByte 4 바이트로 선언
    //    byte[] OnesigData = new byte[193];  // 한 Signal 데이터 388 바이트로 선언
    //    byte[] WholeData = new byte[98816]; // 한 번 전송하는 전체 데이터 12416바이트로 선언
    //    int i = 0;

    //    //bool flg_exist = false;

    //    try
    //    {
    //        //foreach (TSignalUnit obj in m_sigSetting.m_lstSignals)  // TSignalUnit 리스트 반복
    //        //{
    //        //    //if (__flgCloseThread2) goto EXIT_LOOP;  // __figCloseThread2가 True일 때, EXIT_LOOP로 흐름을 바꿔줌
    //        //    if (obj.Checked1 == true)
    //        //    {
    //        //        if (obj.EdgeDetected)//if (obj.SavedValue == true)
    //        //        {
    //        //            byte[] NameByte = new byte[128];    // NameByte 128 바이트로 선언
    //        //            byte[] Comment1Byte = new byte[128];    // Commnet1Byte 128 바이트로 선언
    //        //            byte[] Comment2Byte = new byte[128];    // Comment2Byte 128 바이트로 선언
    //        //            byte[] ValueByte = new byte[4]; // ValueByte 4 바이트로 선언
    //        //            byte[] OnesigData = new byte[388];  // 한 Signal 데이터 388 바이트로 선언
    //        //            WholeData = new byte[12416]; // 한 번 전송하는 전체 데이터 12416바이트로 선언

    //        //            NameByte = Encoding.UTF8.GetBytes(obj.Name);    // TSignalUnit Name string => byte로 변환
    //        //            Comment1Byte = Encoding.UTF8.GetBytes(obj.Comment1);    // TSignalUnit Comment1 string => byte로 변환
    //        //            Comment2Byte = Encoding.UTF8.GetBytes(obj.Comment2);    // TSignalUnit Comment2 string => byte로 변환
    //        //            ValueByte = BitConverter.GetBytes(obj.SavedValue);  // TSignalUnit SavedValue bool => byte로 변환

    //        //            Buffer.BlockCopy(NameByte, 0, OnesigData, 0, NameByte.Length);  // NameByte OnesigData로 카피
    //        //            Buffer.BlockCopy(Comment1Byte, 0, OnesigData, 128, Comment1Byte.Length);    // Comment1 OnesigData로 카피
    //        //            Buffer.BlockCopy(Comment2Byte, 0, OnesigData, 256, Comment2Byte.Length);    // Comment2 OnesigData로 카피
    //        //            Buffer.BlockCopy(ValueByte, 0, OnesigData, 384, ValueByte.Length);  // SavedValue OnesigData로 카피
    //        //            Buffer.BlockCopy(OnesigData, 0, WholeData, 388 * i, OnesigData.Length); // OnesigData Whole로 카피

    //        //            flg_exist = true;
    //        //        }
    //        //    }

    //        //    i++;
    //        //}

    //        //if (flg_exist)   // i와 m_sigSetting.m_lstSignals.Count가 같다면
    //        //{
    //        //    if (WholeData != null && netStream != null)
    //        //    {
    //        //        netStream.Write(WholeData);  // 전체 데이터 전송
    //        //    }
    //        //}

    //        int cnt_edge_detected = 0;  // cnt_edge_detected 변수 선언 및 0으로 할당
    //        foreach (TSignalUnit obj in m_sigSetting.m_lstSignals)  // TSignalUnit 리스트 반복
    //        {
    //            if (__flgDataTransmit)  // 데이터 송신 상태가 true이면
    //            {
    //                //if (__flgCloseThread2) goto EXIT_LOOP;  // __figCloseThread2가 True일 때, EXIT_LOOP로 흐름을 바꿔줌
    //                if (obj.Checked1 == true)
    //                {
    //                    if (obj.EdgeDetected)//if (obj.SavedValue == true)
    //                    {
    //                        NameByte = Encoding.UTF8.GetBytes(obj.Name);    // TSignalUnit Name string => byte로 변환
    //                        Comment1Byte = Encoding.UTF8.GetBytes(obj.Comment1);    // TSignalUnit Comment1 string => byte로 변환
    //                        Comment2Byte = Encoding.UTF8.GetBytes(obj.Comment2);    // TSignalUnit Comment2 string => byte로 변환
    //                        ValueByte = BitConverter.GetBytes(obj.SavedValue);  // TSignalUnit SavedValue bool => byte로 변환

    //                        Buffer.BlockCopy(NameByte, 0, OnesigData, 0, NameByte.Length);  // NameByte OnesigData로 카피
    //                        Buffer.BlockCopy(Comment1Byte, 0, OnesigData, 64, Comment1Byte.Length);    // Comment1 OnesigData로 카피
    //                        Buffer.BlockCopy(Comment2Byte, 0, OnesigData, 128, Comment2Byte.Length);    // Comment2 OnesigData로 카피
    //                        Buffer.BlockCopy(ValueByte, 0, OnesigData, 192, ValueByte.Length);  // SavedValue OnesigData로 카피
    //                        Buffer.BlockCopy(OnesigData, 0, WholeData, 193 * i, OnesigData.Length); // OnesigData Whole로 카피

    //                        cnt_edge_detected++;    // cnt_edge_detected 1씩 증가
    //                        //flg_exist = true;
    //                    }
    //                }

    //                i++;

    //                if (i == m_sigSetting.m_lstSignals.Count && cnt_edge_detected > 0)   // cnt_edge_detected와 m_sigSetting.m_lstSignals.Count가 같다면
    //                {
    //                    if (WholeData != null && netStream != null)
    //                    {
    //                        netStream.Write(WholeData, 0, WholeData.Length);  // 전체 데이터 전송
    //                        System.Diagnostics.Debug.WriteLine(WholeData.Length.ToString() + "Bytes sent complete(" + cnt_edge_detected.ToString() + ")");
    //                    }
    //                }
    //            }
    //        }

    //        System.Diagnostics.Debug.WriteLine("Hello");
    //        flg_process = false;
    //    }
    //    catch (Exception except)
    //    {
    //        MessageBox.Show(except.Message);
    //    }

    //    return flg_process;
    //}

#if ACTIVATE_DEPRECATED
        private void ThreadProcessClient()
        {
            try
            {
                IPEndPoint ipEnd = new IPEndPoint(IPAddress.Parse(textBox3.Text), int.Parse(textBox4.Text));  // IP주소와 Port번호를 할당
                tcpClient = new TcpClient();
                tcpClient.Connect(ipEnd);  // 서버에 연결 요청
                writeRichTextbox("서버 연결됨..."); // 코멘트 디스플레이

                nsWriter = tcpClient.GetStream();  // tcpClient Network Stream 연결

                while (tcpClient.Connected)  // 클라이언트가 연결되어 있는 동안
                {
                    while (nsWriter.CanWrite)   // 데이터 쓰기가 가능하다면
                    {
                        byte[] NameByte = new byte[128];    // NameByte 128 바이트로 선언
                        byte[] Comment1Byte = new byte[128];    // Commnet1Byte 128 바이트로 선언
                        byte[] Comment2Byte = new byte[128];    // Comment2Byte 128 바이트로 선언
                        byte[] ValueByte = new byte[4]; // ValueByte 4 바이트로 선언
                        byte[] OnesigData = new byte[388];  // 한 Signal 데이터 388 바이트로 선언
                        byte[] WholeData = new byte[12416]; // 한 번 전송하는 전체 데이터 12416바이트로 선언
                        int i = 0;

                        foreach (TSignalUnit obj in m_sigSetting.m_lstSignals)  // TSignalUnit 리스트 반복
                        {
                            //if (__flgCloseThread2) goto EXIT_LOOP;  // __figCloseThread2가 True일 때, EXIT_LOOP로 흐름을 바꿔줌
                            if (obj.Checked1 == true)
                            {
                                if (obj.EdgeDetected)//if (obj.SavedValue == true)
                                {
                                    NameByte = Encoding.UTF8.GetBytes(obj.Name);    // TSignalUnit Name string => byte로 변환
                                    Comment1Byte = Encoding.UTF8.GetBytes(obj.Comment1);    // TSignalUnit Comment1 string => byte로 변환
                                    Comment2Byte = Encoding.UTF8.GetBytes(obj.Comment2);    // TSignalUnit Comment2 string => byte로 변환
                                    ValueByte = BitConverter.GetBytes(obj.SavedValue);  // TSignalUnit SavedValue bool => byte로 변환

                                    Buffer.BlockCopy(NameByte, 0, OnesigData, 0, NameByte.Length);  // NameByte OnesigData로 카피
                                    Buffer.BlockCopy(Comment1Byte, 0, OnesigData, 128, Comment1Byte.Length);    // Comment1 OnesigData로 카피
                                    Buffer.BlockCopy(Comment2Byte, 0, OnesigData, 256, Comment2Byte.Length);    // Comment2 OnesigData로 카피
                                    Buffer.BlockCopy(ValueByte, 0, OnesigData, 384, ValueByte.Length);  // SavedValue OnesigData로 카피
                                    Buffer.BlockCopy(OnesigData, 0, WholeData, 388 * i, OnesigData.Length); // OnesigData Whole로 카피
                                }
                            }

                            i++;

                            if (i == m_sigSetting.m_lstSignals.Count)   // i와 m_sigSetting.m_lstSignals.Count가 같다면
                            {
                                if (WholeData != null)
                                {
                                    nsWriter.Write(WholeData);  // 전체 데이터 전송
                                }
                            }
                        }
                        if (__flgCloseThread2) goto EXIT_LOOP;
                    }
                }
            EXIT_LOOP:
                if (null != tcpClient)
                {
                    tcpClient.Close(); // tcpClient이 null이 아니면 tcpClient 종료
                    tcpClient.Dispose();
                }
                __thread2.Join(1000);   // 쓰레드 종료
                writeRichTextbox("클라이언트 연결 종료..."); // RichTextBox에 클라이언트 종료를 알려줌
            }

            catch (Exception ex) // 에러 발생 시 예외 처리
            {
                MessageBox.Show(ex.Message);    // 에러 메세지 MessageBox로 띄움
            }
        }
#else
        //private void ThreadProcessClient()
        //{
        //    try
        //    {
        //        IPEndPoint ipEnd = new IPEndPoint(IPAddress.Parse(textBox3.Text), int.Parse(textBox4.Text));  // IP주소와 Port번호를 할당
        //        tcpClient = new TcpClient();    // TcpClient 인스턴스 생성
        //        tcpClient.Connect(ipEnd);  // 서버에 연결 요청
        //        writeRichTextbox("서버 연결됨..."); // 코멘트 디스플레이

        //        nsWriter = tcpClient.GetStream();  // tcpClient Network Stream 연결

        //        while (tcpClient.Connected)  // 클라이언트가 연결되어 있는 동안
        //        {
        //            while (nsWriter.CanWrite)   // 데이터 쓰기가 가능하다면
        //            {
        //                //__flgCloseThread2 = __SendIntermediateServer(nsWriter);
        //                if (__flgCloseThread2) goto EXIT_LOOP;  // __flgCloseThread2가 true일 시 EXIT_LOOP 실행
        //            }
        //        }
        //        EXIT_LOOP:
        //        if (null != tcpClient) // tcpClient이 null이 아니면
        //        {
        //            tcpClient.Close(); // tcpClient 종료
        //            tcpClient.Dispose();    // tcpClient 처분
        //            nsWriter = null;    // nsWriter null 할당
        //        }
        //        __thread2.Join(1000);   // 쓰레드 종료
        //        writeRichTextbox("클라이언트 연결 종료..."); // RichTextBox에 클라이언트 종료를 알려줌
        //    }

        //    catch (Exception ex) // 에러 발생 시 예외 처리
        //    {
        //        MessageBox.Show(ex.Message);    // 에러 메세지 MessageBox로 띄움
        //    }
        //}
#endif
}