using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;

namespace AlmPIMS
{
    internal class TSetTimer
    {
        private Form2 __pRootView = null;   // Form2를 타입으로 가지는 전역변수 __pRootView를 null로 선언 및 초기화

        private System.Timers.Timer aTimer; // Timer 클래스를 타입으로 가지는 aTimer 전역변수 선언
        private readonly double cycleTime = 1000;   // 스캔할 시간, 1000 = 1초, 읽기 전용 double형 변수 cycleTime 1000으로 선언 및 초기화

        public DateTime yesterdayDate = DateTime.ParseExact(DateTime.Now.AddDays(-1).ToString("yyyyMMdd"), "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture); // 오늘 날짜에 Addday(-1)을 통해 어제의 날짜 할당
        //public readonly string sendTime = "070000"; //HHmmss   전체 정보 전송할 시간 설정, 읽기 전용 string형 변수 sendTime "070000"으로 선언 및 초기화


        public TSetTimer(Form2 pRootView)   // Form2 클래스를 매개변수로 받는 클래스 생성자 생성
        {
            __pRootView = pRootView;    // 매개변수로 전달 받은 Form2 객체를 전역변수 __pRootView에 할당
        }

        public void SetTimer()  // 타이머 설정 메소드
        {
            aTimer = new System.Timers.Timer(cycleTime);    // aTimer에 1초(cycleTime)마다 스캔하는 Timer 객체 생성

            // 이벤트 핸들러 연결
            aTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);    // 매초마다 OnTimeEvent() 메소드 호출

            // Timer에서 Elapsed 이벤트를 반복해서 발생
            aTimer.AutoReset = true;    // 타이머 자동 리셋 on
            aTimer.Enabled = true;  // 타이머 활성화 on
        }

        public void OnTimedEvent(object sender, ElapsedEventArgs e) // 매초마다 실행할 메소드
        {
            DateTime todayDate = DateTime.ParseExact(DateTime.Now.ToString("yyyyMMdd"), "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture); // 오늘 날짜 할당
            DateTime nowTime = DateTime.ParseExact(DateTime.Now.ToString("HHmmss"), "HHmmss", System.Globalization.CultureInfo.InvariantCulture);   // 현재 시간 할당
            DateTime setTime = DateTime.ParseExact(__pRootView.m_sigSetting.SendTime, "HHmmss", System.Globalization.CultureInfo.InvariantCulture);  // sendTime 할당

            //m_IniFileSetting = __pRootView.m_iniSetting;    // m_IniFileSetting에 Form2의 m_iniSetting 객체 할당
            //m_IniFileSetting.saveiniFile(); // TIniFileSetting 클래스의 saveiniFile() 메소드 호출
            if (null != __pRootView) __pRootView.SaveIniFile();

            byte[] transactionCode = new byte[4];    //transactionCode를 4 바이트 배열로 선언
            byte[] dateTime = new byte[14];    // dateTime를 14 바이트 배열로 선언
            byte[] factoryCode = new byte[10];    // factoryCode를 10 바이트 배열로 선언
            byte[] alarmNumber = new byte[10]; // alarmNumber를 10 바이트 배열로 선언
            byte[] equipmentCode = new byte[10]; // equipmentCode를 10 바이트 배열로 선언
            byte[] alarmMessage = new byte[128]; // alarmMessage를 128 바이트 배열로 선언
            byte[] OnesigData = new byte[176];  // 한 Signal 데이터를 176 바이트 배열로 선언
            byte[] WholeData = new byte[90112]; // 한 번 전송하는 전체 데이터를 90112 바이트 배열로 선언
            int i = 0;  // 지역변수 i를 0으로 선언 및 초기화

            if (todayDate > yesterdayDate)  // 어제 날짜보다 오늘 날짜가 더 클 때
            {
                if (nowTime >= setTime) // 현재 시간과 지정한 sendTime이 같으면 전체 신호 정보 전송
                {
                    //수행할 타이머 이벤트
                    try
                    {
                        if (__pRootView.__flgDataTransmit)  // 프로그램이 데이터 송신을 하고 있을 때
                        {
                            short cnt_edge_detected = 0;  // cnt_edge_detected 변수 선언 및 0으로 할당
                            short headerCode = 7777;
                            foreach (TSignalUnit obj in __pRootView.m_sigSetting.m_lstSignals)  // m_sigSetting.m_lstSignals 리스트 반복
                            {
                                OnesigData = Enumerable.Repeat((byte)0x20, OnesigData.Length).ToArray();    // 한 Signal 데이터를 초기화
                                transactionCode = Encoding.UTF8.GetBytes("TC01");   // TC01 string을 byte로 형변환하여 transactionCode 바이트 배열에 할당
                                dateTime = Encoding.UTF8.GetBytes(DateTime.Now.ToString("yyyyMMddHHmmss")); // 현재 시간을 byte로 형변환하여 dateTime 바이트 배열에 할당
                                factoryCode = Encoding.UTF8.GetBytes(__pRootView.m_sigSetting.FactoryCode);    // Form2의 m_sigSetting 객체의 FactorCode를 byte로 형변환하여 factoryCode 바이트 배열에 할당
                                alarmNumber = Encoding.UTF8.GetBytes(obj.Expression);    // Form2의 m_sigSetting 객체의 m_lstSignals 객체의 Expression를 byte로 형변환하여 alarmNumber 바이트 배열에 할당
                                equipmentCode = Encoding.UTF8.GetBytes(obj.Comment1);    // Form2의 m_sigSetting 객체의 m_lstSignals 객체의 Comment1을 byte로 형변환하여 equipmentCode 바이트 배열에 할당
                                alarmMessage = Encoding.UTF8.GetBytes(obj.Name);    // Form2의 m_sigSetting 객체의 m_lstSignals 객체의 Name을 byte로 형변환하여 alarmMessage 바이트 배열에 할당

                                Buffer.BlockCopy(transactionCode, 0, OnesigData, 0, transactionCode.Length);    // transactionCode를 transactionCode의 바이트 배열 크기만큼 OnesigData 바이트 배열에 0부터 복사
                                Buffer.BlockCopy(dateTime, 0, OnesigData, 4, dateTime.Length);    // dateTime를 dateTime의 바이트 배열 크기만큼 OnesigData 바이트 배열에 4부터 복사
                                Buffer.BlockCopy(factoryCode, 0, OnesigData, 18, factoryCode.Length);    // factoryCode를 factoryCode의 바이트 배열 크기만큼 OnesigData 바이트 배열에 18부터 복사
                                Buffer.BlockCopy(alarmNumber, 0, OnesigData, 28, alarmNumber.Length);  // alarmNumber를 alarmNumber의 바이트 배열 크기만큼 OnesigData 바이트 배열에 28부터 복사
                                Buffer.BlockCopy(equipmentCode, 0, OnesigData, 38, equipmentCode.Length);  // equipmentCode를 equipmentCode의 바이트 배열 크기만큼 OnesigData 바이트 배열에 38부터 복사
                                Buffer.BlockCopy(alarmMessage, 0, OnesigData, 48, alarmMessage.Length);  // alarmMessage를 alarmMessage의 바이트 배열 크기만큼 OnesigData 바이트 배열에 48부터 복사
                                Buffer.BlockCopy(OnesigData, 0, WholeData, 176 * i, OnesigData.Length); // OnesigData를 OnesigData의 바이트 배열 크기만큼 WholeData 바이트 배열에 176 * i부터 복사

                                cnt_edge_detected++;    // cnt_edge_detected 1씩 증가

                                i++;    // i 1씩 증가

                                if (!__pRootView.PendingUpdate) // Form2의 PendingUpdate가 true가 아닐 때
                                {
                                    if (i == __pRootView.m_sigSetting.m_lstSignals.Count && cnt_edge_detected > 0)   // cnt_edge_detected와 m_sigSetting.m_lstSignals.Count가 같고, cnt_edge_detected가 0보다 클 때
                                    {
                                        if (WholeData != null && __pRootView.tDriverSocket2.Activated)  // 전체 데이터가 null이 아니고 tDriverSocket2이 연결되어 있을 때
                                        {
                                            byte[] s_buffer = new byte[WholeData.Length + 4]; // 헤더 4Byte + 하나의 신호 데이터 * 데이터의 수의 길이를 가지는 배열 생성
                                            byte[] lp_code = BitConverter.GetBytes(headerCode);
                                            byte[] lp_counter = BitConverter.GetBytes(cnt_edge_detected);   // 정수형 cnt_edge_detected를 byte형으로 형변환하여 Ip_counter에 할당
                                            Buffer.BlockCopy(lp_code, 0, s_buffer, 0, lp_code.Length);    // lp_counter를 lp_counter의 바이트 배열 크기만큼 s_buffer 바이트 배열에 0부터 복사
                                            Buffer.BlockCopy(lp_counter, 0, s_buffer, 2, lp_counter.Length);    // lp_counter를 lp_counter의 바이트 배열 크기만큼 s_buffer 바이트 배열에 0부터 복사
                                            Buffer.BlockCopy(WholeData, 0, s_buffer, 4, s_buffer.Length - 4);    // WholeData를 s_buffer 바이트 배열 크기만큼 s_buffer 바이트 배열에 4부터 복사

                                            __pRootView.tDriverSocket2.Write(s_buffer);    // s_buffer 데이터 전송

                                            string s_value = string.Empty;
                                            s_value += string.Format("'{0}'Bytes Sent", s_buffer.Length) + Environment.NewLine;
                                            s_value = Delta.Common.TUtil.HexDump(s_buffer, s_buffer.Length);
                                            //System.Diagnostics.Debug.WriteLine(s_buffer.Length.ToString() + "Bytes sent complte(" + cnt_edge_detected.ToString() + ")");   // 전송한 신호의 수 Debug창에 띄움
                                            System.Diagnostics.Trace.WriteLine(s_value);
                                            //__pRootView.m_SavedVaules.savedTextFileTC02();

                                            ////netStream.Write(WholeData, 0, WholeData.Length);  // 전체 데이터 전송
                                            //__pRootView.tDriverSocket2.Write(WholeData);    // 전체 데이터 전송
                                            //System.Diagnostics.Debug.WriteLine(WholeData.Length.ToString() + "Bytes sent complte(" + cnt_edge_detected.ToString() + ")");   // 전송한 신호의 수 Debug창에 띄움
                                            ////__pRootView.m_SavedVaules.savedTextFileTC01();
                                        }
                                    }
                                }
                            }

                            System.Diagnostics.Debug.WriteLine(DateTime.Now.ToString("HHmmss") + "OnTimedEvent"); // Debug창에 코멘트 출력
                        }
                    }
                    catch (Exception except)    // 예외 처리
                    {
                        MessageBox.Show(except.Message);    // MessageBox에 예외 출력
                    }
                    Console.WriteLine("The Elapsed event was raised at {0:HH:mm:ss.fff}", e.SignalTime);    // 처리 시간 Debug 창에 띄움

                    yesterdayDate = todayDate;  // yesterdayDate에 todayDate를 할당해주어 하루에 한 번만 실행되도록 함
                }
            }
        }
    }
}
