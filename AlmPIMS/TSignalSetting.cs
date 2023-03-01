using AlmPIMS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace AlmPIMS
{
    internal class TSignalSetting
    {
        private Form2 __pWndRoot = null;    // Form2를 타입으로 가지는 전역변수 __pRootView를 null로 선언 및 초기화

        private string __sIPAddress1 = string.Empty; // __sIPAddress 선언
        private string __sIPAddress2 = string.Empty; // __sIPAddress 선언
        private string __sIPAddress3 = string.Empty; // __sIPAddress 선언
        private int __nPort1 = 0;    // __nPort 선언
        private int __nPort2 = 0;    // __nPort 선언
        private int __nPort3 = 0;    // __nPort 선언
        private int __nModuleNum = 0; // __nModuleNum 선언
        private string __sFactoryCode = string.Empty;   // __sFactoryCode 선언
        private string __sSendTime = "070000";  // __sSendTime 선언

        public string IPAddress1 // IPAddress1 선언
        {
            get { return __sIPAddress1; }    // __sIPAddress1 읽기
            set { __sIPAddress1 = value; }   // __sIPAddress1 쓰기
        }

        public int Port1 // Port1 선언
        {
            get { return __nPort1; } // __nPort1 읽기
            set { __nPort1 = value; }    // __nPort1 쓰기
        }

        public string IPAddress2 // IPAddress2 선언
        {
            get { return __sIPAddress2; }    // __sIPAddress2 읽기
            set { __sIPAddress2 = value; }   // __sIPAddress2 쓰기
        }

        public int Port2 // Port 선언
        {
            get { return __nPort2; } // __nPort2 읽기
            set { __nPort2 = value; }    // __nPort2 쓰기
        }

        public string IPAddress3 // IPAddress2 선언
        {
            get { return __sIPAddress3; }    // __sIPAddress2 읽기
            set { __sIPAddress3 = value; }   // __sIPAddress2 쓰기
        }

        public int Port3 // Port 선언
        {
            get { return __nPort3; } // __nPort2 읽기
            set { __nPort3 = value; }    // __nPort2 쓰기
        }

        public int ModuleNum // ModuleNum 선언
        {
            get { return __nModuleNum; } // __nModuleNum 읽기
            set { __nModuleNum = value; }    // __nModuleNum 쓰기
        }

        public string FactoryCode   // FactoryCode 선언
        {
            get { return __sFactoryCode; }  // __sFactoryCode 읽기
            set { __sFactoryCode = value; } // __sFactoryCode 쓰기
        }

        public string SendTime   // SendTime 선언
        {
            get { return __sSendTime; }  // __sSendTime 읽기
            set { __sSendTime = value; } // __sSendTime 쓰기
        }

        public List<TSignalUnit> m_lstSignals = new List<TSignalUnit>();    // TSignalUnit 클래스를 타입으로 가지는 리스트 객체 생성

        public TSignalSetting(Form2 pWndRoot)   // Form2 클래스를 매개변수로 받는 클래스 생성자 생성
        {
            __pWndRoot = pWndRoot;  // 매개변수로 전달 받은 Form2 객체를 전역변수 __pRootView에 할당
        }

        public void InitContainer(List<TSignalUnit> lstUnits)   // m_lstSignals 객체 초기화하는 메소드
        {
            if (m_lstSignals.Count != 0)    // 객체의 크기가 0이 아닐 경우
            {
                List<TSignalUnit> lst_exists = new List<TSignalUnit>(); // TSignalUnit 클래스를 타입으로 가지는 리스트 객체 생성
                lst_exists.AddRange(m_lstSignals);  // lst_exists 리스트 객체에 기존의 m_lstSignals 객체를 할당
                m_lstSignals.Clear();   // m_lstSignals 객체 비우기
                m_lstSignals.AddRange(lstUnits);    // m_lstSignals 객체에 전달 받은 매개변수 lstUnits의 인자 추가

                if (lst_exists.Count > m_lstSignals.Count)  // 기존의 리스트 크기가 전달 받은 인자의 리스트 크기보다 클 경우
                {
                    for (int i = 0; i < m_lstSignals.Count; i++)    // 전달 받은 인자의 리스트 크기만큼 반복
                    {
                        m_lstSignals[i].Checked1 = lst_exists[i].Checked1;    // m_lstSignals Checked1에 lst_exists Checked1 할당
                        m_lstSignals[i].SavedValue = lst_exists[i].SavedValue;  // m_lstSignals SavedValue에 lst_exists SavedValue 할당
                    }
                }
                else    // 기존의 리스트 크기가 전달 받은 인자의 리스트 크기보다 작을 경우
                {
                    for (int i = 0; i < lst_exists.Count; i++)  // 기존의 리스트 크기만큼 반복
                    {
                        m_lstSignals[i].Checked1 = lst_exists[i].Checked1;   // m_lstSignals Checked1에 lst_exists Checked1 할당
                        m_lstSignals[i].SavedValue = lst_exists[i].SavedValue;  // m_lstSignals SavedValue에 lst_exists SavedValue 할당
                    }
                }
            }
            else m_lstSignals.AddRange(lstUnits);    // 기존의 m_lstSignals 객체의 크기가 0일 경우, 전달 받은 인자를 할당
        }
    }

    internal class TSignalUnit
    {
        #region Member variables
        private int __iId = 0;  // __iId 선언
        private bool __flgChecked1 = false;  // __flgChecked 선언
        private string __sExpression = string.Empty; // __sExpression 선언
        private string __sName = string.Empty;  // __sName 선언
        private string __sComment1 = string.Empty;  // __sComment1 선언
        private string __sComment2 = string.Empty;  // __sComment2 선언
        private bool __flgSavedValue = false;   // __flgSavedValue 선언
        private bool __flgEdgeDetected = false; // __flgEdgeDetected 선언
        #endregion Member variables

        #region Property

        public int Id    // Id 선언
        {
            get { return __iId; }    // __iId 읽기
            set { __iId = value; }   // __iId 쓰기
        }

        public bool Checked1   // Checked 선언
        {
            get { return __flgChecked1; }    // __flgChecked 읽기
            set { __flgChecked1 = value; }   // __flgChecked 쓰기
        }

        public string Expression   // Checked 선언
        {
            get { return __sExpression; }    // __sExpression 읽기
            set { __sExpression = value; }   // __sExpression 쓰기
        }

        public string Name  // Name 선언
        {
            get { return __sName; } // __sName 읽기
            set { __sName = value; }    // __sName 쓰기
        }

        public string Comment1  // Comment1 선언
        {
            get { return __sComment1; } // __sComment1 읽기
            set { __sComment1 = value; }    // __sComment1 쓰기
        }

        public string Comment2  // Comment2 선언
        {
            get { return __sComment2; } // __sComment2 읽기
            set { __sComment2 = value; }    // __sComment2 쓰기
        }

        public bool SavedValue  // SavedValue 선언
        {
            get { return __flgSavedValue; } // __flgSavedValue 읽기
            set
            {
                if (__flgSavedValue == false && value == true) __flgEdgeDetected = true;
                else __flgEdgeDetected = false;

                __flgSavedValue = value;
            }    // __flgSavedValue 쓰기
        }

        public bool EdgeDetected    // EdgeDetected 선언
        {
            get { return __flgEdgeDetected; }   // __flgEdgeDetected 읽기
            //private set { __flgEdgeDetected = value; }
        }
        #endregion Property
    }
}
