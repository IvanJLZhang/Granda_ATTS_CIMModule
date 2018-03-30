namespace SecsClient
{
    partial class Client
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.btnSecSend = new System.Windows.Forms.Button();
            this.btnSendPrimaryMsg = new System.Windows.Forms.Button();
            this.primaryMsgToSend = new System.Windows.Forms.TextBox();
            this.recvdMessage = new System.Windows.Forms.TextBox();
            this.logMsg = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // btnSecSend
            // 
            this.btnSecSend.Location = new System.Drawing.Point(427, 417);
            this.btnSecSend.Name = "btnSecSend";
            this.btnSecSend.Size = new System.Drawing.Size(112, 52);
            this.btnSecSend.TabIndex = 8;
            this.btnSecSend.Text = "SendSec";
            this.btnSecSend.UseVisualStyleBackColor = true;
            this.btnSecSend.Click += new System.EventHandler(this.btnSecSend_Click);
            // 
            // btnSendPrimaryMsg
            // 
            this.btnSendPrimaryMsg.Location = new System.Drawing.Point(427, 111);
            this.btnSendPrimaryMsg.Name = "btnSendPrimaryMsg";
            this.btnSendPrimaryMsg.Size = new System.Drawing.Size(112, 52);
            this.btnSendPrimaryMsg.TabIndex = 7;
            this.btnSendPrimaryMsg.Text = "SendPri";
            this.btnSendPrimaryMsg.UseVisualStyleBackColor = true;
            this.btnSendPrimaryMsg.Click += new System.EventHandler(this.btnSendPrimaryMsg_Click);
            // 
            // primaryMsgToSend
            // 
            this.primaryMsgToSend.Location = new System.Drawing.Point(12, 12);
            this.primaryMsgToSend.Multiline = true;
            this.primaryMsgToSend.Name = "primaryMsgToSend";
            this.primaryMsgToSend.Size = new System.Drawing.Size(409, 528);
            this.primaryMsgToSend.TabIndex = 6;
            // 
            // recvdMessage
            // 
            this.recvdMessage.Location = new System.Drawing.Point(545, 12);
            this.recvdMessage.Multiline = true;
            this.recvdMessage.Name = "recvdMessage";
            this.recvdMessage.ReadOnly = true;
            this.recvdMessage.Size = new System.Drawing.Size(409, 528);
            this.recvdMessage.TabIndex = 5;
            // 
            // logMsg
            // 
            this.logMsg.Location = new System.Drawing.Point(12, 562);
            this.logMsg.Multiline = true;
            this.logMsg.Name = "logMsg";
            this.logMsg.ReadOnly = true;
            this.logMsg.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.logMsg.Size = new System.Drawing.Size(942, 204);
            this.logMsg.TabIndex = 9;
            // 
            // Client
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(966, 778);
            this.Controls.Add(this.logMsg);
            this.Controls.Add(this.btnSecSend);
            this.Controls.Add(this.btnSendPrimaryMsg);
            this.Controls.Add(this.primaryMsgToSend);
            this.Controls.Add(this.recvdMessage);
            this.Name = "Client";
            this.Text = "Client";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnSecSend;
        private System.Windows.Forms.Button btnSendPrimaryMsg;
        private System.Windows.Forms.TextBox primaryMsgToSend;
        private System.Windows.Forms.TextBox recvdMessage;
        private System.Windows.Forms.TextBox logMsg;
    }
}

