using LibUsbDotNet.Info;
using LibUsbDotNet.Main;
using LibUsbDotNet;
using System;
using System.Collections.ObjectModel;

namespace ELY_TRAVEL_DOC
{
    internal class ElychipsetUsb
    {
        // Device VID & PID
        private const int ELYCHIPSET_VID = 0x2B78;
        private const int ELYCHIPSET_PID_CL = 0x42;
        private const int ELYCHIPSET_PID_CNT = 0x41;
        private const int ELYCHIPSET_PID_DUAL = 0x43;

        // Interface type 
        private const byte DFU_CLASS = 0xFE;
        private const byte DFU_SUB_CLASS = 0x01;
        private const byte DFU_PROTOCOL = 0x01;

        // Endpoint direction 
        private const byte LIBUSB_ENDPOINT_IN = 0x80;
        private const byte LIBUSB_ENDPOINT_OUT = 0x00;

        // Recipient bits of the control_setup packet (bmRequestType filed)
        private const byte LIBUSB_RECIPIENT_DEVICE = 0x00;      // Device
        private const byte LIBUSB_RECIPIENT_INTERFACE = 0x01;   // Interface
        private const byte LIBUSB_RECIPIENT_ENDPOINT = 0x02;    // Endpoint
        private const byte LIBUSB_RECIPIENT_OTHER = 0x03;       // Other

        // Request type bits of the control_setup packet (bmRequestType filed)
        private const byte LIBUSB_REQUEST_TYPE_STANDARD = (0x00 << 5);  // Standard
        private const byte LIBUSB_REQUEST_TYPE_CLASS = (0x01 << 5);     // Class
        private const byte LIBUSB_REQUEST_TYPE_VENDOR = (0x02 << 5);    // Vendor
        private const byte LIBUSB_REQUEST_TYPE_RESERVED = (0x03 << 5);  // Reserved

        // Vendor commands
        private const byte DFU_VENDOR_CMD_GET_CONFIG_RECORD = 0x00;

        // Config record offsets
        private const short CONFIG_RECORD_OFFSET_FW_VERSION = 0x08;
        private const short CONFIG_RECORD_OFFSET_MANUFACTURER_STRING = 0x100;

        // Get configuration record through runtime DFU interface
        private static byte[] getConfigRecord(short nConfigOffset)
        {
            UsbDevice MyUsbDevice = null;
            UsbRegDeviceList allDevices = UsbDevice.AllDevices;
            byte byInterfaceNumber = 0;
            bool bDeviceFound = false;
            byte[] abyInData = new byte[256];
            byte[] abyOutData = null;
            int lenOut;
            foreach (UsbRegistry usbRegistry in allDevices)
            {
                if ( (usbRegistry.Vid == ELYCHIPSET_VID) &&
                     ( ((usbRegistry.Pid & 0x00FF) == ELYCHIPSET_PID_CL) ||
                       ((usbRegistry.Pid & 0x00FF) == ELYCHIPSET_PID_CNT) ||
                       ((usbRegistry.Pid & 0x00FF) == ELYCHIPSET_PID_DUAL) ) )
                {
                    if (usbRegistry.Open(out MyUsbDevice))
                    {
                        Console.WriteLine(MyUsbDevice.Info.ToString());
                        for (int iConfig = 0; iConfig < MyUsbDevice.Configs.Count; iConfig++)
                        {
                            UsbConfigInfo configInfo = MyUsbDevice.Configs[iConfig];
                            Console.WriteLine(configInfo.ToString());

                            ReadOnlyCollection<UsbInterfaceInfo> interfaceList = configInfo.InterfaceInfoList;
                            for (byte iInterface = 0; iInterface < interfaceList.Count; iInterface++)
                            {
                                UsbInterfaceInfo interfaceInfo = interfaceList[iInterface];
                                Console.WriteLine(interfaceInfo.ToString());
                                if ( ((byte)interfaceInfo.Descriptor.Class == DFU_CLASS) &&
                                    (interfaceInfo.Descriptor.SubClass == DFU_SUB_CLASS) &&
                                    (interfaceInfo.Descriptor.Protocol == DFU_PROTOCOL) )
                                {
                                    byInterfaceNumber = interfaceInfo.Descriptor.InterfaceID;
                                    bDeviceFound = true;
                                    break;
                                }
                            }
                            if (bDeviceFound)
                                break;
                        }
                    }
                }
                if (bDeviceFound)
                    break;
            }
            if (MyUsbDevice != null)
            {
                if (bDeviceFound)
                {
                    UsbSetupPacket usbSetupPacket = new UsbSetupPacket();
                    usbSetupPacket.RequestType = (LIBUSB_ENDPOINT_IN | LIBUSB_REQUEST_TYPE_VENDOR | LIBUSB_RECIPIENT_INTERFACE);
                    usbSetupPacket.Request = DFU_VENDOR_CMD_GET_CONFIG_RECORD;
                    usbSetupPacket.Value = nConfigOffset;
                    usbSetupPacket.Index = byInterfaceNumber;
                    usbSetupPacket.Length = 0;
                    if (MyUsbDevice.ControlTransfer(ref usbSetupPacket, abyInData, abyInData.Length, out lenOut) && lenOut > 0)
                    {
                        abyOutData = new byte[lenOut];
                        Array.Copy(abyInData, abyOutData, lenOut);
                    }
                }
                MyUsbDevice.Close();
            }
            // Free usb resources.
            // This is necessary for libusb-1.0 and Linux compatibility.
            UsbDevice.Exit();
            return abyOutData;
        }

        // Get FW version record from configuration sector offset 0x08
        public static byte[] getElyChipsetFwVersionRecord()
        {
            return getConfigRecord(CONFIG_RECORD_OFFSET_FW_VERSION);
        }

        // Get FW MFR string record from configuration sector offset 0x100
        public static byte[] getElyChipsetManufacturerStringRecord()
        {
            return getConfigRecord(CONFIG_RECORD_OFFSET_MANUFACTURER_STRING);
        }
    }
}
