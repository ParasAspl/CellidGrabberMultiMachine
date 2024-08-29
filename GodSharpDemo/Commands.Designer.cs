
namespace CligenceCellIDGrabber
{
    partial class Commands
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.Timer timer;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Commands));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            this.metroPanel1 = new MetroFramework.Controls.MetroPanel();
            this.Progrsbr = new MetroFramework.Controls.MetroProgressBar();
            this.btnClear = new MetroFramework.Controls.MetroButton();
            this.cmbMode = new MetroFramework.Controls.MetroComboBox();
            this.lblMode = new MetroFramework.Controls.MetroLabel();
            this.btnSave = new MetroFramework.Controls.MetroButton();
            this.DdlMode = new MetroFramework.Controls.MetroComboBox();
            this.lblRegion = new MetroFramework.Controls.MetroLabel();
            this.lblStatus = new MetroFramework.Controls.MetroLabel();
            this.btnDisconnect = new MetroFramework.Controls.MetroButton();
            this.btnConnect = new MetroFramework.Controls.MetroButton();
            this.btnStop = new MetroFramework.Controls.MetroButton();
            this.btnStart = new MetroFramework.Controls.MetroButton();
            this.metroLabel2 = new MetroFramework.Controls.MetroLabel();
            this.metroLabel1 = new MetroFramework.Controls.MetroLabel();
            this.metroComboBox1 = new MetroFramework.Controls.MetroComboBox();
            this.metroPanel2 = new MetroFramework.Controls.MetroPanel();
            this.lblContactDetail = new MetroFramework.Controls.MetroLabel();
            this.lblDate = new MetroFramework.Controls.MetroLabel();
            this.lblApplicationVersion = new MetroFramework.Controls.MetroLabel();
            this.lblCompanyName = new MetroFramework.Controls.MetroLabel();
            this.serialPort2 = new System.IO.Ports.SerialPort(this.components);
            this.regionloader = new System.ComponentModel.BackgroundWorker();
            this.sfdExcel = new System.Windows.Forms.SaveFileDialog();
            this.loader = new System.Windows.Forms.PictureBox();
            this.bindingSource1 = new System.Windows.Forms.BindingSource(this.components);
            this.metroGrid1 = new MetroFramework.Controls.MetroGrid();
            this.lblmsg = new MetroFramework.Controls.MetroLabel();
            this.metroStyleManager1 = new MetroFramework.Components.MetroStyleManager(this.components);
            timer = new System.Windows.Forms.Timer(this.components);
            this.metroPanel1.SuspendLayout();
            this.metroPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.loader)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.metroGrid1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.metroStyleManager1)).BeginInit();
            this.SuspendLayout();
            // 
            // timer
            // 
            timer.Enabled = true;
            timer.Tick += new System.EventHandler(this.timer_Tick);
            // 
            // metroPanel1
            // 
            this.metroPanel1.AutoSize = true;
            this.metroPanel1.Controls.Add(this.Progrsbr);
            this.metroPanel1.Controls.Add(this.btnClear);
            this.metroPanel1.Controls.Add(this.cmbMode);
            this.metroPanel1.Controls.Add(this.lblMode);
            this.metroPanel1.Controls.Add(this.btnSave);
            this.metroPanel1.Controls.Add(this.DdlMode);
            this.metroPanel1.Controls.Add(this.lblRegion);
            this.metroPanel1.Controls.Add(this.lblStatus);
            this.metroPanel1.Controls.Add(this.btnDisconnect);
            this.metroPanel1.Controls.Add(this.btnConnect);
            this.metroPanel1.Controls.Add(this.btnStop);
            this.metroPanel1.Controls.Add(this.btnStart);
            this.metroPanel1.Controls.Add(this.metroLabel2);
            this.metroPanel1.Controls.Add(this.metroLabel1);
            this.metroPanel1.Controls.Add(this.metroComboBox1);
            this.metroPanel1.HorizontalScrollbarBarColor = true;
            this.metroPanel1.HorizontalScrollbarHighlightOnWheel = false;
            this.metroPanel1.HorizontalScrollbarSize = 10;
            this.metroPanel1.Location = new System.Drawing.Point(20, 96);
            this.metroPanel1.Name = "metroPanel1";
            this.metroPanel1.Size = new System.Drawing.Size(1247, 139);
            this.metroPanel1.TabIndex = 0;
            this.metroPanel1.VerticalScrollbarBarColor = true;
            this.metroPanel1.VerticalScrollbarHighlightOnWheel = false;
            this.metroPanel1.VerticalScrollbarSize = 10;
            // 
            // Progrsbr
            // 
            this.Progrsbr.HideProgressText = false;
            this.Progrsbr.Location = new System.Drawing.Point(3, 106);
            this.Progrsbr.Name = "Progrsbr";
            this.Progrsbr.Size = new System.Drawing.Size(1227, 23);
            this.Progrsbr.TabIndex = 17;
            this.Progrsbr.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnClear
            // 
            this.btnClear.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(174)))), ((int)(((byte)(219)))));
            this.btnClear.ForeColor = System.Drawing.Color.White;
            this.btnClear.Location = new System.Drawing.Point(593, 10);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(100, 34);
            this.btnClear.TabIndex = 12;
            this.btnClear.Text = "CLEAR";
            this.btnClear.UseCustomBackColor = true;
            this.btnClear.UseCustomForeColor = true;
            this.btnClear.UseSelectable = true;
            this.btnClear.UseStyleColors = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // cmbMode
            // 
            this.cmbMode.Enabled = false;
            this.cmbMode.FormattingEnabled = true;
            this.cmbMode.ItemHeight = 23;
            this.cmbMode.Items.AddRange(new object[] {
            "Fast",
            "Deep"});
            this.cmbMode.Location = new System.Drawing.Point(317, 66);
            this.cmbMode.Name = "cmbMode";
            this.cmbMode.Size = new System.Drawing.Size(93, 29);
            this.cmbMode.TabIndex = 11;
            this.cmbMode.UseSelectable = true;
            this.cmbMode.SelectedIndexChanged += new System.EventHandler(this.cmbMode_SelectedIndexChanged);
            // 
            // lblMode
            // 
            this.lblMode.AutoSize = true;
            this.lblMode.Location = new System.Drawing.Point(226, 76);
            this.lblMode.Name = "lblMode";
            this.lblMode.Size = new System.Drawing.Size(85, 19);
            this.lblMode.TabIndex = 10;
            this.lblMode.Text = "Select Mode:";
            // 
            // btnSave
            // 
            this.btnSave.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(174)))), ((int)(((byte)(219)))));
            this.btnSave.ForeColor = System.Drawing.Color.White;
            this.btnSave.Location = new System.Drawing.Point(709, 10);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(100, 34);
            this.btnSave.TabIndex = 9;
            this.btnSave.Text = "SAVE";
            this.btnSave.UseCustomBackColor = true;
            this.btnSave.UseCustomForeColor = true;
            this.btnSave.UseSelectable = true;
            this.btnSave.UseStyleColors = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // DdlMode
            // 
            this.DdlMode.Enabled = false;
            this.DdlMode.FormattingEnabled = true;
            this.DdlMode.ItemHeight = 23;
            this.DdlMode.Location = new System.Drawing.Point(86, 66);
            this.DdlMode.Name = "DdlMode";
            this.DdlMode.Size = new System.Drawing.Size(134, 29);
            this.DdlMode.TabIndex = 8;
            this.DdlMode.UseSelectable = true;
            this.DdlMode.SelectedIndexChanged += new System.EventHandler(this.DdlMode_SelectedIndexChanged);
            // 
            // lblRegion
            // 
            this.lblRegion.AutoSize = true;
            this.lblRegion.Location = new System.Drawing.Point(308, 17);
            this.lblRegion.Name = "lblRegion";
            this.lblRegion.Size = new System.Drawing.Size(57, 19);
            this.lblRegion.TabIndex = 7;
            this.lblRegion.Text = "Region :";
            this.lblRegion.Visible = false;
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new System.Drawing.Point(148, 17);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(131, 19);
            this.lblStatus.TabIndex = 6;
            this.lblStatus.Text = "Status : Disconnected";
            // 
            // btnDisconnect
            // 
            this.btnDisconnect.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(174)))), ((int)(((byte)(219)))));
            this.btnDisconnect.ForeColor = System.Drawing.Color.White;
            this.btnDisconnect.Location = new System.Drawing.Point(16, 10);
            this.btnDisconnect.Name = "btnDisconnect";
            this.btnDisconnect.Size = new System.Drawing.Size(100, 34);
            this.btnDisconnect.TabIndex = 4;
            this.btnDisconnect.Text = "DISCONNECT";
            this.btnDisconnect.UseCustomBackColor = true;
            this.btnDisconnect.UseCustomForeColor = true;
            this.btnDisconnect.UseSelectable = true;
            this.btnDisconnect.Click += new System.EventHandler(this.btnDisconnect_Click);
            // 
            // btnConnect
            // 
            this.btnConnect.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(174)))), ((int)(((byte)(219)))));
            this.btnConnect.ForeColor = System.Drawing.Color.White;
            this.btnConnect.Location = new System.Drawing.Point(16, 10);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(100, 34);
            this.btnConnect.TabIndex = 4;
            this.btnConnect.Text = "CONNECT";
            this.btnConnect.UseCustomBackColor = true;
            this.btnConnect.UseCustomForeColor = true;
            this.btnConnect.UseSelectable = true;
            this.btnConnect.UseStyleColors = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // btnStop
            // 
            this.btnStop.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(174)))), ((int)(((byte)(219)))));
            this.btnStop.ForeColor = System.Drawing.Color.White;
            this.btnStop.Location = new System.Drawing.Point(709, 66);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(100, 34);
            this.btnStop.TabIndex = 4;
            this.btnStop.Text = "STOP";
            this.btnStop.UseCustomBackColor = true;
            this.btnStop.UseCustomForeColor = true;
            this.btnStop.UseSelectable = true;
            this.btnStop.UseStyleColors = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // btnStart
            // 
            this.btnStart.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(174)))), ((int)(((byte)(219)))));
            this.btnStart.Enabled = false;
            this.btnStart.ForeColor = System.Drawing.Color.White;
            this.btnStart.Location = new System.Drawing.Point(709, 66);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(100, 34);
            this.btnStart.TabIndex = 4;
            this.btnStart.Text = "START";
            this.btnStart.UseCustomBackColor = true;
            this.btnStart.UseCustomForeColor = true;
            this.btnStart.UseSelectable = true;
            this.btnStart.UseStyleColors = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // metroLabel2
            // 
            this.metroLabel2.AutoSize = true;
            this.metroLabel2.Location = new System.Drawing.Point(429, 76);
            this.metroLabel2.Name = "metroLabel2";
            this.metroLabel2.Size = new System.Drawing.Size(130, 19);
            this.metroLabel2.TabIndex = 3;
            this.metroLabel2.Text = "Select Network Type:";
            // 
            // metroLabel1
            // 
            this.metroLabel1.AutoSize = true;
            this.metroLabel1.Location = new System.Drawing.Point(3, 76);
            this.metroLabel1.Name = "metroLabel1";
            this.metroLabel1.Size = new System.Drawing.Size(77, 19);
            this.metroLabel1.TabIndex = 3;
            this.metroLabel1.Text = "Select Type:";
            // 
            // metroComboBox1
            // 
            this.metroComboBox1.Enabled = false;
            this.metroComboBox1.FormattingEnabled = true;
            this.metroComboBox1.ItemHeight = 23;
            this.metroComboBox1.Location = new System.Drawing.Point(565, 66);
            this.metroComboBox1.Name = "metroComboBox1";
            this.metroComboBox1.Size = new System.Drawing.Size(128, 29);
            this.metroComboBox1.TabIndex = 2;
            this.metroComboBox1.UseSelectable = true;
            this.metroComboBox1.SelectionChangeCommitted += new System.EventHandler(this.metroComboBox1_SelectionChangeCommitted);
            // 
            // metroPanel2
            // 
            this.metroPanel2.AutoSize = true;
            this.metroPanel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(174)))), ((int)(((byte)(219)))));
            this.metroPanel2.Controls.Add(this.lblContactDetail);
            this.metroPanel2.Controls.Add(this.lblDate);
            this.metroPanel2.Controls.Add(this.lblApplicationVersion);
            this.metroPanel2.Controls.Add(this.lblCompanyName);
            this.metroPanel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.metroPanel2.HorizontalScrollbarBarColor = true;
            this.metroPanel2.HorizontalScrollbarHighlightOnWheel = false;
            this.metroPanel2.HorizontalScrollbarSize = 10;
            this.metroPanel2.Location = new System.Drawing.Point(20, 60);
            this.metroPanel2.Name = "metroPanel2";
            this.metroPanel2.Size = new System.Drawing.Size(1247, 36);
            this.metroPanel2.TabIndex = 2;
            this.metroPanel2.UseCustomBackColor = true;
            this.metroPanel2.UseCustomForeColor = true;
            this.metroPanel2.UseStyleColors = true;
            this.metroPanel2.VerticalScrollbarBarColor = true;
            this.metroPanel2.VerticalScrollbarHighlightOnWheel = false;
            this.metroPanel2.VerticalScrollbarSize = 10;
            // 
            // lblContactDetail
            // 
            this.lblContactDetail.FontSize = MetroFramework.MetroLabelSize.Tall;
            this.lblContactDetail.FontWeight = MetroFramework.MetroLabelWeight.Bold;
            this.lblContactDetail.ForeColor = System.Drawing.Color.White;
            this.lblContactDetail.Location = new System.Drawing.Point(1027, 0);
            this.lblContactDetail.Name = "lblContactDetail";
            this.lblContactDetail.Size = new System.Drawing.Size(200, 33);
            this.lblContactDetail.TabIndex = 10;
            this.lblContactDetail.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lblContactDetail.UseCustomBackColor = true;
            this.lblContactDetail.UseCustomForeColor = true;
            this.lblContactDetail.UseStyleColors = true;
            this.lblContactDetail.WrapToLine = true;
            // 
            // lblDate
            // 
            this.lblDate.FontSize = MetroFramework.MetroLabelSize.Tall;
            this.lblDate.FontWeight = MetroFramework.MetroLabelWeight.Bold;
            this.lblDate.ForeColor = System.Drawing.Color.White;
            this.lblDate.Location = new System.Drawing.Point(623, 0);
            this.lblDate.Name = "lblDate";
            this.lblDate.Size = new System.Drawing.Size(200, 33);
            this.lblDate.TabIndex = 9;
            this.lblDate.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblDate.UseCustomBackColor = true;
            this.lblDate.UseCustomForeColor = true;
            this.lblDate.UseStyleColors = true;
            this.lblDate.WrapToLine = true;
            // 
            // lblApplicationVersion
            // 
            this.lblApplicationVersion.FontSize = MetroFramework.MetroLabelSize.Tall;
            this.lblApplicationVersion.FontWeight = MetroFramework.MetroLabelWeight.Bold;
            this.lblApplicationVersion.ForeColor = System.Drawing.Color.White;
            this.lblApplicationVersion.Location = new System.Drawing.Point(344, 3);
            this.lblApplicationVersion.Name = "lblApplicationVersion";
            this.lblApplicationVersion.Size = new System.Drawing.Size(172, 33);
            this.lblApplicationVersion.TabIndex = 8;
            this.lblApplicationVersion.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblApplicationVersion.UseCustomBackColor = true;
            this.lblApplicationVersion.UseCustomForeColor = true;
            this.lblApplicationVersion.UseStyleColors = true;
            this.lblApplicationVersion.WrapToLine = true;
            // 
            // lblCompanyName
            // 
            this.lblCompanyName.FontSize = MetroFramework.MetroLabelSize.Tall;
            this.lblCompanyName.FontWeight = MetroFramework.MetroLabelWeight.Bold;
            this.lblCompanyName.ForeColor = System.Drawing.Color.White;
            this.lblCompanyName.Location = new System.Drawing.Point(3, 0);
            this.lblCompanyName.Name = "lblCompanyName";
            this.lblCompanyName.Size = new System.Drawing.Size(200, 33);
            this.lblCompanyName.TabIndex = 7;
            this.lblCompanyName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblCompanyName.UseCustomBackColor = true;
            this.lblCompanyName.UseCustomForeColor = true;
            this.lblCompanyName.UseStyleColors = true;
            this.lblCompanyName.WrapToLine = true;
            // 
            // serialPort2
            // 
            this.serialPort2.BaudRate = 115200;
            this.serialPort2.PortName = "None";
            this.serialPort2.ErrorReceived += new System.IO.Ports.SerialErrorReceivedEventHandler(this.serialPort2_ErrorReceived);
            this.serialPort2.PinChanged += new System.IO.Ports.SerialPinChangedEventHandler(this.serialPort2_PinChanged);
            this.serialPort2.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(this.serialPort2_DataReceived);
            // 
            // regionloader
            // 
            this.regionloader.WorkerReportsProgress = true;
            this.regionloader.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
            this.regionloader.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.regionloader_ProgressChanged);
            this.regionloader.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.regionloader_RunWorkerCompleted);
            // 
            // sfdExcel
            // 
            this.sfdExcel.DefaultExt = "Browse  File Location";
            this.sfdExcel.Filter = "Excel files (*.xlsx)|*.xlsx";
            this.sfdExcel.RestoreDirectory = true;
            // 
            // loader
            // 
            this.loader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(174)))), ((int)(((byte)(219)))));
            this.loader.ErrorImage = ((System.Drawing.Image)(resources.GetObject("loader.ErrorImage")));
            this.loader.Image = ((System.Drawing.Image)(resources.GetObject("loader.Image")));
            this.loader.Location = new System.Drawing.Point(480, 348);
            this.loader.Name = "loader";
            this.loader.Size = new System.Drawing.Size(140, 131);
            this.loader.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.loader.TabIndex = 10;
            this.loader.TabStop = false;
            this.loader.Visible = false;
            // 
            // metroGrid1
            // 
            this.metroGrid1.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(174)))), ((int)(((byte)(219)))));
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(174)))), ((int)(((byte)(219)))));
            this.metroGrid1.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.metroGrid1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.metroGrid1.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.DisplayedCellsExceptHeaders;
            this.metroGrid1.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.metroGrid1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.metroGrid1.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.metroGrid1.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(174)))), ((int)(((byte)(219)))));
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(198)))), ((int)(((byte)(247)))));
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(17)))), ((int)(((byte)(17)))));
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.metroGrid1.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.metroGrid1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(136)))), ((int)(((byte)(136)))), ((int)(((byte)(136)))));
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(198)))), ((int)(((byte)(247)))));
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(17)))), ((int)(((byte)(17)))));
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.metroGrid1.DefaultCellStyle = dataGridViewCellStyle3;
            this.metroGrid1.EnableHeadersVisualStyles = false;
            this.metroGrid1.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.metroGrid1.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.metroGrid1.Location = new System.Drawing.Point(20, 262);
            this.metroGrid1.Margin = new System.Windows.Forms.Padding(3, 0, 3, 3);
            this.metroGrid1.Name = "metroGrid1";
            this.metroGrid1.ReadOnly = true;
            this.metroGrid1.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(174)))), ((int)(((byte)(219)))));
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            dataGridViewCellStyle4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(198)))), ((int)(((byte)(247)))));
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(17)))), ((int)(((byte)(17)))));
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.metroGrid1.RowHeadersDefaultCellStyle = dataGridViewCellStyle4;
            this.metroGrid1.RowHeadersWidth = 62;
            this.metroGrid1.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            dataGridViewCellStyle5.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle5.ForeColor = System.Drawing.Color.White;
            this.metroGrid1.RowsDefaultCellStyle = dataGridViewCellStyle5;
            this.metroGrid1.RowTemplate.DefaultCellStyle.BackColor = System.Drawing.Color.White;
            this.metroGrid1.RowTemplate.DefaultCellStyle.ForeColor = System.Drawing.Color.Black;
            this.metroGrid1.RowTemplate.DividerHeight = 1;
            this.metroGrid1.RowTemplate.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.metroGrid1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.metroGrid1.Size = new System.Drawing.Size(1264, 382);
            this.metroGrid1.StandardTab = true;
            this.metroGrid1.Style = MetroFramework.MetroColorStyle.Blue;
            this.metroGrid1.TabIndex = 1;
            this.metroGrid1.UseCustomBackColor = true;
            this.metroGrid1.UseCustomForeColor = true;
            this.metroGrid1.UseStyleColors = true;
            this.metroGrid1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.metroGrid1_KeyDown);
            // 
            // lblmsg
            // 
            this.lblmsg.AutoSize = true;
            this.lblmsg.BackColor = System.Drawing.Color.Transparent;
            this.lblmsg.Enabled = false;
            this.lblmsg.FontWeight = MetroFramework.MetroLabelWeight.Regular;
            this.lblmsg.ForeColor = System.Drawing.Color.Black;
            this.lblmsg.Location = new System.Drawing.Point(36, 240);
            this.lblmsg.Margin = new System.Windows.Forms.Padding(3, 3, 3, 3);
            this.lblmsg.Name = "lblmsg";
            this.lblmsg.Size = new System.Drawing.Size(89, 19);
            this.lblmsg.TabIndex = 15;
            this.lblmsg.Text = "lblmsg fsdfsd";
            this.lblmsg.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.lblmsg.UseCustomBackColor = true;
            this.lblmsg.UseCustomForeColor = true;
            this.lblmsg.UseStyleColors = true;
            this.lblmsg.WrapToLine = true;
            // 
            // metroStyleManager1
            // 
            this.metroStyleManager1.Owner = null;
            // 
            // Commands
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.BorderStyle = MetroFramework.Forms.MetroFormBorderStyle.FixedSingle;
            this.ClientSize = new System.Drawing.Size(1287, 512);
            this.Controls.Add(this.lblmsg);
            this.Controls.Add(this.loader);
            this.Controls.Add(this.metroPanel2);
            this.Controls.Add(this.metroPanel1);
            this.Controls.Add(this.metroGrid1);
            this.Name = "Commands";
            this.ShowIcon = false;
            this.Text = "Cligence Cell ID Grabber";
            this.TransparencyKey = System.Drawing.Color.Red;
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Dashboard5G_FormClosing);
            this.Load += new System.EventHandler(this.Dashboard5G_Load);
            this.metroPanel1.ResumeLayout(false);
            this.metroPanel1.PerformLayout();
            this.metroPanel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.loader)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.metroGrid1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.metroStyleManager1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private MetroFramework.Controls.MetroPanel metroPanel1;
        private MetroFramework.Controls.MetroLabel metroLabel1;
        private MetroFramework.Controls.MetroButton btnDisconnect;
        private MetroFramework.Controls.MetroButton btnConnect;
        private MetroFramework.Controls.MetroButton btnStop;
        private MetroFramework.Controls.MetroButton btnStart;
        private MetroFramework.Controls.MetroLabel metroLabel2;
        private MetroFramework.Controls.MetroComboBox metroComboBox1;
        private MetroFramework.Controls.MetroPanel metroPanel2;
        private System.IO.Ports.SerialPort serialPort2;
        private System.Windows.Forms.BindingSource bindingSource1;
        private System.ComponentModel.BackgroundWorker regionloader;
        private MetroFramework.Controls.MetroLabel lblRegion;
        private MetroFramework.Controls.MetroLabel lblStatus;
        private MetroFramework.Controls.MetroComboBox DdlMode;
        private MetroFramework.Controls.MetroButton btnSave;
        private System.Windows.Forms.PictureBox loader;
        private System.Windows.Forms.SaveFileDialog sfdExcel;
        private MetroFramework.Controls.MetroLabel lblMode;
        private MetroFramework.Controls.MetroComboBox cmbMode;
        private MetroFramework.Controls.MetroButton btnClear;
        private MetroFramework.Controls.MetroGrid metroGrid1;
        private MetroFramework.Controls.MetroLabel lblCompanyName;
        private MetroFramework.Controls.MetroLabel lblApplicationVersion;
        private MetroFramework.Controls.MetroLabel lblDate;
        private MetroFramework.Controls.MetroLabel lblContactDetail;
        private MetroFramework.Controls.MetroLabel lblmsg;
        private MetroFramework.Controls.MetroProgressBar Progrsbr;
        private MetroFramework.Components.MetroStyleManager metroStyleManager1;
    }
}