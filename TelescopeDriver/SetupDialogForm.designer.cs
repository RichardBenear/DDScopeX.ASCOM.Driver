using ASCOM.Utilities;
using System;
using System.Windows.Forms;

namespace ASCOM.DDScopeX.Telescope
{
  partial class SetupDialogForm
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SetupDialogForm));
      this.buttonCmdOK = new System.Windows.Forms.Button();
      this.buttonCmdCancel = new System.Windows.Forms.Button();
      this.picASCOM = new System.Windows.Forms.PictureBox();
      this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
      this.directoryEntry1 = new System.DirectoryServices.DirectoryEntry();
      this.checkBoxTrace = new System.Windows.Forms.CheckBox();
      this.checkBoxSendDateTime = new System.Windows.Forms.CheckBox();
      this.labelPort = new System.Windows.Forms.Label();
      this.panelSiteInformation = new System.Windows.Forms.Panel();
      this.checkBoxSendLocationThisLocation = new System.Windows.Forms.CheckBox();
      this.label1 = new System.Windows.Forms.Label();
      this.textBoxElevation = new System.Windows.Forms.TextBox();
      this.textBoxUtcOffset = new System.Windows.Forms.TextBox();
      this.textBoxLongitude = new System.Windows.Forms.TextBox();
      this.textBoxLatitude = new System.Windows.Forms.TextBox();
      this.labelUtcOffset = new System.Windows.Forms.Label();
      this.labelLongitude = new System.Windows.Forms.Label();
      this.labelLatitude = new System.Windows.Forms.Label();
      this.labelIpAddress = new System.Windows.Forms.Label();
      this.textBoxIpAddress = new System.Windows.Forms.TextBox();
      this.textBoxPort = new System.Windows.Forms.TextBox();
      this.labelSiteInformation = new System.Windows.Forms.Label();
      this.pictureBoxDDScope = new System.Windows.Forms.PictureBox();
      this.panelDateTime = new System.Windows.Forms.Panel();
      this.textBoxUtcTime = new System.Windows.Forms.TextBox();
      this.textBoxLocalTime = new System.Windows.Forms.TextBox();
      this.textBoxLocalDate = new System.Windows.Forms.TextBox();
      this.labelUtcTime = new System.Windows.Forms.Label();
      this.labelLocalTime = new System.Windows.Forms.Label();
      this.labelLocalDate = new System.Windows.Forms.Label();
      this.labelDateTime = new System.Windows.Forms.Label();
      this.buttonConnect_Load = new System.Windows.Forms.Button();
      this.labelConnectionStatus = new System.Windows.Forms.Label();
      this.panel1 = new System.Windows.Forms.Panel();
      this.radioButtonTwoX = new System.Windows.Forms.RadioButton();
      this.radioButtonOnePointFiveX = new System.Windows.Forms.RadioButton();
      this.radioButtonOneX = new System.Windows.Forms.RadioButton();
      this.radioButtonPointSevenFiveX = new System.Windows.Forms.RadioButton();
      this.labelSlewRate = new System.Windows.Forms.Label();
      this.labelTitle = new System.Windows.Forms.Label();
      ((System.ComponentModel.ISupportInitialize)(this.picASCOM)).BeginInit();
      this.panelSiteInformation.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.pictureBoxDDScope)).BeginInit();
      this.panelDateTime.SuspendLayout();
      this.panel1.SuspendLayout();
      this.SuspendLayout();
      // 
      // buttonCmdOK
      // 
      this.buttonCmdOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.buttonCmdOK.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(29)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
      this.buttonCmdOK.DialogResult = System.Windows.Forms.DialogResult.OK;
      this.buttonCmdOK.FlatAppearance.BorderColor = System.Drawing.SystemColors.ControlDark;
      this.buttonCmdOK.FlatAppearance.BorderSize = 2;
      this.buttonCmdOK.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.buttonCmdOK.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.buttonCmdOK.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
      this.buttonCmdOK.Location = new System.Drawing.Point(458, 409);
      this.buttonCmdOK.Name = "buttonCmdOK";
      this.buttonCmdOK.Size = new System.Drawing.Size(72, 30);
      this.buttonCmdOK.TabIndex = 0;
      this.buttonCmdOK.Text = "OK";
      this.buttonCmdOK.UseVisualStyleBackColor = false;
      this.buttonCmdOK.Click += new System.EventHandler(this.CmdOK_Click);
      // 
      // buttonCmdCancel
      // 
      this.buttonCmdCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.buttonCmdCancel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(29)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
      this.buttonCmdCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.buttonCmdCancel.FlatAppearance.BorderColor = System.Drawing.SystemColors.ControlDark;
      this.buttonCmdCancel.FlatAppearance.BorderSize = 2;
      this.buttonCmdCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.buttonCmdCancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.buttonCmdCancel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
      this.buttonCmdCancel.Location = new System.Drawing.Point(458, 459);
      this.buttonCmdCancel.Name = "buttonCmdCancel";
      this.buttonCmdCancel.Size = new System.Drawing.Size(72, 30);
      this.buttonCmdCancel.TabIndex = 1;
      this.buttonCmdCancel.Text = "Cancel";
      this.buttonCmdCancel.UseVisualStyleBackColor = false;
      this.buttonCmdCancel.Click += new System.EventHandler(this.CmdCancel_Click);
      // 
      // picASCOM
      // 
      this.picASCOM.Cursor = System.Windows.Forms.Cursors.Hand;
      this.picASCOM.Image = ((System.Drawing.Image)(resources.GetObject("picASCOM.Image")));
      this.picASCOM.Location = new System.Drawing.Point(21, 415);
      this.picASCOM.Name = "picASCOM";
      this.picASCOM.Size = new System.Drawing.Size(48, 56);
      this.picASCOM.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
      this.picASCOM.TabIndex = 3;
      this.picASCOM.TabStop = false;
      this.picASCOM.Click += new System.EventHandler(this.BrowseToAscom);
      this.picASCOM.DoubleClick += new System.EventHandler(this.BrowseToAscom);
      // 
      // contextMenuStrip1
      // 
      this.contextMenuStrip1.Name = "contextMenuStrip1";
      this.contextMenuStrip1.Size = new System.Drawing.Size(61, 4);
      // 
      // checkBoxTrace
      // 
      this.checkBoxTrace.AutoSize = true;
      this.checkBoxTrace.Location = new System.Drawing.Point(110, 455);
      this.checkBoxTrace.Name = "checkBoxTrace";
      this.checkBoxTrace.Size = new System.Drawing.Size(69, 17);
      this.checkBoxTrace.TabIndex = 6;
      this.checkBoxTrace.Text = "Trace on";
      this.checkBoxTrace.UseVisualStyleBackColor = true;
      // 
      // checkBoxSendDateTime
      // 
      this.checkBoxSendDateTime.AutoSize = true;
      this.checkBoxSendDateTime.Location = new System.Drawing.Point(35, 121);
      this.checkBoxSendDateTime.Name = "checkBoxSendDateTime";
      this.checkBoxSendDateTime.Size = new System.Drawing.Size(169, 17);
      this.checkBoxSendDateTime.TabIndex = 8;
      this.checkBoxSendDateTime.Text = "Send Date / Time on Connect";
      this.checkBoxSendDateTime.UseVisualStyleBackColor = true;
      // 
      // labelPort
      // 
      this.labelPort.AutoSize = true;
      this.labelPort.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.labelPort.Location = new System.Drawing.Point(368, 77);
      this.labelPort.Name = "labelPort";
      this.labelPort.Size = new System.Drawing.Size(35, 16);
      this.labelPort.TabIndex = 5;
      this.labelPort.Text = "Port";
      // 
      // panelSiteInformation
      // 
      this.panelSiteInformation.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
      this.panelSiteInformation.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
      this.panelSiteInformation.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.panelSiteInformation.Controls.Add(this.checkBoxSendLocationThisLocation);
      this.panelSiteInformation.Controls.Add(this.label1);
      this.panelSiteInformation.Controls.Add(this.textBoxElevation);
      this.panelSiteInformation.Controls.Add(this.textBoxUtcOffset);
      this.panelSiteInformation.Controls.Add(this.textBoxLongitude);
      this.panelSiteInformation.Controls.Add(this.textBoxLatitude);
      this.panelSiteInformation.Controls.Add(this.labelUtcOffset);
      this.panelSiteInformation.Controls.Add(this.labelLongitude);
      this.panelSiteInformation.Controls.Add(this.labelLatitude);
      this.panelSiteInformation.ForeColor = System.Drawing.Color.Yellow;
      this.panelSiteInformation.Location = new System.Drawing.Point(21, 195);
      this.panelSiteInformation.Margin = new System.Windows.Forms.Padding(1);
      this.panelSiteInformation.Name = "panelSiteInformation";
      this.panelSiteInformation.Size = new System.Drawing.Size(240, 198);
      this.panelSiteInformation.TabIndex = 9;
      // 
      // checkBoxSendLocationThisLocation
      // 
      this.checkBoxSendLocationThisLocation.AutoSize = true;
      this.checkBoxSendLocationThisLocation.Location = new System.Drawing.Point(8, 160);
      this.checkBoxSendLocationThisLocation.Name = "checkBoxSendLocationThisLocation";
      this.checkBoxSendLocationThisLocation.Size = new System.Drawing.Size(230, 17);
      this.checkBoxSendLocationThisLocation.TabIndex = 22;
      this.checkBoxSendLocationThisLocation.Text = "Set Telescope Location Using PC Location";
      this.checkBoxSendLocationThisLocation.UseVisualStyleBackColor = true;
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(57, 117);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(51, 13);
      this.label1.TabIndex = 6;
      this.label1.Text = "Elevation";
      // 
      // textBoxElevation
      // 
      this.textBoxElevation.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(29)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
      this.textBoxElevation.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.textBoxElevation.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
      this.textBoxElevation.Location = new System.Drawing.Point(114, 117);
      this.textBoxElevation.Name = "textBoxElevation";
      this.textBoxElevation.ReadOnly = true;
      this.textBoxElevation.Size = new System.Drawing.Size(95, 20);
      this.textBoxElevation.TabIndex = 5;
      // 
      // textBoxUtcOffset
      // 
      this.textBoxUtcOffset.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(29)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
      this.textBoxUtcOffset.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.textBoxUtcOffset.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
      this.textBoxUtcOffset.Location = new System.Drawing.Point(114, 83);
      this.textBoxUtcOffset.Name = "textBoxUtcOffset";
      this.textBoxUtcOffset.ReadOnly = true;
      this.textBoxUtcOffset.Size = new System.Drawing.Size(95, 20);
      this.textBoxUtcOffset.TabIndex = 4;
      // 
      // textBoxLongitude
      // 
      this.textBoxLongitude.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(29)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
      this.textBoxLongitude.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.textBoxLongitude.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
      this.textBoxLongitude.Location = new System.Drawing.Point(114, 49);
      this.textBoxLongitude.Name = "textBoxLongitude";
      this.textBoxLongitude.ReadOnly = true;
      this.textBoxLongitude.Size = new System.Drawing.Size(95, 20);
      this.textBoxLongitude.TabIndex = 3;
      // 
      // textBoxLatitude
      // 
      this.textBoxLatitude.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(29)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
      this.textBoxLatitude.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.textBoxLatitude.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
      this.textBoxLatitude.Location = new System.Drawing.Point(114, 15);
      this.textBoxLatitude.Name = "textBoxLatitude";
      this.textBoxLatitude.ReadOnly = true;
      this.textBoxLatitude.Size = new System.Drawing.Size(95, 20);
      this.textBoxLatitude.TabIndex = 0;
      // 
      // labelUtcOffset
      // 
      this.labelUtcOffset.AutoSize = true;
      this.labelUtcOffset.Location = new System.Drawing.Point(9, 85);
      this.labelUtcOffset.Name = "labelUtcOffset";
      this.labelUtcOffset.Size = new System.Drawing.Size(99, 13);
      this.labelUtcOffset.TabIndex = 2;
      this.labelUtcOffset.Text = "UTC Offset (W is +)";
      // 
      // labelLongitude
      // 
      this.labelLongitude.AutoSize = true;
      this.labelLongitude.Location = new System.Drawing.Point(15, 51);
      this.labelLongitude.Name = "labelLongitude";
      this.labelLongitude.Size = new System.Drawing.Size(93, 13);
      this.labelLongitude.TabIndex = 1;
      this.labelLongitude.Text = "Longitude (W is +)";
      // 
      // labelLatitude
      // 
      this.labelLatitude.AutoSize = true;
      this.labelLatitude.Location = new System.Drawing.Point(27, 19);
      this.labelLatitude.Name = "labelLatitude";
      this.labelLatitude.Size = new System.Drawing.Size(81, 13);
      this.labelLatitude.TabIndex = 0;
      this.labelLatitude.Text = "Latitude (N is +)";
      // 
      // labelIpAddress
      // 
      this.labelIpAddress.AutoSize = true;
      this.labelIpAddress.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.labelIpAddress.Location = new System.Drawing.Point(320, 51);
      this.labelIpAddress.Name = "labelIpAddress";
      this.labelIpAddress.Size = new System.Drawing.Size(83, 16);
      this.labelIpAddress.TabIndex = 10;
      this.labelIpAddress.Text = "IP Address";
      // 
      // textBoxIpAddress
      // 
      this.textBoxIpAddress.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(29)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
      this.textBoxIpAddress.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.textBoxIpAddress.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
      this.textBoxIpAddress.Location = new System.Drawing.Point(405, 47);
      this.textBoxIpAddress.Name = "textBoxIpAddress";
      this.textBoxIpAddress.Size = new System.Drawing.Size(100, 20);
      this.textBoxIpAddress.TabIndex = 0;
      // 
      // textBoxPort
      // 
      this.textBoxPort.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(29)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
      this.textBoxPort.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.textBoxPort.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
      this.textBoxPort.Location = new System.Drawing.Point(405, 73);
      this.textBoxPort.Name = "textBoxPort";
      this.textBoxPort.Size = new System.Drawing.Size(100, 20);
      this.textBoxPort.TabIndex = 0;
      // 
      // labelSiteInformation
      // 
      this.labelSiteInformation.AutoSize = true;
      this.labelSiteInformation.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.labelSiteInformation.Location = new System.Drawing.Point(83, 177);
      this.labelSiteInformation.Name = "labelSiteInformation";
      this.labelSiteInformation.Size = new System.Drawing.Size(122, 17);
      this.labelSiteInformation.TabIndex = 13;
      this.labelSiteInformation.Text = "Site Information";
      // 
      // pictureBoxDDScope
      // 
      this.pictureBoxDDScope.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.pictureBoxDDScope.Image = ((System.Drawing.Image)(resources.GetObject("pictureBoxDDScope.Image")));
      this.pictureBoxDDScope.Location = new System.Drawing.Point(21, 47);
      this.pictureBoxDDScope.Name = "pictureBoxDDScope";
      this.pictureBoxDDScope.Size = new System.Drawing.Size(277, 113);
      this.pictureBoxDDScope.TabIndex = 15;
      this.pictureBoxDDScope.TabStop = false;
      // 
      // panelDateTime
      // 
      this.panelDateTime.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
      this.panelDateTime.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
      this.panelDateTime.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
      this.panelDateTime.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.panelDateTime.Controls.Add(this.textBoxUtcTime);
      this.panelDateTime.Controls.Add(this.textBoxLocalTime);
      this.panelDateTime.Controls.Add(this.textBoxLocalDate);
      this.panelDateTime.Controls.Add(this.labelUtcTime);
      this.panelDateTime.Controls.Add(this.labelLocalTime);
      this.panelDateTime.Controls.Add(this.labelLocalDate);
      this.panelDateTime.Controls.Add(this.checkBoxSendDateTime);
      this.panelDateTime.ForeColor = System.Drawing.Color.Yellow;
      this.panelDateTime.Location = new System.Drawing.Point(299, 195);
      this.panelDateTime.Margin = new System.Windows.Forms.Padding(1);
      this.panelDateTime.Name = "panelDateTime";
      this.panelDateTime.Size = new System.Drawing.Size(231, 158);
      this.panelDateTime.TabIndex = 16;
      // 
      // textBoxUtcTime
      // 
      this.textBoxUtcTime.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(29)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
      this.textBoxUtcTime.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.textBoxUtcTime.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
      this.textBoxUtcTime.Location = new System.Drawing.Point(105, 83);
      this.textBoxUtcTime.Name = "textBoxUtcTime";
      this.textBoxUtcTime.ReadOnly = true;
      this.textBoxUtcTime.Size = new System.Drawing.Size(83, 20);
      this.textBoxUtcTime.TabIndex = 4;
      // 
      // textBoxLocalTime
      // 
      this.textBoxLocalTime.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(29)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
      this.textBoxLocalTime.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.textBoxLocalTime.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
      this.textBoxLocalTime.Location = new System.Drawing.Point(104, 51);
      this.textBoxLocalTime.Name = "textBoxLocalTime";
      this.textBoxLocalTime.ReadOnly = true;
      this.textBoxLocalTime.Size = new System.Drawing.Size(84, 20);
      this.textBoxLocalTime.TabIndex = 3;
      // 
      // textBoxLocalDate
      // 
      this.textBoxLocalDate.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(29)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
      this.textBoxLocalDate.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.textBoxLocalDate.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
      this.textBoxLocalDate.Location = new System.Drawing.Point(104, 17);
      this.textBoxLocalDate.Name = "textBoxLocalDate";
      this.textBoxLocalDate.ReadOnly = true;
      this.textBoxLocalDate.Size = new System.Drawing.Size(84, 20);
      this.textBoxLocalDate.TabIndex = 0;
      // 
      // labelUtcTime
      // 
      this.labelUtcTime.AutoSize = true;
      this.labelUtcTime.Location = new System.Drawing.Point(37, 85);
      this.labelUtcTime.Name = "labelUtcTime";
      this.labelUtcTime.Size = new System.Drawing.Size(61, 13);
      this.labelUtcTime.TabIndex = 2;
      this.labelUtcTime.Text = "Time (UTC)";
      // 
      // labelLocalTime
      // 
      this.labelLocalTime.AutoSize = true;
      this.labelLocalTime.Location = new System.Drawing.Point(34, 53);
      this.labelLocalTime.Name = "labelLocalTime";
      this.labelLocalTime.Size = new System.Drawing.Size(65, 13);
      this.labelLocalTime.TabIndex = 1;
      this.labelLocalTime.Text = "Time (Local)";
      // 
      // labelLocalDate
      // 
      this.labelLocalDate.AutoSize = true;
      this.labelLocalDate.Location = new System.Drawing.Point(68, 22);
      this.labelLocalDate.Name = "labelLocalDate";
      this.labelLocalDate.Size = new System.Drawing.Size(30, 13);
      this.labelLocalDate.TabIndex = 0;
      this.labelLocalDate.Text = "Date";
      // 
      // labelDateTime
      // 
      this.labelDateTime.AutoSize = true;
      this.labelDateTime.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.labelDateTime.Location = new System.Drawing.Point(364, 177);
      this.labelDateTime.Name = "labelDateTime";
      this.labelDateTime.Size = new System.Drawing.Size(92, 17);
      this.labelDateTime.TabIndex = 18;
      this.labelDateTime.Text = "Date / Time";
      // 
      // buttonConnect_Load
      // 
      this.buttonConnect_Load.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(29)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
      this.buttonConnect_Load.FlatAppearance.BorderColor = System.Drawing.SystemColors.ControlDark;
      this.buttonConnect_Load.FlatAppearance.BorderSize = 2;
      this.buttonConnect_Load.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.buttonConnect_Load.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.buttonConnect_Load.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
      this.buttonConnect_Load.Location = new System.Drawing.Point(391, 112);
      this.buttonConnect_Load.Name = "buttonConnect_Load";
      this.buttonConnect_Load.Size = new System.Drawing.Size(123, 30);
      this.buttonConnect_Load.TabIndex = 17;
      this.buttonConnect_Load.Text = "Connect / Load";
      this.buttonConnect_Load.UseVisualStyleBackColor = false;
      this.buttonConnect_Load.Click += new System.EventHandler(this.ButtonVerify_Click);
      // 
      // labelConnectionStatus
      // 
      this.labelConnectionStatus.AutoSize = true;
      this.labelConnectionStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.labelConnectionStatus.ForeColor = System.Drawing.Color.Gray;
      this.labelConnectionStatus.Location = new System.Drawing.Point(407, 145);
      this.labelConnectionStatus.Name = "labelConnectionStatus";
      this.labelConnectionStatus.Size = new System.Drawing.Size(88, 15);
      this.labelConnectionStatus.TabIndex = 19;
      this.labelConnectionStatus.Text = "Not Connected";
      // 
      // panel1
      // 
      this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
      this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.panel1.Controls.Add(this.radioButtonTwoX);
      this.panel1.Controls.Add(this.radioButtonOnePointFiveX);
      this.panel1.Controls.Add(this.radioButtonOneX);
      this.panel1.Controls.Add(this.radioButtonPointSevenFiveX);
      this.panel1.ImeMode = System.Windows.Forms.ImeMode.Off;
      this.panel1.Location = new System.Drawing.Point(299, 390);
      this.panel1.Name = "panel1";
      this.panel1.Size = new System.Drawing.Size(136, 99);
      this.panel1.TabIndex = 20;
      // 
      // radioButtonTwoX
      // 
      this.radioButtonTwoX.AutoSize = true;
      this.radioButtonTwoX.Location = new System.Drawing.Point(20, 76);
      this.radioButtonTwoX.Name = "radioButtonTwoX";
      this.radioButtonTwoX.Size = new System.Drawing.Size(48, 17);
      this.radioButtonTwoX.TabIndex = 3;
      this.radioButtonTwoX.Text = "2.0 x";
      this.radioButtonTwoX.UseVisualStyleBackColor = true;
      this.radioButtonTwoX.CheckedChanged += new System.EventHandler(this.SlewSpeed_CheckedChanged);
      // 
      // radioButtonOnePointFiveX
      // 
      this.radioButtonOnePointFiveX.AutoSize = true;
      this.radioButtonOnePointFiveX.Location = new System.Drawing.Point(20, 52);
      this.radioButtonOnePointFiveX.Name = "radioButtonOnePointFiveX";
      this.radioButtonOnePointFiveX.Size = new System.Drawing.Size(48, 17);
      this.radioButtonOnePointFiveX.TabIndex = 2;
      this.radioButtonOnePointFiveX.Text = "1.5 x";
      this.radioButtonOnePointFiveX.UseVisualStyleBackColor = true;
      this.radioButtonOnePointFiveX.CheckedChanged += new System.EventHandler(this.SlewSpeed_CheckedChanged);
      // 
      // radioButtonOneX
      // 
      this.radioButtonOneX.AutoSize = true;
      this.radioButtonOneX.Checked = true;
      this.radioButtonOneX.Location = new System.Drawing.Point(20, 28);
      this.radioButtonOneX.Name = "radioButtonOneX";
      this.radioButtonOneX.Size = new System.Drawing.Size(115, 17);
      this.radioButtonOneX.TabIndex = 1;
      this.radioButtonOneX.TabStop = true;
      this.radioButtonOneX.Text = "1.0 x (7.0 deg/sec)";
      this.radioButtonOneX.UseVisualStyleBackColor = true;
      this.radioButtonOneX.CheckedChanged += new System.EventHandler(this.SlewSpeed_CheckedChanged);
      // 
      // radioButtonPointSevenFiveX
      // 
      this.radioButtonPointSevenFiveX.AutoSize = true;
      this.radioButtonPointSevenFiveX.Location = new System.Drawing.Point(20, 4);
      this.radioButtonPointSevenFiveX.Name = "radioButtonPointSevenFiveX";
      this.radioButtonPointSevenFiveX.Size = new System.Drawing.Size(54, 17);
      this.radioButtonPointSevenFiveX.TabIndex = 0;
      this.radioButtonPointSevenFiveX.Text = "0.75 x";
      this.radioButtonPointSevenFiveX.UseVisualStyleBackColor = true;
      this.radioButtonPointSevenFiveX.CheckedChanged += new System.EventHandler(this.SlewSpeed_CheckedChanged);
      // 
      // labelSlewRate
      // 
      this.labelSlewRate.AutoSize = true;
      this.labelSlewRate.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.labelSlewRate.Location = new System.Drawing.Point(332, 371);
      this.labelSlewRate.Name = "labelSlewRate";
      this.labelSlewRate.Size = new System.Drawing.Size(77, 16);
      this.labelSlewRate.TabIndex = 21;
      this.labelSlewRate.Text = "Slew Rate";
      // 
      // labelTitle
      // 
      this.labelTitle.AutoSize = true;
      this.labelTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.labelTitle.Location = new System.Drawing.Point(121, 1);
      this.labelTitle.Name = "labelTitle";
      this.labelTitle.Size = new System.Drawing.Size(316, 29);
      this.labelTitle.TabIndex = 22;
      this.labelTitle.Text = "ASCOM DDScopeX Setup";
      // 
      // SetupDialogForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(39)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
      this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
      this.ClientSize = new System.Drawing.Size(558, 512);
      this.ControlBox = false;
      this.Controls.Add(this.labelTitle);
      this.Controls.Add(this.labelSlewRate);
      this.Controls.Add(this.panel1);
      this.Controls.Add(this.labelConnectionStatus);
      this.Controls.Add(this.labelDateTime);
      this.Controls.Add(this.buttonConnect_Load);
      this.Controls.Add(this.panelDateTime);
      this.Controls.Add(this.pictureBoxDDScope);
      this.Controls.Add(this.textBoxPort);
      this.Controls.Add(this.textBoxIpAddress);
      this.Controls.Add(this.labelSiteInformation);
      this.Controls.Add(this.labelIpAddress);
      this.Controls.Add(this.panelSiteInformation);
      this.Controls.Add(this.checkBoxTrace);
      this.Controls.Add(this.labelPort);
      this.Controls.Add(this.picASCOM);
      this.Controls.Add(this.buttonCmdCancel);
      this.Controls.Add(this.buttonCmdOK);
      this.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
      this.ImeMode = System.Windows.Forms.ImeMode.Off;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "SetupDialogForm";
      this.Padding = new System.Windows.Forms.Padding(20, 30, 20, 20);
      this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
      this.Text = "DDScope Setup";
      this.Load += new System.EventHandler(this.SetupDialogForm_Load);
      ((System.ComponentModel.ISupportInitialize)(this.picASCOM)).EndInit();
      this.panelSiteInformation.ResumeLayout(false);
      this.panelSiteInformation.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.pictureBoxDDScope)).EndInit();
      this.panelDateTime.ResumeLayout(false);
      this.panelDateTime.PerformLayout();
      this.panel1.ResumeLayout(false);
      this.panel1.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Button buttonCmdOK;
    private System.Windows.Forms.Button buttonCmdCancel;
    private System.Windows.Forms.PictureBox picASCOM;

    private ContextMenuStrip contextMenuStrip1;
    private System.DirectoryServices.DirectoryEntry directoryEntry1;
    private CheckBox checkBoxTrace;
    private Label labelPort;
    private Panel panelSiteInformation;
    private Label labelUtcOffset;
    private Label labelLongitude;
    private Label labelLatitude;
    private Label labelIpAddress;
    private Label labelSiteInformation;
    private TextBox textBoxLatitude;
    private TextBox textBoxIpAddress;
    private TextBox textBoxPort;
    private TextBox textBoxUtcOffset;
    private TextBox textBoxLongitude;
    private PictureBox pictureBoxDDScope;
    private Panel panelDateTime;
    private TextBox textBoxUtcTime;
    private TextBox textBoxLocalTime;
    private TextBox textBoxLocalDate;
    private Label labelUtcTime;
    private Label labelLocalTime;
    private Label labelLocalDate;
    private Button buttonConnect_Load;
    private CheckBox checkBoxSendDateTime;
    private Label labelDateTime;
    private Label labelConnectionStatus;
    private Panel panel1;
    private Label labelSlewRate;
    private RadioButton radioButtonTwoX;
    private RadioButton radioButtonOnePointFiveX;
    private RadioButton radioButtonOneX;
    private RadioButton radioButtonPointSevenFiveX;
    private TextBox textBoxElevation;
    private Label label1;
    private CheckBox checkBoxSendLocationThisLocation;
    private Label labelTitle;
  }

}