using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AlmPIMS
{
    internal class TSocketConnect
    {
        private Form2 __pRootView = null;   // Form2를 타입으로 가지는 전역변수 __pRootView를 null로 선언 및 초기화  

        public TSocketConnect(Form2 pRootView)  // Form2 클래스를 매개변수로 받는 클래스 생성자 생성
        {
            __pRootView = pRootView;    // 매개변수로 전달 받은 Form2 객체를 전역변수 __pRootView에 할당
        }

        public void serverConnect() // TCP Server 연결 메소드
        {
            if (!__pRootView.tDriverSocket1.Activated)
            {
                //tDriverSocket1.BindAddress = textBox1.Text;
                //tDriverSocket1.BindPort = Int32.Parse(textBox2.Text);
                __pRootView.tDriverSocket1.BindAddress = __pRootView.m_sigSetting.IPAddress1;   // 연결할 ibaPDA TCP Client IP 주소
                __pRootView.tDriverSocket1.BindPort = __pRootView.m_sigSetting.Port1;   // 연결할 ibaPDA TCP Client Port 번호
                __pRootView.tDriverSocket1.SocketSessionStyle = Delta.IODriver.SocketSessionStyle.TcpServer;    // TCP Server로 소켓 생성
                //tDriverSocket1.SocketPackageStyle = Delta.IODriver.SocketPackageStyle.Fixed;
                //tDriverSocket1.FixedCapacity = 4;
                __pRootView.tDriverSocket1.SocketPackageStyle = Delta.IODriver.SocketPackageStyle.None;

                __pRootView.tDriverSocket1.ActivateDriver(null);
            }
        }

        public void clientConnect() // TCP Client 연결 메소드
        {
            if (!__pRootView.tDriverSocket2.Activated)
            {
                //tDriverSocket2.PeerAddress = textBox3.Text;
                //tDriverSocket2.PeerPort = Int32.Parse(textBox4.Text);
                __pRootView.tDriverSocket2.PeerAddress = __pRootView.m_sigSetting.IPAddress2;   // 연결할 중계 Server TCP Server IP 주소
                __pRootView.tDriverSocket2.PeerPort = __pRootView.m_sigSetting.Port2;   // 연결할 중계 Server TCP Server Port 번호
                __pRootView.tDriverSocket2.SocketSessionStyle = Delta.IODriver.SocketSessionStyle.TcpClient;    // 소켓을 TCP Client로 생성
                //tDriverSocket2.SocketPackageStyle = Delta.IODriver.SocketPackageStyle.Fixed;
                //tDriverSocket1.FixedCapacity = 4;
                __pRootView.tDriverSocket2.SocketPackageStyle = Delta.IODriver.SocketPackageStyle.None;

                __pRootView.tDriverSocket2.ActivateDriver(null);
            }
        }

        public void clientConnectICT() // 통합모니터링시스템 TCP Client 연결 메소드
        {
            if (!__pRootView.tDriverSocket3.Activated)
            {
                //tDriverSocket3.PeerAddress = textBox3.Text;
                //tDriverSocket3.PeerPort = Int32.Parse(textBox4.Text);
                __pRootView.tDriverSocket3.PeerAddress = __pRootView.m_sigSetting.IPAddress3;   // 연결할 ICT TCP Server IP 주소
                __pRootView.tDriverSocket3.PeerPort = __pRootView.m_sigSetting.Port3;   // 연결할 ICT TCP Server Port 번호
                __pRootView.tDriverSocket3.SocketSessionStyle = Delta.IODriver.SocketSessionStyle.TcpClient;    // 소켓을 TCP Client로 생성
                //tDriverSocket3.SocketPackageStyle = Delta.IODriver.SocketPackageStyle.Fixed;
                //tDriverSocket3.FixedCapacity = 4;
                __pRootView.tDriverSocket3.SocketPackageStyle = Delta.IODriver.SocketPackageStyle.None;

                __pRootView.tDriverSocket3.ActivateDriver(null);
            }
        }

        public bool __WriteData_old()   // 데이터 전송 메소드
        {
            bool flg_process = true;    // 지역변수 flg_process 선언 및 true로 초기화

            byte[] transactionCode = new byte[4];    //transactionCode를 4 바이트 배열로 선언
            byte[] dateTime = new byte[14];    // dateTime를 14 바이트 배열로 선언
            byte[] factoryCode = new byte[10];    // factoryCode를 10 바이트 배열로 선언
            byte[] alarmNumber = new byte[10]; // alarmNumber를 10 바이트 배열로 선언
            byte[] equipmentCode = new byte[10]; // equipmentCode를 10 바이트 배열로 선언
            byte[] alarmMessage = new byte[128]; // alarmMessage를 128 바이트 배열로 선언
            byte[] OnesigData = new byte[176];  // 한 Signal 데이터를 176 바이트 배열로 선언
            byte[] WholeData = new byte[90112]; // 한 번 전송하는 전체 데이터를 90112 바이트 배열로 선언
            int i = 0;  // 지역변수 i를 0으로 선언 및 초기화

            //bool flg_exist = false;

            try
            {
                int cnt_edge_detected = 0;  // cnt_edge_detected 변수 선언 및 0으로 할당
                foreach (TSignalUnit obj in __pRootView.m_sigSetting.m_lstSignals)  // m_sigSetting.m_lstSignals 리스트 반복
                {
                    if (__pRootView.__flgDataTransmit)  // 데이터 송신 상태가 true이면
                    {
                        if (obj.Checked1 == true)   //  CheckBox에 체크된 신호만
                        {
                            if (obj.EdgeDetected)//if (obj.SavedValue == true)  // 상승 엣지가 발생 시,
                            {
                                OnesigData = Enumerable.Repeat((byte)0x20, OnesigData.Length).ToArray();    // 한 Signal 데이터를 초기화
                                transactionCode = Encoding.UTF8.GetBytes("TC02");   // TC02 string을 byte로 형변환하여 transactionCode 바이트 배열에 할당
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
                            }
                        }

                        i++;

                        if (!__pRootView.PendingUpdate) // Form2의 PendingUpdate가 true가 아닐 때
                        {
                            if (i == __pRootView.m_sigSetting.m_lstSignals.Count && cnt_edge_detected > 0)   // cnt_edge_detected와 m_sigSetting.m_lstSignals.Count가 같고, cnt_edge_detected가 0보다 클 때
                            {
                                if (WholeData != null && __pRootView.tDriverSocket2.Activated)  // 전체 데이터가 null이 아니고 tDriverSocket2이 연결되어 있을 때
                                {
                                    //netStream.Write(WholeData, 0, WholeData.Length);  // 전체 데이터 전송
                                    __pRootView.tDriverSocket2.Write(WholeData);    // 전체 데이터 전송
                                    System.Diagnostics.Debug.WriteLine(WholeData.Length.ToString() + "Bytes sent complte(" + cnt_edge_detected.ToString() + ")");   // 전송한 신호의 수 Debug창에 띄움
                                    //__pRootView.m_SavedVaules.savedTextFileTC02();
                                }
                            }
                        }
                    }
                }

                flg_process = false;    // 변수 flg_process에 false 할당
            }
            catch (Exception except)    // 예외 처리
            {
                MessageBox.Show(except.Message);    // MessageBox에 예외 출력
            }

            return flg_process; // flg_process 반환
        }

        public bool __WriteData()   // 데이터 전송 메소드
        {
            bool flg_process = true;    // 지역변수 flg_process 선언 및 true로 초기화

            byte[] transactionCode = new byte[4];    //transactionCode를 4 바이트 배열로 선언
            byte[] dateTime = new byte[14];    // dateTime를 14 바이트 배열로 선언
            byte[] factoryCode = new byte[10];    // factoryCode를 10 바이트 배열로 선언
            byte[] alarmNumber = new byte[10]; // alarmNumber를 10 바이트 배열로 선언
            byte[] equipmentCode = new byte[10]; // equipmentCode를 10 바이트 배열로 선언
            byte[] alarmMessage = new byte[128]; // alarmMessage를 128 바이트 배열로 선언
            byte[] OnesigData = new byte[176];  // 한 Signal 데이터를 176 바이트 배열로 선언
            byte[] WholeData = new byte[90112]; // 한 번 전송하는 전체 데이터를 90112 바이트 배열로 선언
            int i = 0;  // 지역변수 i를 0으로 선언 및 초기화

            //bool flg_exist = false;

            try
            {
                short cnt_edge_detected = 0;  // cnt_edge_detected 변수 선언 및 0으로 할당
                short headerCode = 7777;
                foreach (TSignalUnit obj in __pRootView.m_sigSetting.m_lstSignals)  // m_sigSetting.m_lstSignals 리스트 반복
                {
                    if (__pRootView.__flgDataTransmit)  // 데이터 송신 상태가 true이면
                    {
                        if (obj.Checked1 == true)   //  CheckBox에 체크된 신호만
                        {
                            if (obj.EdgeDetected)//if (obj.SavedValue == true)  // 상승 엣지가 발생 시,
                            {
                                OnesigData = Enumerable.Repeat((byte)0x20, OnesigData.Length).ToArray();    // 한 Signal 데이터를 초기화
                                transactionCode = Encoding.UTF8.GetBytes("TC02");   // TC02 string을 byte로 형변환하여 transactionCode 바이트 배열에 할당
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
                                Buffer.BlockCopy(OnesigData, 0, WholeData, 176 * cnt_edge_detected, OnesigData.Length); // OnesigData를 OnesigData의 바이트 배열 크기만큼 WholeData 바이트 배열에 176 * cnt_edge_detected부터 복사

                                cnt_edge_detected++;    // cnt_edge_detected 1씩 증가
                            }
                        }

                        i++;

                        if (!__pRootView.PendingUpdate) // Form2의 PendingUpdate가 true가 아닐 때
                        {
                            if (i == __pRootView.m_sigSetting.m_lstSignals.Count && cnt_edge_detected > 0)   // cnt_edge_detected와 m_sigSetting.m_lstSignals.Count가 같고, cnt_edge_detected가 0보다 클 때
                            {
                                if (WholeData != null && __pRootView.tDriverSocket2.Activated)  // 전체 데이터가 null이 아니고 tDriverSocket2이 연결되어 있을 때
                                {
                                    byte[] s_buffer = new byte[176 * cnt_edge_detected + 4]; // 헤더 4Byte + 하나의 신호 데이터 * 데이터의 수의 길이를 가지는 배열 생성
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
                                }
                            }
                        }
                    }
                }

                flg_process = false;    // 변수 flg_process에 false 할당
            }
            catch (Exception except)    // 예외 처리
            {
                MessageBox.Show(except.Message);    // MessageBox에 예외 출력
            }

            return flg_process; // flg_process 반환
        }
    }
}
