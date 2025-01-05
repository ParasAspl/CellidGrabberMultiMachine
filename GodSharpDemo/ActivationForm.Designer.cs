
namespace CligenceCellIDGrabber
{
    partial class ActivationForm
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
            this.txtActivationKey = new MetroFramework.Controls.MetroTextBox();
          //  this.metroLabel1 = new MetroFramework.Controls.MetroLabel();
            this.btnActivate = new MetroFramework.Controls.MetroButton();
            this.SuspendLayout();
            // 
            // txtActivationKey
            // 
            // 
            // 
            // 
            this.txtActivationKey.CustomButton.Image = null;
            this.txtActivationKey.CustomButton.Location = new System.Drawing.Point(381, 1);
            this.txtActivationKey.CustomButton.Name = "";
            this.txtActivationKey.CustomButton.Size = new System.Drawing.Size(21, 21);
            this.txtActivationKey.CustomButton.Style = MetroFramework.MetroColorStyle.Blue;
            this.txtActivationKey.CustomButton.TabIndex = 1;
            this.txtActivationKey.CustomButton.Theme = MetroFramework.MetroThemeStyle.Light;
            this.txtActivationKey.CustomButton.UseSelectable = true;
            this.txtActivationKey.CustomButton.Visible = false;
            this.txtActivationKey.Lines = new string[0];
            this.txtActivationKey.Location = new System.Drawing.Point(41, 63);
            this.txtActivationKey.MaxLength = 32767;
            this.txtActivationKey.Name = "txtActivationKey";
            this.txtActivationKey.PasswordChar = '\0';
            this.txtActivationKey.PromptText = "Enter Activation Key";
            this.txtActivationKey.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.txtActivationKey.SelectedText = "";
            this.txtActivationKey.SelectionLength = 0;
            this.txtActivationKey.SelectionStart = 0;
            this.txtActivationKey.ShortcutsEnabled = true;
            this.txtActivationKey.Size = new System.Drawing.Size(403, 23);
            this.txtActivationKey.TabIndex = 0;
            this.txtActivationKey.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txtActivationKey.UseSelectable = true;
            this.txtActivationKey.WaterMark = "Enter Activation Key";
            this.txtActivationKey.WaterMarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(109)))), ((int)(((byte)(109)))));
            this.txtActivationKey.WaterMarkFont = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Pixel);
            // 
            // metroLabel1
            // 
            this.metroLabel1.FontSize = MetroFramework.MetroLabelSize.Tall;
            this.metroLabel1.FontWeight = MetroFramework.MetroLabelWeight.Bold;
            this.metroLabel1.ForeColor = System.Drawing.Color.OrangeRed;
            this.metroLabel1.Location = new System.Drawing.Point(205, 16);
            this.metroLabel1.Name = "metroLabel1";
            this.metroLabel1.Size = new System.Drawing.Size(100, 25);
            this.metroLabel1.TabIndex = 1;
            this.metroLabel1.Text = "Activation";
            this.metroLabel1.UseCustomBackColor = true;
            this.metroLabel1.UseCustomForeColor = true;
            this.metroLabel1.UseStyleColors = true;
            this.metroLabel1.WrapToLine = true;
            // 
            // btnActivate
            // 
            this.btnActivate.Location = new System.Drawing.Point(172, 102);
            this.btnActivate.Name = "btnActivate";
            this.btnActivate.Size = new System.Drawing.Size(146, 33);
            this.btnActivate.TabIndex = 2;
            this.btnActivate.Text = "Activate";
            this.btnActivate.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.btnActivate.UseCustomBackColor = true;
            this.btnActivate.UseCustomForeColor = true;
            this.btnActivate.UseSelectable = true;
            this.btnActivate.UseStyleColors = true;
            this.btnActivate.Click += new System.EventHandler(this.btnActivate_Click);
            // 
            // ActivationForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(483, 173);
            this.Controls.Add(this.btnActivate);
            this.Controls.Add(this.metroLabel1);
            this.Controls.Add(this.txtActivationKey);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ActivationForm";
            this.Padding = new System.Windows.Forms.Padding(23, 60, 23, 20);
            this.Resizable = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Style = MetroFramework.MetroColorStyle.Green;
            this.TextAlign = MetroFramework.Forms.MetroFormTextAlign.Center;
            this.Theme = MetroFramework.MetroThemeStyle.Default;
            this.TopMost = true;
            this.Load += new System.EventHandler(this.ActivationForm_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private MetroFramework.Controls.MetroTextBox txtActivationKey;
        private MetroFramework.Controls.MetroLabel metroLabel1;
        private MetroFramework.Controls.MetroButton btnActivate;
    }
}