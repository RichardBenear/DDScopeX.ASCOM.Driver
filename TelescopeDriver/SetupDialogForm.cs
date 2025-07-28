using ASCOM.Utilities;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Net;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using ASCOM.DDScopeX.Utility;

namespace ASCOM.DDScopeX.Telescope
{
  [ComVisible(false)] // Form not registered for COM!
  public partial class SetupDialogForm : Form
  {
    private Guid verifyConnectionGuid;//= Guid.NewGuid();
    private string selectedSlewSpeedCommand = ":SX93,3#"; // default to 1.0x

    //const string NO_PORTS_MESSAGE = "No COM ports found";
    TraceLogger tl; // Holder for a reference to the driver's trace logged

    public SetupDialogForm(TraceLogger tlDriver)
    {
      InitializeComponent();

      // Save the provided trace logger for use within the setup dialogue
      tl = tlDriver;

      // Initialise current values of user settings from the ASCOM Profile
      InitUI();
    }

    // Save Values to the ASCOM Profile and exit
    private void CmdOK_Click(object sender, EventArgs e) // OK button event handler
    {
      tl.Enabled = checkBoxTrace.Checked;
      try
      {
        using (var profile = new Profile() { DeviceType = "Telescope" })
        {
          profile.WriteValue(Telescope.DriverProgId, TelescopeHardware.ipAddressProfileName, textBoxIpAddress.Text, string.Empty);
          profile.WriteValue(Telescope.DriverProgId, TelescopeHardware.ipPortProfileName, textBoxPort.Text, string.Empty);
          profile.WriteValue(TelescopeHardware.DriverProgId, "SiteLatitude", textBoxLatitude.Text.Trim(), string.Empty);
          profile.WriteValue(TelescopeHardware.DriverProgId, "SiteLongitude", textBoxLongitude.Text.Trim(), string.Empty);
          profile.WriteValue(TelescopeHardware.DriverProgId, "UtcOffset", textBoxUtcOffset.Text.Trim(), string.Empty);
          profile.WriteValue(TelescopeHardware.DriverProgId, "SiteElevation", textBoxElevation.Text.Trim(), string.Empty);
          profile.WriteValue(TelescopeHardware.DriverProgId, "SendDateTime", checkBoxSendDateTime.Checked.ToString());
          profile.WriteValue(TelescopeHardware.DriverProgId, "SendSiteLocation", checkBoxSendLocationThisLocation.Checked.ToString());
          profile.WriteValue(TelescopeHardware.DriverProgId, TelescopeHardware.slewSpeedProfileName, selectedSlewSpeedCommand);
        }
        MessageBox.Show("Settings saved successfully.", "Save", MessageBoxButtons.OK, MessageBoxIcon.Information);
        tl.LogMessage("CmdOK", $"TextBox IP/Port: {textBoxIpAddress.Text}, Port: {textBoxPort.Text}");
        tl.LogMessage("CmdOK", $"Set UI controls to Trace: {checkBoxTrace.Checked}");
        tl.LogMessage("CmdOK", $"CheckBoxSendDateTime: {checkBoxSendDateTime.Checked}");
        tl.LogMessage("CmdOK", $"CheckBoxSendSiteLocation: {checkBoxSendLocationThisLocation.Checked}");
        tl.LogMessage("CmdOK", $"RadioSetSlewSpeed: {selectedSlewSpeedCommand}");
      }
      catch (Exception ex)
      {
        MessageBox.Show("Failed to save settings:\n" + ex.Message, "Save Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    private void CmdCancel_Click(object sender, EventArgs e) // Cancel button event handler
    {
      TelescopeHardware.SetConnected(verifyConnectionGuid, false); // make sure we are disconnected
      Close();
    }

    private void BrowseToAscom(object sender, EventArgs e) // Click on ASCOM logo event handler
    {
      try
      {
        System.Diagnostics.Process.Start("https://ascom-standards.org/");
      }
      catch (Win32Exception noBrowser)
      {
        if (noBrowser.ErrorCode == -2147467259)
          MessageBox.Show(noBrowser.Message);
      }
      catch (Exception other)
      {
        MessageBox.Show(other.Message);
      }
    }

    // Initialise current values of user settings from the ASCOM Profile
    private void InitUI()
    {
      // Initialize checkboxes
      checkBoxTrace.Checked = tl.Enabled;
      checkBoxSendDateTime.Checked = true;

      using (var profile = new Profile() { DeviceType = "Telescope" })
      {
        // Force default IP Address and Port into registry if not already present
        if (string.IsNullOrEmpty(profile.GetValue(TelescopeHardware.DriverProgId, TelescopeHardware.ipAddressProfileName, "")))
          profile.WriteValue(TelescopeHardware.DriverProgId, TelescopeHardware.ipAddressProfileName, "192.168.4.1");

        if (string.IsNullOrEmpty(profile.GetValue(TelescopeHardware.DriverProgId, TelescopeHardware.ipPortProfileName, "")))
          profile.WriteValue(TelescopeHardware.DriverProgId, TelescopeHardware.ipPortProfileName, "4030");

        // Get Profile IP_Address and Port and populate text boxes
        textBoxIpAddress.Text = profile.GetValue(TelescopeHardware.DriverProgId, TelescopeHardware.ipAddressProfileName);
        textBoxPort.Text = profile.GetValue(TelescopeHardware.DriverProgId, TelescopeHardware.ipPortProfileName);

        // Get UTC and local time and populate text boxes
        DateTime utcNow = DateTime.UtcNow;
        DateTime localNow = DateTime.Now;
        textBoxUtcTime.Text = utcNow.ToString("HH:mm:ss");
        textBoxLocalTime.Text = localNow.ToString("HH:mm:ss");
        textBoxLocalDate.Text = localNow.ToString("MM-dd-yyyy");

        // Get Site Information
        textBoxLatitude.Text = profile.GetValue(TelescopeHardware.DriverProgId, "SiteLatitude", string.Empty, "");
        textBoxLongitude.Text = profile.GetValue(TelescopeHardware.DriverProgId, "SiteLongitude", string.Empty, "");
        textBoxUtcOffset.Text = profile.GetValue(TelescopeHardware.DriverProgId, "UtcOffset", string.Empty, "");
        textBoxElevation.Text = profile.GetValue(TelescopeHardware.DriverProgId, "SiteElevation", string.Empty, "");

        // Get Send Date/Time checkbox status
        string valDt = profile.GetValue(TelescopeHardware.DriverProgId, TelescopeHardware.sendDateTimeProfileName, string.Empty, "");
        checkBoxSendDateTime.Checked = bool.TryParse(valDt, out bool parsedDt) && parsedDt;

        // Get Send Location checkbox status
        string valSl = profile.GetValue(TelescopeHardware.DriverProgId, "SendSiteLocation", string.Empty, "");
        checkBoxSendLocationThisLocation.Checked = bool.TryParse(valSl, out bool parsedSl) && parsedSl;

        // Get Selected Slew Speed
        string savedSpeed = profile.GetValue(TelescopeHardware.DriverProgId, TelescopeHardware.slewSpeedProfileName, string.Empty, "");
        selectedSlewSpeedCommand = savedSpeed;

        // Now Set appropriate radio button
        switch (savedSpeed)
        {
          case ":SX93,4#":
            radioButtonPointSevenFiveX.Checked = true;
            break;
          case ":SX93,2#":
            radioButtonOnePointFiveX.Checked = true;
            break;
          case ":SX93,1#":
            radioButtonTwoX.Checked = true;
            break;
          case ":SX93,3#":
          default:
            tl.LogMessage("InitUI", $"Unknown saved slew speed: {savedSpeed}, defaulting to 1.0x");
            radioButtonOneX.Checked = true;
            break;
        }
      }
      this.ActiveControl = labelConnectionStatus; // sets the focus from the Port text box to an uneditable control
      tl.LogMessage("InitUI", $"TextBox IP/Port: {textBoxIpAddress.Text}, Port: {textBoxPort.Text}");
      tl.LogMessage("InitUI", $"Set UI controls to Trace: {checkBoxTrace.Checked}");
      tl.LogMessage("InitUI", $"CheckBoxSendDateTime: {checkBoxSendDateTime.Checked}");
      tl.LogMessage("InitUI", $"RadioSetSlewSpeed: {selectedSlewSpeedCommand}");
    }

    // Load the Form
    private void SetupDialogForm_Load(object sender, EventArgs e)
    {
      // Bring the setup dialogue to the front of the screen
      if (WindowState == FormWindowState.Minimized)
        WindowState = FormWindowState.Normal;
      else
      {
        TopMost = true;
        Focus();
        BringToFront();
        TopMost = false;
      }
    }

    //***** Try TCP/IP Connection and Load Data to/from the Telescope into the Form *****//
    // Why do we need a button?
    // 1) InitUI() is intended to read the values from the profile and display their value on the Form.
    // 2) CmdOK_Click saves new entries by writing to the profile. But it also closes the Form.
    // Therefore only way to test from the Form UI what has been connected and loaded from the telescope is to reopen the Form
    // or try to connect telescope! Uggg.
    // 3) This button provides a way to do dynamic updates of the Form to/from the telescope and verify the results..
    // This button toggles between Connect and Disconnect. Clicking "OK" will disconnect so that the client can reconnect later.
    private void ButtonVerify_Click(object sender, EventArgs e)
    {
      this.ActiveControl = labelConnectionStatus; // force focus to non-input control

      if (TelescopeHardware.IsConnected)
      {
        // === DISCONNECT MODE ===
        try
        {
          TelescopeHardware.SetConnected(verifyConnectionGuid, false);
          labelConnectionStatus.Text = "Disconnected";
          labelConnectionStatus.ForeColor = Color.Gray;
          buttonConnect_Load.Text = "Connect";
        }
        catch (Exception ex)
        {
          MessageBox.Show("Error disconnecting:\n" + ex.Message, "Disconnect Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        return;
      }

      // === CONNECT MODE ===
      string ipStr = textBoxIpAddress.Text.Trim();
      string portStr = textBoxPort.Text.Trim();

      // Validate IP
      if (!IPAddress.TryParse(ipStr, out _))
      {
        MessageBox.Show("Invalid IP address format.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        return;
      }

      // Validate port
      if (!int.TryParse(portStr, out int port) || port < 1 || port > 65535)
      {
        MessageBox.Show("Port must be a number between 1 and 65535.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        return;
      }
     
      verifyConnectionGuid = Guid.NewGuid();

      try
      {
        TelescopeHardware.SetConnected(verifyConnectionGuid, true, ipStr, portStr);

        labelConnectionStatus.Text = "✅ Connected";
        labelConnectionStatus.ForeColor = Color.Green;
        buttonConnect_Load.Text = "Disconnect";

        UpdateDateTime();

        UpdateTelescopeSiteInfo();

      }
      catch (Exception ex)
      {
        labelConnectionStatus.Text = "❌ Failed";
        labelConnectionStatus.ForeColor = Color.Red;
        MessageBox.Show("Error making connection or fetching values:\n" + ex.Message);
        buttonConnect_Load.Enabled = true;
        return;
      }
      this.ActiveControl = labelConnectionStatus; // sets the focus from the Port text box to an uneditable control
    }

    // Slew Speed radio button handler
    private void SlewSpeed_CheckedChanged(object sender, EventArgs e)
    {
      if (sender is RadioButton radio && radio.Checked)
      {
        tl.LogMessage("SlewSpeed_CheckedChanged", $"Selected: {radio.Name}");

        if (radio == radioButtonPointSevenFiveX)
          selectedSlewSpeedCommand = ":SX93,4#";
        else if (radio == radioButtonOneX)
          selectedSlewSpeedCommand = ":SX93,3#";
        else if (radio == radioButtonOnePointFiveX)
          selectedSlewSpeedCommand = ":SX93,2#";
        else if (radio == radioButtonTwoX)
          selectedSlewSpeedCommand = ":SX93,1#";
      }
    }

    // Leave handler for the IPAddress text box
    private void TextBoxIpAddress_Leave(object sender, EventArgs e)
    {
      string ip = textBoxIpAddress.Text.Trim();
      if (IPAddress.TryParse(ip, out _))
      {
        using (var profile = new Profile() { DeviceType = "Telescope" })
          profile.WriteValue(Telescope.DriverProgId, TelescopeHardware.ipAddressProfileName, ip);
        tl.LogMessage("textBoxIpAddress_Leave", $"Saved IP: {ip}");
      }
      else
      {
        MessageBox.Show("Invalid IP address.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        textBoxIpAddress.Focus();
      }
    }

    // Leave handler for the Port textbox
    private void TextBoxPort_Leave(object sender, EventArgs e)
    {
      string portStr = textBoxPort.Text.Trim();
      if (int.TryParse(portStr, out int port) && port >= 1 && port <= 65535)
      {
        using (var profile = new Profile() { DeviceType = "Telescope" })
          profile.WriteValue(Telescope.DriverProgId, TelescopeHardware.ipPortProfileName, portStr);
        tl.LogMessage("textBoxPort_Leave", $"Saved port: {portStr}");
      }
      else
      {
        MessageBox.Show("Invalid port number (1–65535).", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        textBoxPort.Focus();
      }
    }

    // Call this when connected. Will get/send data to/from telescope and load Form
    private void UpdateDateTime()
    {
      DateTime utcNow = DateTime.UtcNow;
      DateTime localNow = DateTime.Now;

      textBoxUtcTime.Text = utcNow.ToString("HH:mm:ss");
      textBoxLocalTime.Text = localNow.ToString("HH:mm:ss");
      textBoxLocalDate.Text = localNow.ToString("MM-dd-yyyy");

      if (checkBoxSendDateTime.Checked)
      {
        string dateCmd = $":SC{localNow:MM/dd/yyyy}#";
        string timeCmd = $":SL{localNow:HH:mm:ss}#";

        bool okDate = TelescopeHardware.CommandBool(dateCmd, true);
        bool okTime = TelescopeHardware.CommandBool(timeCmd, true);

        if (!okDate) throw new ASCOM.DriverException("Failed to set local date.");
        if (!okTime) throw new ASCOM.DriverException("Failed to set local time.");

        tl.LogMessage("SendDateTime", $"Sent date={dateCmd}, time={timeCmd}");
      }
    }

    // Handle either getting or sending Site Location from or to Telescope
    private void UpdateTelescopeSiteInfo()
    {
      try
      {
        bool updateFromPc = checkBoxSendLocationThisLocation.Checked;

        // Set Slew Speed, throw away any returned junk
        string junk = TelescopeHardware.CommandString(selectedSlewSpeedCommand, true);
        tl.LogMessage("SetSlewSpeed", $"Sent {selectedSlewSpeedCommand}");

        // Get/Set Site Latitude
        HandleDoubleField(
          updateFromPc,
          textBoxLatitude,
          () => PcLocationHelper.GetPcLatitude(),
          () => TelescopeHardware.SiteLatitude,
          v => { TelescopeHardware.SiteLatitude = v; 
          tl.LogMessage("ButtonVerify_Click", $"Sent Latitude: {v}"); }, "F2"
        );

        // Get/Set Site Longitude
        HandleDoubleField(
          updateFromPc,
          textBoxLongitude,
          () => PcLocationHelper.GetPcLongitude(tl),
          () => TelescopeHardware.SiteLongitude,
          v => { TelescopeHardware.SiteLongitude = v; 
          tl.LogMessage("ButtonVerify_Click", $"Sent Longitude: {v}"); }, "F2"
        );

        // Get/Set UTC Offset -- Opposite of time zone value
        if (updateFromPc)
        { // TelescopeHardware doesn't support writing UTC offset so we send to the telescope here
          DateTime utcNow = DateTime.UtcNow;
          DateTime localNow = DateTime.Now;
          
          TimeSpan offset = utcNow - localNow; // Amount to "add" to the local time to get UTC time
          int offsetHours = (int)offset.TotalHours;
          string sign = offsetHours >= 0 ? "+" : "-";

          string offsetCmd = $":SG{sign}{Math.Abs(offsetHours):00}#";
          bool okOffset = TelescopeHardware.CommandBool(offsetCmd, true);
          if (!okOffset) throw new ASCOM.DriverException("Failed to set UTC offset.");

          tl.LogMessage("ButtonVerify_Click", $"Sent UTC Offset: {offsetCmd}");
          textBoxUtcOffset.Text = offset.TotalHours.ToString("+0.0;-0.0");
        }
        else // get the UTC Offset from the telescope
        {
          double utcOffset = TelescopeHardware.UtcOffset;
          textBoxUtcOffset.Text = utcOffset.ToString("F1");
        }

        // Get/Set Elevation
        HandleDoubleField(
          updateFromPc,
          textBoxElevation,
          () => PcLocationHelper.GetPcElevation(),
          () => TelescopeHardware.SiteElevation,
          v => { TelescopeHardware.SiteElevation = v; 
          tl.LogMessage("ButtonVerify_Click", $"Sent Elevation: {v}"); }, "F1"
        );
      }
      catch (Exception ex)
      {
        MessageBox.Show("Error updating form:\n" + ex.Message, "Update Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
      }

      // Local helper
      void HandleDoubleField(
        bool updateFromPc,
        TextBox textBox,
        Func<double> pcFunc,
        Func<double> telescopeFunc,
        Action<double> setFunc,
        string format
      )
      {
        try
        {
          if (updateFromPc)
          {
            double val = pcFunc();
            setFunc(val);
            textBox.Text = val.ToString(format);
          }
          else
          {
            double val = telescopeFunc();
            textBox.Text = val.ToString(format);
          }
        }
        catch (Exception ex)
        {
          MessageBox.Show($"Failed to update {textBox.Name}: {ex.Message}", "Update Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
      }
    }
  }
}