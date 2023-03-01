using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace AlmPIMS
{
    internal class TDataGirdSetting
    {
        private Form2 __pRootView = null;   // 전역변수 __pRootView null로 선언 및 할당

        public TDataGirdSetting(Form2 pRootView)    // Form2 클래스를 매개변수로 받는 클래스 생성자 생성
        {
            __pRootView = pRootView;    // 매개변수로 전달 받은 Form2 객체를 전역변수 __pRootView에 할당
        }

        public void DataGridSetting(string path)
        {
            try
            {
                //lock (__pRootView.m_lckSigSetting)  // 특정 블럭의 코드(Critical Section)를 한번에 하나의 쓰레드만 실행할 수 있도록 함.
                {
                    System.Diagnostics.Debug.WriteLine("Hello");
                    XmlDocument xml = new XmlDocument();    // Xml 파일을 읽기위한 모듈

                    xml.Load(path);    // 읽을 file 설정

                    XmlNodeList xnList = xml.SelectNodes("/IOConfiguration/Modules/Module/ModuleNr[text()='" + __pRootView.m_sigSetting.ModuleNum + "']/../Links/Link/Digital/Signal");  // 읽은 Xml에서 필요한 노드를 위한 필터링

                    if (xnList != null) // xnList 변수가 null이 아니면
                    {
                        List<TSignalUnit> lst_units = new List<TSignalUnit>();  // TSignalUnit 클래스를 데이터형으로 가지는 List 인스턴스 생성
                        int n_index = 0;
                        foreach (XmlNode xn in xnList)  // 필요한 노드의 정보를 반복하며 찾기
                        {
                            if (xn["Name"].InnerText != string.Empty)
                            {
                                TSignalUnit obj = new TSignalUnit();    // TSignalUnit 클래스 obj 변수에 담음.

                                obj.Id = n_index++;
                                obj.Name = xn["Name"].InnerText;   // obj의 Name은 XML Name 노드의 이너 텍스트를 가짐.
                                obj.Expression = xn["Expression"].InnerText; //// obj의 Expression은 XML Expression 노드의 이너 텍스트를 가짐.
                                obj.Comment1 = xn["Comment1"].InnerText;   // obj의 Comment1은 XML Comment1 노드의 이너 텍스트를 가짐.
                                obj.Comment2 = xn["Comment2"].InnerText;   // obj의 Comment2는 XML Comment2 노드의 이너 텍스트를 가짐.

                                lst_units.Add(obj); // TSignalUnit에 리스트로 obj 담음.
                            }
                        }
                        __pRootView.m_sigSetting.InitContainer(lst_units);  // lst_units 리스트를 인자로 m_sigSetting.InitContainer 매개변수로 전달함.
                    }
                }

                //lock (__pRootView.m_lckSigSetting)
                {

                    __pRootView.dataGridView1.Invoke((MethodInvoker)delegate { __pRootView.dataGridView1.Rows.Clear(); });  // dataGridView1 모든 열 지우기

                    //__pRootView.dataGridView1.Rows.Clear(); // dataGridView1 모든 열 지우기
                    //__pRootView.dataGridView1.Refresh();    // dataGridView1 새로고침

                    foreach (TSignalUnit obj in __pRootView.m_sigSetting.m_lstSignals)  // m_sigSetting 인스턴스의 m_lstSignals의 인스턴스를 obj 변수에 담음
                    {
                        if (__pRootView.InvokeRequired)
                        {
                            __pRootView.Invoke(new MethodInvoker(delegate ()
                            {
                                __pRootView.dataGridView1.Rows.Add(obj.Id, obj.Checked1, obj.Expression, obj.Name, obj.Comment1, obj.Comment2);  // obj 변수에 담긴 프로퍼티 데이터를 dataGridView Rows에 할당
                            }));
                        }
                        else
                        {
                            __pRootView.dataGridView1.Rows.Add(obj.Id, obj.Checked1, obj.Expression, obj.Name, obj.Comment1, obj.Comment2);  // obj 변수에 담긴 프로퍼티 데이터를 dataGridView Rows에 할당
                        }
                        //__UpdateAlmBackColor(obj.Id, obj.SavedValue);   // obj.Id와 obj.SavedValue를 인자로 전달
                    }
                    System.Diagnostics.Debug.WriteLine("Hello");

                }
            }
            catch (Exception ex)    // 예외처리
            {
                MessageBox.Show("XML 문제 발생\r\n" + ex);  // MessageBox로 예외처리된 에러 디스플레이
            }
        }

        public void __UpdateAlmBackColor(int nIDX, bool flgValue)
        {
            if (flgValue == true)   // lstValues가 ture이면
            {
                __pRootView.dataGridView1.Invoke((MethodInvoker)delegate { __pRootView.dataGridView1.Rows[nIDX].DefaultCellStyle.ForeColor = Color.White; });   // 글자색 흰색
                __pRootView.dataGridView1.Invoke((MethodInvoker)delegate { __pRootView.dataGridView1.Rows[nIDX].DefaultCellStyle.BackColor = Color.Firebrick; });   // 배경색 빨간색

                //__pRootView.dataGridView1.Rows[nIDX].DefaultCellStyle.ForeColor = Color.White;  // 글자색 흰색
                //__pRootView.dataGridView1.Rows[nIDX].DefaultCellStyle.BackColor = Color.Firebrick;  // 배경색 빨간색
            }
            else    // 그 외(lstValues가 false)이면 
            {
                __pRootView.dataGridView1.Invoke((MethodInvoker)delegate { __pRootView.dataGridView1.Rows[nIDX].DefaultCellStyle.ForeColor = Color.Black; });   // 글자색 검정색
                __pRootView.dataGridView1.Invoke((MethodInvoker)delegate { __pRootView.dataGridView1.Rows[nIDX].DefaultCellStyle.BackColor = Color.White; });   // 배경색 흰색

                //__pRootView.dataGridView1.Rows[nIDX].DefaultCellStyle.ForeColor = Color.Black;  // 글자색 검정색
                //__pRootView.dataGridView1.Rows[nIDX].DefaultCellStyle.BackColor = Color.White;  // 배경색 흰색
            }
        }
    }


}
