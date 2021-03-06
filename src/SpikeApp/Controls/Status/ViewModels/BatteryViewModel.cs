﻿using SpikeApp.Utilities;
using SpikeLib.Messages;

namespace SpikeApp.Controls.Status.ViewModels
{
    public class BatteryViewModel : ViewModelBase
    {
        private float voltage;
        private int percentage;

        public float Voltage
        {
            get => voltage;
            set => RaiseAndSetIfChanged(ref voltage, value);
        }

        public int Percentage
        {
            get => percentage;
            set => RaiseAndSetIfChanged(ref percentage, value);
        }

        public void Update(BatteryMessage message)
        {
#pragma warning disable CA1062 // Validate arguments of public methods
            Voltage = message.Voltage;
            Percentage = message.Percentage;
#pragma warning restore CA1062 // Validate arguments of public methods
        }
    }
}
