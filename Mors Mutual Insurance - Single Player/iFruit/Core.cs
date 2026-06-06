using System;
using GTA;

namespace MMI_SP.iFruit
{
    public class Core : Script
    {
        internal static Core Instance { get; private set; }
        private PhoneService _phoneService;
        public static bool MechanicDeliveryRequested {
            get => Instance?._phoneService?.MechanicDeliveryRequested ?? false;
            set { if (Instance?._phoneService != null) Instance._phoneService.MechanicDeliveryRequested = value; }
        }

        public Core() {
            Instance = this;
            _phoneService = new PhoneService();
            Tick += Initialize;
            Aborted += OnAborted;
        }

        private void Initialize(object sender, EventArgs e) {
            while (!Insurance.Observer.Manager.Initialized) Yield();
            _phoneService.Initialize();
            Tick -= Initialize;
            Tick += OnTick;
        }

        private void OnTick(object sender, EventArgs e) => _phoneService.OnTick();

        private void OnAborted(object sender, EventArgs e) => _phoneService.OnAborted();
    }
}