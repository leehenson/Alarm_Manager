using AlmPIMS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlmPIMS
{
    internal class TDataProcess
    {
        Form2 __pRootView = null;    // Form2를 타입으로 가지는 __pRootView 변수를 null로 선언
        byte[] __lstIOBytes = null; // 바이트 배열 __lstIOBytes 변수를 null로 선언
        byte[] __flgContainer = // byte를 bit로 매칭
        {
            0x01,   // 첫번째 bit 자리(16진수 1)
            0x02,   // 두번째 bit 자리(16진수 2)
            0x04,   // 세번째 bit 자리(16진수 4)
            0x08,   // 네번째 bit 자리(16진수 8)
            0x10,   // 다섯번째 bit 자리(16진수 16)
            0x20,   // 여섯번째 bit 자리(16진수 32)
            0x40,   // 일곱번째 bit 자리(16진수 64)
            0x80,   // 여덟번째 bit 자리(16진수 128)
        };
        //TSignalSetting m_sigSetting = null;

        public TDataProcess(Form2 pRootView)    // Form2 클래스를 매개변수로 받는 클래스 생성자 생성
        {
            __pRootView = pRootView;  // 매개변수로 전달 받은 Form2 객체를 전역변수 __pRootView에 할당
        }

        public void ProcessPackage(byte[] lpIOBytes)    // TCP Server로 통해 수신된 신호를 처리하는 메소드, 바이트 배열을 매개 변수를 받는 메소드
        {
            if (null != lpIOBytes && lpIOBytes.Any())   // 매개변수를 통해 전달 받은 데이터가 null이 아니고, 데이터가 존재할 때
            {
                __lstIOBytes = new byte[lpIOBytes.Length];  // 매개변수로 전달 받은 데이터 크기만큼의 크기를 갖는 바이트 배열 __lstIOBytes 이름으로 선언
                Buffer.BlockCopy(lpIOBytes, 0, __lstIOBytes, 0, __lstIOBytes.Length);   // 매개 변수로 전달 받은 데이터를 __lstIOBytes에 복사
            }

            if (null == lpIOBytes || !lpIOBytes.Any()) return;  // 매개변수를 통해 전달 받은 데이터가 null이거나, 데이터가 존재하지 않으면 반환

            Int32 n_value = BitConverter.ToInt32(__lstIOBytes, 0); // 바이트 배열 __lstIOBytes를 int형으로 형변환하여 n_value 변수에 담음

            List<bool> lst_values = new List<bool>();   // bool형의 데이터를 갖는 List 객체 생성

            // 바이트를 비트로 쪼개는 로직
            for (int j_cnt = 0; j_cnt < lpIOBytes.Length; j_cnt++)  // 매개 변수로 전달 받은 lpIOBytes 크기 만큼 반복
            {
                for (int i = 0; i < __flgContainer.Length; i++) // __figContainer 크기 만큼 반복
                {
                    int n_flag = lpIOBytes[j_cnt] & __flgContainer[i];  // 바이트당 비트를 n_flag 변수에 담음

                    lst_values.Add(n_flag == __flgContainer[i] ? true : false); // n_flag 와 __flgContainer가 같으면 true, 다르면 false를 lst_values에 추가
                }
            }
            //string s_log = string.Format(">>>{0}Bytes => '{1}'", __lstIOBytes.Length, n_value); // s_log 변수 string.Format으로 선언 및 할당
            //System.Diagnostics.Debug.WriteLine(s_log);  // s_log를 Debug창에 띄움
            //s_log = string.Empty;   // s_log 힙을 비움
            //System.Diagnostics.Debug.WriteLine("01234567"); // 비트 자릿수를 띄움
            for (int i = 0; i < lst_values.Count; i++)  // lst_vlues의 길이만큼 i를 반복
            {
                //s_log += string.Format("{0}", lst_values[i] ? '1' : '0');   // s_log에 string.Format으로 True이면 1, false이면 0으로 출력
                if (i != 0 && (i + 1) % 8 == 0) // i가 0이 아니고, i+1이 8로 나누었을 때 나머지가 0이면
                {
                    //System.Diagnostics.Debug.WriteLine(s_log);  // s_log를 Debug창에 띄움
                    //s_log = string.Empty;   // s_log 힙을 비움
                }
            }
             if (!__pRootView.__flginvokeStop) __pRootView.ConcreteInvoke(lst_values);  // Form2 __flginvokeStop이 true이면  lst_values 리스트를 인자로 Form2 ConcreteInvoke() 메소드 호출       
        }
    }
}

