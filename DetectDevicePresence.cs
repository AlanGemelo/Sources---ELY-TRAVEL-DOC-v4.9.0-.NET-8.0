using System;
using System.Runtime.InteropServices;

internal static class UsbNotification
{
    public const int DbtDeviceArrival = 0x8000; // system detected a new device
    public const int DbtDeviceRemoveComplete = 0x8004; // device is gone
    public const int WmDeviceChange = 0x0219; // device change event
    public const int DbtDevTypeDeviceInterface = 5; // For device interface based devices (applicable for CCID in our case)
    public const int DbtDevTypePort = 3; // For serial and parallel port based devices (incl. VCOM)
    private static readonly Guid GuidDevInterfaceUsbVcomDevice = new Guid("4D36E978-E325-11CE-BFC1-08002BE10318"); // USB VCOM devices
    private static readonly Guid GuidDevInterfaceUsbCcidDevice = new Guid("50DD5230-BA8A-11D1-BF5D-0000F805F530"); // USB CCID devices
    private static IntPtr notificationHandleForUsbVcom;
    private static IntPtr notificationHandleForUsbCcid;

    /// <summary>
    /// Registers a window to receive notifications when USB VCOM are plugged or unplugged.
    /// </summary>
    /// <param name="windowHandle">Handle to the window receiving notifications.</param>
    public static IntPtr RegisterUsbDeviceNotification(IntPtr windowHandle, Guid guid)
    {
        DevBroadcastDeviceinterface dbi = new DevBroadcastDeviceinterface
        {
            DeviceType = DbtDevTypeDeviceInterface,
            Reserved = 0,
            ClassGuid = guid,
            Name = 0
        };

        dbi.Size = Marshal.SizeOf(dbi);
        IntPtr buffer = Marshal.AllocHGlobal(dbi.Size);
        Marshal.StructureToPtr(dbi, buffer, true);

        return RegisterDeviceNotification(windowHandle, buffer, 0);
    }

    /// <summary>
    /// Registers a window to receive notifications when USB VCOM devices are plugged or unplugged.
    /// </summary>
    /// <param name="windowHandle">Handle to the window receiving notifications.</param>
    public static void RegisterUsbVcomDeviceNotification(IntPtr windowHandle)
    {
        notificationHandleForUsbVcom = RegisterUsbDeviceNotification(windowHandle, GuidDevInterfaceUsbVcomDevice);
    }

    /// <summary>
    /// Unregisters the window for USB device notifications
    /// </summary>
    public static void UnregisterUsbVcomDeviceNotification()
    {
        UnregisterDeviceNotification(notificationHandleForUsbVcom);
    }

    /// <summary>
    /// Registers a window to receive notifications when USB VCOM devices are plugged or unplugged.
    /// </summary>
    /// <param name="windowHandle">Handle to the window receiving notifications.</param>
    public static void RegisterUsbCcidDeviceNotification(IntPtr windowHandle)
    {
        notificationHandleForUsbCcid = RegisterUsbDeviceNotification(windowHandle, GuidDevInterfaceUsbCcidDevice);
    }

    /// <summary>
    /// Unregisters the window for USB device notifications
    /// </summary>
    public static void UnregisterUsbCcidDeviceNotification()
    {
        UnregisterDeviceNotification(notificationHandleForUsbCcid);
    }

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr RegisterDeviceNotification(IntPtr recipient, IntPtr notificationFilter, int flags);

    [DllImport("user32.dll")]
    private static extern bool UnregisterDeviceNotification(IntPtr handle);

    [StructLayout(LayoutKind.Sequential)]
    private struct DevBroadcastDeviceinterface
    {
        internal int Size;
        internal int DeviceType;
        internal int Reserved;
        internal Guid ClassGuid;
        internal short Name;
    }
}
