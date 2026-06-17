using System.ComponentModel;
using HidSharp;

namespace GHelper.AnimeMatrix.Communication.Platform
{
    internal class WindowsUsbProvider : UsbProvider
    {
        protected HidDevice? HidDevice { get; }
        protected HidStream? HidStream { get; }
        private readonly bool _connected;

        public WindowsUsbProvider(ushort vendorId, ushort productId, string path, int timeout = 500) : base(vendorId, productId)
        {
            try
            {
                HidDevice = DeviceList.Local.GetHidDevices(vendorId, productId)
                   .First(x => x.DevicePath.Contains(path));
            }
            catch
            {
                Logger.WriteLine("HID device was not found on your machine.");
                _connected = false;
                return;
            }

            try
            {
                var config = new OpenConfiguration();
                config.SetOption(OpenOption.Interruptible, true);
                config.SetOption(OpenOption.Exclusive, false);
                config.SetOption(OpenOption.Priority, 10);
                HidStream = HidDevice.Open(config);
                HidStream.ReadTimeout = timeout;
                HidStream.WriteTimeout = timeout;
                _connected = true;
            }
            catch (Exception ex)
            {
                Logger.WriteLine($"Failed to open HID device: {ex.Message}");
                _connected = false;
            }
        }

        public WindowsUsbProvider(ushort vendorId, ushort productId, int maxFeatureReportLength, string name = "Matrix")
            : base(vendorId, productId)
        {
            try
            {
                HidDevice = DeviceList.Local
                    .GetHidDevices(vendorId, productId)
                    .First(x => x.GetMaxFeatureReportLength() >= maxFeatureReportLength);
                Logger.WriteLine($"{name} Device: " + HidDevice.DevicePath + " " + HidDevice.GetMaxFeatureReportLength());
            }
            catch
            {
                Logger.WriteLine($"{name} control device was not found on your machine.");
                _connected = false;
                return;
            }

            try
            {
                var config = new OpenConfiguration();
                config.SetOption(OpenOption.Interruptible, true);
                config.SetOption(OpenOption.Exclusive, false);
                config.SetOption(OpenOption.Priority, 10);
                HidStream = HidDevice.Open(config);
                _connected = true;
            }
            catch (Exception ex)
            {
                Logger.WriteLine($"Failed to open {name} HID device: {ex.Message}");
                _connected = false;
            }
        }

        public override void Set(byte[] data)
        {
            if (HidStream is null) return;
            WrapException(() =>
            {
                HidStream.SetFeature(data);
                HidStream.Flush();
            });
        }

        public override byte[] Get(byte[] data)
        {
            if (HidStream is null) return data;
            var outData = new byte[data.Length];
            Array.Copy(data, outData, data.Length);

            WrapException(() =>
            {
                HidStream.GetFeature(outData);
                HidStream.Flush();
            });

            return outData;
        }

        public override void Read(byte[] data)
        {
            if (HidStream is null) return;
            WrapException(() =>
            {
                HidStream.Read(data);
            });
        }

        public override void Write(byte[] data)
        {
            if (HidStream is null) return;
            WrapException(() =>
            {
                HidStream.Write(data);
                HidStream.Flush();
            });
        }

        public override void Dispose()
        {
            HidStream?.Dispose();
        }

        private void WrapException(Action action)
        {
            try
            {
                action();
            }
            catch (IOException e)
            {
                if (e.InnerException is Win32Exception w32e)
                {
                    if (w32e.NativeErrorCode != 0)
                    {
                        throw;
                    }
                }
            }
        }
    }
}