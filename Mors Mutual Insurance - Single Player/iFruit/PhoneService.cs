using GTA;
using GTA.Native;
using iFruitAddon2;
using MMI_SP.Debug;
using MMI_SP.Helpers;
using MMI_SP.iFruit.Config;
using MMI_SP.iFruit.Mechanic;
using MMI_SP.iFruit.MMI;
using MMI_SP.Insurance.Delivery;
using System;

namespace MMI_SP.iFruit
{
    internal class PhoneService
    {
        private readonly CustomiFruit _iFruit;
        private MMIMenu _menu;
        private ConfigMenu _configMenu;
        private MechanicMenu _mechanicMenu;
        private bool _wasMenuVisible;
        private bool _wasMechanicMenuVisible;
        internal bool MechanicDeliveryRequested = false;

        public PhoneService()
        {
            _iFruit = new CustomiFruit();
        }

        public void Initialize()
        {
            while (!Insurance.Observer.Manager.Initialized) Script.Yield();

            _menu = new MMIMenu();
            _configMenu = new ConfigMenu();
            _mechanicMenu = new MechanicMenu();

            Script.Wait(2000);

            var contactMMI = new iFruitContact("Mors Mutual Insurance") { DialTimeout = 4000, Active = true, Icon = ContactIcon.MP_MorsMutual };
            contactMMI.Answered += OnMMIAnswered;

            var contactMechanic = new iFruitContact("Mecánico") { DialTimeout = 4000, Active = true, Icon = ContactIcon.LSCustoms };
            contactMechanic.Answered += OnMechanicAnswered;

            var contactConfig = new iFruitContact("Configuración") { DialTimeout = 0, Active = true, Icon = ContactIcon.MP_FmContact };
            contactConfig.Answered += OnConfigAnswered;

            _iFruit.Contacts.Add(contactMMI);
            _iFruit.Contacts.Add(contactMechanic);
            _iFruit.Contacts.Add(contactConfig);
        }

        public void OnTick()
        {
            _menu?.MenuPoolProcessMenus();
            _configMenu?.MenuPoolProcessMenus();
            _mechanicMenu?.MenuPoolProcessMenus();

            Manager.ForceArrivalIfNear();

            Timers.ExecuteWhenExpired("ClosePhone");

            bool isMMIVisible = _menu != null && _menu.IsAnyMenuVisible;
            bool isMechanicVisible = _mechanicMenu != null && _mechanicMenu.IsAnyMenuVisible;

            if (_wasMenuVisible && !isMMIVisible) MMISound.Play(MMISound.SoundFamily.Bye);

            if (_wasMechanicMenuVisible && !isMechanicVisible) {
                if (!MechanicDeliveryRequested) MechanicSound.Play(MechanicSound.SoundFamily.Deny);
                MechanicDeliveryRequested = false;
            }

            _wasMenuVisible = isMMIVisible;
            _wasMechanicMenuVisible = isMechanicVisible;

            if (!Function.Call<bool>(Hash.IS_MOBILE_PHONE_CALL_ONGOING)) _iFruit.Update();
        }

        public void OnAborted() {
            if (_iFruit?.Contacts?.Count > 0) _iFruit.Contacts.ForEach(c => c.EndCall());
        }

        private void OnMMIAnswered(iFruitContact contact)
        {
            ExecuteSafe("OnMMIAnswered", () => {
                MMISound.Play(MMISound.SoundFamily.Hello);
                _menu.Reset(true);
                _menu.Show();
                _wasMenuVisible = true;
                Timers.StartCustomTimer("ClosePhone", 2000, () => _iFruit.Close());
            });
        }

        private void OnConfigAnswered(iFruitContact contact)
        {
            ExecuteSafe("OnConfigAnswered", () => {
                _configMenu.Show();
                _iFruit.Close();
            });
        }

        private void OnMechanicAnswered(iFruitContact contact)
        {
            ExecuteSafe("OnMechanicAnswered", () => {
                MechanicSound.Play(MechanicSound.SoundFamily.Hello);
                _mechanicMenu.Reset(true);
                _mechanicMenu.Show();
                _wasMechanicMenuVisible = true;
                Timers.StartCustomTimer("ClosePhone", 2000, () => _iFruit.Close());
            });
        }

        private void ExecuteSafe(string context, Action action)
        {
            try { action(); }
            catch (Exception ex) { Logger.Error($"iFruit.Core.{context}: {ex.Message}"); }
        }
    }
}