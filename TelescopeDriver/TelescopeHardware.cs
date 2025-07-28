// TODO fill in this information for your driver, then remove this line!
//
// ASCOM Telescope hardware class for DDScope
//
// Description:	 An ASCOM Telescope Driver for the DDScope (Direct Drive TeleScope)
//
// Implements:	ASCOM Telescope interface version: 7
// Author:		Richard Benear 7/21/2025


using ASCOM;
using ASCOM.Astrometry;
using ASCOM.Astrometry.AstroUtils;
using ASCOM.Astrometry.NOVAS;
using ASCOM.DeviceInterface;
using ASCOM.Utilities;
using ASCOM.Utilities.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ASCOM.DDScopeX.Telescope
{
  //
  // NOTE You should not need to customise the code in the Connecting, Connect() and Disconnect() members as these are already fully implemented and call SetConnected() when appropriate.
  //
  // TODO Replace the not implemented exceptions with code to implement the functions or throw the appropriate ASCOM exceptions.
  //

  /// <summary>
  /// ASCOM Telescope hardware class for DDScopeX.
  /// </summary>
  [HardwareClass()] // Class attribute flag this as a device hardware class that needs to be disposed by the local server when it exits.
  internal static class TelescopeHardware
  {
    // Constants used for Profile persistence
    internal const string comPortProfileName = "COM Port";
    internal const string comPortDefault = "COM1";
    internal const string traceStateProfileName = "Trace Level";
    internal const string traceStateDefault = "true";

    public static string DriverProgId = ""; // ASCOM DeviceID (COM ProgID) for this driver, the value is set by the driver's class initialiser.
    private static string DriverDescription = ""; // The value is set by the driver's class initialiser.
    internal static string comPort; // COM port name (if required)
    private static bool connectedState; // Local server's connected state
    private static bool runOnce = false; // Flag to enable "one-off" activities only to run once.
    internal static Util utilities; // ASCOM Utilities object for use as required
    internal static AstroUtils astroUtilities; // ASCOM AstroUtilities object for use as required
    internal static TraceLogger tl; // Local server's trace logger object for diagnostic log with information that you specify

    private static List<Guid> uniqueIds = new List<Guid>(); // List of driver instance unique IDs

    internal const string ipAddressProfileName = "IP_Address";
    internal const string ipAddressDefault = "192.168.4.1";
    internal static string ipAddress;

    internal const string ipPortProfileName = "Port";
    internal const string ipPortDefault = "4030";
    internal static string ipPort;
    internal static int ipPortInt;

    internal const string sendDateTimeProfileName = "SendDateTime";
    public static bool SendDateTimeOnConnect { get; set; } = false;

    internal const string slewSpeedProfileName = "SlewSpeed";

    private static TcpClient client;
    private static NetworkStream stream;

    private static double targetRA = 0.0;
    private static double targetDEC = 0.0;

    internal static string SetLongitudeCommand = null;

    /// <summary>
    /// Initializes a new instance of the device Hardware class.
    /// </summary>
    static TelescopeHardware()
    {
      try
      {
        // Create the hardware trace logger in the static initialiser.
        // All other initialisation should go in the InitialiseHardware method.
        tl = new TraceLogger("", "DDScopeX.Hardware");

        // DriverProgId has to be set here because it used by ReadProfile to get the TraceState flag.
        DriverProgId = Telescope.DriverProgId; // Get this device's ProgID so that it can be used to read the Profile configuration values

        // ReadProfile has to go here before anything is written to the log because it loads the TraceLogger enable / disable state.
        ReadProfile(); // Read device configuration from the ASCOM Profile store, including the trace state

        LogMessage("TelescopeHardware", $"Static initialiser completed.");
      }
      catch (Exception ex)
      {
        try { LogMessage("TelescopeHardware", $"Initialisation exception: {ex}"); } catch { }
        MessageBox.Show($"TelescopeHardware - {ex.Message}\r\n{ex}", $"Exception creating {Telescope.DriverProgId}", MessageBoxButtons.OK, MessageBoxIcon.Error);
        throw;
      }
    }

    /// <summary>
    /// Place device initialisation code here
    /// </summary>
    /// <remarks>Called every time a new instance of the driver is created.</remarks>
    internal static void InitialiseHardware()
    {
      // This method will be called every time a new ASCOM client loads your driver
      LogMessage("InitialiseHardware", $"Start.");

      // Add any code that you want to run every time a client connects to your driver here
      using (var profile = new Profile())
      {
        profile.DeviceType = "Telescope";

        // Read persistent values (do NOT connect yet)
        string savedIp = profile.GetValue(DriverProgId, ipAddressProfileName, "", ipAddressDefault);
        string savedPort = profile.GetValue(DriverProgId, ipPortProfileName, "", ipPortDefault);
        LogMessage("InitialiseHardware", $"Loaded IP: {savedIp}, Port: {savedPort}");
      }

      // Add any code that you only want to run when the first client connects in the if (runOnce == false) block below
      if (runOnce == false)
      {
        LogMessage("InitialiseHardware", $"Starting one-off initialisation.");

        DriverDescription = Telescope.DriverDescription; // Get this device's Chooser description

        LogMessage("InitialiseHardware", $"ProgID: {DriverProgId}, Description: {DriverDescription}");

        connectedState = false; // Initialise connected to false
        utilities = new Util(); //Initialise ASCOM Utilities object
        astroUtilities = new AstroUtils(); // Initialise ASCOM Astronomy Utilities object

        LogMessage("InitialiseHardware", "Completed basic initialisation");

        // Add your own "one off" device initialisation here e.g. validating existence of hardware and setting up communications
        // If you are using a serial COM port you will find the COM port name selected by the user through the setup dialogue in the comPort variable.

        LogMessage("InitialiseHardware", $"One-off initialisation complete.");
        runOnce = true; // Set the flag to ensure that this code is not run again
      }
    }

    // PUBLIC COM INTERFACE ITelescopeV4 IMPLEMENTATION

    #region Common properties and methods.

    /// <summary>
    /// Displays the Setup Dialogue form.
    /// If the user clicks the OK button to dismiss the form, then
    /// the new settings are saved, otherwise the old values are reloaded.
    /// THIS IS THE ONLY PLACE WHERE SHOWING USER INTERFACE IS ALLOWED!
    /// </summary>
    public static void SetupDialog()
    {
      // Don't permit the setup dialogue if already connected
      if (IsConnected)
      {
        MessageBox.Show("Already connected, just press OK");
        return; // Exit the method if already connected
      }

      using (SetupDialogForm F = new SetupDialogForm(tl))
      {
        var result = F.ShowDialog();
        if (result == DialogResult.OK)
        {
          WriteProfile(); // Persist device configuration values to the ASCOM Profile store
        }
      }
    }

    /// <summary>Returns the list of custom action names supported by this driver.</summary>
    /// <value>An ArrayList of strings (SafeArray collection) containing the names of supported actions.</value>
    public static ArrayList SupportedActions
    {
      get
      {
        LogMessage("SupportedActions Get", "Returning empty ArrayList");
        return new ArrayList();
      }
    }

    /// <summary>Invokes the specified device-specific custom action.</summary>
    /// <param name="ActionName">A well known name agreed by interested parties that represents the action to be carried out.</param>
    /// <param name="ActionParameters">List of required parameters or an <see cref="String.Empty">Empty String</see> if none are required.</param>
    /// <returns>A string response. The meaning of returned strings is set by the driver author.
    /// <para>Suppose filter wheels start to appear with automatic wheel changers; new actions could be <c>QueryWheels</c> and <c>SelectWheel</c>. The former returning a formatted list
    /// of wheel names and the second taking a wheel name and making the change, returning appropriate values to indicate success or failure.</para>
    /// </returns>
    public static string Action(string actionName, string actionParameters)
    {
      LogMessage("Action", $"Action {actionName}, parameters {actionParameters} is not implemented");
      throw new ActionNotImplementedException("Action " + actionName + " is not implemented by this driver");
    }

    /// <summary>
    /// Transmits an arbitrary string to the device and does not wait for a response.
    /// Optionally, protocol framing characters may be added to the string before transmission.
    /// </summary>
    /// <param name="Command">The literal command string to be transmitted.</param>
    /// <param name="Raw">
    /// if set to <c>true</c> the string is transmitted 'as-is'.
    /// If set to <c>false</c> then protocol framing characters may be added prior to transmission.
    /// </param>
    public static void CommandBlind(string command, bool raw)
    {
      CheckConnected("CommandBlind");
      command = command.Trim();  // Clean up whitespace

      try
      {
        if (client == null || stream == null || !client.Connected)
          throw new ASCOM.DriverException("Not connected to TCP device.");

        // Ensure command ends with #
        if (!command.EndsWith("#"))
          command += "#";

        LogMessage("CommandBlind", $"Sending command: '{command}'");

        // Set timeouts (e.g., 3 seconds)
        stream.WriteTimeout = 3000;

        // Send command
        byte[] buffer = Encoding.ASCII.GetBytes(command);
        stream.Write(buffer, 0, buffer.Length);
        stream.Flush();

        // Clear any unexpected bytes in the read buffer
        int cleared = 0;
        var sw = Stopwatch.StartNew();
        while (stream.DataAvailable && sw.ElapsedMilliseconds < 400)
        {
          stream.ReadByte(); // discard
          cleared++;
        }
        if (cleared > 0)
          LogMessage("CommandBlind", $"Cleared {cleared} extraneous bytes from stream.");
      }

      catch (IOException ex)
      {
        LogMessage("CommandBlind", $"Timeout or I/O error: {ex.Message}");
        throw new ASCOM.DriverException("CommandBlind TCP timeout or I/O error: " + ex.Message);
      }
      catch (Exception ex)
      {
        LogMessage("CommandBlind", $"Exception: {ex.Message}");
        throw new ASCOM.DriverException("CommandBlind TCP error: " + ex.Message);
      }
    }

    /// <summary>
    /// Transmits an arbitrary string to the device and waits for a boolean response.
    /// Optionally, protocol framing characters may be added to the string before transmission.
    /// </summary>
    /// <param name="Command">The literal command string to be transmitted.</param>
    /// <param name="Raw">
    /// if set to <c>true</c> the string is transmitted 'as-is'.
    /// If set to <c>false</c> then protocol framing characters may be added prior to transmission.
    /// </param>
    /// <returns>
    /// Returns the interpreted boolean response received from the device.
    /// </returns>
    public static bool CommandBool(string command, bool raw)
    {
      CheckConnected("CommandBool");
      command = command.Trim();

      try
      {
        if (client == null || stream == null || !client.Connected)
          throw new ASCOM.DriverException("Not connected to TCP device.");

        if (!command.EndsWith("#"))
          command += "#";

        LogMessage("CommandBool", $"Sending command: '{command}'");

        stream.ReadTimeout = 3000;
        stream.WriteTimeout = 3000;

        // Send command
        byte[] buffer = Encoding.ASCII.GetBytes(command);
        stream.Write(buffer, 0, buffer.Length);
        stream.Flush();

        // Read first non-whitespace char
        int b;
        do
        {
          b = stream.ReadByte();
          if (b == -1)
            throw new IOException("No response from telescope.");
        } while (char.IsWhiteSpace((char)b));

        char resultChar = (char)b;
        LogMessage("CommandBool", $"Received response char: '{resultChar}'");

        // Drain entire response until no more data is available
        int drained = 0;
        while (stream.DataAvailable)
        {
          stream.ReadByte(); // discard
          drained++;
        }
        if (drained > 0)
          LogMessage("CommandBool", $"Drained {drained} leftover byte(s) from response stream.");

        // Interpret result
        return resultChar == '1';
      }
      catch (IOException ex)
      {
        LogMessage("CommandBool", $"Timeout or I/O error: {ex.Message}");
        throw new ASCOM.DriverException("CommandBool TCP timeout or I/O error: " + ex.Message);
      }
      catch (Exception ex)
      {
        LogMessage("CommandBool", $"Exception: {ex.Message}");
        throw new ASCOM.DriverException("CommandBool TCP error: " + ex.Message);
      }
    }

    /// <summary>
    /// Transmits an arbitrary string to the device and waits for a string response.
    /// Optionally, protocol framing characters may be added to the string before transmission.
    /// </summary>
    /// <param name="Command">The literal command string to be transmitted.</param>
    /// <param name="Raw">
    /// if set to <c>true</c> the string is transmitted 'as-is'.
    /// If set to <c>false</c> then protocol framing characters may be added prior to transmission.
    /// </param>
    /// <returns>
    /// Returns the string response received from the device.
    /// </returns>
    public static string CommandString(string command, bool raw)
    {
      CheckConnected("CommandString");
      command = command.Trim();  // Clean up whitespace

      try
      {
        if (client == null || stream == null || !client.Connected)
          throw new ASCOM.DriverException("Not connected to TCP device.");

        // Ensure command ends with #
        if (!command.EndsWith("#"))
          command += "#";

        LogMessage("CommandString", $"Sending command: '{command}'");

        // Set timeouts (e.g., 3 seconds)
        stream.ReadTimeout = 3000;   // in milliseconds
        stream.WriteTimeout = 3000;

        // Send command
        byte[] buffer = Encoding.ASCII.GetBytes(command);
        stream.Write(buffer, 0, buffer.Length);
        stream.Flush();

        // Read response until '#'
        StringBuilder response = new StringBuilder();
        int b;
        while ((b = stream.ReadByte()) != -1)
        {
          char c = (char)b;
          response.Append(c);
          //LogMessage("CommandString[Debug]", $"Received byte: 0x{b:X2} ({(char.IsControl(c) ? "." : c.ToString())})");
          if (c == '#') break;
        }

        string result = response.ToString().Trim();
        LogMessage("CommandString", $"Received response: '{result}'");
        return result;
      }
      catch (IOException ex)
      {
        LogMessage("CommandString", $"Timeout or I/O error: {ex.Message}");
        throw new ASCOM.DriverException("CommandString TCP timeout or I/O error: " + ex.Message);
      }
      catch (Exception ex)
      {
        LogMessage("CommandString", $"Exception: {ex.Message}");
        throw new ASCOM.DriverException("CommandString TCP error: " + ex.Message);
      }

      throw new MethodNotImplementedException($"CommandString - Command:{command}, Raw: {raw}.");
    }


    /// <summary>
    /// Deterministically release both managed and unmanaged resources that are used by this class.
    /// </summary>
    /// <remarks>
    /// TODO: Release any managed or unmanaged resources that are used in this class.
    /// 
    /// Do not call this method from the Dispose method in your driver class.
    ///
    /// This is because this hardware class is decorated with the <see cref="HardwareClassAttribute"/> attribute and this Dispose() method will be called 
    /// automatically by the  local server executable when it is irretrievably shutting down. This gives you the opportunity to release managed and unmanaged 
    /// resources in a timely fashion and avoid any time delay between local server close down and garbage collection by the .NET runtime.
    ///
    /// For the same reason, do not call the SharedResources.Dispose() method from this method. Any resources used in the static shared resources class
    /// itself should be released in the SharedResources.Dispose() method as usual. The SharedResources.Dispose() method will be called automatically 
    /// by the local server just before it shuts down.
    /// 
    /// </remarks>
    public static void Dispose()
    {
      try { LogMessage("Dispose", $"Disposing of assets and closing down."); } catch { }

      try
      {
        // Clean up the trace logger and utility objects
        tl.Enabled = false;
        tl.Dispose();
        tl = null;
      }
      catch { }

      try
      {
        utilities.Dispose();
        utilities = null;
      }
      catch { }

      try
      {
        astroUtilities.Dispose();
        astroUtilities = null;
      }
      catch { }
    }

    /// <summary>
    /// Synchronously connects to or disconnects from the hardware
    /// </summary>
    /// <param name="uniqueId">Driver's unique ID</param>
    /// <param name="newState">New state: Connected or Disconnected</param>
    //public static void SetConnected(Guid uniqueId, bool newState)
    public static void SetConnected(Guid uniqueId, bool newState, string overrideIp = null, string overridePort = null)
    {
      LogMessage("SetConnected", $"Called by: {new StackTrace().GetFrame(1).GetMethod().Name}");

      // Check whether we are connecting or disconnecting
      if (newState) // We are connecting
      {
        // Check whether this driver instance has already connected
        if (uniqueIds.Contains(uniqueId)) // Instance already connected
        {
          // Ignore the request, the unique ID is already in the list
          LogMessage("SetConnected", $"Ignoring request to connect because the device is already connected.");
        }
        else // Instance not already connected, so connect it
        {
          // Check whether this is the first connection to the hardware
          if (uniqueIds.Count == 0) // This is the first connection to the hardware so initiate the hardware connection
          {
            LogMessage("SetConnected", "Connecting via TCP/IP...");
            using (var profile = new ASCOM.Utilities.Profile())
            {
              profile.DeviceType = "Telescope";

              ipAddress = overrideIp ?? profile.GetValue(DriverProgId, TelescopeHardware.ipAddressProfileName, string.Empty, TelescopeHardware.ipAddressDefault);
              LogMessage("Set Connected", $"Using IP address: '{ipAddress}'");

              ipPort = overridePort ?? profile.GetValue(DriverProgId, TelescopeHardware.ipPortProfileName, string.Empty, TelescopeHardware.ipPortDefault);
              LogMessage("Set Connected", $"Using port: '{ipPort}'");

              if (!int.TryParse(ipPort, out ipPortInt))
              {
                LogMessage("Connected Set", $"Invalid port value in profile: '{ipPort}'");
                throw new ASCOM.DriverException($"Invalid port value in profile: '{ipPort}'");
              }
            }
            try
            {
              client = new TcpClient();
              var connectTask = client.ConnectAsync(ipAddress, ipPortInt);
              if (!connectTask.Wait(3000)) // timeout in milliseconds
              {
                client.Close();
                connectedState = false;
                throw new ASCOM.DriverException($"Connection timeout to {ipAddress}:{ipPortInt} after 3 seconds.");
              }

              stream = client.GetStream();
              connectedState = true;
              LogMessage("Connected Set", "TCP/IP connection established.");
            }
            catch (Exception ex)
            {
              connectedState = false;
              throw new ASCOM.DriverException("Could not connect: " + ex.Message);
            }
          }
          else // Other device instances are connected so the hardware is already connected
          {
            // Since the hardware is already connected no action is required
            LogMessage("SetConnected", $"Hardware already connected.");
          }

          // The hardware either "already was" or "is now" connected, so add the driver unique ID to the connected list
          uniqueIds.Add(uniqueId);
          LogMessage("SetConnected", $"Unique id {uniqueId} added to the connection list.");
        }
      }
      else // We are disconnecting
      {
        // Check whether this driver instance has already disconnected
        if (!uniqueIds.Contains(uniqueId)) // Instance not connected so ignore request
        {
          // Ignore the request, the unique ID is not in the list
          LogMessage("SetConnected", $"Ignoring request to disconnect because the device is already disconnected.");
        }
        else // Instance currently connected so disconnect it
        {
          // Remove the driver unique ID to the connected list
          uniqueIds.Remove(uniqueId);
          LogMessage("SetConnected", $"Unique id {uniqueId} removed from the connection list.");

          // Check whether there are now any connected driver instances 
          if (uniqueIds.Count == 0) // There are no connected driver instances so disconnect from the hardware
          {
            //
            // Add hardware disconnect logic here
            //
            if (stream != null)
            {
              try
              {
                stream.Close();
                LogMessage("SetConnected", "Network stream closed.");
              }
              catch (Exception ex)
              {
                LogMessage("SetConnected", $"Error closing stream: {ex.Message}");
              }
              finally
              {
                stream = null;
              }
            }

            if (client != null)
            {
              try
              {
                client.Close();
                LogMessage("SetConnected", "TCP client closed.");
              }
              catch (Exception ex)
              {
                LogMessage("SetConnected", $"Error closing TCP client: {ex.Message}");
              }
              finally
              {
                client = null;
              }
            }
            connectedState = false;
            LogMessage("SetConnected", "connectedState set to false.");
          }
          else // Other device instances are connected so do not disconnect the hardware
          {
            // No action is required
            LogMessage("SetConnected", $"Hardware already connected.");
          }
        }
      }

      // Log the current connected state
      LogMessage("SetConnected", $"Currently connected driver ids:");
      foreach (Guid id in uniqueIds)
      {
        LogMessage("SetConnected", $" ID {id} is connected");
      }
    }

    /// <summary>
    /// Returns a description of the device, such as manufacturer and model number. Any ASCII characters may be used.
    /// </summary>
    /// <value>The description.</value>
    public static string Description
    {
      get
      {
        LogMessage("Description Get", DriverDescription);
        return DriverDescription;
      }
    }

    /// <summary>
    /// Descriptive and version information about this ASCOM driver.
    /// </summary>
    public static string DriverInfo
    {
      get
      {
        Version version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
        string driverInfo = $"Information about the driver itself. Version: {version.Major}.{version.Minor}";
        LogMessage("DriverInfo Get", driverInfo);
        return driverInfo;
      }
    }

    /// <summary>
    /// A string containing only the major and minor version of the driver formatted as 'm.n'.
    /// </summary>
    public static string DriverVersion
    {
      get
      {
        Version version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
        string driverVersion = $"{version.Major}.{version.Minor}";
        LogMessage("DriverVersion Get", driverVersion);
        return driverVersion;
      }
    }

    /// <summary>
    /// The interface version number that this device supports.
    /// </summary>
    public static short InterfaceVersion
    {
      // set by the driver wizard
      get
      {
        LogMessage("InterfaceVersion Get", "4");
        return Convert.ToInt16("4");
      }
    }

    /// <summary>
    /// The short name of the driver, for display purposes
    /// </summary>
    public static string Name
    {
      get
      {
        string name = "DDScopeX";
        LogMessage("Name Get", name);
        return name;
      }
    }

    #endregion

    #region ITelescope Implementation

    /// <summary>
    /// Stops a slew in progress.
    /// </summary>
    internal static void AbortSlew()
    {
      CheckConnected("AbortSlew");
      CommandBlind(":Q#", true);
    }

    /// <summary>
    /// The alignment mode of the mount (Alt/Az, Polar, German Polar).
    /// </summary>
    internal static AlignmentModes AlignmentMode
    {
      get
      {
        LogMessage("AlignmentMode Get", "Returning AltAz");
        return AlignmentModes.algAltAz;
      }
    }

    /// <summary>
    /// The Altitude above the local horizon of the telescope's current position (degrees, positive up)
    /// </summary>
    internal static double Altitude
    {
      get
      {
        LogMessage("Altitude", "Not implemented");
        throw new PropertyNotImplementedException("Altitude", false);
      }
    }

    /// <summary>
    /// The area of the telescope's aperture, taking into account any obstructions (square meters)
    /// </summary>
    internal static double ApertureArea
    {
      get
      {
        LogMessage("ApertureArea Get", "Not implemented");
        throw new PropertyNotImplementedException("ApertureArea", false);
      }
    }

    /// <summary>
    /// The telescope's effective aperture diameter (meters)
    /// </summary>
    internal static double ApertureDiameter
    {
      get
      {
        LogMessage("ApertureDiameter Get", "Not implemented");
        throw new PropertyNotImplementedException("ApertureDiameter", false);
      }
    }

    /// <summary>
    /// True if the telescope is stopped in the Home position. Set only following a <see cref="FindHome"></see> operation,
    /// and reset with any slew operation. This property must be False if the telescope does not support homing.
    /// </summary>
    internal static bool AtHome
    {
      get
      {
        LogMessage("AtHome", "Get - " + false.ToString());
        return false;
      }
    }

    /// <summary>
    /// True if the telescope has been put into the parked state by the seee <see cref="Park" /> method. Set False by calling the Unpark() method.
    /// </summary>
    internal static bool AtPark
    {
      get
      {
        LogMessage("AtPark", "Get - " + false.ToString());
        return false;
      }
    }

    /// <summary>
    /// Determine the rates at which the telescope may be moved about the specified axis by the <see cref="MoveAxis" /> method.
    /// </summary>
    /// <param name="Axis">The axis about which rate information is desired (TelescopeAxes value)</param>
    /// <returns>Collection of <see cref="IRate" /> rate objects</returns>
    internal static IAxisRates AxisRates(TelescopeAxes Axis)
    {
      LogMessage("AxisRates", "Get - " + Axis.ToString());
      return new AxisRates(Axis);
    }

    /// <summary>
    /// The azimuth at the local horizon of the telescope's current position (degrees, North-referenced, positive East/clockwise).
    /// </summary>
    internal static double Azimuth
    {
      get
      {
        LogMessage("Azimuth Get", "Not implemented");
        throw new PropertyNotImplementedException("Azimuth", false);
      }
    }

    /// <summary>
    /// True if this telescope is capable of programmed finding its home position (<see cref="FindHome" /> method).
    /// </summary>
    internal static bool CanFindHome
    {
      get
      {
        LogMessage("CanFindHome", "Get - " + false.ToString());
        return false;
      }
    }

    /// <summary>
    /// True if this telescope can move the requested axis
    /// </summary>
    internal static bool CanMoveAxis(TelescopeAxes Axis)
    {
      LogMessage("CanMoveAxis", "Get - " + Axis.ToString());
      switch (Axis)
      {
        case TelescopeAxes.axisPrimary: return false;
        case TelescopeAxes.axisSecondary: return false;
        case TelescopeAxes.axisTertiary: return false;
        default: throw new InvalidValueException("CanMoveAxis", Axis.ToString(), "0 to 2");
      }
    }

    /// <summary>
    /// True if this telescope is capable of programmed parking (<see cref="Park" />method)
    /// </summary>
    internal static bool CanPark
    {
      get
      {
        LogMessage("CanPark", "Get - " + false.ToString());
        return false;
      }
    }

    /// <summary>
    /// True if this telescope is capable of software-pulsed guiding (via the <see cref="PulseGuide" /> method)
    /// </summary>
    internal static bool CanPulseGuide
    {
      get
      {
        LogMessage("CanPulseGuide", "Get - " + false.ToString());
        return false;
      }
    }

    /// <summary>
    /// True if the <see cref="DeclinationRate" /> property can be changed to provide offset tracking in the declination axis.
    /// </summary>
    internal static bool CanSetDeclinationRate
    {
      get
      {
        LogMessage("CanSetDeclinationRate", "Get - " + false.ToString());
        return false;
      }
    }

    /// <summary>
    /// True if the guide rate properties used for <see cref="PulseGuide" /> can ba adjusted.
    /// </summary>
    internal static bool CanSetGuideRates
    {
      get
      {
        LogMessage("CanSetGuideRates", "Get - " + false.ToString());
        return false;
      }
    }

    /// <summary>
    /// True if this telescope is capable of programmed setting of its park position (<see cref="SetPark" /> method)
    /// </summary>
    internal static bool CanSetPark
    {
      get
      {
        LogMessage("CanSetPark", "Get - " + false.ToString());
        return false;
      }
    }

    /// <summary>
    /// True if the <see cref="SideOfPier" /> property can be set, meaning that the mount can be forced to flip.
    /// </summary>
    internal static bool CanSetPierSide
    {
      get
      {
        LogMessage("CanSetPierSide", "Get - " + false.ToString());
        return false;
      }
    }

    /// <summary>
    /// True if the <see cref="RightAscensionRate" /> property can be changed to provide offset tracking in the right ascension axis.
    /// </summary>
    internal static bool CanSetRightAscensionRate
    {
      get
      {
        LogMessage("CanSetRightAscensionRate", "Get - " + false.ToString());
        return false;
      }
    }

    /// <summary>
    /// True if the <see cref="Tracking" /> property can be changed, turning telescope sidereal tracking on and off.
    /// </summary>
    internal static bool CanSetTracking
    {
      get
      {
        LogMessage("CanSetTracking", "Get - " + true.ToString());
        return true;
      }
    }

    /// <summary>
    /// True if this telescope is capable of programmed slewing (synchronous or asynchronous) to equatorial coordinates
    /// </summary>
    internal static bool CanSlew
    {
      get
      {
        LogMessage("CanSlew", "Get - " + true.ToString());
        return true;
      }
    }

    /// <summary>
    /// True if this telescope is capable of programmed slewing (synchronous or asynchronous) to local horizontal coordinates
    /// </summary>
    internal static bool CanSlewAltAz
    {
      get
      {
        LogMessage("CanSlewAltAz", "Get - " + true.ToString());
        return true;
      }
    }

    /// <summary>
    /// True if this telescope is capable of programmed asynchronous slewing to local horizontal coordinates
    /// </summary>
    internal static bool CanSlewAltAzAsync
    {
      get
      {
        LogMessage("CanSlewAltAzAsync", "Get - " + true.ToString());
        return true;
      }
    }

    /// <summary>
    /// True if this telescope is capable of programmed asynchronous slewing to equatorial coordinates.
    /// </summary>
    internal static bool CanSlewAsync
    {
      get
      {
        LogMessage("CanSlewAsync", "Get - " + true.ToString());
        return true;
      }
    }

    /// <summary>
    /// True if this telescope is capable of programmed syncing to equatorial coordinates.
    /// </summary>
    internal static bool CanSync
    {
      get
      {
        LogMessage("CanSync", "Get - " + true.ToString());
        return true;
      }
    }

    /// <summary>
    /// True if this telescope is capable of programmed syncing to local horizontal coordinates
    /// </summary>
    internal static bool CanSyncAltAz
    {
      get
      {
        LogMessage("CanSyncAltAz", "Get - " + true.ToString());
        return true;
      }
    }

    /// <summary>
    /// True if this telescope is capable of programmed unparking (<see cref="Unpark" /> method).
    /// </summary>
    internal static bool CanUnpark
    {
      get
      {
        LogMessage("CanUnpark", "Get - " + false.ToString());
        return false;
      }
    }

    /// <summary>
    /// The declination (degrees) of the telescope's current equatorial coordinates, in the coordinate system given by the <see cref="EquatorialSystem" /> property.
    /// Reading the property will raise an error if the value is unavailable.
    /// </summary>
    internal static double Declination
    {
      get
      {
        CheckConnected("Declination (get)");
        string response = CommandString(":GD#", true).TrimEnd('#');

        return (utilities.DMSToDegrees(response));
      }
    }

    /// <summary>
    /// The declination tracking rate (arcseconds per SI second, default = 0.0)
    /// </summary>
    internal static double DeclinationRate
    {
      get
      {
        double declination = 0.0;
        LogMessage("DeclinationRate", "Get - " + declination.ToString());
        return declination;
      }
      set
      {
        LogMessage("DeclinationRate Set", "Not implemented");
        throw new PropertyNotImplementedException("DeclinationRate", true);
      }
    }

    /// <summary>
    /// Predict side of pier for German equatorial mounts at the provided coordinates
    /// </summary>
    internal static PierSide DestinationSideOfPier(double RightAscension, double Declination)
    {
      LogMessage("DestinationSideOfPier Get", "Not implemented");
      throw new PropertyNotImplementedException("DestinationSideOfPier", false);
    }

    /// <summary>
    /// True if the telescope or driver applies atmospheric refraction to coordinates.
    /// </summary>
    internal static bool DoesRefraction
    {
      get
      {
        LogMessage("DoesRefraction Get", "False (no refraction)");
        return false;
      }
      set
      {
        LogMessage("DoesRefraction Set", value.ToString());
        // Only “false” makes sense; reject any attempt to turn it on.
        if (value)
          throw new ASCOM.PropertyNotImplementedException("DoesRefraction", true);
        // if value==false, just ignore
      }
    }

    /// <summary>
    /// Equatorial coordinate system used by this telescope (e.g. Topocentric or J2000).
    /// </summary>
    internal static EquatorialCoordinateType EquatorialSystem
    {
      get
      {
        EquatorialCoordinateType equatorialSystem = EquatorialCoordinateType.equTopocentric;
        LogMessage("DeclinationRate", "Get - " + equatorialSystem.ToString());
        return equatorialSystem;
      }
    }

    /// <summary>
    /// Locates the telescope's "home" position (synchronous)
    /// </summary>
    internal static void FindHome()
    {
      LogMessage("FindHome", "Not implemented");
      throw new MethodNotImplementedException("FindHome");
    }

    /// <summary>
    /// The telescope's focal length, meters
    /// </summary>
    internal static double FocalLength
    {
      get
      {
        LogMessage("FocalLength Get", "Not implemented");
        throw new PropertyNotImplementedException("FocalLength", false);
      }
    }

    /// <summary>
    /// The current Declination movement rate offset for telescope guiding (degrees/sec)
    /// </summary>
    internal static double GuideRateDeclination
    {
      get
      {
        LogMessage("GuideRateDeclination Get", "Not implemented");
        throw new PropertyNotImplementedException("GuideRateDeclination", false);
      }
      set
      {
        LogMessage("GuideRateDeclination Set", "Not implemented");
        throw new PropertyNotImplementedException("GuideRateDeclination", true);
      }
    }

    /// <summary>
    /// The current Right Ascension movement rate offset for telescope guiding (degrees/sec)
    /// </summary>
    internal static double GuideRateRightAscension
    {
      get
      {
        LogMessage("GuideRateRightAscension Get", "Not implemented");
        throw new PropertyNotImplementedException("GuideRateRightAscension", false);
      }
      set
      {
        LogMessage("GuideRateRightAscension Set", "Not implemented");
        throw new PropertyNotImplementedException("GuideRateRightAscension", true);
      }
    }

    /// <summary>
    /// True if a <see cref="PulseGuide" /> command is in progress, False otherwise
    /// </summary>
    internal static bool IsPulseGuiding
    {
      get
      {
        LogMessage("IsPulseGuiding Get", "Not implemented");
        throw new PropertyNotImplementedException("IsPulseGuiding", false);
      }
    }

    /// <summary>
    /// Move the telescope in one axis at the given rate.
    /// </summary>
    /// <param name="Axis">The physical axis about which movement is desired</param>
    /// <param name="Rate">The rate of motion (deg/sec) about the specified axis</param>
    internal static void MoveAxis(TelescopeAxes Axis, double Rate)
    {
      LogMessage("MoveAxis", "Not implemented");
      throw new MethodNotImplementedException("MoveAxis");
    }

    /// <summary>
    /// Move the telescope to its park position, stop all motion (or restrict to a small safe range), and set <see cref="AtPark" /> to True.
    /// </summary>
    internal static void Park()
    {
      LogMessage("Park", "Not implemented");
      throw new MethodNotImplementedException("Park");
    }

    /// <summary>
    /// Moves the scope in the given direction for the given interval or time at
    /// the rate given by the corresponding guide rate property
    /// </summary>
    /// <param name="Direction">The direction in which the guide-rate motion is to be made</param>
    /// <param name="Duration">The duration of the guide-rate motion (milliseconds)</param>
    internal static void PulseGuide(GuideDirections Direction, int Duration)
    {
      LogMessage("PulseGuide", "Not implemented");
      throw new MethodNotImplementedException("PulseGuide");
    }

    /// <summary>
    /// The right ascension (hours) of the telescope's current equatorial coordinates,
    /// in the coordinate system given by the EquatorialSystem property
    /// </summary>
    internal static double RightAscension
    {
      get
      {
        CheckConnected("RightAscension (get)");
        string response = CommandString(":GR#", true).TrimEnd('#');

        if (TimeSpan.TryParse(response, out TimeSpan raTime))
        {
          //LogMessage("RightAscension", "Get - " + utilities.HoursToHMS(raTime));
          return raTime.TotalHours;
        }
        else
        {
          throw new ASCOM.InvalidValueException($"Unable to parse RA: '{response}'");
        }
      }
    }

    /// <summary>
    /// The right ascension tracking rate offset from sidereal (seconds per sidereal second, default = 0.0)
    /// </summary>
    internal static double RightAscensionRate
    {
      get
      {
        double rightAscensionRate = 0.0;
        LogMessage("RightAscensionRate", "Get - " + rightAscensionRate.ToString());
        return rightAscensionRate;
      }
      set
      {
        LogMessage("RightAscensionRate Set", "Not implemented");
        throw new PropertyNotImplementedException("RightAscensionRate", true);
      }
    }

    /// <summary>
    /// Sets the telescope's park position to be its current position.
    /// </summary>
    internal static void SetPark()
    {
      LogMessage("SetPark", "Not implemented");
      throw new MethodNotImplementedException("SetPark");
    }

    /// <summary>
    /// Indicates the pointing state of the mount. Read the articles installed with the ASCOM Developer
    /// Components for more detailed information.
    /// </summary>
    internal static PierSide SideOfPier
    {
      get
      {
        LogMessage("SideOfPier Get", "Not implemented");
        throw new PropertyNotImplementedException("SideOfPier", false);
      }
      set
      {
        LogMessage("SideOfPier Set", "Not implemented");
        throw new PropertyNotImplementedException("SideOfPier", true);
      }
    }

    /// <summary>
    /// The local apparent sidereal time from the telescope's internal clock (hours, sidereal)
    /// </summary>
    internal static double SiderealTime
    {
      get
      {
        double siderealTime = 0.0; // Sidereal time return value

        // Use NOVAS 3.1 to calculate the sidereal time
        using (var novas = new NOVAS31())
        {
          double julianDate = utilities.DateUTCToJulian(DateTime.UtcNow);
          novas.SiderealTime(julianDate, 0, novas.DeltaT(julianDate), GstType.GreenwichApparentSiderealTime, Method.EquinoxBased, Accuracy.Full, ref siderealTime);
        }

        // Adjust the calculated sidereal time for longitude using the value returned by the SiteLongitude property, allowing for the possibility that this property has not yet been implemented
        try
        {
          siderealTime += SiteLongitude / 360.0 * 24.0;
        }
        catch (PropertyNotImplementedException) // SiteLongitude hasn't been implemented
        {
          // No action, just return the calculated sidereal time unadjusted for longitude
        }
        catch (Exception) // Some other exception occurred so return it to the client
        {
          throw;
        }

        // Reduce sidereal time to the range 0 to 24 hours
        siderealTime = astroUtilities.ConditionRA(siderealTime);

        LogMessage("SiderealTime", "Get - " + siderealTime.ToString());
        return siderealTime;
      }
    }

    /// <summary>
    /// The elevation above mean sea level (meters) of the site at which the telescope is located
    /// </summary>
    internal static double SiteElevation
    {
      get
      {
        LogMessage("SiteElevation Get", "Querying telescope...");
        string response = CommandString(":Gv#", true)?.TrimEnd('#');  // Get elevation

        if (double.TryParse(response, out double elevation))
        {
          LogMessage("SiteElevation Get", $"Parsed: {elevation} meters");
          return elevation;
        }

        LogMessage("SiteElevation Get", $"Invalid response: '{response}'");
        throw new ASCOM.DriverException("Failed to parse elevation from telescope.");
      }

      set
      {
        LogMessage("SiteElevation Set", $"Setting elevation to {value:F1} meters");

        // Format value as signed decimal with 1 digit after the decimal point
        string formatted = value.ToString("+#0.0;-#0.0");
        string command = $":Sv{formatted}#";

        bool success = CommandBool(command, true);  // Send set command
        success = true; // OnStep always returns false even though it sets the elevation internally. Logic error in the site.command.
        if (!success)
        {
          LogMessage("SiteElevation Set", $"Failed to set elevation with command: {command}");
          throw new ASCOM.DriverException($"Failed to set elevation with command: {command}");
        }

        LogMessage("SiteElevation Set", $"Successfully sent: {command}");
      }
    }

    /// <summary>
    /// The geodetic (map) latitude (degrees, positive North, WGS84) of the site at which the telescope is located.
    /// </summary>
    internal static double SiteLatitude
    {
      get
      {
        CheckConnected("SiteLatitude (get)");

        string response = CommandString(":Gt#", true).TrimEnd('#');
        LogMessage("SiteLatitude Get", $"Response from :Gt#: '{response}'");

        try
        {
          double latitude = utilities.DMSToDegrees(response);
          return latitude;
        }
        catch (Exception ex)
        {
          throw new ASCOM.InvalidValueException($"Unable to parse latitude string '{response}': {ex.Message}");
        }
      }

      set
      {
        CheckConnected("SiteLatitude (set)");

        if (value < -90.0 || value > 90.0)
          throw new ASCOM.InvalidValueException($"Latitude {value} out of range (-90 to +90)");

        // Ensure the sign is included explicitly
        string sign = value >= 0 ? "+" : "-";
        double absVal = Math.Abs(value);
        string dmsStr = sign + utilities.DegreesToDMS(absVal, "*", ":", "", 0);
        string latCmd = $":St{dmsStr}#";

        LogMessage("SiteLatitude Set", $"Sending: {latCmd}");

        if (!CommandBool(latCmd, true))
        {
          throw new ASCOM.DriverException($"Failed to set SiteLatitude using '{latCmd}'");
        }
      }
    }

    /// <summary>
    /// The longitude (degrees, positive East, WGS84) of the site at which the telescope is located.
    /// </summary>
    internal static double SiteLongitude
    {
      get
      {
        CheckConnected("SiteLongitude (get)");

        string response = CommandString(":Gg#", true).TrimEnd('#');
        LogMessage("SiteLongitude Get", $"Response from :Gg#: '{response}'");

        try
        {
          double lon = utilities.DMSToDegrees(response);

          // Convert 0–360 LX200 format to -180–+180 ASCOM standard
          if (lon > 180.0)
            lon -= 360.0;

          return lon;
        }
        catch (Exception ex)
        {
          throw new ASCOM.InvalidValueException($"Unable to parse longitude string '{response}': {ex.Message}");
        }
      }

      set
      {
        CheckConnected("SiteLongitude (set)");

        // Convert -180 to +180 ASCOM format to 0–360 LX200 format
        double lxLon = (value < 0) ? value + 360.0 : value;

        string dmsStr = utilities.DegreesToDMS(lxLon, "*", ":", "", 0);
        string lonStr = $":Sg{dmsStr}#";  // No space after :Sg

        LogMessage("SiteLongitude Set", $"Sending: {lonStr}");

        if (!CommandBool(lonStr, true))
        {
          throw new ASCOM.DriverException($"Failed to set SiteLongitude using command: '{lonStr}'");
        }
      }
    }

    /// <summary>
    /// Specifies a post-slew settling time (sec.).
    /// </summary>
    internal static short SlewSettleTime
    {
      get
      {
        LogMessage("SlewSettleTime Get", "Not implemented");
        throw new PropertyNotImplementedException("SlewSettleTime", false);
      }
      set
      {
        LogMessage("SlewSettleTime Set", "Not implemented");
        throw new PropertyNotImplementedException("SlewSettleTime", true);
      }
    }

    /// <summary>
    /// This Method must be implemented if <see cref="CanSlewAltAzAsync" /> returns True.
    /// It does not return to the caller until the slew is complete.
    /// </summary>
    internal static void SlewToCoordinates(double RightAscension, double Declination)
    {
      CheckConnected("SlewToCoordinates");

      TargetRightAscension = RightAscension;
      TargetDeclination = Declination;

      SlewToTarget(); // blocks until complete
    }

    /// <summary>
    /// Move the telescope to the given equatorial coordinates, return with Slewing set to True immediately after starting the slew.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "VSTHRD200:Use \"Async\" suffix for async methods", Justification = "internal static method name used for many years.")]
    internal static void SlewToCoordinatesAsync(double RightAscension, double Declination)
    {
      CheckConnected("SlewToCoordinatesAsync");

      TargetRightAscension = RightAscension;
      TargetDeclination = Declination;

      SlewToTargetAsync(); // starts slew but returns immediately
    }

    /// <summary>
    // Move the telescope to the <see cref="TargetRightAscension" /> and <see cref="TargetDeclination" /> coordinates, return when slew complete.
    ///// </summary>
    internal static void SlewToTarget()
    {
      CheckConnected("SlewToTarget");

      SlewToTargetAsync();

      // Wait for Slewing to complete
      int timeoutMs = 60000;
      int interval = 1000;
      int elapsed = 0;

      while (Slewing)
      {
        Thread.Sleep(interval);
        elapsed += interval;
        if (elapsed >= timeoutMs)
          throw new ASCOM.DriverException("Slew did not complete within timeout period.");
      }

      LogMessage("SlewToTarget", "Slew complete.");
    }

    /// <summary>
    /// Move the telescope to the <see cref="TargetRightAscension" /> and <see cref="TargetDeclination" />  coordinates,
    /// returns immediately after starting the slew with Slewing set to True.
    /// </summary>
    internal static void SlewToTargetAsync()
    {
      CheckConnected("SlewToTargetAsync");

      double ra = TargetRightAscension;
      double dec = TargetDeclination;

      string raStr = utilities.DegreesToHMS(ra * 15.0, ":", ":", "", 0);
      string decStr = utilities.DegreesToDMS(Math.Abs(dec), "*", ":", "", 0);

      // Returns for :MS#:
      //              0=goto is possible
      //              1=below the horizon limit
      //              2=above overhead limit
      //              3=controller in standby
      //              4=mount is parked
      //              5=goto in progress
      //              6=outside limits
      //              7=hardware fault
      //              8=already in motion
      //              9=unspecified error
      string slewCommandReturnStr = CommandString(":MS#", true);
      if (slewCommandReturnStr != "0#")
      {
        string errorDescription;

        switch (slewCommandReturnStr)
        {
          case "1#":
            errorDescription = "Target is below the horizon limit.";
            break;
          case "2#":
            errorDescription = "Target is above the overhead limit.";
            break;
          case "3#":
            errorDescription = "Controller is in standby mode.";
            break;
          case "4#":
            errorDescription = "Mount is parked.";
            break;
          case "5#":
            errorDescription = "A GOTO is already in progress.";
            break;
          case "6#":
            errorDescription = "Target is outside movement limits.";
            break;
          case "7#":
            errorDescription = "Hardware fault detected.";
            break;
          case "8#":
            errorDescription = "Mount is already in motion.";
            break;
          case "9#":
            errorDescription = "Unspecified error.";
            break;
          default:
            errorDescription = "Unknown response code.";
            break;
        }
        throw new ASCOM.DriverException(
            $"Slew command failed: code '{slewCommandReturnStr}' – {errorDescription}");
      }

      LogMessage("SlewToTargetAsync", $"Slew started to RA={raStr}, DEC={decStr}");
    }

    /// <summary>
    /// Move the telescope to the given local horizontal coordinates, return when slew is complete
    /// </summary>
    internal static void SlewToAltAz(double Azimuth, double Altitude)
    {
      SlewToAltAzAsync(Azimuth, Altitude);

      // Wait until complete
      int timeoutMs = 60000;
      int interval = 1000;
      int elapsed = 0;

      while (Slewing)
      {
        Thread.Sleep(interval);
        elapsed += interval;
        if (elapsed >= timeoutMs)
          throw new ASCOM.DriverException("AltAz slew did not complete within timeout period.");
      }

      LogMessage("SlewToAltAz", "AltAz slew complete.");
    }

    /// <summary>
    /// This Method must be implemented if <see cref="CanSlewAltAzAsync" /> returns True.
    /// It returns immediately, with Slewing set to True
    /// </summary>
    /// <param name="Azimuth">Azimuth to which to move</param>
    /// <param name="Altitude">Altitude to which to move to</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "VSTHRD200:Use \"Async\" suffix for async methods", Justification = "internal static method name used for many years.")]
    internal static void SlewToAltAzAsync(double Azimuth, double Altitude)
    {
      CheckConnected("SlewToAltAzAsync");

      string azStr = utilities.DegreesToDMS(Azimuth, "*", ":", "", 0);
      string altStr = utilities.DegreesToDMS(Altitude, "*", ":", "", 0);

      if (!CommandBool($":Sz{azStr}#", true))
        throw new ASCOM.DriverException($"Failed to set Azimuth: {azStr}");

      if (!CommandBool($":Sa{altStr}#", true))
        throw new ASCOM.DriverException($"Failed to set Altitude: {altStr}");

      if (!CommandBool(":MS#", true))
        throw new ASCOM.DriverException("AltAz slew command failed");

      LogMessage("SlewToAltAzAsync", $"Slew started to Az={azStr}, Alt={altStr}");
    }

    /// <summary>
    /// True if telescope is in the process of moving in response to one of the
    /// Slew methods or the <see cref="MoveAxis" /> method, False at all other times.
    /// </summary>
    internal static bool Slewing
    {
      get
      {
        CheckConnected("Slewing");

        // DDScope Encoder is 2^14 counts or 16384 steps/counts per 360 deg or 0.022 deg or 79 arcsec
        const double positionToleranceDeg = 0.03; //<---Scope specific value - tune

        double currentRA = RightAscension;   // From telescope (in hours)
        double currentDEC = Declination;     // From telescope (in degrees)

        double targetRA = TargetRightAscension;    // Store in hours
        double targetDEC = TargetDeclination;      // Store in degrees

        // Convert RA to degrees for comparison
        double deltaRA = Math.Abs((currentRA - targetRA) * 15.0);
        double deltaDEC = Math.Abs(currentDEC - targetDEC);

        bool slewing = deltaRA > positionToleranceDeg || deltaDEC > positionToleranceDeg;
        LogMessage("Slewing Get", $"Current: RA={currentRA:F4}h, DEC={currentDEC:F4}° | Target: RA={targetRA:F4}h, DEC={targetDEC:F4}° | Slewing={slewing}");

        return slewing;
      }
    }

    /// <summary>
    /// Matches the scope's equatorial coordinates to the given equatorial coordinates.
    /// </summary>
    internal static void SyncToCoordinates(double RightAscension, double Declination)
    {
      CheckConnected("SyncToCoordinates");

      TargetRightAscension = RightAscension;
      TargetDeclination = Declination;

      SyncToTarget();
    }

    /// <summary>
    /// Matches the scope's equatorial coordinates to the target equatorial coordinates.
    /// </summary>
    internal static void SyncToTarget()
    {
      CheckConnected("SyncToTarget");

      double ra = TargetRightAscension;
      double dec = TargetDeclination;

      string raStr = utilities.DegreesToHMS(ra * 15.0, ":", ":", "", 0);
      string decStr = utilities.DegreesToDMS(Math.Abs(dec), "*", ":", "", 0);

      LogMessage("SyncToTarget", "Sending sync command :CM#");
      string syncCommandReturnStr = CommandString(":CM#", true);
      if (syncCommandReturnStr != "N/A#")
        throw new ASCOM.DriverException("Sync command failed");

      LogMessage("SyncToTarget", $"Sync'd at RA={raStr}, DEC={decStr}");
    }

    /// <summary>
    /// Matches the scope's local horizontal coordinates to the given local horizontal coordinates.
    /// </summary>
    internal static void SyncToAltAz(double Azimuth, double Altitude)
    {
      LogMessage("SyncToAltAz", "Not implemented — OnStep LX200 mode does not support AltAz sync.");
      throw new MethodNotImplementedException("SyncToAltAz");
    }


    /// <summary>
    /// The declination (degrees, positive North) for the target of an equatorial slew or sync operation
    /// </summary>
    internal static double TargetDeclination
    {
      get
      {
        CheckConnected("TargetDeclination (get)");
        LogMessage("TargetDeclination Get", $"Returning stored DEC: {targetDEC}°");
        return targetDEC;
      }

      set
      {
        CheckConnected("TargetDeclination (set)");
        targetDEC = value;

        string dms = utilities.DegreesToDMS(value, "*", ":", "", 0); // "+DD*MM:SS" format
        string command = $":Sd{(value < 0 ? "" : "+")}{dms}#"; // if positive, add the '+' else if negative skip, already has '-'

        LogMessage("TargetDeclination Set", $"Sending command: '{command}'");

        if (!CommandBool(command, true))
        {
          throw new ASCOM.DriverException($"Failed to set DEC using command: '{command}'");
        }
      }
    }

    /// <summary>
    /// The right ascension (hours) for the target of an equatorial slew or sync operation
    /// </summary>
    internal static double TargetRightAscension
    {
      get
      {
        CheckConnected("TargetRightAscension (get)");
        LogMessage("TargetRightAscension Get", $"Returning stored RA: {targetRA}h");
        return targetRA;
      }

      set
      {
        CheckConnected("TargetRightAscension (set)");
        targetRA = value;

        double raDegrees = value * 15.0; // Convert hours to degrees
        string hms = utilities.DegreesToHMS(raDegrees, ":", ":", "", 0); // Format as HH:MM:SS
        string command = $":Sr{hms}#";

        LogMessage("TargetRightAscension Set", $"Sending command: '{command}'");

        if (!CommandBool(command, true))
        {
          throw new ASCOM.DriverException($"Failed to set RA using command: '{command}'");
        }
      }
    }

    /// <summary>
    /// Controls whether the telescope is actively tracking.
    /// </summary>
    internal static bool Tracking
    {
      get
      {
        CheckConnected("Tracking (get)");

        string response = CommandString(":GT#", true).TrimEnd('#');
        LogMessage("Tracking Get", $"Response: {response}");

        // Interpret response: "0.0" = not tracking, anything else = tracking
        if (double.TryParse(response, out double rate))
        {
          return rate != 0.0;
        }
        else
        {
          throw new ASCOM.DriverException($"Unexpected tracking rate format: '{response}'");
        }
      }
      set
      {
        CheckConnected("Tracking (set)");

        string cmd = value ? ":Te#" : ":Td#";
        LogMessage("Tracking Set", $"Sending command: {cmd}");

        if (!CommandBool(cmd, true))
        {
          throw new ASCOM.DriverException($"Failed to {(value ? "start" : "stop")} tracking with command '{cmd}'");
        }
      }
    }

    /// <summary>
    /// The current tracking rate of the telescope's sidereal drive
    /// </summary>
    internal static DriveRates TrackingRate
    {
      get
      {
        const DriveRates DEFAULT_DRIVERATE = DriveRates.driveSidereal;
        LogMessage("TrackingRate Get", $"{DEFAULT_DRIVERATE}");
        return DEFAULT_DRIVERATE;
      }
      set
      {
        LogMessage("TrackingRate Set", "Not implemented");
        throw new PropertyNotImplementedException("TrackingRate", true);
      }
    }

    /// <summary>
    /// Returns a collection of supported <see cref="DriveRates" /> values that describe the permissible
    /// values of the <see cref="TrackingRate" /> property for this telescope type.
    /// </summary>
    internal static ITrackingRates TrackingRates
    {
      get
      {
        ITrackingRates trackingRates = new TrackingRates();
        LogMessage("TrackingRates", "Get - ");
        foreach (DriveRates driveRate in trackingRates)
        {
          LogMessage("TrackingRates", "Get - " + driveRate.ToString());
        }
        return trackingRates;
      }
    }

    /// <summary>
    /// The UTC date/time of the telescope's internal clock
    /// </summary>
    internal static DateTime UTCDate
    {
      get
      {
        DateTime utcDate = DateTime.UtcNow;
        LogMessage("UTCDate", "Get - " + String.Format("MM/dd/yy HH:mm:ss", utcDate));
        return utcDate;
      }
      set
      {
        LogMessage("UTCDate Set", "Not implemented");
        throw new PropertyNotImplementedException("UTCDate", true);
      }
    }

    /// <summary>
    /// Takes telescope out of the Parked state.
    /// </summary>
    internal static void Unpark()
    {
      LogMessage("Unpark", "Not implemented");
      throw new MethodNotImplementedException("Unpark");
    }

    /// <summary>
    /// The Telescope Local TimeZone or UTC offset
    /// </summary>
    internal static double UtcOffset
    {
      get
      {
        CheckConnected("UtcOffset (get)");

        string response = CommandString(":GG#", true).TrimEnd('#');
        LogMessage("UtcOffset Get", $"Response from :GG#: '{response}'");
        double utcOff = utilities.HMSToHours(response);
        return utcOff;
      }
      set
      {
        // No-op or log warning, since this can't be set on the PC
        LogMessage("UtcOffset Set", $"Attempted to set UTC offset to {value}, but this is read-only.");
        throw new ASCOM.PropertyNotImplementedException("UtcOffset", true);
      }
    }

    #endregion

    #region Private properties and methods
    // Useful methods that can be used as required to help with driver development

    /// <summary>
    /// Returns true if there is a valid connection to the driver hardware
    /// </summary>
    //private static bool IsConnected
    internal static bool IsConnected  // Need access to this in the UI!!!
    {
      get
      {
        return connectedState;
      }
    }

    /// <summary>
    /// Use this function to throw an exception if we aren't connected to the hardware
    /// </summary>
    /// <param name="message"></param>
    private static void CheckConnected(string message)
    {
      if (!IsConnected)
      {
        throw new NotConnectedException(message);
      }
    }

    /// <summary>
    /// Read the device configuration from the ASCOM Profile store
    /// </summary>
    internal static void ReadProfile()
    {
      using (Profile driverProfile = new Profile())
      {
        driverProfile.DeviceType = "Telescope";
        tl.Enabled = Convert.ToBoolean(driverProfile.GetValue(DriverProgId, traceStateProfileName, string.Empty, traceStateDefault));
        comPort = driverProfile.GetValue(DriverProgId, comPortProfileName, string.Empty, comPortDefault);
      }
    }

    /// <summary>
    /// Write the device configuration to the  ASCOM  Profile store
    /// </summary>
    internal static void WriteProfile()
    {
      using (Profile driverProfile = new Profile())
      {
        driverProfile.DeviceType = "Telescope";
        driverProfile.WriteValue(DriverProgId, traceStateProfileName, tl.Enabled.ToString());
        driverProfile.WriteValue(DriverProgId, comPortProfileName, comPort.ToString());
      }
    }

    /// <summary>
    /// Log helper function that takes identifier and message strings
    /// </summary>
    /// <param name="identifier"></param>
    /// <param name="message"></param>
    internal static void LogMessage(string identifier, string message)
    {
      tl.LogMessageCrLf(identifier, message);
    }

    /// <summary>
    /// Log helper function that takes formatted strings and arguments
    /// </summary>
    /// <param name="identifier"></param>
    /// <param name="message"></param>
    /// <param name="args"></param>
    internal static void LogMessage(string identifier, string message, params object[] args)
    {
      var msg = string.Format(message, args);
      LogMessage(identifier, msg);
    }
    #endregion

  }
}

