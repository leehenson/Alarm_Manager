namespace AlmSimulator
{
    partial class Form1
    {
        /// <summary>
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 디자이너에서 생성한 코드

        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.tDriverSocket1 = new Delta.IODriver.TDriverSocket(this.components);
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // tDriverSocket1
            // 
            this.tDriverSocket1.BindAddress = "127.0.0.1";
            this.tDriverSocket1.BindPort = 7011;
            this.tDriverSocket1.FixedCapacity = 0;
            this.tDriverSocket1.PeerAddress = "127.0.0.1";
            this.tDriverSocket1.PeerPort = 7012;
            this.tDriverSocket1.SocketPackageStyle = Delta.IODriver.SocketPackageStyle.None;
            this.tDriverSocket1.SocketSessionStyle = Delta.IODriver.SocketSessionStyle.TcpClient;
            this.tDriverSocket1.Connected += new Delta.IODriver.TDriverSocketEventDispatcher(this.tDriverSocket1_Connected);
            this.tDriverSocket1.Received += new Delta.IODriver.TDriverSocketEventDispatcher(this.tDriverSocket1_Received);
            this.tDriverSocket1.Disconnected += new Delta.IODriver.TDriverSocketEventDispatcher(this.tDriverSocket1_Disconnected);
            this.tDriverSocket1.DeltaAlmInformation += new Delta.Common.TDeltaEventDispatcher(this.tDriverSocket1_DeltaAlmInformation);
            // 
            // timer1
            // 
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private Delta.IODriver.TDriverSocket tDriverSocket1;
        private System.Windows.Forms.Timer timer1;
    }
}

