using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AlmSimulator
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //tDriverSocket1.PeerAddress = textBox3.Text;
            //tDriverSocket1.PeerPort = Int32.Parse(textBox4.Text);
            tDriverSocket1.PeerAddress = "127.0.0.1";   // 연결할 PIMS Alarm Manager IP 주소
            tDriverSocket1.PeerPort = 5012;   // 연결할 PIMS Alarm Manager Port 번호
            tDriverSocket1.SocketSessionStyle = Delta.IODriver.SocketSessionStyle.TcpClient;    // 소켓을 TCP Client로 생성
            //tDriverSocket2.SocketPackageStyle = Delta.IODriver.SocketPackageStyle.Fixed;
            //tDriverSocket1.FixedCapacity = 4;
            tDriverSocket1.SocketPackageStyle = Delta.IODriver.SocketPackageStyle.None;

            tDriverSocket1.ActivateDriver(null);

            timer1.Start();
        }

        private void tDriverSocket1_Connected(object sender, Delta.IODriver.TDriverSocketEventArgs pevent)
        {

        }

        private void tDriverSocket1_DeltaAlmInformation(object sender, Delta.Common.TDeltaEventArgs pevent)
        {

        }

        private void tDriverSocket1_Disconnected(object sender, Delta.IODriver.TDriverSocketEventArgs pevent)
        {

        }

        private void tDriverSocket1_Received(object sender, Delta.IODriver.TDriverSocketEventArgs pevent)
        {

        }

        static int idx = 0;
        private void timer1_Tick(object sender, EventArgs e)
        {
            byte[] lp_bytes = new byte[64];

            lp_bytes[idx % lp_bytes.Length] = 0xFF;
            idx++;
            if (idx == lp_bytes.Length) idx = 0;
            lp_bytes[0] = 0xFF;

            int ret_process = tDriverSocket1.Write(lp_bytes);

            if (ret_process == lp_bytes.Length) System.Diagnostics.Debug.WriteLine("Wrike OK");
            else System.Diagnostics.Debug.WriteLine("Wrike NG");
        }
    }
}
