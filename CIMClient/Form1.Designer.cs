namespace CIMClient
{
    partial class Form1
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
            this.components = new System.ComponentModel.Container();
            this.tb_ip = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.tb_port = new System.Windows.Forms.TextBox();
            this.lb_connectionState = new System.Windows.Forms.Label();
            this.tb_msg = new System.Windows.Forms.TextBox();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.clearToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.btn_start = new System.Windows.Forms.Button();
            this.rbtn_active = new System.Windows.Forms.RadioButton();
            this.rbtn_passive = new System.Windows.Forms.RadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btn_i_offline = new System.Windows.Forms.Button();
            this.btn_i_online = new System.Windows.Forms.Button();
            this.btn_i_datetime = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.btn_d_prr = new System.Windows.Forms.Button();
            this.btn_r_process_control = new System.Windows.Forms.Button();
            this.btn_rc_rcr = new System.Windows.Forms.Button();
            this.btn_e_send_display_message = new System.Windows.Forms.Button();
            this.btn_d_ecc = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.contextMenuStrip1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.groupBox7.SuspendLayout();
            this.SuspendLayout();
            // 
            // tb_ip
            // 
            this.tb_ip.Location = new System.Drawing.Point(93, 21);
            this.tb_ip.Name = "tb_ip";
            this.tb_ip.Size = new System.Drawing.Size(107, 21);
            this.tb_ip.TabIndex = 0;
            this.tb_ip.Text = "192.168.0.143";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(28, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(59, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "IPAddress";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(232, 24);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(29, 12);
            this.label2.TabIndex = 3;
            this.label2.Text = "Port";
            // 
            // tb_port
            // 
            this.tb_port.Location = new System.Drawing.Point(267, 21);
            this.tb_port.Name = "tb_port";
            this.tb_port.Size = new System.Drawing.Size(49, 21);
            this.tb_port.TabIndex = 2;
            this.tb_port.Text = "7000";
            // 
            // lb_connectionState
            // 
            this.lb_connectionState.AutoSize = true;
            this.lb_connectionState.Font = new System.Drawing.Font("Segoe UI Symbol", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb_connectionState.Location = new System.Drawing.Point(447, 21);
            this.lb_connectionState.Name = "lb_connectionState";
            this.lb_connectionState.Size = new System.Drawing.Size(74, 25);
            this.lb_connectionState.TabIndex = 4;
            this.lb_connectionState.Text = "Disable";
            // 
            // tb_msg
            // 
            this.tb_msg.ContextMenuStrip = this.contextMenuStrip1;
            this.tb_msg.Location = new System.Drawing.Point(15, 82);
            this.tb_msg.Multiline = true;
            this.tb_msg.Name = "tb_msg";
            this.tb_msg.ReadOnly = true;
            this.tb_msg.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tb_msg.Size = new System.Drawing.Size(764, 321);
            this.tb_msg.TabIndex = 5;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.clearToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(107, 26);
            // 
            // clearToolStripMenuItem
            // 
            this.clearToolStripMenuItem.Name = "clearToolStripMenuItem";
            this.clearToolStripMenuItem.Size = new System.Drawing.Size(106, 22);
            this.clearToolStripMenuItem.Text = "Clear";
            this.clearToolStripMenuItem.Click += new System.EventHandler(this.clearToolStripMenuItem_Click);
            // 
            // btn_start
            // 
            this.btn_start.Location = new System.Drawing.Point(591, 15);
            this.btn_start.Name = "btn_start";
            this.btn_start.Size = new System.Drawing.Size(158, 31);
            this.btn_start.TabIndex = 6;
            this.btn_start.Text = "START";
            this.btn_start.UseVisualStyleBackColor = true;
            this.btn_start.Click += new System.EventHandler(this.btn_start_Click);
            // 
            // rbtn_active
            // 
            this.rbtn_active.AutoSize = true;
            this.rbtn_active.Checked = true;
            this.rbtn_active.Location = new System.Drawing.Point(340, 11);
            this.rbtn_active.Name = "rbtn_active";
            this.rbtn_active.Size = new System.Drawing.Size(59, 16);
            this.rbtn_active.TabIndex = 7;
            this.rbtn_active.TabStop = true;
            this.rbtn_active.Text = "Active";
            this.rbtn_active.UseVisualStyleBackColor = true;
            // 
            // rbtn_passive
            // 
            this.rbtn_passive.AutoSize = true;
            this.rbtn_passive.Location = new System.Drawing.Point(340, 42);
            this.rbtn_passive.Name = "rbtn_passive";
            this.rbtn_passive.Size = new System.Drawing.Size(65, 16);
            this.rbtn_passive.TabIndex = 8;
            this.rbtn_passive.Text = "Passive";
            this.rbtn_passive.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btn_i_offline);
            this.groupBox1.Controls.Add(this.btn_i_online);
            this.groupBox1.Location = new System.Drawing.Point(15, 409);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(379, 83);
            this.groupBox1.TabIndex = 9;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Initialize";
            // 
            // btn_i_offline
            // 
            this.btn_i_offline.Location = new System.Drawing.Point(209, 26);
            this.btn_i_offline.Name = "btn_i_offline";
            this.btn_i_offline.Size = new System.Drawing.Size(158, 31);
            this.btn_i_offline.TabIndex = 1;
            this.btn_i_offline.Text = "Offline";
            this.toolTip1.SetToolTip(this.btn_i_offline, "Launch Offline Process");
            this.btn_i_offline.UseVisualStyleBackColor = true;
            this.btn_i_offline.Click += new System.EventHandler(this.btn_i_offline_Click);
            // 
            // btn_i_online
            // 
            this.btn_i_online.Location = new System.Drawing.Point(27, 26);
            this.btn_i_online.Name = "btn_i_online";
            this.btn_i_online.Size = new System.Drawing.Size(158, 31);
            this.btn_i_online.TabIndex = 0;
            this.btn_i_online.Text = "Online";
            this.toolTip1.SetToolTip(this.btn_i_online, "Launch Online Process");
            this.btn_i_online.UseVisualStyleBackColor = true;
            this.btn_i_online.Click += new System.EventHandler(this.btn_i_online_Click);
            // 
            // btn_i_datetime
            // 
            this.btn_i_datetime.Location = new System.Drawing.Point(27, 26);
            this.btn_i_datetime.Name = "btn_i_datetime";
            this.btn_i_datetime.Size = new System.Drawing.Size(158, 31);
            this.btn_i_datetime.TabIndex = 2;
            this.btn_i_datetime.Text = "DateTime";
            this.toolTip1.SetToolTip(this.btn_i_datetime, "Launch Request Date and Time Process");
            this.btn_i_datetime.UseVisualStyleBackColor = true;
            this.btn_i_datetime.Click += new System.EventHandler(this.btn_i_datetime_Click);
            // 
            // btn_d_prr
            // 
            this.btn_d_prr.Location = new System.Drawing.Point(27, 26);
            this.btn_d_prr.Name = "btn_d_prr";
            this.btn_d_prr.Size = new System.Drawing.Size(158, 31);
            this.btn_d_prr.TabIndex = 0;
            this.btn_d_prr.Text = "Process Result Report";
            this.toolTip1.SetToolTip(this.btn_d_prr, "Launch Process Result Report");
            this.btn_d_prr.UseVisualStyleBackColor = true;
            this.btn_d_prr.Click += new System.EventHandler(this.btn_d_prr_Click);
            // 
            // btn_r_process_control
            // 
            this.btn_r_process_control.Location = new System.Drawing.Point(27, 26);
            this.btn_r_process_control.Name = "btn_r_process_control";
            this.btn_r_process_control.Size = new System.Drawing.Size(158, 31);
            this.btn_r_process_control.TabIndex = 0;
            this.btn_r_process_control.Text = "Process Control Result";
            this.toolTip1.SetToolTip(this.btn_r_process_control, "Launch Process Control Report");
            this.btn_r_process_control.UseVisualStyleBackColor = true;
            this.btn_r_process_control.Click += new System.EventHandler(this.btn_r_process_control_Click);
            // 
            // btn_rc_rcr
            // 
            this.btn_rc_rcr.Location = new System.Drawing.Point(27, 26);
            this.btn_rc_rcr.Name = "btn_rc_rcr";
            this.btn_rc_rcr.Size = new System.Drawing.Size(158, 31);
            this.btn_rc_rcr.TabIndex = 0;
            this.btn_rc_rcr.Text = "Recipe Change Report";
            this.toolTip1.SetToolTip(this.btn_rc_rcr, "Launch Process Control Report");
            this.btn_rc_rcr.UseVisualStyleBackColor = true;
            this.btn_rc_rcr.Click += new System.EventHandler(this.btn_rc_rcr_Click);
            // 
            // btn_e_send_display_message
            // 
            this.btn_e_send_display_message.Location = new System.Drawing.Point(27, 26);
            this.btn_e_send_display_message.Name = "btn_e_send_display_message";
            this.btn_e_send_display_message.Size = new System.Drawing.Size(158, 31);
            this.btn_e_send_display_message.TabIndex = 0;
            this.btn_e_send_display_message.Text = "Send Display Message";
            this.toolTip1.SetToolTip(this.btn_e_send_display_message, "Launch Online Process");
            this.btn_e_send_display_message.UseVisualStyleBackColor = true;
            this.btn_e_send_display_message.Click += new System.EventHandler(this.btn_e_send_display_message_Click);
            // 
            // btn_d_ecc
            // 
            this.btn_d_ecc.Location = new System.Drawing.Point(206, 26);
            this.btn_d_ecc.Name = "btn_d_ecc";
            this.btn_d_ecc.Size = new System.Drawing.Size(158, 31);
            this.btn_d_ecc.TabIndex = 1;
            this.btn_d_ecc.Text = "EQT Constant Change";
            this.toolTip1.SetToolTip(this.btn_d_ecc, "Launch Process Result Report");
            this.btn_d_ecc.UseVisualStyleBackColor = true;
            this.btn_d_ecc.Click += new System.EventHandler(this.btn_d_ecc_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btn_d_ecc);
            this.groupBox2.Controls.Add(this.btn_d_prr);
            this.groupBox2.Location = new System.Drawing.Point(400, 409);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(379, 83);
            this.groupBox2.TabIndex = 10;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Data Collection";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.btn_r_process_control);
            this.groupBox3.Location = new System.Drawing.Point(15, 587);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(379, 83);
            this.groupBox3.TabIndex = 10;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Remote Control";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.btn_rc_rcr);
            this.groupBox4.Location = new System.Drawing.Point(400, 587);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(379, 83);
            this.groupBox4.TabIndex = 11;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Recipe Management";
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.btn_e_send_display_message);
            this.groupBox5.Location = new System.Drawing.Point(15, 498);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(379, 83);
            this.groupBox5.TabIndex = 10;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Equipment Terminal Service";
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.rbtn_passive);
            this.groupBox6.Controls.Add(this.tb_ip);
            this.groupBox6.Controls.Add(this.label1);
            this.groupBox6.Controls.Add(this.tb_port);
            this.groupBox6.Controls.Add(this.label2);
            this.groupBox6.Controls.Add(this.lb_connectionState);
            this.groupBox6.Controls.Add(this.rbtn_active);
            this.groupBox6.Controls.Add(this.btn_start);
            this.groupBox6.Location = new System.Drawing.Point(15, 12);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(765, 64);
            this.groupBox6.TabIndex = 12;
            this.groupBox6.TabStop = false;
            // 
            // groupBox7
            // 
            this.groupBox7.Controls.Add(this.btn_i_datetime);
            this.groupBox7.Location = new System.Drawing.Point(400, 498);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Size = new System.Drawing.Size(379, 83);
            this.groupBox7.TabIndex = 11;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "Clock";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(798, 685);
            this.Controls.Add(this.groupBox7);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.tb_msg);
            this.Controls.Add(this.groupBox6);
            this.Name = "Form1";
            this.Text = "Equipment";
            this.contextMenuStrip1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.groupBox5.ResumeLayout(false);
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            this.groupBox7.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tb_ip;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tb_port;
        private System.Windows.Forms.Label lb_connectionState;
        private System.Windows.Forms.TextBox tb_msg;
        private System.Windows.Forms.Button btn_start;
        private System.Windows.Forms.RadioButton rbtn_active;
        private System.Windows.Forms.RadioButton rbtn_passive;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btn_i_online;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Button btn_i_offline;
        private System.Windows.Forms.Button btn_i_datetime;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btn_d_prr;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button btn_r_process_control;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Button btn_rc_rcr;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.Button btn_e_send_display_message;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem clearToolStripMenuItem;
        private System.Windows.Forms.Button btn_d_ecc;
        private System.Windows.Forms.GroupBox groupBox7;
    }
}

